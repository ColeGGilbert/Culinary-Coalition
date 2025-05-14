using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventBoolOnEnable : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The event which should be raised on enable.")]
    private GameEventBool onEnable = null;

    [SerializeField]
    [Tooltip("The value which should be passed when onEnable event is raised.")]
    private BoolScriptableObj value = null;

    private void Start()
    {
        onEnable?.Raise(value.GetValue());
    }
    private void OnEnable()
    {
        onEnable?.Raise(value.GetValue());
    }
}
