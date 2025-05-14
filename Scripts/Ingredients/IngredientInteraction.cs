using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

//This script covers the player interacting with various ingredients and stations
public class IngredientInteraction : MonoBehaviour
{
    [Header("FMOD")]
    [EventRef] private string eToss = "{30db7632-f299-479d-a5f8-42e1fa0aa5aa}";
    [EventRef] private string eServe = "{095c68da-3d54-417b-9055-a5edd77b2aac}";
    [EventRef] private string eServeFlourish = "{5815f704-b708-4517-8221-2f3ff0c953e6}"; // Flourish sound, not spatialised

    public static IngredientInteraction instance;
    private Player controls;

    [SerializeField] ParticleSystem servedPS;
    [SerializeField] Animator tableAnim;

    public bool canBin;
    public bool canCook;
    public bool canServe;
    public bool canSend;
    public bool canMix;

    public delegate void PlacedFood(List<IngredientType> served);
    public static event PlacedFood FoodPlaced;
    public delegate void MixerOn();
    public static event MixerOn OnMix;
    public delegate void OvenActive();
    public static event OvenActive OnOvenStart;

    public delegate void RemoveItem();
    public static event RemoveItem OnItemUsed;

    public GameObject table;
    public GameObject oven;
    public GameObject mixer;

    [SerializeField] Image heldObjImage;
    [SerializeField] Sprite badIngImage;
    [SerializeField] Sprite uiMask;
    public GameObject fryingPanObjectRef;

    private List<IngredientType> itemsOnTable = new List<IngredientType>();
    private List<GameObject> itemsOnTableObj = new List<GameObject>();

    float distanceBetweenIngs;

    private void Awake()
    {
        controls = new Player();
        controls.Gameplay.Interaction.performed += ctx => Interact();
        controls.Gameplay.ServeEarly.performed += ctx => SendEarly();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        instance = this;  //Allows this script be called in other scripts
        if(IngGameController.instance != null)
        {
            distanceBetweenIngs = (2*transform.localScale.x) / IngGameController.instance.ingsPerMeal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.paused)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Interact();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SendEarly();
            }

            /*
            Debug.Log("displayF: " + displayF);
            if (displayF == true && gameObject.transform.parent != null)
            {
                F.SetActive(true);
            }
            else
            {
                Debug.Log("not Displaying");
                F.SetActive(false);
            }
            */
        }
    }

    public void OnTriggerStay(Collider other)
    {
        //Decicion that determines what events can happen depending on which station the player is at/ingredient they are holding
        if(Holding.instance != null)
        {
            if (Holding.instance.Carrying)
            {
                if (other.tag == "Bin")
                {
                    canBin = true;
                    PopUp.instance.canInteract = true;
                }
                else if (other.tag == "Oven" && Holding.instance.heldObj.tag == "Good" && (!Oven.instance.isCooking || Oven.instance.isBurning))
                {
                    canCook = true;
                    oven = other.gameObject;
                    PopUp.instance.canInteract = true;
                }
                else if (other.tag == "Oven" && Holding.instance.heldObj.tag != "Good")
                {
                    canCook = false;
                    PopUp.instance.canInteract = false;
                }
                else if (other.tag == "Table")
                {
                    if (Holding.instance.heldObj.tag == "Cooked")
                    {
                        canServe = true;
                        table = other.gameObject;
                        PopUp.instance.canInteract = true;
                    }
                }
                else if (other.tag == "Table" && Holding.instance.heldObj.tag != "Cooked")
                {
                    canServe = false;
                    PopUp.instance.canInteract = false;
                }
                else if (other.tag == "Mixer" && Holding.instance.heldObj.tag == "Good")
                {
                    canMix = true;
                    mixer = other.gameObject;
                    PopUp.instance.canInteract = true;
                }

            }
            else
            {
                canBin = false;
                canCook = false;
                canServe = false;
                canMix = false;
                PopUp.instance.canInteract = false;
            }

            if (other.tag == "Table" && itemsOnTable.Count > 0)  //used to check if you can send cooked ings off early
            {
                canSend = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(Holding.instance != null)
        {
            //Ensures the player can't interact with a station after leaving it
            if (Holding.instance.Carrying)
            {
                if (other.tag == "Bin")
                {
                    canBin = false;
                }
                else if (other.tag == "Oven")
                {
                    canCook = false;
                }
                else if (other.tag == "Table")
                {
                    canServe = false;
                    canSend = false;
                }
                else if (other.tag == "Mixer")
                {
                    canMix = false;
                }

                PopUp.instance.canInteract = false;
            }
            else if (other.tag == "Table")  //used to check if you can send cooked ings off early
            {
                canSend = false;
            }
        }
    }

    private void Interact()  //This function covers the player using a held ingredient at the different stations
    {
        if (Time.timeScale > 0)
        {
            if (canBin)
            {
                canBin = false;
                RuntimeManager.PlayOneShot(eToss, transform.position);
                Holding.instance.heldObj.transform.parent = null;
                Holding.instance.heldObj.SetActive(false);
                Holding.instance.Carrying = false;
                Holding.instance.heldObj = null;
            }
            else if (canCook && Oven.instance.isCooking == false)
            {
                canCook = false;
                //RuntimeManager.PlayOneShot(ePutInOven, transform.position);
                Holding.instance.heldObj.transform.parent = oven.transform;
                Holding.instance.heldObj.transform.localPosition = new Vector3(0, 1, 0);
                OnOvenStart?.Invoke();        //event to start the oven in the oven script
                PopUp.instance.F.SetActive(false);
            }
            else if (canServe)
            {
                canServe = false;
                Holding.instance.heldObj.transform.parent = table.transform;
                if (servedPS != null)
                {
                    servedPS.Play();

                }

                Holding.instance.heldObj.transform.localPosition = new Vector3(-distanceBetweenIngs + (itemsOnTable.Count * distanceBetweenIngs), 1, 0);
                itemsOnTable.Add(Holding.instance.heldObj.GetComponent<PickUp>().ingredientType);
                itemsOnTableObj.Add(Holding.instance.heldObj);
                Holding.instance.heldObj.GetComponent<PickUp>().amServed = true;
                Holding.instance.heldObj.transform.localScale = Holding.instance.heldIngType.size;
                Holding.instance.heldIngType = null;
                Holding.instance.Carrying = false;
                Holding.instance.heldObj = null;

                // Resets num of items on table
                if (itemsOnTable.Count == IngGameController.instance.ingsPerMeal)
                {
                    //Play Serve Animation
                    tableAnim.SetTrigger("sentOff");
                    IngGameController.instance.ServeMeal(itemsOnTable);
                    foreach (GameObject item in itemsOnTableObj)
                    {
                        Destroy(item, 3);
                    }
                    RuntimeManager.StudioSystem.setParameterByName("numServed", itemsOnTable.Count);    // Reset numServed FMOD param
                    itemsOnTable.Clear();
                    itemsOnTableObj.Clear();
                }

                FoodPlaced?.Invoke(itemsOnTable);        //event to update the satisfaction bar UI

                // FMOD: Play Serve SFX
                RuntimeManager.StudioSystem.setParameterByName("numServed", itemsOnTable.Count);
                RuntimeManager.PlayOneShot(eServe, transform.position);
                RuntimeManager.PlayOneShot(eServeFlourish);
            }
            else if (canMix)
            {
                Holding.instance.heldObj.transform.parent = null;
                Holding.instance.heldObj.SetActive(false);
                Holding.instance.Carrying = false;
                Holding.instance.heldObj = null;
                canMix = false;
                OnMix?.Invoke();        //event to start the mixer in the mixer script 
            }
            UpdateHeldUI();
        }
    }

    public void UpdateHeldUI()
    {
        if(Holding.instance != null)
        {
            if (Holding.instance.heldObj && (Holding.instance.heldIngType != null || Holding.instance.heldObj.CompareTag("Bad")))
            {
                if (Holding.instance.heldObj.CompareTag("Bad"))
                {
                    heldObjImage.sprite = badIngImage;
                }
                else
                {
                    heldObjImage.sprite = Holding.instance.heldIngType.ingredientImage;
                }
            }
            else
            {
                OnItemUsed?.Invoke();
                heldObjImage.sprite = uiMask;
            }
        }
    }

    private void SendEarly() 
    {
        if (canSend)
        {
            // Resets num of items on table
            IngGameController.instance.ServeMeal(itemsOnTable);
            //Play Serve Animation
            tableAnim.SetTrigger("sentOff");
            foreach (GameObject item in itemsOnTableObj)
            {
                Destroy(item, 3);
            }
            RuntimeManager.StudioSystem.setParameterByName("numServed", itemsOnTable.Count);    // Reset numServed FMOD param
            itemsOnTable.Clear();
            itemsOnTableObj.Clear();
            FoodPlaced?.Invoke(itemsOnTable);        //event to update the UI ticks


            // FMOD: Play Serve SFX
            RuntimeManager.StudioSystem.setParameterByName("numServed", itemsOnTable.Count);
            RuntimeManager.PlayOneShot(eServe, transform.position);
            RuntimeManager.PlayOneShot(eServeFlourish);
        }
    }
}
