using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomisedDeathMessage : MonoBehaviour
{
    private TextMeshProUGUI deathText;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private string[] messages;

    private void Start()
    {
        deathText = GetComponent<TextMeshProUGUI>();
        deathText.text = messages[Random.Range(0, messages.Length - 1)]; // Random message
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, 0); // Initialise opacity
        yield return new WaitForSeconds(initialDelay);
        float currentTime = 0;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, deathText.color.a + (Time.deltaTime/fadeTime)); // Increase opacity
            yield return new WaitForEndOfFrame();
        }
    }
}
