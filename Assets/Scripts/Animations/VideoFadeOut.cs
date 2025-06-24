using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System;

[RequireComponent(typeof(VideoPlayer))]
public class VideoFaderGUI : MonoBehaviour
{
    [Header("Configurações de Fade")]
    public float fadeDuration = 1f;

    private VideoPlayer vp;
    private float alpha = 1f;
    private bool isFading = false;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        vp.Stop();
        alpha = 1f;

        vp.loopPointReached += OnVideoEnd;

        StartCoroutine(Fade(1f, 0f, fadeDuration, () => vp.Play()));
    }

    private void OnVideoEnd(VideoPlayer source)
    {

        StartCoroutine(Fade(0f, 1f, fadeDuration, () => vp.Stop()));
    }

    IEnumerator Fade(float from, float to, float duration, Action onComplete)
    {
        if (isFading) yield break;
        isFading = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        alpha = to;
        onComplete?.Invoke();
        isFading = false;
    }

    void OnGUI()
    {
        if (alpha <= 0f) return;

        var old = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, alpha);

        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            Texture2D.whiteTexture);

        GUI.color = old;
    }
}
