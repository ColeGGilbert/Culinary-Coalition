using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Options : MonoBehaviour
{

    Resolution[] resolutions;
    bool firstPressed = false;

    public delegate void SaveFileDeleted();
    public static event SaveFileDeleted OnDeleteSave;

    [SerializeField] TextMeshProUGUI confirmDeleteText;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

     void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)

            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void DeleteSaveFile()
    {
        if(firstPressed == false)
        {
            firstPressed = true;
            StartCoroutine(ConfirmDeleteFile());
        }
        else if(firstPressed == true)
        {
            StopAllCoroutines();
            confirmDeleteText.enabled = false;
            firstPressed = false;
            string path = Application.persistentDataPath + "/gameSaveData.sfs";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.LogError("File not found at path: " + path);
            }
            OnDeleteSave();
            Time.timeScale = 1;
            GameManager.instance.sceneToLoad = 0;
            SceneManager.LoadScene(5);
        }
    }

    IEnumerator ConfirmDeleteFile()
    {
        confirmDeleteText.enabled = true;
        confirmDeleteText.text = "PRESS AGAIN TO CONFIRM (3...)";
        yield return new WaitForSecondsRealtime(1);
        confirmDeleteText.text = "PRESS AGAIN TO CONFIRM (2...)";
        yield return new WaitForSecondsRealtime(1);
        confirmDeleteText.text = "PRESS AGAIN TO CONFIRM (1...)";
        yield return new WaitForSecondsRealtime(1);
        confirmDeleteText.enabled = false;
        firstPressed = false;
    }
}
