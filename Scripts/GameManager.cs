using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;

    public static int loadHub;
    public static int loadLevel1;
    public static int levelsBeaten;
    private bool beatenLysus;
    private bool beatenCremon;
    private bool beatenGreasopolis;
    private bool beatenChiloo;

    public bool displayHatcuumTutorial;
    public int returnToScene;

    bool rbHeld;
    bool lbHeld;

    Coroutine countdown;

    private Player controls;

    public bool canPause = true;

    /// This section of code will get any active instancce of the game manager and assign it so that it can be called and accessed by other scripts
    /// If there is no active instance of the game manager, then a new game manager is created
    public static GameManager instance;
    /*{
        get
        {
            if(s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }

            if(s_Instance == null)
            {
                var obj = new GameObject("GameManager");
                s_Instance = obj.AddComponent<GameManager>();
            }

            return s_Instance;
        }
    }*/

    private void Awake()
    {       
        //caps fps to 120
        Application.targetFrameRate = 120;
        if (s_Instance == null)
        {
            s_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
        }

        if (s_Instance == null)
        {
            var obj = new GameObject("GameManager");
            instance = obj.AddComponent<GameManager>();
        }

        controls = new Player();

        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 8 || scene.buildIndex == 9)
        {
            if(levelsBeaten >= 1)
            {
                FindObjectOfType<HatcuumController>().enabled = true; FindObjectOfType<AttatchToPlayer>().enabled = true;
                GameObject.Find("Hatcuum Canvas").SetActive(true);
            }
            else
            {
                FindObjectOfType<HatcuumController>().enabled = false; FindObjectOfType<AttatchToPlayer>().enabled = false;
                GameObject.Find("Hatcuum Canvas").SetActive(false);
            }
        }
        else if (scene.buildIndex == 10 || scene.buildIndex == 11)
        {
            FindObjectOfType<HatcuumController>().enabled = true; FindObjectOfType<AttatchToPlayer>().enabled = true;
            GameObject.Find("Hatcuum Canvas").SetActive(true);
        }
    }

    private void OnEnable()
    {
        // Check for active game manager objects and destroy self if there is already a game manager
        if (GameObject.FindGameObjectsWithTag("GameManager").Length <= 1)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            Debug.Log("Run Load");

            PlayerSaveData save = SaveSystem.LoadPlayer();
            if(save != null)
            {
                levelsBeaten = save.data.level;
            }

            if (levelsBeaten > 4)
            {
                beatenCremon = true;
                beatenLysus = true;
                beatenGreasopolis = true;
                beatenChiloo = true;
            }
            else if (levelsBeaten > 3)
            {
                beatenCremon = true;
                beatenLysus = true;
                beatenGreasopolis = true;
            }
            else if (levelsBeaten > 2)
            {
                beatenLysus = true;
                beatenCremon = true;
            }
            else if (levelsBeaten > 1)
            {
                beatenLysus = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }

        EndingText.OnFinishedStage += UpdateLevelsBeaten;
        Options.OnDeleteSave += ResetSave;
        controls.Gameplay.ResetFailsafeL.performed += ctx => LBPressed();
        controls.Gameplay.ResetFailsafeR.performed += ctx => RBPressed();
        controls.Gameplay.Enable();
    }

    public void ResetSave()
    {
        levelsBeaten = 0;

        beatenChiloo = false;
        beatenCremon = false;
        beatenLysus = false;
        beatenGreasopolis = false;
        displayHatcuumTutorial = false;

        loadHub = 0;
    }

    private void OnDisable()
    {
        EndingText.OnFinishedStage -= UpdateLevelsBeaten;
        Options.OnDeleteSave -= ResetSave;

        controls.Gameplay.ResetFailsafeL.performed -= ctx => LBPressed();
        controls.Gameplay.ResetFailsafeR.performed -= ctx => RBPressed();
        controls.Gameplay.Disable();
    }

    void OnApplicationQuit()
    {
        s_Instance = null;
        if (SceneManager.GetActiveScene().buildIndex != 12)
        {
            SaveSystem.SavePlayerState(levelsBeaten);
        }
    }

    // Hold the prefab for the dialogue system script
    [SerializeField] public GameObject dialoguePrefab;

    [HideInInspector] public int sceneToLoad = 0; // Scene index used in Loading Screen
    [HideInInspector] public int currentLevel = 2; // Scene index of level player is on (Tutorial, Level 1, etc.)
    [HideInInspector] public int currentPhase = 0;

    /// <summary>
    /// Freezes the game temporarily and slowly restores the timescale to it's original state after taking damage
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator FreezeFrameDamage(float delay)
    {
        Time.timeScale = 0.05f;
        RuntimeManager.StudioSystem.setParameterByName("isFreezeFrameDamageActive", 1f);
        yield return new WaitForSeconds(delay/10);
        while(Time.timeScale < 1)
        {
            yield return new WaitForSeconds(delay / 50);
            Time.timeScale += 0.05f;
        }
        RuntimeManager.StudioSystem.setParameterByName("isFreezeFrameDamageActive", 0f);
    }

    /// <summary>
    /// Freezes time for a duration
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    public IEnumerator FreezeFrame(float delay)
    {
        Time.timeScale = 0.001f;
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1;
    }

	public IEnumerator DelayInactive(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
	private void UpdateLevelsBeaten(int buildIndex) 
    {
        if (!beatenLysus && (buildIndex == 2 || buildIndex == 7 || buildIndex == 8 || buildIndex == 9))        //lysus
        {
            levelsBeaten = 1;
            beatenLysus = true;
            displayHatcuumTutorial = true;
        }
        else if (!beatenCremon && (buildIndex == 10 || buildIndex == 11))        //cremon
        {
            levelsBeaten = 2;
            beatenCremon = true;
        }
        else if (!beatenGreasopolis && buildIndex == 99)        //greasopolis
        {
            levelsBeaten = 3;
            beatenGreasopolis = true;
        }
        else if (!beatenChiloo && buildIndex == 99)        //chiloo
        {
            levelsBeaten = 4;
            beatenChiloo = true;
        }
        Debug.Log("LEVELS BEATEN: " + levelsBeaten);
        SaveSystem.SavePlayerState(levelsBeaten);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftBracket) && Input.GetKey(KeyCode.RightBracket))
        {
            if (countdown == null)
            {
                countdown = StartCoroutine(TimeForFailsafe());
            }
        }
        else if(Input.GetKeyUp(KeyCode.LeftBracket) || Input.GetKeyUp(KeyCode.RightBracket))
        {
            StopCoroutine(countdown);
            countdown = null;
        }

        if(lbHeld && rbHeld)
        {
            ResetFailsafe();
        }
    }

    private void LBPressed()
    {
        lbHeld = true;
        StartCoroutine(DisableLB());
    }

    private void RBPressed()
    {
        rbHeld = true;
        StartCoroutine(DisableRB());
    }

    private void ResetFailsafe()
    {
        Debug.Log("Reset Game --");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
        DestroyImmediate(gameObject);
    }

    private IEnumerator TimeForFailsafe()
    {
        yield return new WaitForSecondsRealtime(3f);
        ResetFailsafe();
    }

    private IEnumerator DisableLB()
    {
        yield return new WaitForSecondsRealtime(1.6f);
        lbHeld = false;
    }

    private IEnumerator DisableRB()
    {
        yield return new WaitForSecondsRealtime(1.6f);
        rbHeld = false;
    }
}