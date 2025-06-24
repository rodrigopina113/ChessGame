using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PlanetRotator : MonoBehaviour,
                             IPointerEnterHandler,
                             IPointerExitHandler,
                             IPointerClickHandler
{
    [Header("Rotation Speeds")]
    public float baseSpeed  = 10f;
    public float hoverSpeed = 50f;

    [Header("Scale On Hover")]
    [Tooltip("Multiplier relative to original scale")]
    public float scaleFactor = 1.2f;

    private float   currentSpeed;
    private Vector3 originalScale;
    private LevelSelectorController selector;

    void Awake()
    {
        originalScale = transform.localScale;
        currentSpeed  = baseSpeed;
    }

    void Start()
    {

        selector = FindFirstObjectByType<LevelSelectorController>();
    }

    void Update()
    {

        transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentSpeed = hoverSpeed;
        StopAllCoroutines();
        StartCoroutine( ScaleTo(originalScale * scaleFactor) );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentSpeed = baseSpeed;
        StopAllCoroutines();
        StartCoroutine( ScaleTo(originalScale) );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selector.ConfirmSelection();
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float   t     = 0f;
        float   dur   = 0.2f;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.localScale = target;
    }
}
