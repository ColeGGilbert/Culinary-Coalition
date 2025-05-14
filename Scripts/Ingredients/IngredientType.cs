using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "ScriptableObjects/Ingredient", order = 1)]
public class IngredientType : ScriptableObject
{
    public string ingName;
    public IngGameController.IngTypes ingType;
    public int points;
    public Vector3 size;
    public Mesh modelPrefab;
    public Material material;
    public Material cookedMaterial;
    public Sprite ingredientImage;

    public IngredientType(int i_points, string i_ingName, Mesh i_model, IngGameController.IngTypes i_type, Material i_material)
    {
        points = i_points;
        ingName = i_ingName;
        modelPrefab = i_model;
        ingType = i_type;
        material = i_material;
    }
}
