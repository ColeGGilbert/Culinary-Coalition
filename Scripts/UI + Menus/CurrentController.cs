using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentController : MonoBehaviour
{
    public static CurrentController instance;
    public bool usingController;

    private Player controls;

    private float currentTime = -10;

    [SerializeField]
    [Tooltip("Time in seconds the player needs to idle before loading the idle scene")]
    private float timeToIdle = 120;

    private bool isIdle = false;

    [SerializeField]
    [Tooltip("Scene which should load on idle")]
    private int idleSceneIndex = 13;

    [SerializeField]
    [Tooltip("Scene which should be loaded when returning from idle")]
    private int returnSceneIndex = 0;

    [SerializeField]
    [Tooltip("If true, idle will be tracked on all scenes except the idle scene. Otherwise, idle will only be tracked in scenes from trackIdleScenes")]
    private bool allScenes = true;

    [SerializeField]
    [Tooltip("Build indices of scenes which should track idle")]
    private int[] trackIdleScenes = null;

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Dash.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.SkipDialogue.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Pausing.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Interaction.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Hatcuum.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Eating.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Movement.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.ServeEarly.performed += ctx => ReceivedControllerInput();
        controls.Gameplay.Any.performed += ctx => ReceivedControllerInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void ReceivedControllerInput() 
    {
        usingController = true;
        ResetIdle();
    }

    private void ReceivedKeyboardInput() 
    {
        usingController = false;
        ResetIdle();
    }

    private void ResetIdle()
    {
        isIdle = false;
        currentTime = 0;

        if (InScene(idleSceneIndex))
        {
            SceneManager.LoadScene(returnSceneIndex);
        }
    }

    private void CheckKeyboardActions() 
    {
        if (Input.anyKey || Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse Y") > 0 || Input.GetAxis("Mouse Y") < 0)
        {
            ReceivedKeyboardInput();
        }
    }


    private void Update()
    {
        CheckKeyboardActions();

        if (!allScenes)
        {
            // Check if player is in any idle tracking scenes
            for (int i = 0; i < trackIdleScenes.Length; i++)
            {
                if (InScene(trackIdleScenes[i]))
                {
                    IdleTracker();
                    break;
                }      
            }
        }
        // Prevent tracking idle if in the idle (trailer) scene
        else if (!InScene(idleSceneIndex))
        {
            IdleTracker();
        }
    }

    // Check if player is in given scene
    private bool InScene(int scene)
    {
        return SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(scene);
    }

    private void IdleTracker()
    {
        if (currentTime < timeToIdle)
        {
            currentTime += Time.unscaledDeltaTime;
        }
        else
        {
            isIdle = true;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(idleSceneIndex);
        }
    }
}