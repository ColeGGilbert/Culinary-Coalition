using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class VCAInitialiser : MonoBehaviour
{
    [SerializeField]
    private VCASlider[] sliders = null;

    private void Start()
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            float defaultVolume = PlayerPrefs.GetFloat($"vca:/{sliders[i].GetName()}", sliders[i].GetSlider().value);
            sliders[i].SetVCA(RuntimeManager.GetVCA($"vca:/{sliders[i].GetName()}"));
            sliders[i].SetVolume(defaultVolume);
            sliders[i].GetSlider().value = defaultVolume;
        }
    }
}