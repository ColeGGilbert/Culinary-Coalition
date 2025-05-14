using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RenderCookedIngredient : MonoBehaviour
{

    [SerializeField] bool includeIngredientName;
    [SerializeField] GameObject holder;
    [SerializeField] float yOffset;
    SpriteRenderer ingImage;
    TextMeshProUGUI text;
    GameObject oven;

    private void OnEnable()
    {
        Oven.ItemCooked += UpdateUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        oven = GameObject.FindGameObjectWithTag("Oven");
        text = holder.GetComponentInChildren<TextMeshProUGUI>();
        ingImage = holder.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void UpdateUI(IngredientType ing)
    {
        holder.SetActive(true);
        holder.transform.position = new Vector3(oven.transform.position.x, oven.transform.position.y + yOffset, oven.transform.position.z);
        if (includeIngredientName)
        {
            text.text = "Cooked " + ing.name + "!";
        }
        ingImage.sprite = ing.ingredientImage;
        StartCoroutine(DelayInactive(holder, 1.5f));
    }

    private void OnDisable()
    {
        Oven.ItemCooked -= UpdateUI;
    }

    IEnumerator DelayInactive(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
