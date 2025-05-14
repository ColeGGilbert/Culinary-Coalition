using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
    [SerializeField] GameObject good;
    [SerializeField] GameObject bad;
    [SerializeField] GameObject cooked;
    [SerializeField] GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        debugSpawn();
    }

    private void debugSpawn()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(good, new Vector3(-3f,1f,9f),Quaternion.identity);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
             Instantiate(bad, new Vector3(0,1f,9f), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
             Instantiate(cooked, new Vector3(3f,1f,9f), Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
               Instantiate(bullet, new Vector3(6f,1f,9f), Quaternion.identity);
        }
    }
}
