using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is a standalone scripts that can be used by any other script to keep track of what the player is currently holding
public class Holding : MonoBehaviour
{
    public static Holding instance;
    public bool Carrying = false;
    public GameObject heldObj;

    public IngredientType heldIngType;

    private Animator anim;

    void Start()
    {
        instance = this;
    }

    void FixedUpdate()
    {
        if (Carrying == true)
        {
            anim = heldObj.GetComponent<Animator>();
            if(anim != null)
            {
                anim.enabled = false;
            }
        }
    }
}
