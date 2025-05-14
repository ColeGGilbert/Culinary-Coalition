using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TornadoBullet : MonoBehaviour
{

	public string[] Tags;

	[SerializeField] List<GameObject> hitObjects = new List<GameObject> { };

    public float maxDistance = 5f;

    public float spiralPower = 5f;

    public float attractionPower = 5f;

    public float timeToWait = .25f;

    [Tooltip("Before explosion")] public float lifetime = 5f;

    bool doSuck;

    private Animator Tornado;

    // FMOD
    private string eTornadoCharge = "{f5293cb0-cd09-4dd3-9322-3294b35b9c2a}";
    private EventInstance iTornadoCharge;
    private string eTornadoRelease = "{d7e17a7c-2c1c-4d41-b317-996a25af80de}";

    public void Initiate()
    {
        StartCoroutine(ReleaseSuckedInObjects());
        Tornado = GetComponent<Animator>();
        Invoke("AnimEnter", .025f);
        Invoke("DelaySuckStart", timeToWait);
        if(FindObjectsOfType<TornadoBullet>().Length > 1)
        {
            Destroy(this);
        }
       
        PlaySound();
    }

    // Update is called once per frame
    void Update()
    {
        if (doSuck)
        {
            SearchForObjects();
            MoveObjects();
        }
    }

    private void OnDisable()
    {
        StopSound();
        Destroy(this);
    }

    void DelaySuckStart()
    {
        doSuck = true;
    }

    void SearchForObjects()
	{
		// Clear the list of objects that were hit by raycasts to ensure objects are still in the effect range
		hitObjects.Clear();

		List<GameObject> taggedObjects = new List<GameObject> { };

		foreach(string tag in Tags)
        {
			taggedObjects.AddRange(GameObject.FindGameObjectsWithTag(tag));
		}

        foreach(GameObject obj in taggedObjects)
        {
            if(Vector3.Distance(obj.transform.position, transform.position) < maxDistance)
            {
                hitObjects.Add(obj);
            }
        }

        //Debug.Log(taggedObjects.Count + " in taggedObjects");
        //Debug.Log(hitObjects.Count + " in range");
    }

	void MoveObjects()
    {
        foreach (GameObject hit in hitObjects)
        {
            Vector3 pullDir = (transform.position - hit.transform.position);
            pullDir = new Vector3(pullDir.x, 0, pullDir.z);

            TrailRenderer trail = hit.gameObject.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.enabled = true;
            }

            // Checking for a player object to work with gameplay demos
            if (!hit.CompareTag("Player"))
            {
                hit.GetComponent<BulletMovement>().isSnowfall = false;
                hit.GetComponent<BulletMovement>().returnbullet = false;
                Vector3 newForce = ((pullDir * attractionPower) + (new Vector3(pullDir.z, 0, -pullDir.x)) * (spiralPower * Random.Range(0.75f, 1.25f))) * (1f / Vector3.Distance(transform.position, hit.transform.position)) * Time.deltaTime;
                if(Equals(newForce, newForce))
                {
                    hit.GetComponent<Rigidbody>().velocity = newForce;
                }
            }
            else
            {
                hit.GetComponent<Movement>().additionalForce = pullDir * attractionPower * (.15f / Vector3.Distance(transform.position, hit.transform.position)) * Time.deltaTime;
            }
        }
    }

    void FireObjects()
    {
        StopSound();
        RuntimeManager.PlayOneShot(eTornadoRelease, transform.position);

        foreach (GameObject hit in hitObjects)
        {
            // Checking for a player object to work with gameplay demos
            if (!hit.CompareTag("Player"))
            {
                hit.GetComponent<Rigidbody>().velocity *= 1.25f;
            }
        }
    }

    IEnumerator ReleaseSuckedInObjects()
    {
        yield return new WaitForSeconds(lifetime);
        Tornado.SetTrigger("Exit");
        GameManager.instance.DelayInactive(1.2f,gameObject);
    }

    public void AnimEnter()
    {
        Tornado.SetTrigger("Enter");
    }

    public void AnimExit()
    {
        Tornado.SetTrigger("Exit");
        doSuck = false;
        FireObjects();
    }

    public void PlaySound()
    {
        iTornadoCharge = RuntimeManager.CreateInstance(eTornadoCharge);
        RuntimeManager.AttachInstanceToGameObject(iTornadoCharge, transform, GetComponent<Rigidbody>());
        iTornadoCharge.start();
    }

    public void StopSound()
    {
        iTornadoCharge.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        iTornadoCharge.release();
    }
}
