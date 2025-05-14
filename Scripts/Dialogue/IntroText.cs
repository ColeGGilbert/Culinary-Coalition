using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class IntroText : MonoBehaviour
{
    [SerializeField] private GameEvent onFinishedIntroText = null;

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
    private Scene currentScene;
    DialogueSystem dialogueSystem;
    [SerializeField] Animator levelStartText;
    private bool started;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();     //attatches the dialogue system script 
    }

    private void Update()
    {
        if (!started) StartText();      //can't be run in start due to needing the DialogueSystem script
    }

    private void StartText()
    {
        started = true;     //stops the function from being run again
        currentScene = SceneManager.GetActiveScene();

        if (GameManager.loadHub == 1 && currentScene.buildIndex == 1)       //skips the text if it has already run
        {
            Time.timeScale = 1;     
            gameObject.SetActive(false);
        }
        else
        {
            DisplayText();
        }
    }

    /// <summary>
    /// Passes information into the DialogueSystem script
    /// </summary>
    private void DisplayText()
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
                FinishedDialogue();
            }
            else if (currentSpeaker == speakers.Count - 1)      //ends the dialogue if the last speaker has spoken
            {
                FinishedDialogue();
            }
            else if (currentSpeaker + 1 < speakers.Count)       //starts the next speaker dialogue
            {
                currentSpeaker++;
                DisplayText();
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
    private void FinishedDialogue()
    {
        onFinishedIntroText?.Raise();

        if (currentScene.buildIndex == 1)   //hub
        {
            GameManager.loadHub = 1;      //keep tracks of if the text has already been displayed
        }

        if (levelStartText != null)       //starts the cook animation flying onto the screen
        {
            levelStartText.SetTrigger("COOK");
        }

        gameObject.SetActive(false);
    }
}