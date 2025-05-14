using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

//This script allows ingredients to be picked up by the player
public class PickUp : MonoBehaviour
{
    [Header("FMOD")]
    [EventRef] private string eCatch = "{0f9d256b-2937-4bd6-b805-c34c1594e9ec}";
    [EventRef] private string eCatchBad = "{f495ee96-f4ad-44b7-8807-48b7967bbe67}";
    [EventRef] private string eCatchGood = "{f732797b-46ab-42a8-8ae7-5a2781756caa}";
    [SerializeField] private ParticleSystem pickUpGood;
    [SerializeField] private ParticleSystem pickUpBad;

    public delegate void OnPickUp(IngredientType ing);
    public static event OnPickUp PickedUp;

    public IngredientType ingredientType;

    public bool amServed = false;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ignore") && !other.CompareTag("Oven") && !other.CompareTag("Table") && !other.CompareTag("Bullet") && !amServed)
        {
            //Contamination condition
            if (other.tag == "Player" && gameObject.name == "Bad Ingredient(Clone)" && Holding.instance.Carrying)
            {
                Holding.instance.heldObj.transform.parent = null;
                //Debug.Log("Work Pls");
                if (Holding.instance.heldObj.CompareTag("Cooked"))
                {
                    Destroy(Holding.instance.heldObj);
                }
                else
                {
                    Holding.instance.heldObj.SetActive(false);
                    Holding.instance.heldObj.transform.parent = null;
                }
                Holding.instance.heldObj = null;
                Holding.instance.Carrying = false;
            }

            if (other.tag == "Player" && Holding.instance.Carrying == false)
            {
                //Stops all coroutines related to bullet-like movement
                RuntimeManager.PlayOneShot(eCatch, transform.position);
                if (GetComponent<BulletMovement>())
                {
                    GetComponent<BulletMovement>().StopAllCoroutines();
                }
                //Attaches to the player as a child object
                enabled = false;
                gameObject.layer = 0;
                transform.parent = other.GetComponent<IngredientInteraction>().fryingPanObjectRef.transform;
                gameObject.layer = 0;
                transform.localPosition = new Vector3(0, 1.5f, 0);
                Holding.instance.Carrying = true;
                Holding.instance.heldObj = gameObject;
                if(ingredientType != null)
                {
                    Holding.instance.heldIngType = ingredientType;
                    // Activate PickedUp Render PopUp
                    PickedUp?.Invoke(ingredientType);
                }
                
                //Paritcles
                if (gameObject.name == "Good Ingredient(Clone)")
                {
                    RuntimeManager.PlayOneShot(eCatchGood);
                    if (pickUpGood != null) { pickUpGood.Play(); }
                }
                if (gameObject.name == "Bad Ingredient(Clone)")
                {
                    RuntimeManager.PlayOneShot(eCatchBad);
                    if (pickUpBad != null) { pickUpBad.Play(); }
                }

                other.GetComponent<IngredientInteraction>().UpdateHeldUI();

                Destroy(this);
            }
            else if (Holding.instance.Carrying == true && other.CompareTag("Player") && Holding.instance.heldObj != gameObject)
            {
                gameObject.SetActive(false);
            }

            if (other.CompareTag("Player"))
            {
                other.GetComponent<IngredientInteraction>().UpdateHeldUI();
            }
        }
    }
}