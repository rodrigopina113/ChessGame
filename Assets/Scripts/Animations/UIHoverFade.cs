using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup labelCanvasGroup;
    public float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        labelCanvasGroup.alpha = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(0f);
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    System.Collections.IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = labelCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            labelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        labelCanvasGroup.alpha = targetAlpha;
    }
}
