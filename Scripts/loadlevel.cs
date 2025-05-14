using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadlevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K))
        {
            LoadALevel(7);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadALevel(10);
        }*/
    }

    private void LoadALevel(int level) 
    {
        GameManager.instance.sceneToLoad = level;
        SceneManager.LoadScene(5);
    }

    public void LoadHub()
    {
        GameManager.instance.sceneToLoad = 1;
        SceneManager.LoadScene(5);
    }

    public void GameOver()
    {
        GameManager.instance.currentLevel = SceneManager.GetActiveScene().buildIndex; // Updates which scene should be loaded by Retry button in Game Over scene
        SceneManager.LoadScene(3); // Loads Game Over scene
    }
}
