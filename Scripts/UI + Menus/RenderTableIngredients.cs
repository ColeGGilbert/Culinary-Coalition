using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTableIngredients : MonoBehaviour
{

    [SerializeField] Sprite defaultImage;
    [SerializeField] SpriteRenderer[] ingredientSlots;

    private void OnEnable()
    {
        IngredientInteraction.FoodPlaced += UpdateUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        ClearUI();
    }

    // Update is called once per frame
    void UpdateUI(List<IngredientType> ings)
    {
        for(int i = 0; i < ings.Count; i++)
        {
            ingredientSlots[i].sprite = ings[i].ingredientImage;
        }
        
        if(ings.Count == 0)
        {
            ClearUI();
        }
    }

    void ClearUI()
    {
        foreach (SpriteRenderer spr in ingredientSlots)
        {
            spr.sprite = defaultImage;
        }
    }

    private void OnDisable()
    {
        IngredientInteraction.FoodPlaced -= UpdateUI;
    }
}
