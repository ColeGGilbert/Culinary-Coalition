using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class HatcuumController : MonoBehaviour
{

    [SerializeField] Hatcuum settings;
    int chargesLeft;
    float cooldownLeft;
    float currentTime;
    public static bool hatcuumActive { get; set; }
    [SerializeField] ParticleSystem hatcuumPS;
    [SerializeField] TextMeshProUGUI chargesText;
    [SerializeField] TextMeshProUGUI cooldownText;
    [SerializeField] KeyCode activateKeybind;
    [SerializeField] float cameraStress;
    List<GameObject> leftoverBullets = new List<GameObject> { };
    private Player controls;

    public delegate void OnHatcuumActivated(float shakeAmount);
    public static event OnHatcuumActivated OnActive;

    Movement playerMovement;
    Health playerHealth;

    [Header("UI")]
    [SerializeField] private Image hatcuumIcon = null;
    [SerializeField] private Image hatcuumChargesLeft = null;
    [SerializeField] private Sprite[] hatcuumCharges = null;
    [SerializeField] private Color cooldownColor;

    [Header("FMOD")]
    [EventRef] private string eHatcuum = "{d6cebde0-d73b-46d5-8c79-09a0b8f8121e}";
    private EventInstance iHatcuum;
    [EventRef] private string eReady = "{3609fcae-b389-4d7c-b23e-44f72ccd51a4}";
    private bool hasReadyPlayed = true; // Prevent sound from playing on start

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Hatcuum.performed += ctx => ReceivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        chargesLeft = settings.numCharges;
        playerMovement = GetComponent<Movement>();
        playerHealth = GetComponent<Health>();

        SceneManager.sceneLoaded += SceneLoaded;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hatcuumActive)
        {
            OnActive?.Invoke(cameraStress);

            if (!hatcuumPS.isPlaying)
            {
                hatcuumPS.Play();
            }

            List<GameObject> nearbyBullets = new List<GameObject> { };

            foreach(string tag in settings.suctionTags)
            {
                nearbyBullets.AddRange(FindNearbyTag(tag));
            }

            foreach (GameObject bullet in nearbyBullets)
            {
                if (bullet.CompareTag("Good") && Holding.instance.heldObj == bullet)
                {
                    bullet.transform.localPosition = new Vector3(1, 0.5f, 0);
                    bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                else
                {
                    bullet.GetComponent<BulletMovement>().hatcuumSucked = true;
                    bullet.GetComponent<BulletMovement>().overrideMovement = true;
                    bullet.GetComponent<Rigidbody>().velocity = (transform.position - bullet.transform.position).normalized * settings.suctionSpeed;
                }
            }
        }
        else if (hatcuumPS.isPlaying)
        {
            hatcuumPS.Stop();
        }

        if (!hatcuumActive)
        {
            foreach (GameObject bullet in leftoverBullets)
            {
                if (bullet.CompareTag("Good") && Holding.instance.heldObj == bullet)
                {
                    bullet.transform.localPosition = new Vector3(1, 0.5f, 0);
                    bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                else
                {
                    bullet.GetComponent<BulletMovement>().hatcuumSucked = true;
                    bullet.GetComponent<BulletMovement>().overrideMovement = true;
                    bullet.GetComponent<Rigidbody>().velocity = (transform.position - bullet.transform.position).normalized * settings.suctionSpeed;
                }
            }
        }
    }

    List<GameObject> FindNearbyTag(string tag)
    {
        List<GameObject> nearbyObjs = new List<GameObject> { };

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(tag))
        {
            if (obj.activeSelf)
            {
                if (Vector3.Distance(obj.transform.position, transform.position) < settings.maxDist)
                {
                    nearbyObjs.Add(obj);
                }
            }
        }

        return nearbyObjs;
    }

    void Update()
    {
        if (Input.GetKeyDown(activateKeybind) && !PauseMenu.instance.paused && !DialogueSystem.instance.freeze)
        {
            ReceivedInput();
        }

        if (chargesLeft <= 0)
        {
            cooldownText.text = "";
        }
        else if (cooldownLeft > 0 && !hatcuumActive)
        {
            cooldownLeft -= Time.deltaTime;
            currentTime += Time.deltaTime;
            hatcuumIcon.fillAmount = currentTime / settings.cooldownDur;
            //cooldownText.text = "Hatcuum Cooldown: " + Mathf.RoundToInt(cooldownLeft);
        }
        else if (!hatcuumActive)
        {
            //cooldownText.text = "Hatcuum ready...";
            if (!hasReadyPlayed)
            {
                hasReadyPlayed = true;
                RuntimeManager.PlayOneShot(eReady);
                hatcuumIcon.color = Color.white;
                currentTime = 0;
            }
        }
        else if (hatcuumActive)
        {
            //cooldownText.text = "Hatcuum ACTIVE!";
        }
    }

    bool CooldownEnded()
    {
        return cooldownLeft <= 0;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hatcuumActive)
        {
            hatcuumActive = false;
        }
    }

    IEnumerator DeactiveHatcuum(float duration, float speed)
    {
        playerMovement.SetSpeed(settings.moveSpeed);
        yield return new WaitForSeconds(duration);
        playerHealth.InvTime(3.0f, false);
        playerMovement.SetSpeed(speed);
        hatcuumActive = false;

        //This foreach deals with bullets still being sucked towards the player upon hatcuum deactivation
        GameObject[] allBullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach(GameObject Bullet in allBullets)
        {
            if (Vector3.Distance(transform.position, Bullet.transform.position) < settings.maxDist)
            {
                leftoverBullets.Add(Bullet);
            }
        }

        iHatcuum.setParameterByName("hatcuumState", 1); // Deactivated
        iHatcuum.release();

        hasReadyPlayed = false; // Reset to allow Ready sound to play again once cooldown is finished

        yield return new WaitForSeconds(1.5f);
        leftoverBullets.Clear();
    }

    private void ReceivedInput() 
    {
        if (!hatcuumActive && chargesLeft > 0 && CooldownEnded())
        {
            chargesLeft--;
            cooldownLeft = settings.cooldownDur;
            if (hatcuumCharges[0] == null && chargesLeft == 0)
            {
                hatcuumChargesLeft.gameObject.SetActive(false);
            }
            else hatcuumChargesLeft.sprite = hatcuumCharges[chargesLeft];               
            iHatcuum = RuntimeManager.CreateInstance(eHatcuum);
            iHatcuum.setParameterByName("hatcuumState", 0); // Activated
            iHatcuum.start();
            hatcuumIcon.color = cooldownColor;
            hatcuumActive = true;
            float speedReset = playerMovement.GetSpeed();
            StartCoroutine(DeactiveHatcuum(settings.duration, speedReset));
        }
    }

    public void IncreaseCharges(int i)
    {
        if(chargesLeft < 3)
        {
            chargesLeft += i;
            hatcuumChargesLeft.sprite = hatcuumCharges[chargesLeft];
        }
    }

    public void DecreaseCharges(int i)
    {
        if(chargesLeft > 0)
        {
            chargesLeft -= i;
            hatcuumChargesLeft.sprite = hatcuumCharges[chargesLeft];
        }
    }
}
