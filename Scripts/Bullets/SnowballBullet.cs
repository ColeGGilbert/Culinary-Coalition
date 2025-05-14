using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SnowballBullet : BulletMovement
{
    //variables passed in from BulletSpawning
    public BulletSpawning spawner;
    public Bullet bul;
    public float bulletCountModifier;
    public int bulletAmount;

    private Rigidbody rb;   //needed for the collision function
    private bool growing;   //if the bullets are growing bigger
    private float maxSize = 1.5f;   //set to this cause bullets die if they are over the scale of 1.5, don't know why they do this
    private float sizeModifer = 1.004f;  //the size that the bullets get bigger at

    // FMOD
    private string eSnowballCharge = "{e675d609-f9a0-4ffe-b4f2-a8c931258fa4}";
    private EventInstance iSnowballCharge;
    private string eSnowballRelease = "{612e1472-b7e7-484b-9616-4cb6f9f291da}";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(WaitTime(1));    //starts the growing function after a delay
        PlaySound();
    }

    // Update is called once per frame
    void Update()
    {
        if (growing)    //grows the bullet if true
        {
            Grow();
        }
    }

    public override void OnTriggerEnter(Collider other)     //overrided version from BulletMovement, used to make the bullet split when they collide with another object
    {
        if (!other.CompareTag("Ignore") && !other.CompareTag("Table") && !other.CompareTag("Oven") && !other.CompareTag("Bin") && !other.CompareTag("Bullet") && !other.CompareTag("Magnet") && !other.CompareTag("Bad") && !other.CompareTag("Good") && !other.CompareTag("Cooked"))
        {
            if (other.CompareTag("Player") && !isIng && !hatcuumSucked)
            {
                other.GetComponent<Health>().Damage(dam, false);
            }
            else if (other.CompareTag("Player") && !isIng && hatcuumSucked)
            {
                other.GetComponent<Health>().Damage(dam, true);
            }
            else if (other.CompareTag("Player") && isIng)
            {
                // Sets ingredient velocity to 0 so that it doesn't fly away from the plate
                rb.velocity = Vector3.zero;
            }

            if (!isIng)
            {
                Split();    //spliting happens when the bullet hits another object
            }
            else
            {
                if (!other.GetComponent<BulletMovement>() && !other.CompareTag("Player") && (rb.velocity.magnitude > Vector3.zero.magnitude || isSnowfall))
                {
                    Split();    //spliting happens when the bullet hits another object
                }
            }
            StopSound();
            Destroy(this);
        }
    }

    /// <summary>
    /// Splits the bullets into smaller bullets on collision
    /// </summary>
    private void Split()
    {
        RuntimeManager.PlayOneShot(eSnowballRelease, transform.position);

        int totalBullets = Mathf.RoundToInt(bulletAmount * bulletCountModifier);
        float angleDif = 360 / totalBullets;    //sometimes totalBullets is equal to 0 and causes the script to error
        float startAngleX = 0;
        float startAngleZ = 0;
        for (int i = 0; i < totalBullets; i++)
        {
            float newAngX = startAngleX + (angleDif * i);
            float newAngZ = startAngleZ + (angleDif * i);
            Vector3 newDir = new Vector3(Mathf.Sin(newAngX * Mathf.Deg2Rad), 0, Mathf.Cos(newAngZ * Mathf.Deg2Rad));
            spawner.DecideBullet(newDir, i, bul, totalBullets, transform.position);
        }

        Destroy(this);
        gameObject.SetActive(false);    //disabled the original bullet
    }

    public override void OnDisable()
    {
        StopSound();
        Destroy(this);  //turns of the script when the bullet gets disabled
    }

    /// <summary>
    /// Makes the bullets larger
    /// </summary>
    private void Grow() 
    {
        if (this.transform.localScale.x >= maxSize)
        {
            growing = false;
        }
        else 
        {
            this.transform.localScale *= 1 + ((sizeModifer-1) * ((sizeModifer * Time.deltaTime) * 105));
        }
    }

    /// <summary>
    /// Time until the bullets get larger
    /// </summary>
    /// <param name="timeuntilgrow"></param>
    /// <returns></returns>
    private IEnumerator WaitTime(float timeuntilgrow) 
    {
        yield return new WaitForSeconds(timeuntilgrow);
        growing = true;
    }

    /// <summary>
    /// Plays charge sound attached to the snowball
    /// </summary>
    private void PlaySound()
    {
        iSnowballCharge = RuntimeManager.CreateInstance(eSnowballCharge);
        RuntimeManager.AttachInstanceToGameObject(iSnowballCharge, transform, rb);
        iSnowballCharge.start();
    }

    /// <summary>
    /// Stops charge sound
    /// </summary>
    private void StopSound()
    {
        iSnowballCharge.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        iSnowballCharge.release();
    }
}
