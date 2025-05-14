using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ReverseExplodeBullet : MonoBehaviour
{
    public float bulletCountModifier;
    public int bulletAmount;
    public BulletSpawning spawner;
    public Bullet bul;

    Renderer cubeRenderer;
    private bool changingColour;
    private float currentTime = 0;
    private float maxTime = 2.5f;
    private float g = 0;
    private float gMax = 1;

    private MeshRenderer mesh;
    private SphereCollider cole;
    private Rigidbody rb;
    private BulletMovement bm;

    [Header("FMOD")]
    private string eOmegaExplode = "{8318ca67-52b7-4dfa-a22d-a089363e6400}";
    private string eOmegaPreReverse = "{fc7d750a-8ce7-410d-bbaa-779d6226fcbb}";
    private string eOmegaForm = "{3d62e732-466b-4247-9f7d-72ca0640e111}";

    // Start is called before the first frame update
    void Start()
    {
        changingColour = true;
        cubeRenderer = gameObject.GetComponent<Renderer>();

        mesh = GetComponent<MeshRenderer>();
        cole = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        bm = GetComponent<BulletMovement>();
    }

    void OnDisable()
    {
        // Removes the explode script and stops any active explosions so that this object can be recycled into another bullet
        StopAllCoroutines();
        Destroy(this);
    }

    private void Update()
    {
        if (changingColour) ChangeColour();
    }

    /// <summary>
    /// After a delay, the omega bullet will explode, creating a large explosion of smaller bullets in an area around it
    /// </summary>
    /// <returns></returns>
    private void Explode()
    {
        RuntimeManager.PlayOneShot(eOmegaExplode, transform.position);
        int totalBullets = Mathf.RoundToInt(bulletAmount * bulletCountModifier);
        float angleDif = 360 / totalBullets;
        float startAngleX = 0;
        float startAngleZ = 0;
        for (int i = 0; i < totalBullets; i++)
        {
            float newAngX = startAngleX + (angleDif * i);
            float newAngZ = startAngleZ + (angleDif * i);
            Vector3 newDir = new Vector3(Mathf.Sin(newAngX * Mathf.Deg2Rad), 0, Mathf.Cos(newAngZ * Mathf.Deg2Rad));
            GameObject bullet = spawner.DecideBullet(newDir, i, bul, totalBullets, transform.position);
            bullet.GetComponent<BulletMovement>().returnbullet = true;      //tells the spawned bullets to return back to were they where spawned
        }

        StartCoroutine(Return());
    }

    private void ChangeColour() 
    {
        float gradient = -(gMax / maxTime);

        if (currentTime >= maxTime) //runs when the timer is complete,g should be 0 by this point
        {
            changingColour = false;
            Explode();
        }
        else 
        {
            currentTime += Time.deltaTime;
            g = (gradient * currentTime) + gMax;
            cubeRenderer.material.color = new Color(1, g, 0, 1);
        }
    }

    private IEnumerator Return() 
    {
        RuntimeManager.PlayOneShot(eOmegaPreReverse, transform.position);
        Vector3 currentVelocity = rb.velocity;
        //disables the omega bullet until the smaller bullets regroup
        mesh.enabled = false;
        cole.enabled = false;
        rb.velocity = Vector3.zero;
        bm.StopAllCoroutines();
        yield return new WaitForSeconds(bul.lifespan * 2);
        //tells the omega bullet to continue its path
        RuntimeManager.PlayOneShot(eOmegaForm, transform.position);
        mesh.enabled = true;
        cole.enabled = true;
        rb.velocity = currentVelocity;
        yield return new WaitForSeconds(bm.lifetime / 2);  //to stop the bullet living forever
        gameObject.SetActive(false);
    }
}
