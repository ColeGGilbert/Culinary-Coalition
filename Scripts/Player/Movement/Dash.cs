using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
//using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    public static Dash instance;
    [SerializeField] private float dashStress; //how much you want it to shake when damaged
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    [SerializeField] private float timeBetweenDash;
    private float currentTime;
    private bool canDash = true;

    ParticleSystem dashPS;
    ParticleSystem dashReadyPS;
    [SerializeField] Animation dashUIanim;
    public bool dashing = false;
    public Vector3 target;
    private CharacterController CC;
    private Player controls;
    private Animator anim;
    [SerializeField] private float invulnerableTime;
    private Health playerHealth;

    public delegate void CameraShake(float shakeAmount);
    public static event CameraShake OnCameraShake;

    [Header("UI")]
    [SerializeField] private Image dashIcon;
    [SerializeField] private Color cooldownColor;

    [Header("FMOD")]
    [EventRef] private string eDash = "{744e3da6-13d6-4617-80f3-352bf8d5a140}";
    [EventRef] private string eDashCharged = "{8be2097f-6f09-4bcb-826c-4fa4408fd1e8}";

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Dash.performed += ctx => PlayerDash();
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
        instance = this;
        currentTime = timeBetweenDash + 7;
        CC = GetComponent<CharacterController>();
        dashPS = GetComponentInChildren<ParticleSystem>();
        dashReadyPS = gameObject.transform.GetChild(3).GetComponent<ParticleSystem>();
        anim = GetComponentInChildren<Animator>();
        playerHealth = GetComponent<Health>();
        
    }

    private void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            //Debug.Log(currentTime);
            //Debug.Log(canDash);
            if (!canDash) Cooldown();
            if (Input.GetKeyDown(KeyCode.LeftShift)) PlayerDash();
            //Debug.Log(movement.instance.playerVelocity);
            if (dashing == true)
            {
                //Debug.Log("dashing");
                CC.SimpleMove(Movement.instance.playerVelocity * dashSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!canDash)
        {
            UpdateUI();     //needs to not be updated every frame
        }
    }

    private void PlayerDash()
    {
        if (Mathf.Abs(Movement.instance.playerVelocity.magnitude) > 0 && canDash)
        {
            dashing = true;
            anim.SetTrigger("isDash");
            StartCoroutine(dashTime());
            RuntimeManager.PlayOneShotAttached(eDash, gameObject);
            OnCameraShake?.Invoke(dashStress);        //unity event that starts the InduceStress in the ShakeableTransform script
            dashPS.Play();
            currentTime = 0;
            dashIcon.color = cooldownColor;
            canDash = false;
            playerHealth.DashIFrames(invulnerableTime);     //makes the player invulnerable while dashing
            //playerHealth.InvTime(invulnerableTime, false); this doesn't work and i don't know why as it works in hatcuum controller >:(
        }
    }

    /// <summary>
    /// Counts down once the player has dashed,
    /// Once it reaches 0 it allowes the player to dash again.
    /// </summary>
    private void Cooldown() 
    {
        if (currentTime >= timeBetweenDash)
        {
            canDash = true;
            RuntimeManager.PlayOneShot(eDashCharged);
            dashIcon.color = Color.white;
            dashReadyPS.Play(); //particle effect
            dashUIanim.Play(); //UI animation for ready dash

        }
        else currentTime = currentTime + Time.deltaTime;
    }

    private void UpdateUI()         //updates dash slider ui
    {
        dashIcon.fillAmount = currentTime / timeBetweenDash;
    }

    IEnumerator dashTime()
    {
        yield return new WaitForSeconds(dashLength);
        dashing = false;
    }
}
