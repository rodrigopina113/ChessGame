using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoIntro : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image fadeImage;
    public float fadeInDuration = 2.5f;
    public float fadeOutDuration = 1f;
    public string nextSceneName;
    public Button continueButton;

    private bool isTransitioning = false;

    void Start()
    {
        SetFadeAlpha(1f);

        // Nada muda no botão: ele está sempre visível e clicável

        videoPlayer.Play();

        StartCoroutine(FadeIn());
    }

    public void OnContinueButtonClicked()
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        // Impede múltiplos cliques
        //continueButton.interactable = false;

        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeInDuration)
        {
            float t = time / fadeInDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            color.a = 1f - easedT;
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        SetFadeAlpha(0f);
    }

    IEnumerator FadeOutAndLoadScene()
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeOutDuration)
        {
            float t = time / fadeOutDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            color.a = easedT;
            fadeImage.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        SetFadeAlpha(1f);
        SceneManager.LoadScene(nextSceneName);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}