using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public bool paused;
    //[SerializeField] private GameObject pausePanel;
    //[SerializeField] private GameObject optionsPanel;

    public GameObject firstButtonMainMenu;
    public GameObject firstButtonControls;
    public GameObject firstButtonFromControls;
    private Player controls;

    [Header("Events")]
    [SerializeField] private GameEvent onPause;
    [SerializeField] private GameEvent onUnpause;

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Pausing.performed += ctx => RecivedInput();
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
        instance = this;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonMainMenu); //frick this line of code >:(
    }
    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(EventSystem.current);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RecivedInput();
        }
    }

    private void Pause() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonMainMenu);

        onPause?.Raise();
        RuntimeManager.StudioSystem.setParameterByName("isPaused", 1);
        Time.timeScale = 0;
        //pausePanel.SetActive(true);
    }

    public void Unpause()   //function is public so it can be accessed by a button
    {
        onUnpause?.Raise();
        RuntimeManager.StudioSystem.setParameterByName("isPaused", 0);
        paused = false;
        //pausePanel.SetActive(false);
        //optionsPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void RecivedInput() 
    {
        if (GameManager.instance.canPause)
        {
            paused = !paused;
            if (paused) { Pause(); }
            else { Unpause(); }
        }
    }
}
