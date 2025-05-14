using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttatchToPlayer : MonoBehaviour
{

    [SerializeField] GameObject player;

    private void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }
}
