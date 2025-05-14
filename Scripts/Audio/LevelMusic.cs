using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LevelMusic : MonoBehaviour
{
    [Header("Music info")]
    [EventRef]
    [SerializeField]
    private string eMusic = "";

    private EventInstance iMusic;

    [Header("Events")]
    [SerializeField]
    private GameEventBool onMusicValidation = null;

    // Determines which value to pass into FMOD
    private enum CurrentLevel
    {
        Lysus = 0,
        Cremon = 1,
        Greasopolis = 2,
        Chiloo = 3,
        Announcer = 4,
    }
    [SerializeField]
    private CurrentLevel currentLevel = 0;

    private void OnEnable()
    {
        RuntimeManager.StudioSystem.setParameterByName("currentPlanet", (float)currentLevel);
        SetPhase(GameManager.instance.currentLevel);
        Play();
        onMusicValidation?.Raise(iMusic.isValid());
    }

    private void OnDisable()
    {
        Stop();
        onMusicValidation?.Raise(iMusic.isValid());
    }

    public void SetPhase(int phase)
    {
        iMusic.setParameterByName("Judge Phase", phase);
    }

    public void Play()
    {
        iMusic = RuntimeManager.CreateInstance(eMusic);
        iMusic.start();
    }
    public void Stop()
    {
        if (iMusic.isValid())
        {
            iMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            iMusic.release();
        }
    }
}
