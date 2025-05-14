using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioSettings : MonoBehaviour
{
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private void Awake()
    {
        // Adds OnSceneUnloaded function to sceneUnloaded
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        // Gets the buses from FMOD and assigns them to the Bus variables
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        DontDestroyOnLoad(gameObject);
    }

    // Listen for when game freezes during dialogue, then pause SFX
    private void OnEnable()
    {
        DialogueSystem.onDialogueFreeze += SetSFXPaused;
    }

    private void OnDisable()
    {
        DialogueSystem.onDialogueFreeze -= SetSFXPaused;
    }

    public void StopAllEvents()
    {
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    // Pauses/unpauses SFX
    public void SetSFXPaused(bool paused)
    {
        sfxBus.setPaused(paused);
    }
    // Pauses/unpauses Music
    public void SetMusicPaused(bool paused)
    {
        musicBus.setPaused(paused);
    }

    private void OnSceneUnloaded(Scene current)
    {
        StopAllEvents();
    }
}
