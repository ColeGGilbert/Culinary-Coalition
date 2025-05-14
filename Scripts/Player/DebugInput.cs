using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInput : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerPressH = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            onPlayerPressH?.Raise();
        }
    }
}
