using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomBrightnessImage : MonoBehaviour
{
    public float minBrightness = 0.8f;   // Dim level
    public float maxBrightness = 1.5f;   // Bright level
    public float pulseSpeed = 10f;        // Pulses per second

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

        // Ping-pong value between 0 and 1 over time
        float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;

        // Convert to brightness range
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, t);

        // Apply brightness while preserving alpha
        Color brightened = originalColor * brightness;
        brightened.a = originalColor.a;
        image.color = brightened;
    }
}