using UnityEngine;
using System.Collections;
using UnityEditor;

public class Movement : MonoBehaviour
{
    private CharacterController CC;
    [SerializeField] private float speed;
    public Vector3 playerVelocity;
    public Vector3 additionalForce;
    private Animator anim;
    private float currentTime = 0;
    [SerializeField] private float timeToPlayIdle;
    private float vertical;
    private float horizontal;

    public static Movement instance;

    [SerializeField] ParticleSystem movementPS;

    void Start()
    {
        instance = this;
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update() 
    {
        //Debug.Log(playerVelocity);
        //Debug.Log(Speed);
        if (Dash.instance.dashing == false)
        {
            PlayerMovement();
            
        }
    }

    /// <summary>
    /// Handles player movement inputs for vertical and horizontal movement and calculates velocity and rotation as a result
    /// </summary>
    private void PlayerMovement() 
    {
        vertical = Input.GetAxis("Vertical");       //gets w/s input
        horizontal = Input.GetAxis("Horizontal");       //gets a/d input

        float yRotation = CalculateRotation(Mathf.RoundToInt(vertical), Mathf.RoundToInt(horizontal));

        IdleTracker();

        if (vertical != 0 || horizontal != 0)
        {
            anim.SetBool("isRun", true);
        }
        else
        {
            anim.SetBool("isRun", false);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, yRotation, transform.rotation.z), 0.1f);

        playerVelocity = new Vector3(horizontal, 0, vertical);
        playerVelocity = Vector3.ClampMagnitude(playerVelocity, 1.2f) + additionalForce;      //slows down diagonal movement
        additionalForce = Vector3.zero;

        CC.SimpleMove(playerVelocity * speed);
        if(MoveCheck())
        {
            if(movementPS != null)
            {
                if (!movementPS.isPlaying)
                {
                    movementPS.Play();
                }
            }
        }
        else
        {
            if(movementPS != null)
            {
                movementPS.Stop();
            }
        }
        
    }

    private bool MoveCheck()
    {
        return playerVelocity.magnitude > Vector3.zero.magnitude;
    }

    /// <summary>
    /// Takes the current player movement inputs and returns the desired y rotation as a float
    /// </summary>
    /// <param name="vert"></param>
    /// <param name="hori"></param>
    /// <returns></returns>
    private float CalculateRotation(int vert, int hori)
    {
        float rot = transform.rotation.eulerAngles.y;

        switch (vert)
        {
            case (1):
                switch (hori)
                {
                    case (1):
                        rot = 45;
                        break;

                    case (0):
                        rot = 0;
                        break;

                    case (-1):
                        rot = 315;
                        break;
                }
                break;

            case (-1):
                switch (hori)
                {
                    case (1):
                        rot = 135;
                        break;

                    case (0):
                        rot = 180;
                        break;

                    case (-1):
                        rot = 225;
                        break;
                }
                break;

            case (0):
                switch (hori)
                {
                    case (1):
                        rot = 90;
                        break;

                    case (-1):
                        rot = -90;
                        break;
                }
                break;
        }

        return rot;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public float GetSpeed()
    {
        return speed;
    }

    private void IdleTracker() 
    {
        if (vertical == 0 && horizontal == 0)
        {
            if (currentTime < timeToPlayIdle)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                anim.SetTrigger("isIdle");
                currentTime = 0;
            }
        }
        else 
        {
            currentTime = 0;
        }
    }
}