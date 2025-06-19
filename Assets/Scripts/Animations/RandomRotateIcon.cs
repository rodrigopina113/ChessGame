using UnityEngine;
using System.Collections;

public class RandomRotateIcon : MonoBehaviour
{
    public float minRotation = -30f;
    public float maxRotation = 30f;

    public float minDuration = 0.2f;
    public float maxDuration = 1.0f;

    private RectTransform rectTransform;
    private Quaternion currentRotation;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        currentRotation = rectTransform.localRotation;
        StartCoroutine(RotateEffectLoop());
    }

    IEnumerator RotateEffectLoop()
    {
        while (true)
        {
            // Get next target rotation and duration
            float randomZ = Random.Range(minRotation, maxRotation);
            float duration = Random.Range(minDuration, maxDuration);
            Quaternion startRot = rectTransform.localRotation;
            Quaternion endRot = Quaternion.Euler(0f, 0f, randomZ);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                rectTransform.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Finalize rotation
            rectTransform.localRotation = endRot;

            // Wait 0 frames before continuing — loop continues immediately
            yield return null;
        }
    }
}