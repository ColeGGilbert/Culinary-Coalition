using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubTutorial : MonoBehaviour
{
    private bool WASDactive = true;
    private bool UIEnded;

    [SerializeField] private GameEvent onLeaveTrigger;

    private void OnTriggerExit(Collider other)
    {
        if (WASDactive && UIEnded) 
        {
            WASDactive = false;
            onLeaveTrigger?.Raise();
        }
    }

    public void TurnOnUI(bool skip) 
    {
        if (!UIEnded)
        {
            UIEnded = true;
        }

    }
    void OnEnable()
    {
        DialogueSystem.OnFinishedText += TurnOnUI;
    }

    void OnDisable()
    {
        DialogueSystem.OnFinishedText -= TurnOnUI;
    }
}
