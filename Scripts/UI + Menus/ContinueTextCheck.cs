using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class ContinueTextCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/gameSaveData.sfs";
        if (File.Exists(path))
        {
            GetComponent<TextMeshProUGUI>().text = "CONTINUE";
        }
        else
        {
            GetComponent<TextMeshProUGUI>().text = "PLAY";
        }
    }
}
