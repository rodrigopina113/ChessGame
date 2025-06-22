using UnityEngine;

public class SliderTickMarks : MonoBehaviour
{
    [Header("Container & Count")]
    public RectTransform tickContainer;    // assign your empty child under the Slider
    public int tickCount = 10;             // total ticks

    [Header("Prefabs")]
    public GameObject normalTickPrefab;    // default state
    public GameObject defeatedTickPrefab;  // for any index < highestUnlocked
    public GameObject selectedTickPrefab;  // for index == currentIndex

    /// <summary>
    /// Clears and rebuilds all ticks, choosing which prefab for each index.
    /// </summary>
    public void RefreshTicks(int currentIndex, int highestUnlocked)
    {
        if (tickContainer == null || tickCount < 1) return;

        // 1) wipe old ticks
        for (int i = tickContainer.childCount - 1; i >= 0; i--)
            Destroy(tickContainer.GetChild(i).gameObject);

        // 2) spawn new
        for (int i = 0; i < tickCount; i++)
        {
            GameObject prefab;
            if (i == currentIndex)
                prefab = selectedTickPrefab;
            else if (i < highestUnlocked)
                prefab = defeatedTickPrefab;
            else
                prefab = normalTickPrefab;

            if (prefab == null) continue;

            float norm = tickCount == 1 ? 0f : (float)i / (tickCount - 1);
            var tickGO = Instantiate(prefab, tickContainer);
            var rt = tickGO.GetComponent<RectTransform>();
            if (rt == null)
                continue;

            // 1) Anchor X at norm, Y at center
            rt.anchorMin = new Vector2(norm, 0.5f);
            rt.anchorMax = new Vector2(norm, 0.5f);

            // 2) Reset position & keep the prefab’s own size
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(100f, 100f);
            // You can explicitly set sizeDelta if you need to override:
            // rt.sizeDelta = new Vector2(prefabWidth, prefabHeight);
        }
    }
}
