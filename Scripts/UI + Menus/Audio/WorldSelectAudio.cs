using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSelectAudio : MonoBehaviour
{
    [SerializeField]
    private FMODEvent defaultSound = null;

    private FMODEvent interactSound = null;

    private bool interacting = false;

    private void Start()
    {
        interactSound = defaultSound;
    }

    public void SetSound(FMODEvent sound)
    {
        interactSound = sound;
    }

    public void SetInteracting(bool value)
    {
        interacting = value;
    }

    public void Play()
    {
        if (interacting)
        {
            interactSound.Play();
        }
    }
}
