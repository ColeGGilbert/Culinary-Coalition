using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MidRoundText : MonoBehaviour
{
    [SerializeField] private GameEvent onFinishedMidRoundText = null;

    [System.Serializable]
    public class Speaker
    {
        public string name;
        public Sprite portrait;
        public string[] text;
        public int[] musicTransition;
        [EventRef] public string eSound = "";
    }
    public List<Speaker> speakers;

    [System.Serializable]
    public class Speaker2
    {
        public string name;
        public Sprite portrait;
        public string[] text;
        public int[] musicTransition;
        [EventRef] public string eSound = "";
    }
    public List<Speaker2> speakers2;

    private int currentSpeaker = 0;
    DialogueSystem dialogueSystem;
    private bool showedFirstText;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();     //attatches the dialogue system script
    }

    /// <summary>
    /// Runs when a speaker finsihed talking
    /// </summary>
    /// <param name="skip"></param>
    private void OnSpeakerFinishedTalking(bool skip)
    {
        if (skip)   //Skips to the end of the dialogue 
        {
            Finished();
        }
        else if ((currentSpeaker == speakers.Count - 1) || (currentSpeaker == speakers2.Count - 1))   //Ends the dialogue when the last speaker has finsihed talking
        {
            Finished();
        }
        else if ((currentSpeaker + 1 < speakers.Count) || (currentSpeaker + 1 < speakers2.Count))   //Tells the next speaker to start talking
        {
            currentSpeaker++;
            PlayText(false);
        }
    }

    /// <summary>
    /// Passes information into the DialogueSystem script
    /// </summary>
    private void PlayText(bool firstTime)
    {
        if (true) FMODUpdateDialogueState(1);
        if (!showedFirstText)       //checks to see if the first set of mid round text has played
        {
            dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, true, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
            //dialogueSystem.EnableMusicTransition(speakers[currentSpeaker].musicTransition);
        }
        else
        {
            dialogueSystem.ShowDialogue(speakers2[currentSpeaker].text, true, speakers2[currentSpeaker].name, speakers2[currentSpeaker].portrait, speakers2[currentSpeaker].eSound, gameObject.transform.position);
            //dialogueSystem.EnableMusicTransition(speakers2[currentSpeaker].musicTransition);
        }
    }

    private void StartDialogue() 
    {
        PlayText(true);
    }

    void OnEnable()
    {
        BulletSpawning.OnEnding += StartDialogue;      //listens out for event in BulletSpawning
        DialogueSystem.OnFinishedText += OnSpeakerFinishedTalking;      //listens out for event in DialogueSystem
    }

    void OnDisable()
    {
        BulletSpawning.OnEnding -= StartDialogue;
        DialogueSystem.OnFinishedText -= OnSpeakerFinishedTalking;
    }

    /// <summary>
    /// Runs when the last speaker has finsihed talking
    /// </summary>
    private void Finished() 
    {
        FMODUpdateDialogueState(5);

        if (!showedFirstText)       //tells the script to use the next set of text when the script is called again
        {
            showedFirstText = true;
            currentSpeaker = 0;
        }
        else
        {
            onFinishedMidRoundText?.Raise();
            gameObject.SetActive(false);    //disables the gameobject so the DialogueSystem on the object is not used again
        }
    }

    /// <summary>
    /// Updates the dialogueState parameter in FMOD
    /// </summary>
    /// <param name="state"></param>
    private void FMODUpdateDialogueState(int state)
    {
        RuntimeManager.StudioSystem.setParameterByName("dialogueState", state);
        //Debug.Log("Updating Dialogue State: " + state);
    }
}