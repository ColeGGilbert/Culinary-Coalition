using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AnimFlightEndCheck : MonoBehaviour
{
    public bool inFlight;
    public bool removeTree;
    [SerializeField] BulletSpawning bulletSpawner;
    bool inCoroutine;

    [Header("FMOD")]
    [EventRef] private string eBananaThrow = "{7bbf507e-50ea-4df0-8e14-87ffabb354e0}";
    private EventInstance iBananaThrow;

    private void Update()
    {
        iBananaThrow.setParameterByName("bananaRotation", transform.rotation.y);
        if (inFlight && !inCoroutine)
        {
            StartCoroutine(Flying());
        }
        if (removeTree)
        {
            bulletSpawner.RemoveTree();
            removeTree = false;
        }
    }

    IEnumerator Flying()
    {
        iBananaThrow = RuntimeManager.CreateInstance(eBananaThrow);
        RuntimeManager.AttachInstanceToGameObject(iBananaThrow, transform, GetComponent<Rigidbody>());
        iBananaThrow.start();
        while (inFlight)
        {
            bulletSpawner.bananaInFlight = true;
            inCoroutine = true;
            yield return new WaitForSeconds(.1f);
        }
        iBananaThrow.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        iBananaThrow.release();
        inCoroutine = false;
        bulletSpawner.bananaFlightEnded = true;
        bulletSpawner.bananaInFlight = false;
    }
}
