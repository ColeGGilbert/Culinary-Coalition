using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDetector : MonoBehaviour
{
    private bool firstLetter;
    private bool secondLetter;
    private bool thirdLetter;
    private bool fourthLetter;
    [SerializeField] private GameObject secret;

    //private int[] passcode = new int[] { 0, 0, 1, 1, 2, 3, 2, 3 };
    private int[] passcode = new int[] { 0, 0, 1, 1, 2, 3, 2, 3, 4, 5, };
    private int currentLetter;

    private Player controls;
    private float currentTime;
    private float maxTime = 5;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            firstLetter = true;
            currentTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.A) && firstLetter)
        {
            secondLetter = true;
            currentTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.Y) && secondLetter)
        {
            thirdLetter = true;
            currentTime = 0;
        }

        if (Input.GetKeyDown(KeyCode.T) && thirdLetter)
        {
            fourthLetter = true;
            currentTime = 0;
        }

        if (fourthLetter) 
        {
            if (secret.active)
            {
                secret.SetActive(false);
            }
            else
            {
                secret.SetActive(true);
            }
            firstLetter = false;
            secondLetter = false;
            thirdLetter = false;
            fourthLetter = false;
        }

        Timer();
    }

    private void Timer()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= maxTime)
        {
            currentTime = 0;
            firstLetter = false;
            secondLetter = false;
            thirdLetter = false;
            fourthLetter = false;
        }
    }

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Up.performed += ctx => ControllerInput(0);
        controls.Gameplay.Down.performed += ctx => ControllerInput(1);
        controls.Gameplay.Left.performed += ctx => ControllerInput(2);
        controls.Gameplay.Right.performed += ctx => ControllerInput(3);

        controls.Gameplay.SkipDialogue.performed += ctx => ControllerInput(4);
        controls.Gameplay.Interaction.performed += ctx => ControllerInput(5);

    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void ControllerInput(int dir) 
    {
        //Debug.Log("test");
        //Debug.Log(passcode[currentLetter]);
        //Debug.Log(dir);

        if (passcode[currentLetter] == dir)
        {
            //Debug.Log("win");
            //Debug.Log(passcode.Length);
            //Debug.Log(currentLetter);

            currentTime = 0;
            currentLetter++;
            if (passcode.Length <= currentLetter)
            {
                //Debug.Log("win win");

                currentLetter = 0;

                if (secret.active)
                {
                    secret.SetActive(false);
                }
                else
                {
                    secret.SetActive(true);
                }
            }
        }
        else 
        {
            //Debug.Log("fail");
            currentLetter = 0;
        }
    }
}
