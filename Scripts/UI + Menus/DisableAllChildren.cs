using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAllChildren : MonoBehaviour
{
    public void Disable()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
