using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;

public class DialogueSystem : MonoBehaviour        //talking is a parent of the introtext script, hence why a lot of functions and variables are protected
{
    [SerializeField] [TextArea] private string[] text;
    [SerializeField] private float textSpeed = 0.03f;
    [SerializeField] private int translationDelay = 2;
    private string speakers;
    private Sprite faces;

    // FMOD
    private string eSound = null;
    private string defaultSound = "{d2e048df-5614-4d97-a89f-418d887bc941}"; // Default sound if no other GUID is passed in
    private EventInstance iSound;
    private Vector3 dialogueSource;
    private bool canTransition;
    private int[] musicTransitionLines;
    private int currentMusicState;

    private int currentSpeaker;
    private int size;
    private int say;
    public bool freeze;
    private bool isTalking;
    private bool loadingText;
    private string alienText;
    private Player controls;

    public static DialogueSystem instance;

    public delegate void TextFinsihed(bool skip);
    public static event TextFinsihed OnFinishedText;

    public delegate void DialogueFreeze(bool isFrozen);
    public static event DialogueFreeze onDialogueFreeze;

    public delegate void LoadNewLine();
    public static event LoadNewLine onLoadNewLine;

    [SerializeField] [Tooltip("Dialogue Canvas with the uiText panel being the first child")] GameObject canvasPrefab;
    TextMeshProUGUI uiText;
    TextMeshProUGUI uiName;
    GameObject uiPanel;
    Image portrait;

    /// <summary>
    /// When the dialogue system is called using 'new DialogueSystem' this function is run automatically
    /// </summary>
    public DialogueSystem()
    {
        if (translationDelay <= 0) translationDelay = 1;
    }

    void CreateUIPanels()
    {
        // Assigns the variables to the newly created canvas
        canvasPrefab = GameManager.instance.dialoguePrefab;
        GameObject canvas = Instantiate(canvasPrefab);
        uiPanel = canvas.transform.GetChild(0).gameObject;
        uiText = uiPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        uiName = uiPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        portrait = uiPanel.transform.GetChild(4).transform.GetChild(0).GetComponent<Image>();
    }

    /// <summary>
    /// Display the paramater 'text' on the canvas
    /// Param 'sound' decides which sound to play for dialogue, leave empty ("") for default
    /// Param 'soundSource' decides where the sound should be played from in the scene. Simply use gameObject.transform.position as default
    /// </summary>
    /// </summary>
    /// <param name="text"></param>
    public void ShowDialogue(string[] t, bool f, string names, Sprite face, string soundGUID, Vector3 soundSource)
    {
        // Assigns a new sound if one is passed in
        if (soundGUID != "")
        {
            eSound = soundGUID;
        }
        else
        {
            eSound = defaultSound;
        }
        // Assigns sound source
        dialogueSource = soundSource;

        currentSpeaker = 0;
        say = 0;
        if (canvasPrefab == null)
        {
            CreateUIPanels();
        }
        text = t;
        size = text.Length;
        ResetText();
        isTalking = true;
        freeze = f;
        if (freeze)
        {
            onDialogueFreeze?.Invoke(true);
            Time.timeScale = 0;
        }
        if (!CheckActive()) uiPanel.SetActive(true);
        speakers = names;
        uiName.text = names;
        portrait.sprite = face;
        faces = face;

        StartCoroutine(DisplayText());
    }

    public void EnableMusicTransition(int[] transitionLines) 
    {
        transitionLines.CopyTo(musicTransitionLines, 2);
        //musicTransitionLines = transitionLines;
        canTransition = true;
        currentMusicState = 1;

        Debug.Log(musicTransitionLines.Length);
        foreach (int number in musicTransitionLines) 
        {
            Debug.Log(number);
        }
    }

    /// <summary>
    /// Hides the dialogue canvas from view
    /// </summary>
    public void HideDialogue(bool skip)
    {
        if (isTalking)
        {
            canTransition = false;
            currentMusicState = 0;
            iSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isTalking = false;
            if (freeze && Time.timeScale != 1)
            {
                freeze = false;
                if (PauseMenu.instance == null || !PauseMenu.instance.paused) //added so you can pause while mid dialogue
                {
                    onDialogueFreeze?.Invoke(false);
                    Time.timeScale = 1;
                }
            }
            if (CheckActive())
            {
                uiPanel.SetActive(false);
            }
            OnFinishedText?.Invoke(skip);
        }
    }

    /// <summary>
    /// Checks if the canvas is currently visible or not
    /// </summary>
    /// <returns></returns>
    public bool CheckActive()
    {
        return uiPanel.activeSelf;
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Pausing.performed += ctx => HideDialogue(true);
        controls.Gameplay.SkipDialogue.performed += ctx => HideDialogue(true);
        controls.Gameplay.Interaction.performed += ctx => RecivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        if(SceneManager.GetActiveScene().buildIndex == 6)
        {
            textSpeed = .0175f;
        }
        else
        {
            textSpeed = .03f;
        }
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Start()
    {
        instance = this;

        if (translationDelay <= 0) translationDelay = 1;

        // Assigns the variables to the newly created canvas
        canvasPrefab = GameManager.instance.dialoguePrefab;
        GameObject canvas = Instantiate(canvasPrefab);
        uiPanel = canvas.transform.GetChild(0).gameObject;
        uiText = uiPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        uiName = uiPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        portrait = uiPanel.transform.GetChild(4).transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        if (freeze)
        {
            onDialogueFreeze?.Invoke(true);
            Time.timeScale = 0;
        }

        if ((PauseMenu.instance == null || !PauseMenu.instance.paused) && isTalking)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.F))
            {
                RecivedInput();
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                HideDialogue(true);       //ends the text immediately 
            }
        }
    }

    private void UpdateText()
    {
        say++;
        if (canTransition && musicTransitionLines[currentMusicState] == say) 
        {
            currentMusicState++;
            FMODUpdateDialogueState(currentMusicState);
        }

        if (say >= size)    //turns off the panel when you are at the end of the conversation and continue the conversation
        {
            HideDialogue(false);
        }
        else       //prints next line of the conversation
        {
            StartCoroutine(DisplayText());
        }
    }

    private void ResetText()
    {
        say = 0;
        uiText.text = "";
    }

    private IEnumerator DisplayText()
    {
        onLoadNewLine?.Invoke();

        if (canTransition) FMODUpdateDialogueState(1);
        if (translationDelay <= 0) translationDelay = 2;
        if (translationDelay > text[say].Length) translationDelay = text[say].Length - 1;
        Translation();
        StringBuilder currentText = new StringBuilder(text[say].Length);      //sets a empty stringbuilder with the length of the current text
        loadingText = true;

        iSound = RuntimeManager.CreateInstance(eSound);
        EventDescription soundDescription;
        iSound.getDescription(out soundDescription);
        bool isSound3D;
        soundDescription.is3D(out isSound3D);
        if (isSound3D)
        {
            iSound.set3DAttributes(RuntimeUtils.To3DAttributes(dialogueSource));
        }
        RuntimeManager.StudioSystem.setParameterByName("isTalking", 1);
        iSound.start();
        

        for (int i = 0; i < text[say].Length + translationDelay; i++)      //loops through each letter of the current text
        {
            if (i >= translationDelay)     //starts translation to english after the off set set in the insector
            {
                currentText[i - translationDelay] = text[say][i - translationDelay];
            }
            if (i >= text[say].Length)      //runs when i is over the lenght of the text (last bit of translation)
            {
                currentText[i - translationDelay] = text[say][i - translationDelay];
                uiText.text = currentText.ToString();        //adds english text to the string
                yield return new WaitForSecondsRealtime(textSpeed);     //recommended delay set to 0.03f
            }
            else
            {
                if (i <= text[say].Length) currentText.Append(alienText[i]);       //adds current alien letter to the stringbuilder 
                uiText.text = currentText.ToString();        //prints current letter
                yield return new WaitForSecondsRealtime(textSpeed);         //recommended delay set to 0.03f
            }
            if (!loadingText)   //breaks if the player continue dialogue while the text is loading
            {
                uiText.text = text[say];     //prints full translated text
                break;
            }
        }
        loadingText = false;

        RuntimeManager.StudioSystem.setParameterByName("isTalking", 0);
        iSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        iSound.release();
    }

    private void SkipLine()
    {
        loadingText = false;    //activates the break in the for loop 
    }

    private void Translation()
    {
        StringBuilder currentText = new StringBuilder(text[say].Length);
        for (int i = 0; i < text[say].Length; i++)      //loops through each letter of the current text
        {
            if (text[say][i] == ' ') currentText.Append(' ');
            else currentText.Append(Random.Range(0, 9));   //adds current letter to the stringbuilder 
            alienText = currentText.ToString();    //prints current letter
        }
    }

    private void RemoveEndSpaces()
    {
        for (int a = 0; a < text.Length; a++)
        {
            for (int b = 0; b < text[a].Length; b++)      //loops through each letter of the current text
            {
                StringBuilder currentText = new StringBuilder(text[a].Length);
                currentText.Append(text[a]);
                if (text[a][b] == ' ')
                {
                    if (b + 1 < text[a].Length && text[a][b + 1] == ' ')
                    {
                        Debug.Log("detected double space");
                    }
                }
            }
        }
    }

    private void RecivedInput() 
    {
        if (CheckActive() && isTalking && loadingText)
        {
            SkipLine();     //skips to the end of the current text
        }
        else if (CheckActive() && isTalking && !loadingText)
        {
            UpdateText();       //goes to the next line of text
        }
    }

    private void FMODUpdateDialogueState(int state)
    {
        RuntimeManager.StudioSystem.setParameterByName("dialogueState", state);
        //Debug.Log("Updating Dialogue State: " + state);
    }
}
