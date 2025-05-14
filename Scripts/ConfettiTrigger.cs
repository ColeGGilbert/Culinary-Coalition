using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private ParticleSystem[] PFX_Confetti;

    void TriggerConfetti()
    {
        
        foreach (ParticleSystem confetti in PFX_Confetti)
        {
            confetti.Play();
        }
    }

    private void OnEnable()
    {
        IngGameController.OnServedCombo += TriggerConfetti;
    }

    private void OnDisable()
    {
        IngGameController.OnServedCombo -= TriggerConfetti;
    }
}
