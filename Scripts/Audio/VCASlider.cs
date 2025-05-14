using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class VCASlider : MonoBehaviour
{
    [Tooltip("Needs to match the name of the VCA in FMOD")]
    [SerializeField]
    private string vcaName = "";

    [SerializeField]
    private Slider slider = null;

    private VCA vca;

    private void Start()
    {
        /*
        vca = RuntimeManager.GetVCA($"vca:/{vcaName}");

        // Initialise volume
        float defaultVol = PlayerPrefs.GetFloat($"vca:/{vcaName}", slider.value);
        Debug.Log($"vca:/{vcaName}");
        slider.value = defaultVol;
        SetVolume(defaultVol);
        */
    }

    public void SetVolume(float value)
    {
        vca.setVolume(value);
        PlayerPrefs.SetFloat($"vca:/{vcaName}", value);
    }

    public string GetName()
    {
        return vcaName;
    }
    public void SetName(string value)
    {
        vcaName = value;
    }

    public Slider GetSlider()
    {
        return slider;
    }
    public void SetSlider(Slider value)
    {
        slider = value;
    }

    public VCA GetVCA()
    {
        return vca;
    }
    public void SetVCA(VCA value)
    {
        vca = value;
    }
}