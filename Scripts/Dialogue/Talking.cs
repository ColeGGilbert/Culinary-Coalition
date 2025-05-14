using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using TMPro;

public class Talking : MonoBehaviour        //talking is a parent of the introtext script, hence why a lot of functions and variables are protected
{
    [SerializeField] [TextArea] protected string[] text;
    [SerializeField] protected float textSpeed = 0.03f;
    [SerializeField] protected int translationDelay = 2;
    [SerializeField] protected TMP_Text panelText;
    [SerializeField] protected GameObject panel;
    protected int size;
    protected int say;
    private bool isTalking;
    //protected StringBuilder currentText;
    protected bool loadingText;
    protected string alienText;

    DialogueSystem dialogue;

    [Header("FMOD")]
    [EventRef] private string eDialogue = "{98622cca-a118-4631-b142-956b9522f70a}";
    private EventInstance eDialogueInst;
    [EventRef] protected string eDefaultDialogue = "{d2e048df-5614-4d97-a89f-418d887bc941}";
    private EventInstance eDefaultDialogueInst;

    // Start is called before the first frame update
    void Start()
    {
        if (translationDelay <= 0) translationDelay = 1;

        size = text.Length;
        ResetText();

        // Creates instance of dialogue and sets it at the position of the gameObject
        eDialogueInst = RuntimeManager.CreateInstance(eDialogue);
        eDialogueInst.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));

        dialogue = new DialogueSystem();
    }

    private void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) && panel.active == true && isTalking && loadingText)
            {
                SkipLine();     //skips to the end of the current text
            }
            else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) && panel.active == true && isTalking && !loadingText)
            {
                UpdateText();       //goes to the next line of text
            }
        }
    }

    private void UpdateText() 
    {
        say = say + 1;
        if (say >= size)    //turns off the panel when you are at the end of the conversation and continue the conversation
        {
            ClosePanel();
        }
        else       //prints next line of the conversation
        {
            StartCoroutine(DisplayText(translationDelay));
        }
    }

    private void ResetText()
    {
        say = 0;
        panelText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTalking)     //makes sure the function is only called once 
        {
            StopCoroutine(DisplayText(translationDelay));   //stops the function if it is all ready running
            isTalking = true;
            ResetText();        //resets conversation back to the start
            StartCoroutine(DisplayText(translationDelay));  
            panel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ClosePanel();       //closes the panel when the player is far away
    }

    protected IEnumerator DisplayText(int translationOffSet)
    {
        // Plays dialogue sound
        RuntimeManager.StudioSystem.setParameterByName("isTalking", 1);
        eDialogueInst.start();

        if (translationOffSet > text[say].Length) translationOffSet = text[say].Length - 1;
        Translation();
        StringBuilder currentText = new StringBuilder(text[say].Length);      //sets a empty stringbuilder with the length of the current text
        loadingText = true;
        for (int i = 0; i < text[say].Length + translationOffSet; i++)      //loops through each letter of the current text
        {
            if (i >= translationOffSet)     //starts translation to english after the off set set in the insector
            {
                currentText[i - translationOffSet] = text[say][i - translationOffSet];   
            }
            if (i >= text[say].Length)      //runs when i is over the lenght of the text (last bit of translation)
            {
                currentText[i - translationOffSet] = text[say][i - translationOffSet];
                panelText.text = currentText.ToString();        //adds english text to the string
                yield return new WaitForSeconds(textSpeed);     //recommended delay set to 0.03f
            }
            else
            {
                if(i <= text[say].Length) currentText.Append(alienText[i]);       //adds current alien letter to the stringbuilder 
                panelText.text = currentText.ToString();        //prints current letter
                yield return new WaitForSeconds(textSpeed);         //recommended delay set to 0.03f
            }
            if (!loadingText)   //breaks if the player continue dialogue while the text is loading
            {
                panelText.text = text[say];     //prints full translated text
                break;
            }
        }
        loadingText = false;
        FMODStopDialogue();
    }

    protected void SkipLine()
    {
        loadingText = false;    //activates the break in the for loop 
    }

    private void ClosePanel() 
    {
        panel.SetActive(false);
        isTalking = false;
        FMODStopDialogue();
    }

    protected void Translation() 
    {
        StringBuilder currentText = new StringBuilder(text[say].Length);
        for (int i = 0; i < text[say].Length; i++)      //loops through each letter of the current text
        {
            if (text[say][i] == ' ') currentText.Append(' ');
            else currentText.Append(Random.Range(0, 9));   //adds current letter to the stringbuilder 
            alienText = currentText.ToString();    //prints current letter
        }
    }

    protected void RemoveEndSpaces() 
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

    private void FMODStopDialogue()
    {
        eDialogueInst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.StudioSystem.setParameterByName("isTalking", 0);
    }

    public void FMODSetDialoguePause(bool isPaused)
    {
        eDialogueInst.setPaused(isPaused);
    }
}
