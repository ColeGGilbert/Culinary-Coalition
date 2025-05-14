using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RenderPickedUp : MonoBehaviour
{

    [SerializeField] bool includeIngredientName;
    [SerializeField] GameObject uiHolder;
    [SerializeField] float zOffset = 1;
    private GameObject player;
    SpriteRenderer ingredientImage;
    TextMeshProUGUI pickedUp_text;

    private void OnEnable()
    {
        PickUp.PickedUp += UpdateUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        ingredientImage = uiHolder.GetComponentInChildren<SpriteRenderer>();

        if (includeIngredientName)
        {
            pickedUp_text = uiHolder.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void UpdateUI(IngredientType ing)
    {
        StopAllCoroutines();

        transform.position = new Vector3(player.transform.position.x, 4.5f, player.transform.position.z + zOffset);

        uiHolder.SetActive(true);

        ingredientImage.sprite = ing.ingredientImage;

        if (includeIngredientName)
        {
            pickedUp_text.text =  ing.ingName + "!";
        }

        StartCoroutine(DelayInactive(2));
    }

    IEnumerator DelayInactive(float time)
    {
        yield return new WaitForSeconds(time);
        uiHolder.SetActive(false);
    }

    private void OnDisable()
    {
        PickUp.PickedUp -= UpdateUI;
    }
}
