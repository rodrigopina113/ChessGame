using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomBrightnessImage : MonoBehaviour
{
    public float minBrightness = 0.8f;
    public float maxBrightness = 1.5f;
    public float pulseSpeed = 10f;

    private Image image;
    private Color originalColor;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            originalColor = image.color;
        }
    }

    void Update()
    {
        if (image == null) return;

        float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;

        float brightness = Mathf.Lerp(minBrightness, maxBrightness, t);
        Color brightened = originalColor * brightness;
        brightened.a = originalColor.a;
        image.color = brightened;
    }
}