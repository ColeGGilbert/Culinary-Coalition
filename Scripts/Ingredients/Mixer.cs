using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mixer : MonoBehaviour
{
    [SerializeField] private GameObject cookedIngredient;
    private int amountOfIngredients = 0;
    [SerializeField] private int maxAmountOfIngredients = 3;
    private float amountMixed;
    [SerializeField] private float MaxMixed = 5;
    private bool updateMixer;
    private bool canMix;
    private bool insideMixer; // inside Mixer Zone

    [SerializeField] Slider slider;
    [SerializeField] Image[] slots;
    [SerializeField] GameObject mixerPanel;
    [SerializeField] GameObject slotPanel;

    private void Start()    //use get child for the refrences in the UI in the future 
    {
        slotPanel.SetActive(true);         
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if (updateMixer) UpdateMixer();
            if (insideMixer && canMix && Input.GetKey(KeyCode.F)) Mixing();

            //Debug.Log(insideMixer);
        }
    }

    private void UpdateMixer()
    {
        if (amountOfIngredients < maxAmountOfIngredients)
        {
            amountOfIngredients++;
            UpdateSlots();
            updateMixer = false;
            if (amountOfIngredients == maxAmountOfIngredients)
            {
                canMix = true;      //start the mixing function
                mixerPanel.SetActive(true);
            }
        }
    }

    private void Mixing()
    {
        amountMixed += Time.deltaTime;
        UpdateSlider();
        if (amountMixed >= MaxMixed)
        {
            amountMixed = 0;        //spawns cooked ingredients and resets values
            amountOfIngredients = 0;
            canMix = false;
            Instantiate(cookedIngredient, new Vector3(transform.position.x + 2.5f, transform.position.y, transform.position.z), Quaternion.identity);
            ResetUI();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            canMix = false;     //runs when the user stops mixing and the ingredients need more mixing 
        }
    }

    private void StartMixer() 
    {
        updateMixer = true;
    }

    void OnEnable()
    {
        IngredientInteraction.OnMix += StartMixer;     //listens for event in Ingredient script
    }

    void OnDisable()
    {
        IngredientInteraction.OnMix -= StartMixer;
    }

    private void UpdateSlider()
    {
        slider.value = (amountMixed / MaxMixed) * 100;
    }

    private void UpdateSlots()      //changes the UI slots to green
    {
        if (amountOfIngredients == 1) slots[0].GetComponent<Image>().color = Color.green;
        else if (amountOfIngredients == 2) slots[1].GetComponent<Image>().color = Color.green;
        else slots[2].GetComponent<Image>().color = Color.green;
    }

    private void ResetUI()
    {
        foreach (Image element in slots)        //resets UI back to white
        {
            element.color = Color.white;
        }
        slider.value = 0;
        mixerPanel.SetActive(false);
        slotPanel.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the player is inside the mixer zone
        if (other.CompareTag("Player"))
        {
            insideMixer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Checks if the player has left the mixer zone
        if (other.CompareTag("Player"))
        {
            insideMixer = false;
        }
    }
}
