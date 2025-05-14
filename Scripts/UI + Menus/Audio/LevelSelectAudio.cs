using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LevelSelectAudio : MonoBehaviour
{
    // Music
    [EventRef] [SerializeField] private string eSelectMusic = "";
    private EventInstance iSelectMusic;
    bool interactingWithPanel;

    // SFX
    [EventRef] private string eButtonHover = "{2237eebc-7e16-4e8f-b489-f17e53d2e804}";
    [EventRef] private string eButtonEnter = "{f7a6d735-0962-427d-ae71-9b6255b9307b}";
    [EventRef] private string eButtonReturn = "{053bc135-5f8a-4afd-929c-78e13f41352b}";
    [EventRef] private string eButtonConfirm = "{1c4b94b2-afe4-4df8-a156-bae4139f6e3e}";

    private void Start()
    {
        // Create music instance, set its position in the world space and start it
        if (eSelectMusic != "")
        {
            iSelectMusic = RuntimeManager.CreateInstance(eSelectMusic);
            iSelectMusic.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
            iSelectMusic.start();
        }
    }

    void PanelCheck(int phase)
    {
        switch (phase)
        {
            case (2):
                interactingWithPanel = true;
                break;

            default:
                interactingWithPanel = false;
                break;
        }
    }

    // Starts listening for onLevelSelectInteract event
    private void OnEnable()
    {
        LevelSelect.onLevelSelectInteract += FMODSetSelectPhase;
        LevelSelect.closeLevelSelectInteract += PanelCheck;
        LevelSelect.onLevelSelectInteract += PanelCheck;
    }

    // Stops listening when disabled
    private void OnDisable()
    {
        LevelSelect.onLevelSelectInteract -= FMODSetSelectPhase;
        LevelSelect.onLevelSelectInteract -= PanelCheck;
        LevelSelect.closeLevelSelectInteract -= PanelCheck;
    }

    // Assigns a new value to the lsPhase parameter in FMOD, making the music transition
    public void FMODSetSelectPhase(int phase)
    {
        if (eSelectMusic != "")
        {
            iSelectMusic.setParameterByName("lsPhase", phase);
        }
    }

    public void PlayHover()
    {
        if (interactingWithPanel) RuntimeManager.PlayOneShot(eButtonHover);
    }
    public void PlayEnter()
    {
        if (interactingWithPanel) RuntimeManager.PlayOneShot(eButtonEnter);
    }
    public void PlayReturn()
    {
        if (interactingWithPanel) RuntimeManager.PlayOneShot(eButtonReturn);
    }
    public void PlayConfirm()
    {
        if (interactingWithPanel) RuntimeManager.PlayOneShot(eButtonConfirm);
    }
}
