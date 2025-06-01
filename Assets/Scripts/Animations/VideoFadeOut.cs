using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoFadeOut : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image fadeImage;
    public float fadeDuration = 2f;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(FadeToBlack());
    }

    IEnumerator FadeToBlack()
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            // Ease-out usando curva quadrática
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            color.a = easedT;
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        // Garante 100% opaco no final
        color.a = 1f;
        fadeImage.color = color;
    }
}