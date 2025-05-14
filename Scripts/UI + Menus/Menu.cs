using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using System.IO;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject firstButtonMainMenu;
    public GameObject firstButtonControls;
    public GameObject firstButtonFromControls;
    public GameObject firstButtonControlsMenu;
    public GameObject firstButtonFromControlsMenu;
    public GameObject firstButtonExit;
    public GameObject firstButtonFromExit;
    public GameObject firstButtonCredits;
    public GameObject firstButtonFromCredits;
    public GameObject firstButtonTutorial;
    public GameObject firstButtonFromTutorial;

    private void Start()
    {
        /*if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);
        }*/
    }

    public void MainMenu()
    {
        SaveSystem.SavePlayerState(GameManager.levelsBeaten);
        InitialiseFMODParameters();
        Time.timeScale = 1;
        GameManager.instance.sceneToLoad = 0;
        SceneManager.LoadScene(5);
    }

    public void StartGame()
    {
        string path = Application.persistentDataPath + "/gameSaveData.sfs";
        if (!File.Exists(path))
        {
            SceneManager.LoadScene(12);
        }
        else
        {
            InitialiseFMODParameters();
            Time.timeScale = 1;
            GameManager.instance.sceneToLoad = 1;
            SceneManager.LoadScene(5);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 1;
        GameManager.instance.sceneToLoad = 3;
        SceneManager.LoadScene(5);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void LoadPreviousLevel()
    {
        InitialiseFMODParameters();
        Time.timeScale = 1;
        GameManager.instance.sceneToLoad = GameManager.instance.currentLevel;
        SceneManager.LoadScene(5);
    }

    // Resets isLowHealth parameter
    private void InitialiseFMODParameters()
    {
        RuntimeManager.StudioSystem.setParameterByName("isLowHealth", 0);
    }

    public void OpensOptions() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonControls);
    }

    public void CloseOptions()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonFromControls);
    }

    public void OpenControls() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonControlsMenu);
    }
    public void CloseControls()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonFromControlsMenu);
    }

    public void OpensExit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonExit);
    }

    public void CloseExit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonFromExit);
    }

    public void OpensCredits()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonCredits);
    }

    public void ClosesCredits()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonFromCredits);
    }

    public void OpensTutorial()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonTutorial);
    }

    public void ClosesTutorial()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonFromTutorial);
    }

    public void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}