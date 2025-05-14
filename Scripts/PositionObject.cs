using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPosition", menuName = "ScriptableObjects/ObjectPosition", order = 1)]
public class PositionObject : ScriptableObject
{
    public Vector3 position;
    public Quaternion rotation;
    public Transform parentObject;

    public PositionObject(Vector3 pos, Quaternion rot, Transform parent)
    {
        position = pos;
        rotation = rot;
        parentObject = parent;
    }
}
