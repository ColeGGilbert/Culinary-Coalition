using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcManager : MonoBehaviour
{
    public Volume volume;
    public Vignette vignette;
    [SerializeField] private float maxIntensity = 0.5f;     //the intensity it goes to when on low health
    [SerializeField] private float defultIntensity = 0;
    public bool uppingVignette;
    public bool loweringVignette;

    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out vignette);
        vignette.intensity.value = 0;
        
    }

    void Update()
    {
        if (uppingVignette) IncreaseVignette();
        else if (loweringVignette) LowerVignette();
    }

     public void LowHealthVignette(bool onLowHeath)
    {
        if (onLowHeath) uppingVignette = true;
        else loweringVignette = true;
    }

     void IncreaseVignette() 
    {
        vignette.intensity.value += Time.deltaTime;

        if (vignette.intensity.value >= maxIntensity)
        {
            uppingVignette = false;
        }
    }

     void LowerVignette()
    {
        vignette.intensity.value -= Time.deltaTime;

        if (vignette.intensity.value <= defultIntensity)
        {
            loweringVignette = false;
        }
    }

    private void OnEnable()
    {
        Health.OnHealthChange += LowHealthVignette;
    }

    private void OnDisable()
    {
        Health.OnHealthChange -= LowHealthVignette;
    }
}
