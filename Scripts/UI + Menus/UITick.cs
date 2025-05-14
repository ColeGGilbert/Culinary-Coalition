using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITick : MonoBehaviour
{
    private int currentTick = 0;
    [SerializeField] private GameObject[] tick;       //references to gameobject ticks on the player HUD

    private void OnEnable()
    {
        //IngredientInteraction.FoodPlaced += UpdateTick;       //keeps track of event from ingredient scripts 
    }

    private void OnDisable()
    {
        //IngredientInteraction.FoodPlaced -= UpdateTick;
    }
    
    void UpdateTick()
    {
        currentTick ++;
        for (int i = 1; i < tick.Length + 1; i++) 
        {
            if (currentTick >= i) tick[i - 1].SetActive(true);
        }
        if (currentTick == tick.Length) StartCoroutine(DisableTicks());
    }

    private IEnumerator DisableTicks()      //resets all the ticks for a new phase 
    {
        currentTick = 0;
        yield return new WaitForSeconds(4);     //ticks are reset after a set amount of time, should be better linked to phase in the future 
        ResetTicks();
        for (int i = 1; i < tick.Length + 1; i++)
        {
            if (currentTick >= i) tick[i - 1].SetActive(true);      //updates ticks after the reset incase the player servered something during the delay
        }       
    }

    private void ResetTicks()
    {
        for (int i = 0; i < tick.Length; i++)
        {
            tick[i].SetActive(false);
        }
    }
}
