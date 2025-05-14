using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public void SaveSettings()
    {
        Debug.Log("Prefs Saved");
        PlayerPrefs.Save();
    }
}
