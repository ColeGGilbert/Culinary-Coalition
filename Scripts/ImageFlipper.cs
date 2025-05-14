using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using TMPro;
using UnityEngine.SceneManagement;

public class ImageFlipper : MonoBehaviour
{
    [SerializeField] private GameObject textObject;

    [SerializeField] private Sprite[] intropages;
    [SerializeField] private string[] introtext;
    private int currentText = 0;
    private Image image = null;
    private TextMeshProUGUI text = null;
    private Player controls;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        text = textObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.F))
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        currentText++;

        if (currentText >= 9)
        {
            End();
        }
        else
        {
            image.sprite = intropages[currentText];
            text.text = introtext[currentText];
        }
    }

    private void End()
    {
        InitialiseFMODParameters();
        Time.timeScale = 1;
        GameManager.instance.sceneToLoad = 1;
        SceneManager.LoadScene(5);
    }

    private void InitialiseFMODParameters()
    {
        RuntimeManager.StudioSystem.setParameterByName("isLowHealth", 0);
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Interaction.performed += ctx => UpdateText();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
