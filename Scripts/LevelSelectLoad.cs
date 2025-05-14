using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectLoad : MonoBehaviour
{

    [SerializeField] Button[] LevelButtons;
    [SerializeField] Image[] LevelPlanets;
    [SerializeField] Image[] LevelLocks;
    string[] levelNames = new string[4]{"Lysus", "Cremon", "Greasopolis", "Chiloo"};

    [SerializeField] GameObject hatcuumTutorial;
    private bool checkForInput;
    bool interactingWithPanel;

    private Player controls;

    // FMOD
    [SerializeField]
    private FMODEvent lockedInteractSound = null;
    [SerializeField]
    private FMODEvent unlockedInteractSound = null;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLevelSelect();

        if (GameManager.instance.displayHatcuumTutorial)
        {
            Debug.Log("Enable Hatcuum Tutorial");
            Time.timeScale = 0;
            hatcuumTutorial.SetActive(true);
            StartCoroutine(WaitForTime(.4f));
            GameManager.instance.displayHatcuumTutorial = false;
        }
    }

    void UpdateLevelSelect()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i <= GameManager.levelsBeaten && i < 2)
            {
                Debug.Log("UNLOCKING LEVEL: " + levelNames[i]);

                if (lockedInteractSound != null)
                {
                    LevelButtons[i].GetComponent<WorldSelectAudio>().SetSound(unlockedInteractSound);
                }

                LevelPlanets[i].color = Color.white;
                LevelLocks[i].enabled = false;

                if(interactingWithPanel) LevelButtons[i].interactable = true;
                else LevelButtons[i].interactable = false;
            }
            else
            {
                Debug.Log("LOCKING LEVEL: " + levelNames[i]);

                if (unlockedInteractSound != null)
                {
                    LevelButtons[i].GetComponent<WorldSelectAudio>().SetSound(lockedInteractSound);
                }

                LevelPlanets[i].color = new Color(.05f, .05f, .05f, 1);
                LevelLocks[i].enabled = true;

                LevelButtons[i].interactable = false;

            }
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

        for (int i = 0; i < LevelButtons.Length; i++)
        {
            LevelButtons[i].GetComponent<WorldSelectAudio>().SetInteracting(interactingWithPanel);
        }

        UpdateLevelSelect();
    }

    private void Update()
    {
        if (hatcuumTutorial.activeSelf)
        {
            if (Input.anyKey)
            {
                RecivedInput();
            }
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

        Options.OnDeleteSave += UpdateLevelSelect;
        LevelSelect.onLevelSelectInteract += PanelCheck;
        LevelSelect.closeLevelSelectInteract += PanelCheck;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();

        Options.OnDeleteSave -= UpdateLevelSelect;
        LevelSelect.onLevelSelectInteract -= PanelCheck;
        LevelSelect.closeLevelSelectInteract -= PanelCheck;
    }

    IEnumerator WaitForTime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        checkForInput = true;
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Any.performed += ctx => RecivedInput();
    }

    private void RecivedInput() 
    {
        if (checkForInput)
        {
            Time.timeScale = 1;
            hatcuumTutorial.SetActive(false);
            checkForInput = false;
        }
    }
}
