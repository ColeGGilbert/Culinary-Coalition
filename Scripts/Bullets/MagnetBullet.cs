using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MagnetBullet : BulletMovement
{
    public bool homingBullet;
    public float magnetLifespan;
    public BulletSpawning spawner;
    public Bullet bul;
    public GameObject sisterMagnet;
    GameObject player;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(DieTime());
    }

    public override void OnDisable()
    {
        base.OnDisable();

        // Removes the magnet script and stops any active explosions so that this object can be recycled into another bullet
        StopAllCoroutines();
        if (!gameObject.GetComponent<BulletMovement>().enabled)
        {
            gameObject.GetComponent<BulletMovement>().enabled = true;
        }
        Destroy(this);
    }

    public void CombineMagnets(GameObject other)
    {
        GameObject homingBullet = spawner.DecideBullet(new Vector3(0, 0, 0), 0, bul, 1, transform.position);
        MagnetBullet homing = homingBullet.AddComponent<MagnetBullet>();
        homing.homingBullet = true;
        homing.bul = bul;
        homing.isIng = homingBullet.GetComponent<BulletMovement>().isIng;
        homingBullet.GetComponent<BulletMovement>().enabled = false;
        other.tag = "Bullet";
        gameObject.tag = "Bullet";
        other.SetActive(false);
        gameObject.SetActive(false);
        Destroy(this);
    }

    public void Update()
    {
        if (homingBullet)
        {
            float newDirX = player.transform.position.x - transform.position.x;
            float newDirZ = player.transform.position.z - transform.position.z;
            Vector3 newDir = new Vector3(newDirX, 0, newDirZ).normalized * 1.4f;
            rb.velocity = newDir * bul.speed * Time.fixedDeltaTime;
        }
        else
        {
            if(Vector3.Distance(gameObject.transform.position, sisterMagnet.transform.position) < 1)
            {
                CombineMagnets(sisterMagnet);
            }
        }
    }

    // Basic checks to see if the player should take damage or not
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (homingBullet)
        {
            if (!other.CompareTag("Ignore") && !other.CompareTag("Table") && !other.CompareTag("Oven") && !other.CompareTag("Bin") && !other.CompareTag("Bullet") && !other.CompareTag("Magnet") && !other.CompareTag("Bad") && !other.CompareTag("Good") && !other.CompareTag("Cooked"))
            {
                if (other.CompareTag("Player") && !isIng && !hatcuumSucked)
                {
                    other.GetComponent<Health>().Damage(bul.damage, false);
                }
                else if (other.CompareTag("Player") && !isIng && hatcuumSucked)
                {
                    other.GetComponent<Health>().Damage(bul.damage, true);
                }
                else if (other.CompareTag("Player") && isIng)
                {
                    // Sets ingredient velocity to 0 so that it doesn't fly away from the plate
                    rb.velocity = Vector3.zero;
                }

                if (!isIng)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    if (!other.GetComponent<BulletMovement>() && !other.CompareTag("Player") && rb.velocity.magnitude > Vector3.zero.magnitude)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    IEnumerator DieTime()
    {
        if (homingBullet)
        {
            // Lifetime for the bullet
            yield return new WaitForSeconds(bul.lifespan);
        }
        else
        {
            yield return new WaitForSeconds(magnetLifespan);
        }
        gameObject.SetActive(false);
    }
}
