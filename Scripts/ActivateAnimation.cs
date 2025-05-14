using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimation : MonoBehaviour
{

    [SerializeField] float stressAmount;
    public delegate void TextParticle(float stress);
    public static event TextParticle onPlay;

    private void OnEnable()
    {
        GetComponent<ParticleSystem>().Play();
        onPlay?.Invoke(stressAmount);
    }
}
