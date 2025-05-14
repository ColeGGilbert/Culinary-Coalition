using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject LSPanel = null; // Level Select Panel
    [SerializeField] private GameObject F = null; // Press F Panel
    [SerializeField] private GameObject BSPanel = null; // Badge Select Panel
    [SerializeField] private GameObject arrow = null; // TutorialArrow
    private bool canActive;
    private Player controls;
    [SerializeField] private Movement playerMovement;
    private bool difficultyNormal;
    private bool difficultyHard = false;

    [SerializeField] private Animator camera;

    [SerializeField] private Button exitButtonPanel;

    public GameObject firstButtonLevelSelect;
    public GameObject firstButtonBadge;

    [Space(10)]
    [SerializeField] float delayTime = 0f;
    private float animDelayTime = 0f;

    public delegate void LevelSelectInteract(int phase);
    public static event LevelSelectInteract onLevelSelectInteract;
    public static event LevelSelectInteract closeLevelSelectInteract;

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Interaction.performed += ctx => RecivedInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstButtonLevelSelect);
        if (F != null)
        {
            F.SetActive(false);
        }

        animDelayTime = 0;
    }

    private void Update()
    {
        //Debug.Log(EventSystem.current);
        if (Input.GetKeyDown(KeyCode.F))
        {
            RecivedInput();
        }

        if(animDelayTime > 0)
        {
            animDelayTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)         //sets e panel to be active when the players get close
    {
        if (other.CompareTag("Player"))
        {
            onLevelSelectInteract?.Invoke(1);
            if (F != null)
            {
                F.SetActive(true);
            }
            canActive = true;
            arrow.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)      //turns off all panels when the player gets far away
    {
        if(exitButtonPanel != null)
        {
            exitButtonPanel.interactable = false;
        } 
        LSPanel.SetActive(false);
        if (F != null)
        {
            F.SetActive(false);
        }
        BSPanel.SetActive(false);
        canActive = false;
    }

    public void LoadLevel(string level)
    {
        switch (level)
        {
            case ("lys"):
                GameManager.instance.sceneToLoad = 8;

                if (difficultyHard)
                {
                    GameManager.instance.sceneToLoad = 9;
                }
                break;

            case ("cre"):
                GameManager.instance.sceneToLoad = 10;

                if (difficultyHard)
                {
                    GameManager.instance.sceneToLoad = 11;
                }
                break;

            case ("tut"):
                GameManager.instance.sceneToLoad = 6;

                GameManager.instance.returnToScene = SceneManager.GetActiveScene().buildIndex;
                break;
        }

        SceneManager.LoadScene(5);
    }

    private void RecivedInput()
    {
        if (canActive && animDelayTime <= 0f)
        {
            if (LSPanel.activeSelf == false && BSPanel.activeSelf == false)     //turns on the level select panel
            {
                onLevelSelectInteract?.Invoke(2);

                Debug.Log("level select");

                Invoke("ButtonSelectDelay", delayTime);

                // Enables camera transition when interacting with LS
                if (camera != null) camera.SetTrigger("EnterLevelSelect");

                //Disables player movement
                StartCoroutine("DelayPlayerDisable");

                LSPanel.SetActive(true);
                if (F != null)
                {
                    F.SetActive(false);
                }
            }
            else if (LSPanel.activeSelf == true && BSPanel.activeSelf == false)     //turns e panel on
            {
                Debug.Log("e");

                //FMODSetLSPhaseParam(1);
                //LSPanel.SetActive(false);
                //F.SetActive(true);
            }
            else if (LSPanel.activeSelf == false || BSPanel.activeSelf == true)     //turns badge select panel on
            {
                Debug.Log("badge");

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstButtonBadge);

                BSPanel.SetActive(true);
                if (F != null)
                {
                    F.SetActive(false);
                }
            }
        }
    }

    public void ButtonSelectDelay()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonLevelSelect);

        if (exitButtonPanel != null)
        {
            exitButtonPanel.interactable = true;
        }
    }

    public void ExitLevelSelectPanel()
    {
        EventSystem.current.SetSelectedGameObject(null);
        exitButtonPanel.interactable = false;
        animDelayTime = 1.5f;
        closeLevelSelectInteract?.Invoke(0);
        LSPanel.SetActive(false);
        camera.SetTrigger("ExitLevelSelect");
        playerMovement.gameObject.SetActive(true);
        difficultyNormal = false;
        difficultyHard = false;
        canActive = true;

    }

    public void OpenLevelSelectPanel()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonLevelSelect);

    }

    public void OpenBadgePanel()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonBadge);
    }

    private IEnumerator DelayPlayerDisable()
    {
        yield return new WaitForSeconds(1);
        playerMovement.gameObject.SetActive(false);
    }

    public void SetDifficultyNormal()
    {
        difficultyNormal = true;
        difficultyHard = false;
    }

    public void SetDifficultyHard()
    {
        difficultyHard = true;
        difficultyNormal = false;
    }
}
