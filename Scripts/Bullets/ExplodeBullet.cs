using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ExplodeBullet : MonoBehaviour
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

    [Header("FMOD")]
    [EventRef] private string eOmegaExplode = "{8318ca67-52b7-4dfa-a22d-a089363e6400}";

    // Start is called before the first frame update
    void Start()
    {
        changingColour = true;
        cubeRenderer = gameObject.GetComponent<Renderer>();
    }

    void OnDisable()
    {
        // Removes the explode script and stops any active explosions so that this object can be recycled into another bullet
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
            spawner.DecideBullet(newDir, i, bul, totalBullets, transform.position);
        }
        gameObject.SetActive(false);
    }

    private void ChangeColour() 
    {
        float gradient = -(gMax / maxTime);

        if (currentTime >= maxTime) //runs when the timer is complete, should be 0 by this point
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
}
