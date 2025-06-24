using UnityEngine;
using UnityEngine.UI;

public class SliderTickMarks : MonoBehaviour
{
    [Header("Container & Count")]
    public RectTransform tickContainer;
    public int tickCount = 10;

    [Header("Prefabs")]
    public GameObject normalTickPrefab;
    public GameObject defeatedTickPrefab;
    public GameObject selectedTickPrefab;

    [Header("Reference")]
    public LevelSelectorController levelSelector;  // Adiciona referência ao controlador principal

    public void RefreshTicks(int currentIndex, int highestUnlocked)
    {
        if (tickContainer == null || tickCount < 1) return;

        // Limpa ticks antigos
        for (int i = tickContainer.childCount - 1; i >= 0; i--)
            Destroy(tickContainer.GetChild(i).gameObject);

        // Cria novos ticks
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

            rt.anchorMin = new Vector2(norm, 0.5f);
            rt.anchorMax = new Vector2(norm, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(100f, 100f);

            // Torna cada tick um botão clicável
            var btn = tickGO.GetComponent<Button>();
            if (btn == null)
                btn = tickGO.AddComponent<Button>();

            int index = i; // Captura o índice certo no lambda
            btn.onClick.AddListener(() =>
            {
                if (levelSelector != null)
                    levelSelector.JumpToIndex(index);
            });
        }
    }
}
