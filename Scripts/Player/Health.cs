using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;


public class Health : MonoBehaviour
{
    //Screenshake variables required 
    [SerializeField] private float damageStress; //how much you want it to shake when damaged
    [SerializeField] private float damageStressModifier; //how more violent screenshake should be when taking more than 1 damage

    private int health;
    private int previousHealth;
    private bool invulnerable;      //i-frames 
    private bool invincible;        //for debugging
    private bool firstUpdate = true;
    [SerializeField] private int maxHealth;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private Animation[] hearts_anim;
    private Animator anim;

    [SerializeField] private GameObject transitionObject;
    private Animator transitionAnim;

    [SerializeField] private GameObject playermodel;
    [SerializeField] private GameObject panmodel;

    [SerializeField] PostProcManager ppManager;
    
    private Player controls;

    public delegate void CameraShake(float shakeAmount);
    public static event CameraShake OnCameraShake;

    public delegate void PlayerDamage(bool onLowHeath);
    public static event PlayerDamage OnHealthChange;

    [SerializeField] ParticleSystem damagePS;
    [SerializeField] ParticleSystem healPS;

    [SerializeField] private BoolScriptableObj debugState = null;

    [Header("FMOD")]
    [SerializeField] [Tooltip("Duration in seconds of how soon the sound can be played after an instance has started to play")]
    private float suckBulletCooldown = 1f;
    [SerializeField] [Tooltip("Duration in seconds of how long before the Hatcuum combo resets")]
    private float hatcuumComboTimer = 1f;

    // Events
    [EventRef] private string eInjured = "{9b759de0-d4dd-44d4-a18e-a82174539316}";
    [EventRef] private string eDead = "{a0e425e8-8005-4aa2-ad9b-07a22b9ea79f}";
    [EventRef] private string eHeal = "{6f1c2b28-ae3e-41f3-a15d-0700e17c7ecc}";
    [EventRef] private string eMaxHeal = "{d2863679-dc91-41a7-b27f-128f7012f1f9}";
    [EventRef] private string eEatIngredient = "{269fbf84-1a1f-4293-af51-a5f3dd2b1dcc}";
    [EventRef] private string eSuckBullet = "{cdc04524-f337-448a-8f98-b04d13476224}";

    private int numOfBulletsSucked = 0;
    private bool suckCooldownActive = false;
    private IEnumerator timer;


    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Eating.performed += ctx => RecivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        previousHealth = -1;
        UpdateUI();
        UpdateText();
        timer = HatcuumComboTimer();
        anim = GetComponentInChildren<Animator>();

        transitionAnim = transitionObject.GetComponent<Animator>();

        if (ppManager == null)
        {
            ppManager = Camera.main.GetComponentInChildren<PostProcManager>();
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts_anim[i] = hearts[i].GetComponent<Animation>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if(Input.GetKeyDown(KeyCode.E)) RecivedInput();

            if (Input.GetKeyDown(KeyCode.Q) && debugState.GetValue())    //turns on invincible mode
            {
                invincible = !invincible;
                UpdateText();
            }
        }
    }

    //Handles what happens when the player takes damage
    public void Damage(int dam, bool ignoreDam)
    {
        if (!debugState.GetValue()) invincible = false;
        if (!invulnerable && !invincible && !HatcuumController.hatcuumActive && !ignoreDam)
        {
            RuntimeManager.PlayOneShot(eInjured);
            if (invulnerable) StopCoroutine(InvTime(1, true));
            StartCoroutine(InvTime(1, true));
            health = health - dam;
            if (health <= 0) Death();
            if (health >= 1) StartCoroutine(GameManager.instance.FreezeFrameDamage(0.2f));
            damagePS.Simulate(10 * Time.unscaledDeltaTime, true, true);
            damagePS.Play();        
            UpdateUI();
            if (dam == 1) OnCameraShake?.Invoke(damageStress);        //unity event that starts the InduceStress in the ShakeableTransform script
            if (dam >= 2) OnCameraShake?.Invoke(damageStress * damageStressModifier);        //unity event that starts the InduceStress in the ShakeableTransform script
        }
        // Hatcuum sucking in a bullet
        else if (HatcuumController.hatcuumActive || ignoreDam)
        {
            // Create instance of event and override its cooldown in FMOD
            EventInstance iSuckBullet = RuntimeManager.CreateInstance(eSuckBullet);
            iSuckBullet.setProperty(EVENT_PROPERTY.COOLDOWN, suckBulletCooldown);

            // Reset Combo
            StopCoroutine(timer);           // Stop previous coroutine
            timer = HatcuumComboTimer();    // Create new coroutine
            StartCoroutine(timer);          // Start new coroutine

            // Set param in FMOD and activate cooldown before param can be set again
            if (numOfBulletsSucked < 5 && !suckCooldownActive)
            {
                StartCoroutine(HatcuumSuckCooldown());
                numOfBulletsSucked++;
                RuntimeManager.StudioSystem.setParameterByName("numSucked", numOfBulletsSucked);
            }

            // Play SuckBullet SFX
            iSuckBullet.start();
            iSuckBullet.release();
        }
    }


    public void Heal(int healingAmount) 
    {
        RuntimeManager.PlayOneShot(eHeal);
        healPS.Play();
        if (health < maxHealth) health = health + healingAmount;
        UpdateUI();

    }

    private void MaxHeal()
    {
        RuntimeManager.PlayOneShot(eMaxHeal);
        health = maxHealth;
        UpdateUI();
    }

    private void Death()
    {
        //Debug.Log("dead");
        RuntimeManager.PlayOneShot(eDead);
        anim.SetTrigger("isDead");

        this.GetComponent<Movement>().enabled = false;
        invulnerable = true;

        Invoke("TransitionDeath", 2.0f);
    }

    public void TransitionDeath()
    {
        transitionObject.SetActive(true);
        transitionAnim.SetTrigger("Dead");
    }

    private void LoadNewScene() 
    {
        GameManager.instance.currentLevel = SceneManager.GetActiveScene().buildIndex; // Updates which scene should be loaded by Retry button in Game Over scene
        SceneManager.LoadScene(3); // Loads Game Over scene
    }

    public int CurrentHealth()
    {
        return health;
    }

    private void UpdateText() 
    {
        debugText.text = "(Q) Invincible: " + invincible;
    }

    private void UpdateUI() 
    {
        bool onlowheath = false;

        // Updates the playerHealth parameter in FMOD
        RuntimeManager.StudioSystem.setParameterByName("playerHealth", health);

        if (!firstUpdate)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[1].transform.rotation = Quaternion.Euler(0, 0, -18.545f);
                if (health > i) { if (i > 0 && (i + 1) > previousHealth) hearts_anim[i].Play("ANI_HealHeart" + (hearts.Length - i)); }
                else if (i < previousHealth) hearts_anim[i].Play("ANI_DamageHeart" + (hearts.Length - i));
            }
        }
        else
        {
            firstUpdate = false;
        }

        switch (health)
        {
            case (1):
                onlowheath = true;
                break;
            default:
                onlowheath = false;
                break;
        }

        previousHealth = health;

        OnHealthChange?.Invoke(onlowheath);
        /*
        if (health == 1)
        {
            ppManager.LowHealthVignette(); // Enables the Vignette low health post processing effect
        }
        if (health <= 0) Death();
        */
    }

    public IEnumerator InvTime(float dur, bool flashing)   //i-frames so the player doesn't get hit by the same object twice
    {
        invulnerable = true;

        float invincibilityDuration = dur;
        float invincibilityFrames = dur / 10;

        yield return new WaitForSeconds(0.3f);

        for (float i = 0; i < invincibilityDuration; i += invincibilityFrames)
        {
            if (playermodel.activeSelf == true && flashing)
            {
                playermodel.SetActive(false);
                panmodel.SetActive(false);
            }
            else if (playermodel.activeSelf == false && flashing)
            {
                playermodel.SetActive(true);
                panmodel.SetActive(true);
            }

            yield return new WaitForSeconds(invincibilityFrames);
        }

        if (playermodel.activeSelf == false || panmodel.activeSelf)
        {
            playermodel.SetActive(true);
            panmodel.SetActive(true);
        }

        invulnerable = false;
    }

    private void RecivedInput() 
    {
        if (Holding.instance.Carrying == true) //eats held ingredient
        {
            if (health < maxHealth)
            {
                bool eaten = false;
                if (Holding.instance.heldObj.name == "Good Ingredient(Clone)")
                {
                    Heal(1);    //good ingredient
                    eaten = true;
                }
                else if (Holding.instance.heldObj.name == "Cooked Ingredient(Clone)")
                {
                    MaxHeal();  //cooked ingredient
                    eaten = true;
                }

                if (eaten)
                {
                    PopUp.instance.F.SetActive(false);
                    RuntimeManager.PlayOneShotAttached(eEatIngredient, gameObject);

                    Holding.instance.Carrying = false;
                    Holding.instance.heldObj.transform.parent = null;
                    Holding.instance.heldObj.gameObject.SetActive(false);
                    Holding.instance.heldObj = null;

                    IngredientInteraction.instance.UpdateHeldUI();
                }

            }
            if (Holding.instance.Carrying == true) //eats held ingredient
            {
                if (Holding.instance.heldObj.name == "Bad Ingredient(Clone)")
                {
                    if (!invincible)
                    {
                        RuntimeManager.PlayOneShot(eInjured);
                        health = health - 1;    //health is taken away here so the player still takes damage while they are invulnerable
                        if (health <= 0) Death();
                    }
                    UpdateUI();     //bad ingredient

                    PopUp.instance.F.SetActive(false);
                    RuntimeManager.PlayOneShotAttached(eEatIngredient, gameObject);

                    Holding.instance.Carrying = false;
                    Holding.instance.heldObj.transform.parent = null;
                    Holding.instance.heldObj.gameObject.SetActive(false);
                    Holding.instance.heldObj = null;
                }
            }
        }
    }

    #region Hatcuum Combo & Cooldown
    /// <summary>
    /// After hatcuumComboTimer seconds, the combo resets
    /// </summary>
    /// <returns></returns>
    private IEnumerator HatcuumComboTimer()
    {
        yield return new WaitForSeconds(hatcuumComboTimer);
        numOfBulletsSucked = 0;
        RuntimeManager.StudioSystem.setParameterByName("numSucked", numOfBulletsSucked);
    }

    /// <summary>
    /// Activates a cooldown before the FMOD param can be updated again i.e. param is only updated when the event plays
    /// This prevents the pitch increase going from 1-5 instantly rather than in steps
    /// </summary>
    /// <returns></returns>
    private IEnumerator HatcuumSuckCooldown()
    {
        suckCooldownActive = true;
        yield return new WaitForSeconds(suckBulletCooldown - (0.05f * suckBulletCooldown));    // Cooldown finishes just before event plays
        suckCooldownActive = false;
    }
    #endregion

    public void DashIFrames(float Iframestime) 
    {
        if (invulnerable) StopCoroutine(InvTime(Iframestime, false));
        StartCoroutine(InvTime(Iframestime, false));
    }
}
