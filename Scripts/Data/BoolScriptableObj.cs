using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bool", menuName = "ScriptableObjects/DataTypes/Bool")]
public class BoolScriptableObj : ScriptableObject
{
    private bool value = false;

    [SerializeField]
    private GameEventBool onValueChanged = null;

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool value)
    {
        this.value = value;
        onValueChanged?.Raise(value);
    }

    public void InvertValue()
    {
        value = !value;
        onValueChanged?.Raise(value);
    }
}
