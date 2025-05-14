using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script covers popups/tooltips that appear when the player can interact with something
public class PopUp : MonoBehaviour
{
    public GameObject playerHUD;
    public GameObject F;
    public RectTransform FTransform;
    public GameObject SendIt;
    public RectTransform SendTransform;
    public static PopUp instance;
    public bool canInteract = false;

    private Outline currentlyActive;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;  //Allows this script be called in other scripts
        //Finds UI elements used in scene
        if (GameObject.Find("PlayerHUD") != null)
        {
            playerHUD = GameObject.Find("PlayerHUD");
            if (playerHUD.transform.Find("Interact Canvas") != null)
            {
                F = playerHUD.transform.Find("Interact Canvas").transform.Find("Press F Panel").gameObject;
                FTransform = F.GetComponent<RectTransform>();
                SendIt = playerHUD.transform.Find("Interact Canvas").transform.Find("Send It Panel").gameObject;
                //SendTransform = SendIt.GetComponent<RectTransform>();
            }
        }
        
        //F.transform.LookAt(Camera.main.transform);
        //F.transform.Rotate(0, 180, 0);
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(currentlyActive != null)
            {
                currentlyActive.OutlineMode = Outline.Mode.OutlineHidden;
                currentlyActive = null;
            }
        }
    }

    void ActivatePanel(GameObject obj)
    {
        if(obj.GetComponentInParent<Outline>())
        {
            obj.GetComponentInParent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            currentlyActive = obj.GetComponentInParent<Outline>();
        }
        F.SetActive(true);
    }

    public void OnTriggerStay(Collider other)
    {
        if (IngredientInteraction.instance.canSend)
        {
            SendIt.SetActive(true);
        }
        if (other.tag != "Ignore" && other.tag != "Bullet" && other.tag != "Good" && other.tag != "Bad" && other.tag != "Cooked")
        {
            //Makes the Press F popup appear when the player is holding an ingredient next to a relevant station, or next to the oven when it's done cooking
            if(Holding.instance != null && OvenTimer.instance != null)
            {
                if ((canInteract == true && Holding.instance.Carrying == true) || OvenTimer.instance.canTake == true)
                {
                    if (Holding.instance.Carrying)
                    {
                        if (!(other.CompareTag("Oven") && Holding.instance.heldObj.CompareTag("Cooked")))
                        {
                            ActivatePanel(other.gameObject);
                        }
                    }
                    else if (OvenTimer.instance.canTake && other.tag == "Oven")
                    {
                        ActivatePanel(other.gameObject);
                    }
                    float OffsetPositionZ = other.transform.position.z + 4f;
                    Vector3 offsetPosition = new Vector3(other.transform.position.x, other.transform.position.y, OffsetPositionZ);
                    Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPosition);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(playerHUD.transform.Find("Interact Canvas").GetComponent<RectTransform>(), screenPoint, null, out Vector2 newPos);
                    Debug.Log(newPos);
                    //FTransform.localPosition = Camera.main.ScreenToViewportPoint(other.transform.position);

                    if (newPos.x < 0)
                    {
                        newPos.x = newPos.x + (newPos.x / 8);
                    }



                    FTransform.localPosition = newPos;
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponentInParent<Outline>())
        {
            if(other.gameObject.GetComponentInParent<Outline>().OutlineMode != Outline.Mode.OutlineHidden)
            {
                other.gameObject.GetComponentInParent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            }
        }
        //Makes popup dissapear when leaving a station
        if(F != null)
        {
            F.SetActive(false);
        }
        if(SendIt != null)
        {
            SendIt.SetActive(false);
        }
    }
}
