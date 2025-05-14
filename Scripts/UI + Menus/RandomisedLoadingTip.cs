using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomisedLoadingTip : MonoBehaviour
{
    [Header("Tip Times")]
    [SerializeField] [Tooltip("Time in seconds it takes for each tip to fade in and out")]
    private float fadeTime = 1f;
    [SerializeField] [Tooltip("Delay in seconds before the first tip start to fade in")]
    private float initialDelay = 1f;
    [SerializeField] [Tooltip("Delay in seconds before the next tip shows")]
    private float delayBetweenTips = 1f;
    [SerializeField] [Tooltip("How long a tip stays up before fading out")]
    private float tipDuration = 4f;

    [Header("Declarations")]
    [SerializeField] private List<Sprite> availableTips;
    private List<Sprite> seenTips;
    [SerializeField] private Image tip;
    private int currentTip = 0;


    private void Start()
    {
        tip.color = Color.clear;
        tip.gameObject.SetActive(false);
        seenTips = new List<Sprite>();
        
        // Check if any tips were assigned
        if (availableTips.Count >= 1)
        {
            NewTip();
        }
        else
        {
            Debug.Log("No Loading Screen tips assigned to this object.");
        }
    }

    /// <summary>
    /// Gets a new tip from availableTips, then adds it to seenTips.
    /// If no tips exist, will reset the availableTips.
    /// </summary>
    private void NewTip()
    {
        // Check if there are any available tips
        if (availableTips.Count >= 1)
        {
            // Obtain a new tip
            currentTip = Random.Range(0, availableTips.Count - 1);
            tip.sprite = availableTips[currentTip];
            seenTips.Add(availableTips[currentTip]);
            availableTips.RemoveAt(currentTip);

            StartCoroutine(FadeTip(true));
        }
        else
        {
            ResetTips();
        }
    }

    /// <summary>
    /// Fades in then out a tip.
    /// Then, gets a new tip.
    /// </summary>
    /// <param name="fadeIn"> Decides whether the tip should fade in or out</param>
    /// <returns></returns>
    private IEnumerator FadeTip(bool fadeIn)
    {
        tip.gameObject.SetActive(true);

        float factor;
        if (fadeIn)
        {
            factor = 1;
            tip.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(initialDelay);
        }
        else
        {
            factor = -1;
            tip.color = Color.white;
        }

        // Increase opacity over time
        float currentTime = 0;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            tip.color = new Color(tip.color.r, tip.color.g, tip.color.b, tip.color.a + (factor * (Time.deltaTime / fadeTime)));
            yield return new WaitForEndOfFrame();
        }

        // Fade out tip if active
        if (fadeIn)
        {
            yield return new WaitForSeconds(tipDuration);
            StartCoroutine(FadeTip(false));
        }
        else
        {
            yield return new WaitForSeconds(delayBetweenTips);
            NewTip();
        }
    }

    /// <summary>
    /// Copies all seenTips back into availableTips
    /// </summary>
    private void ResetTips()
    {
        // Reset availableTips
        availableTips.AddRange(seenTips);

        // Clear seenTips
        seenTips.Clear();

        NewTip();
    }
}
