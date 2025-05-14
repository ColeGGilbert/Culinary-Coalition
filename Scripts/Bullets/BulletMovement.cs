using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BulletMovement : MonoBehaviour, IPooledObject
{

    Rigidbody rb;
    public float speed;
    public Vector3 dir;
    public int dam;
    public float lifetime;
    public bool isIng;
    public bool hatcuumSucked;
    public bool returnbullet;
    public bool isSnowfall;
    public bool overrideMovement;

    ObjectPooler objectPool;

    List<BezierCurve> snowfallCurve;
    public float snowfallPower = -.025f;
    float currentProgress;

    // FMOD
    private string eSnowfall = "{6bf94070-ceb8-4fbb-a303-293d37ed167d}";
    private string eReverse = "{f86ae7eb-43e4-4190-a25a-dea71d178aa9}";

    /// <summary>
    /// This function is called when the object pooling system recycles this object
    /// The function will reset all bullet values and variables to default
    /// </summary>
    public void OnObjectSpawn()
    {
        transform.parent = null;
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir * speed * Time.fixedDeltaTime;

        if (isIng)
        {
            if (!GetComponent<PickUp>())
            {
                gameObject.AddComponent<PickUp>().enabled = true;
            }
        }
        StartCoroutine(DieTime());
    }

    public virtual void OnDisable()
    {
        if (!gameObject.CompareTag("Bullet") && !isIng)
        {
            gameObject.tag = "Bullet";
        }
    }

    private void Start()
    {
        objectPool = ObjectPooler.Instance;

        if(isSnowfall)
        {
            snowfallCurve = GenerateCurves();
            rb.velocity = Vector3.zero;
        }
    }

    private List<BezierCurve> GenerateCurves()
    {
        List<BezierCurve> tempCurves = new List<BezierCurve> { };

        Vector2 pos = new Vector2(transform.position.x, transform.position.z);

        tempCurves.Add(new BezierCurve(pos, new Vector2(pos.x + (50 * snowfallPower), pos.y + (100 * snowfallPower)), new Vector2(pos.x + (100 * snowfallPower), pos.y + (100 * snowfallPower)), new Vector2(pos.x + (150 * snowfallPower), pos.y + (75 * snowfallPower))));

        pos = new Vector2(pos.x + (150 * snowfallPower), pos.y + (75 * snowfallPower));

        for (int i = 0; i < 6; i++)
        {
            tempCurves.Add(new BezierCurve(pos, new Vector2(pos.x - (100 * snowfallPower), pos.y + (150 * snowfallPower)), new Vector2(pos.x - (150 * snowfallPower), pos.y + (150 * snowfallPower)), new Vector2(pos.x - (250 * snowfallPower), pos.y + (100 * snowfallPower))));
            pos = new Vector2(pos.x - (250 * snowfallPower), pos.y + (100 * snowfallPower));
            tempCurves.Add(new BezierCurve(pos, new Vector2(pos.x + (100 * snowfallPower), pos.y + (150 * snowfallPower)), new Vector2(pos.x + (150 * snowfallPower), pos.y + (150 * snowfallPower)), new Vector2(pos.x + (259 * snowfallPower), pos.y + (100 * snowfallPower))));
            pos = new Vector2(pos.x + (250 * snowfallPower), pos.y + (100 * snowfallPower));
        }

        return tempCurves;
    }

    private void Update()
    {
        if (isSnowfall && !overrideMovement)
        {
            if(currentProgress > snowfallCurve.Count - 1)
            {
                gameObject.SetActive(false);
            }

            int curCurve = Mathf.FloorToInt(currentProgress); float posOnCurve = currentProgress % 1;

            // Play sound right before starting new curve
            if (posOnCurve > 0.97f)
            {
                RuntimeManager.PlayOneShot(eSnowfall);
            }

            //Debug.Log(curCurve + " " + posOnCurve);
            Vector2 drawPos = snowfallCurve[curCurve].ReturnCurve(posOnCurve);

            Vector3 moveTo = new Vector3(drawPos.x, transform.position.y, drawPos.y);
            transform.position = Vector3.Lerp(transform.position, moveTo, .1f);

            currentProgress += .0001f * speed * Time.deltaTime * 75;
        }
    }

    // Basic checks to see if the player should take damage or not
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ignore") && !other.CompareTag("Table") && !other.CompareTag("Oven") && !other.CompareTag("Bin") && !other.CompareTag("Bullet") && !other.CompareTag("Magnet") && !other.CompareTag("Bad") && !other.CompareTag("Good") && !other.CompareTag("Cooked"))
        {
            if (other.CompareTag("Player") && !isIng && !hatcuumSucked)
            {
                other.GetComponent<Health>().Damage(dam, false);
            }
            else if(other.CompareTag("Player") && !isIng && hatcuumSucked)
            {
                other.GetComponent<Health>().Damage(dam, true);
            }
            else if(other.CompareTag("Player") && isIng)
            {
                // Sets ingredient velocity to 0 so that it doesn't fly away from the plate
                rb.velocity = Vector3.zero;
            }

            if (!isIng)
            {
                GameObject impact_pfx = objectPool.SpawnNonBulletFromPool("BulletImpact_PFX", transform.position, Quaternion.identity);
                impact_pfx.GetComponent<ParticleSystem>().Play();
                GameManager.instance.DelayInactive(1.1f, impact_pfx);
                gameObject.SetActive(false);
            }
            else
            {
                if (!other.GetComponent<BulletMovement>() && !other.CompareTag("Player") && (rb.velocity.magnitude > Vector3.zero.magnitude || isSnowfall))
                {
                    if (transform.CompareTag("Bad"))
                    {
                        GameObject impact_pfx = objectPool.SpawnNonBulletFromPool("BadIngImpact_PFX", transform.position, Quaternion.identity);
                        impact_pfx.GetComponent<ParticleSystem>().Play();
                        GameManager.instance.DelayInactive(1.1f, impact_pfx);
                    }
                    else
                    {
                        GameObject impact_pfx = objectPool.SpawnNonBulletFromPool("IngImpact_PFX", transform.position, Quaternion.identity);
                        impact_pfx.GetComponent<ParticleSystem>().Play();
                        GameManager.instance.DelayInactive(1.1f, impact_pfx);
                    }
                    gameObject.SetActive(false);
                }
            }

            Destroy(this);
        }
    }

    IEnumerator DieTime()
    {
        // Lifetime for the bullet
        yield return new WaitForSeconds(lifetime);
        if (returnbullet)
        {
            RuntimeManager.PlayOneShot(eReverse, transform.position);
            rb.velocity = -rb.velocity;
            returnbullet = false;
            StartCoroutine(DieTime());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
