/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IngGameController))]
public class IngGameController_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        IngGameController controller = target as IngGameController;

        DrawDefaultInspector();

        int i = 0;
        if(controller.CompiledIngredients == null)
        {
            controller.CompiledIngredients = new IngGameController.CompiledIng[controller.SpawnableIngredients.Count];
        }
        foreach(IngGameController.IngTypes ing in controller.SpawnableIngredients)
        {
            controller.CompiledIngredients[i].type = controller.SpawnableIngredients[i];
            i++;
        }
    }
}*/ // Old code that might be useful for trying to make autofill inspector results
