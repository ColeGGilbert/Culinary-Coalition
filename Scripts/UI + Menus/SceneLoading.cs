using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoading : MonoBehaviour
{
    [SerializeField] private Slider progressBar = null;
    [SerializeField] private TextMeshProUGUI sceneLoadingText = null;
    [SerializeField] private TextMeshProUGUI loadingText = null;
    [SerializeField] [Tooltip("Initial delay in seconds before loading the scene (ONLY APPLIES OUTSIDE OF EDITOR)")] private float loadingDelay = 1f;
    private bool finishedLoading = false;
    private AsyncOperation level = null;
    private Player controls;

    private void Start()
    {
        Time.timeScale = 1;
        GetComponent<Canvas>().enabled = true;

        // Load the Main Menu if no Game Manager exists
        int sceneIndex = GameManager.s_Instance == null ? 0 : GameManager.instance.sceneToLoad;
        StartCoroutine(LoadAsyncOperation(sceneIndex));

#if UNITY_EDITOR
        sceneLoadingText.text = "Now Loading: " + System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
#else
        sceneLoadingText.gameObject.SetActive(false);
#endif
    }

    private IEnumerator LoadAsyncOperation(int index)
    {
#if !UNITY_EDITOR
        // Delay loading
        new WaitForSeconds(1);
#endif

        GameManager.instance.canPause = true;

        // Create async operation and load level
        level = SceneManager.LoadSceneAsync(index);
        level.allowSceneActivation = false;

        // Update progress bar while loading
        float t = 0.0f;
        while (level.progress < 0.9f)
        {
            progressBar.value = Mathf.Lerp(progressBar.value, level.progress, t);
            t += 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        t = 0.0f;
        while (t < 0.95f)
        {
            progressBar.value = Mathf.Lerp(progressBar.value, 1, t);
            t += 0.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Allow press key/button to continue
        finishedLoading = true;
        loadingText.text = "Press any button to continue";
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            ReceiveInput();
        }
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Any.performed += ctx => ReceiveInput();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }


    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void ReceiveInput() 
    {
        if (finishedLoading)
        {
            level.allowSceneActivation = true;
        }
    }
}
