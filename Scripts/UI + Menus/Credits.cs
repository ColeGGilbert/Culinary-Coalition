using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [Header("Declarations")]
    [SerializeField] private GameObject[] objectsToScroll = null;
    private Vector3[] initialPositions;

    [Header("Scroll Settings")]
    [SerializeField] [Tooltip("Base speed of which objects should scroll")]
    private float scrollSpeed = 1.0f;
    [SerializeField] [Tooltip("The maximum scroll speed multiplier. The multiplier approaches this when vertical input approaches -1 (i.e., pushing down on the thumbstick all the way).")]
    private float scrollMaxMultiplier = 1.5f;
    [SerializeField] [Tooltip("The direction in which objects should scroll")]
    private Vector3 scrollDirection = Vector3.up;
    [SerializeField] [Tooltip("Delay in seconds (on enable) before objects begin to scroll")]
    private float initialDelay = 1.0f;

    private float currentMultiplier = 1.0f;
    private bool canScroll = false;

    private void Start()
    {
        // Initialise positions
        initialPositions = new Vector3[objectsToScroll.Length];
        for (int i = 0; i < objectsToScroll.Length; i++)
        {
            initialPositions[i] = objectsToScroll[i].transform.position;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DoScrollDelay());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ResetPosition();
    }

    /// <summary>
    /// Waits initialDelay seconds before objects begin to scroll
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoScrollDelay()
    {
        canScroll = false;
        yield return new WaitForSeconds(initialDelay);
        canScroll = true;
    }

    private void FixedUpdate()
    {
        // Only calculate multiplier and scroll if canScroll
        if (canScroll)
        {
            float v = Input.GetAxis("Vertical");
            CalculateMultiplier(-v);
            Scroll();
        }
    }

    /// <summary>
    /// Takes in vertical input and uses this to determine the scroll speed multiplier.
    /// Lerps between 1 and the max multiplier by the input.
    /// </summary>
    /// <param name="inputValue"></param>
    private void CalculateMultiplier(float inputValue)
    {
        currentMultiplier = Mathf.Lerp(1, scrollMaxMultiplier, inputValue);
    }

    /// <summary>
    /// Updates each objectsToScrolls' positions. Affected by scrollDirection, scrollSpeed and currentMultiplier.
    /// </summary>
    private void Scroll()
    {
        for (int i = 0; i < objectsToScroll.Length; i++)
        {
            objectsToScroll[i].transform.position += scrollDirection * scrollSpeed * currentMultiplier;
        }
    }

    /// <summary>
    /// Resets all objectsToScrolls' positions to their starting positions when initialised
    /// </summary>
    private void ResetPosition()
    {
        for (int i = 0; i < objectsToScroll.Length; i++)
        {
            objectsToScroll[i].transform.position = initialPositions[i];
        }
    }
}
