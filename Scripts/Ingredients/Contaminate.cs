using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contaminate : MonoBehaviour
{
    [SerializeField] private GameObject badIngredient;
    private Transform player;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Bad Ingredient(Clone)" && other.gameObject.transform.parent != null)
        {
            Debug.Log("Contaminate");
            player = GameObject.Find("Player").transform;
            gameObject.SetActive(false);
            transform.parent = null;
            Instantiate(badIngredient, player.position, Quaternion.identity);
        }
    }
}
