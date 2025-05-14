using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class EndingText : MonoBehaviour
{
    [System.Serializable]
    public class Speaker
    {
        public string name;
        public Sprite portrait;
        public string[] text;
        [EventRef] public string eSound = "";

    }
    public List<Speaker> speakers;

    public delegate void FinishedStage(int buildIndex);
    public static event FinishedStage OnFinishedStage;

    public delegate void StageClear();
    public static event StageClear OnStageClear;

    private int currentSpeaker = 0;
    DialogueSystem dialogueSystem;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();     //attatches the dialogue system script 
    }

    /// <summary>
    /// Passes information into the DialogueSystem script
    /// </summary>
    private void PlayText()
    {
        dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, true, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
    }

    /// <summary>
    /// Runs when a speaker finsihed talking
    /// </summary>
    /// <param name="skip"></param>
    private void OnSpeakerFinishedTalking(bool skip)
    {
        if (skip)
        {
            //Finished();
        }
        if (currentSpeaker == speakers.Count - 1)
        {
            Finished();
        }
        if (currentSpeaker + 1 < speakers.Count)
        {
            currentSpeaker++;
            PlayText();
        }
    }

    void OnEnable()
    {
        BulletSpawning.OnEnding += PlayText;      //listens out for event in BulletSpawning
        DialogueSystem.OnFinishedText += OnSpeakerFinishedTalking;      //listens out for event in DialogueSystem
    }

    void OnDisable()
    {
        BulletSpawning.OnEnding -= PlayText;
        DialogueSystem.OnFinishedText -= OnSpeakerFinishedTalking;
    }

    /// <summary>
    /// Runs when the last speaker has finsihed talking
    /// </summary>
    private void Finished()
    {
        GameManager.instance.canPause = false;

        OnFinishedStage?.Invoke(SceneManager.GetActiveScene().buildIndex);

        OnStageClear?.Invoke();

        //GameManager.instance.sceneToLoad = 1;
        //SceneManager.LoadScene(5);
    }
}