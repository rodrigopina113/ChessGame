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
        // Garante vídeo parado e tela preta no início
        vp.Stop();
        alpha = 1f;
        // Quando o vídeo terminar, faz fade out
        vp.loopPointReached += OnVideoEnd;
        // Inicia o fade in e, ao fim, dispara o play
        StartCoroutine(Fade(1f, 0f, fadeDuration, () => vp.Play()));
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        // Quando acabar, faz fade out
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
        // Salva cor anterior
        var old = GUI.color;
        // Define preto com alpha
        GUI.color = new Color(0f, 0f, 0f, alpha);
        // Desenha retângulo full-screen usando a textura branca padrão
        GUI.DrawTexture(
            new Rect(0, 0, Screen.width, Screen.height),
            Texture2D.whiteTexture);
        // Restaura cor
        GUI.color = old;
    }
}
