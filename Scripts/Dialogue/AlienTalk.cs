using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AlienTalk : MonoBehaviour
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
    private Player controls;
    bool isTalking;
    private bool canUseController = true;
    [SerializeField] private GameObject chatPrompt;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystem = gameObject.AddComponent<DialogueSystem>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RecivedInput();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTalking)   //the isTalking check is there to makes sure the check is only called once 
        {
            isTalking = true;
            chatPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndText();
        }
    }

    /// <summary>
    /// Runs when a speaker finsihed talking
    /// </summary>
    /// <param name="skip"></param>
    private void OnSpeakerFinishedTalking(bool skip)
    {
        if (skip && isTalking) 
        {
            currentSpeaker = speakers.Count;    //used to end the text early
            anim.SetBool("isTalking", false);
        }
        if (isTalking && currentSpeaker + 1 < speakers.Count)
        {
            currentSpeaker++;
            dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, false, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
            //Passes information into the DialogueSystem script
        }
    }

    void OnEnable()
    {
        DialogueSystem.OnFinishedText += OnSpeakerFinishedTalking;            //listens out for event in DialogueSystem
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        DialogueSystem.OnFinishedText -= OnSpeakerFinishedTalking;            //listens out for event in DialogueSystem
        controls.Gameplay.Disable();
    }

    /// <summary>
    /// Resets values when the player finished talking to the npc
    /// </summary>
    private void EndText() 
    {
        anim.SetBool("isTalking", false);
        dialogueSystem.HideDialogue(false);
        isTalking = false;      //closes the panel when the player is far away
        currentSpeaker = 0;
        canUseController = true;
        chatPrompt.SetActive(false);
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Interaction.performed += ctx => RecivedInput();
    }

    private void RecivedInput() 
    {
        if (isTalking && canUseController)  //the canUseController check is there to disable the controller support while it is used for progressing through dialogue boxes
        {
            isTalking = true;
            canUseController = false;
            dialogueSystem.ShowDialogue(speakers[currentSpeaker].text, false, speakers[currentSpeaker].name, speakers[currentSpeaker].portrait, speakers[currentSpeaker].eSound, gameObject.transform.position);
            //Passes information into the DialogueSystem script
            anim.SetBool("isTalking", true);
            chatPrompt.SetActive(false);
        }
    }
}
