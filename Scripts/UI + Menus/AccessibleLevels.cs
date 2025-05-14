using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessibleLevels : MonoBehaviour
{
    [SerializeField] private bool canAccess2 = false;
    [SerializeField] private bool canAccess3 = false;
    [SerializeField] private bool canAccess4 = false;
    [SerializeField] private GameObject button2;
    [SerializeField] private GameObject button3;
    [SerializeField] private GameObject button4;

    // Start is called before the first frame update
    void Start()        //script needs to be updated
    {
        UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpdateButtons();
        }
    }

    private void UpdateButtons()
    {
        button2.GetComponent<Button>().enabled = canAccess2;
        button3.GetComponent<Button>().enabled = canAccess3;
        button4.GetComponent<Button>().enabled = canAccess4;
    }

    /*
    private void UpdateButtons() 
    {
        if (canaccess2)
        {
            button2.SetActive(true);
        }
        else
        {
            button2.SetActive(false);
        }

        if (canaccess3)
        {
            button3.SetActive(true);
        }
        else
        {
            button3.SetActive(false);
        }

        if (canaccess4)
        {
            button4.SetActive(true);
        }
        else
        {
            button4.SetActive(false);
        }
    }
    */
}
