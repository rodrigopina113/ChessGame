using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Escala")]
    [Tooltip("Quanto aumenta ao entrares com o rato")]
    public float scaleFactor = 1.1f;
    [Tooltip("Tempo da animação em segundos")]
    public float scaleDuration = 0.15f;

    [Header("Cor")]
    [Tooltip("Cor normal")]
    public Color normalColor = Color.white;
    [Tooltip("Cor no hover")]
    public Color highlightColor = new Color(1f, 0.9f, 0.2f);

    RectTransform _rect;
    Graphic     _graphic;
    Vector3     _initialScale;
    Coroutine   _currentAnim;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _graphic = GetComponent<Graphic>();
        _initialScale = _rect.localScale;

        if (_graphic != null)
            _graphic.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartHover(false);
    }

    void StartHover(bool over)
    {
        if (_currentAnim != null)
            StopCoroutine(_currentAnim);

        _currentAnim = StartCoroutine(DoHoverAnimation(over));
    }

    IEnumerator DoHoverAnimation(bool over)
    {
        float elapsed = 0f;
        Vector3 fromScale = _rect.localScale;
        Vector3 toScale   = over
            ? _initialScale * scaleFactor
            : _initialScale;

        Color fromColor = (_graphic != null) ? _graphic.color : Color.white;
        Color toColor   = over ? highlightColor : normalColor;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            _rect.localScale = Vector3.Lerp(fromScale, toScale, t);
            if (_graphic != null)
                _graphic.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }

        _rect.localScale = toScale;
        if (_graphic != null)
            _graphic.color = toColor;
    }
}

