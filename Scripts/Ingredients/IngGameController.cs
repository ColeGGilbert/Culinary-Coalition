using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using TMPro;

public class IngGameController : MonoBehaviour
{
    public delegate void ServedCombo();
    public static event ServedCombo OnServedCombo;

    public delegate void CameraShake(float shakeAmount);
    public static event CameraShake OnCameraShake;

    public delegate void IngPlaced();
    public static event IngPlaced OnIngPlaced;

    public static IngGameController instance;
    [SerializeField] private float ComboStress;

    [SerializeField] Animation newPhaseanim;

    public enum IngTypes 
    {
        Banana,
        Apple,
        Mango,
        Pineapple,
        Watermelon,
        Strawberry,
        Cookie,
        Marshmallow,
        IceCream,
        Chocolate,
        Waffle,
        RockCandy,
    }

    [System.Serializable]
    public class Combo
    {
        public string comboName;
        public IngredientType[] combination;
        public int bonusPoints;
        public Sprite comboImage;
    }

    [Space(10)]
    public List<Combo> IngredientCombinations;

    [Space(10)]
    public IngredientType[] SpawnableIngredients;

    public delegate void IngredientEvents();
    public static event IngredientEvents SatisfactionUpdated;

    [HideInInspector] int currentSatisfaction;
    [HideInInspector] int projectedSatisfaction;

    [Space(10)]
    [SerializeField] int requiredSatisfaction;

    [SerializeField] Image comboSprite;

    public int ingsPerMeal = 3;

    [SerializeField] float comboImageDuration;

    void Awake()
    {
        instance = this;
        
    }

    private void OnEnable()
    {
        IngredientInteraction.FoodPlaced += CalculateProjected;
    }

    public void ServeMeal(List<IngredientType> served)
    {
        currentSatisfaction += PointCheck(served);
        SatisfactionUpdated?.Invoke();
    }

    public int[] GetSatisfaction()
    {
        return new int[3]{currentSatisfaction, requiredSatisfaction, projectedSatisfaction};
    }

    public bool AmISatisfied()
    {
        return currentSatisfaction >= requiredSatisfaction;
    }

    public void ResetSatisfaction()
    {
        newPhaseanim.Play();
        currentSatisfaction = 0;
        projectedSatisfaction = 0;
        SatisfactionUpdated?.Invoke();
       
    }

    void CalculateProjected(List<IngredientType> onTable)
    {
        projectedSatisfaction = currentSatisfaction + PointCheck(onTable, true);
        OnIngPlaced?.Invoke();
        //Debug.Log("Projected Satisfaction: " + projectedSatisfaction);
    }

    public Combo CheckForCombos(IngredientType ing)
    {
        List<Combo> possibleCombos = new List<Combo> { };

        foreach(Combo com in IngredientCombinations)
        {
            foreach(IngredientType type in com.combination)
            {
                if(type == ing)
                {
                    possibleCombos.Add(com);
                }
            }
        }

        if(possibleCombos.Count == 0)return null;
        else
        {
            return possibleCombos[Random.Range(0, possibleCombos.Count)];
        }
    }

    int PointCheck(List<IngredientType> ings, bool doNotSend = false)
    {
        int highestPointTotal = 0;
        int totalPoints = 0;
        foreach(IngredientType ing in ings)
        {
            totalPoints += ing.points;
        }
        if(totalPoints > highestPointTotal)
        {
            highestPointTotal = totalPoints;
        }
        foreach (Combo comb in IngredientCombinations)
        {
            int ingsInCommon = 0;
            IngredientType[] com = new IngredientType[comb.combination.Length]; comb.combination.CopyTo(com, 0);
            for (int i = 0; i < ings.Count; i++)
            {
                for (int j = 0; j < com.Length; j++)
                {
                    if (com[j] != null && ings[i] != null)
                    {
                        if (com[j] == ings[i])
                        {
                            ingsInCommon++;
                            com[j] = null;
                            break;
                        }
                    }
                }
            }
            // If it's a combo meal
            if (ingsInCommon == comb.combination.Length)
            {
                if (!doNotSend)
                {
                    RuntimeManager.StudioSystem.setParameterByName("isCombo", 1);   // Update FMOD parameter
                    OnServedCombo?.Invoke();
                    OnCameraShake?.Invoke(ComboStress);
                    projectedSatisfaction = currentSatisfaction;
                }

                if (highestPointTotal < totalPoints + comb.bonusPoints)
                {
                    highestPointTotal = totalPoints + comb.bonusPoints;
                }

                if (comb.comboImage != null && !doNotSend)
                {
                    comboSprite.transform.parent.gameObject.SetActive(true); comboSprite.sprite = comb.comboImage; comboSprite.GetComponentInChildren<TextMeshProUGUI>().text = comb.comboName;

                    StartCoroutine(GameManager.instance.DelayInactive(comboImageDuration, comboSprite.transform.parent.gameObject));
                }

            }
        }
        return highestPointTotal;
    }
}
