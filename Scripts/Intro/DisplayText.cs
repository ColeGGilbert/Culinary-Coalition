using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class DisplayText : MonoBehaviour
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

    private int currentSpeaker = 0;
    DialogueSystem dialogueSystem;
    private bool started;

    [SerializeField] private GameObject dialogueCanvasPrefab = null;
    private GameObject previousCanvasPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();     //attatches the dialogue system script 
        previousCanvasPrefab = GameManager.instance.dialoguePrefab;
        GameManager.instance.dialoguePrefab = dialogueCanvasPrefab;
    }

    private void Update()
    {
        if (!started) StartText();      //can't be run in start due to needing the DialogueSystem script
    }

    private void StartText()
    {
        started = true;     //stops the function from being run again
        Display();
    }

    /// <summary>
    /// Passes information into the DialogueSystem script
    /// </summary>
    private void Display()
    {
        dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, true, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
    }

    /// <summary>
    /// Runs when a speaker finsihed talking
    /// </summary>
    /// <param name="skip"></param>
    private void OnSpeakerFinishedTalking(bool skip)
    {
        if (started)
        {
            if (skip)   //ends the dialogue if the player pressed the skip button
            {
                End();
            }
            else if (currentSpeaker == speakers.Count - 1)      //ends the dialogue if the last speaker has spoken
            {
                End();
            }
            else if (currentSpeaker + 1 < speakers.Count)       //starts the next speaker dialogue
            {
                currentSpeaker++;
                Display();
            }
        }
    }

    void OnEnable()
    {
        DialogueSystem.OnFinishedText += OnSpeakerFinishedTalking;      //listens out for event in DialogueSystem
    }

    void OnDisable()
    {
        DialogueSystem.OnFinishedText -= OnSpeakerFinishedTalking;
    }

    /// <summary>
    /// Runs when the dialogue has reached the end
    /// </summary>
    private void End()
    {
        GameManager.instance.dialoguePrefab = previousCanvasPrefab;
        GameManager.instance.sceneToLoad = 1;
        GameManager.instance.canPause = true;
        SceneManager.LoadScene(5);
    }
}
