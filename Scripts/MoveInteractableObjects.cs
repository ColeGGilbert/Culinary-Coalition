using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject
{
    public PositionObject[] position;
    public GameObject obj;

    public InteractableObject(PositionObject[] pos, GameObject ob)
    {
        position = pos;
        obj = ob;
    }
}

public class MoveInteractableObjects : MonoBehaviour
{
    [Header("Scriptable Objects containing position information")]
    [SerializeField] PositionObject[] ovenPosition;
    [SerializeField] PositionObject[] binPosition;
    [SerializeField] PositionObject[] treePosition;
    [SerializeField] PositionObject[] mixerPosition;
    [SerializeField] PositionObject[] tablePosition;

    [Header("Interactable Objects in this level")]
    [Space(10)]
    [Tooltip("This will determine what objects can be moved in the scene (NO DUPLICATES)")][SerializeField] InteractableObjects[] objectsInScene;

    [Header("Object references to move")]
    [Space(10)]
    [SerializeField] GameObject ovenObj;
    [SerializeField] GameObject binObj;
    [SerializeField] GameObject treeObj;
    [SerializeField] GameObject mixerObj;
    [SerializeField] GameObject tableObj;

    Dictionary<int, InteractableObject> compiledObjects;

    private enum InteractableObjects
    {
        oven,
        bin,
        tree,
        mixer,
        table,
    }

    private void OnEnable()
    {
        compiledObjects = new Dictionary<int, InteractableObject> { };
        BulletSpawning.OnChange += MoveObjects;
    }

    private void OnDisable()
    {
        BulletSpawning.OnChange -= MoveObjects;
    }

    private void Start()
    {
        int i = 0;
        foreach(InteractableObjects interactable in objectsInScene)
        {
            switch (interactable)
            {
                case (InteractableObjects.oven):
                    compiledObjects.Add(i, new InteractableObject(ovenPosition, ovenObj));
                    break;

                case (InteractableObjects.bin):
                    compiledObjects.Add(i, new InteractableObject(binPosition, binObj));
                    break;

                case (InteractableObjects.tree):
                    compiledObjects.Add(i, new InteractableObject(treePosition, treeObj));
                    break;

                case (InteractableObjects.mixer):
                    compiledObjects.Add(i, new InteractableObject(mixerPosition, mixerObj));
                    break;

                case (InteractableObjects.table):
                    compiledObjects.Add(i, new InteractableObject(tablePosition, tableObj));
                    break;
            }
            i++;
        }
    }

    void MoveObjects(int phase)
    {
        for(int i=0; i < compiledObjects.Count; i++)
        {
            UpdatePosition(i, phase-1);
        }
    }

    void UpdatePosition(int key, int phase)
    {
        if (compiledObjects[key].position.Length >= phase+1)
        {

            if (compiledObjects[key].position[phase].position != null)
            {
                compiledObjects[key].obj.transform.position = compiledObjects[key].position[phase].position;
            }

            if (compiledObjects[key].position[phase].rotation != null)
            {
                compiledObjects[key].obj.transform.rotation = compiledObjects[key].position[phase].rotation;
            }

            if (compiledObjects[key].position[phase].parentObject != null)
            {
                compiledObjects[key].obj.transform.parent = compiledObjects[key].position[phase].parentObject;
            }
        }
    }
}
