using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeOnStart : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onStart = null;

    private void Start()
    {
        onStart?.Invoke();
    }
}
