using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatisfactionUI_Updates : MonoBehaviour
{
    [SerializeField] RectTransform[] satisfactionMask;
    [SerializeField] Animation confirmAnim;
    Image satisfactionMaskImage;
    private float spacing;
    private float targetAlpha;
    private bool firstUpdate = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        IngGameController.SatisfactionUpdated += UpdateUI;
        IngGameController.OnIngPlaced += UpdateUI;
    }

    void Start()
    {
        if(confirmAnim == null)
        {
            confirmAnim = GetComponentInChildren<Animation>();
        }

        spacing = 290 / IngGameController.instance.GetSatisfaction()[1];
        targetAlpha = 0.1f;
        UpdateUI();
    }

    void UpdateUI()
    {
        satisfactionMask[0].offsetMin = new Vector2(IngGameController.instance.GetSatisfaction()[2] * spacing, satisfactionMask[0].offsetMin.y);
        satisfactionMask[1].offsetMax = new Vector2(-290 + IngGameController.instance.GetSatisfaction()[2] * spacing, satisfactionMask[1].offsetMax.y);
        satisfactionMask[1].offsetMin = new Vector2(IngGameController.instance.GetSatisfaction()[0] * spacing, satisfactionMask[1].offsetMin.y);
        if (!firstUpdate)
        {
            confirmAnim.Play();
        }
        else
        {
            firstUpdate = false;
        }
    }

    private void Update()
    {
        if(satisfactionMaskImage == null)
        {
            satisfactionMaskImage = satisfactionMask[1].GetComponent<Image>();
        }

        if(satisfactionMaskImage.color.a >= 0.69f)
        {
            targetAlpha = 0.1f;
        }
        if(satisfactionMaskImage.color.a <= 0.11f)
        {
            targetAlpha = 0.7f;
        }
        satisfactionMaskImage.color = Color.Lerp(satisfactionMaskImage.color, new Color(satisfactionMaskImage.color.r, satisfactionMaskImage.color.g, satisfactionMaskImage.color.b, targetAlpha), 0.1f);
    }

    void OnDisable()
    {
        IngGameController.SatisfactionUpdated -= UpdateUI;
        IngGameController.OnIngPlaced -= UpdateUI;
    }

    /*
    [SerializeField] GameObject lineHolder;
    List<GameObject> activeLines = new List<GameObject>();

    [SerializeField] GameObject overlayLineHolder;
    [SerializeField] Sprite filledImage;
    [SerializeField] Sprite emptyImage;

    // Start is called before the first frame update
    void OnEnable()
    {
        IngGameController.SatisfactionUpdated += UpdateUI;
    }

    void Start()
    {
        for(int i=0; i< lineHolder.transform.childCount; i++)
        {
            if(i < IngGameController.instance.GetSatisfaction()[1]-1)
            {
                activeLines.Add(lineHolder.transform.GetChild(i).gameObject);
            }
            else
            {
                lineHolder.transform.GetChild(i).gameObject.SetActive(false);
                overlayLineHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        for(int i=0; i < activeLines.Count; i++)
        {
            if(i < IngGameController.instance.GetSatisfaction()[0])
            {
                activeLines[i].GetComponent<Image>().sprite = filledImage;
            }
            else
            {
                activeLines[i].GetComponent<Image>().sprite = emptyImage;
            }
        }
    }

    void OnDisable()
    {
        IngGameController.SatisfactionUpdated -= UpdateUI;
    }
    */
}
