using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectorController : MonoBehaviour
{
    [Header("Data")]
    public List<LevelData> levels;

    [Header("Scene Changer")]
    public SceneChanger sceneChanger;

    [Header("UI Elements")]
    public Slider progressBar;
    public TMP_Text levelNameText;

    [Header("Preview Display")]
    public SpriteRenderer previewRenderer;

    [Header("Planet Display")]
    public Transform planetHolder;
    private GameObject currentPlanet;

    [Header("Transition Settings")]
    public float planetOffset = 10f;
    public float transitionTime = 0.5f;

    [Header("Tick Marks (optional)")]
    public SliderTickMarks tickMarks;

    private int currentIndex = 0;
    private int maxUnlocked;
    private bool isTransitioning = false;

    private void Start()
    {
        maxUnlocked = LevelProgressManager.Instance.GetHighestUnlocked();
        maxUnlocked = Mathf.Clamp(maxUnlocked, 0, levels.Count);
        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Min(maxUnlocked, levels.Count - 1));

        tickMarks?.RefreshTicks(currentIndex, maxUnlocked);
        UpdateProgressUI();
        UpdateUIInstant();
        LevelProgressManager.Instance.OnProgressUnlocked += OnUnlocked;
    }

    private void OnUnlocked(int newMax)
    {
        maxUnlocked = Mathf.Clamp(newMax, 0, levels.Count);
        UpdateProgressUI();
        tickMarks?.RefreshTicks(currentIndex, maxUnlocked);
    }

    private void UpdateProgressUI()
    {
        progressBar.value = (levels.Count > 1)
            ? (float)maxUnlocked / (levels.Count - 1)
            : 1f;
    }

    public void ChangeIndex(int delta)
    {
        if (isTransitioning) return;
        int maxSel = Mathf.Min(maxUnlocked, levels.Count - 1);
        int target = Mathf.Clamp(currentIndex + delta, 0, maxSel);
        if (target != currentIndex)
            StartCoroutine(AnimatePlanetTransition(target, target > currentIndex ? 1 : -1));
    }

    public void JumpToIndex(int index)
    {
        if (isTransitioning) return;
        int maxSel = Mathf.Min(maxUnlocked, levels.Count - 1);
        int target = Mathf.Clamp(index, 0, maxSel);
        if (target != currentIndex)
            StartCoroutine(AnimatePlanetTransition(target, target > currentIndex ? 1 : -1));
    }

    public void OnLevelComplete()
    {
        LevelProgressManager.Instance.UnlockLevel(currentIndex + 1);
    }

    private IEnumerator AnimatePlanetTransition(int newIndex, int direction)
    {
        isTransitioning = true;
        GameObject oldPlanet = currentPlanet;
        LevelData data = levels[newIndex];
        GameObject newPlanet = Instantiate(data.planetPrefab, planetHolder);

        newPlanet.transform.localPosition = Vector3.right * planetOffset * direction;
        newPlanet.transform.localRotation = Quaternion.identity;
        newPlanet.transform.localScale    = data.planetPrefab.transform.localScale;

        currentIndex       = newIndex;
        UpdateProgressUI();
        levelNameText.text = data.levelName;
        tickMarks?.RefreshTicks(currentIndex, maxUnlocked);
        StartCoroutine(FlipAndChangePreview(data.previewSprite));

        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);

            if (oldPlanet != null)
                oldPlanet.transform.localPosition = Vector3.Lerp(
                    Vector3.zero,
                    Vector3.right * -planetOffset * direction,
                    t
                );

            newPlanet.transform.localPosition = Vector3.Lerp(
                Vector3.right * planetOffset * direction,
                Vector3.zero,
                t
            );

            yield return null;
        }

        if (oldPlanet != null) Destroy(oldPlanet);
        newPlanet.transform.localPosition = Vector3.zero;
        currentPlanet = newPlanet;
        isTransitioning = false;
    }

    private void UpdateUIInstant()
    {
        LevelData data = levels[currentIndex];
        progressBar.value     = (levels.Count > 1)
            ? (float)currentIndex / (levels.Count - 1)
            : 1f;
        levelNameText.text    = data.levelName;
        previewRenderer.sprite = data.previewSprite;

        if (currentPlanet != null)
            Destroy(currentPlanet);

        currentPlanet = Instantiate(data.planetPrefab, planetHolder);
        currentPlanet.transform.localPosition = Vector3.zero;
    }

    public void ConfirmSelection()
    {
        LevelData data = levels[currentIndex];
        if (sceneChanger != null)
        {
            sceneChanger.nextLevelSceneName   = data.sceneName;
            sceneChanger.nextCutsceneFileName = data.cutsceneFileName;
            sceneChanger.PlayNextLevelCutscene();
        }
        else
        {
            Debug.LogError("LevelSelectorController: referência a SceneChanger não atribuída!");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))  ChangeIndex(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeIndex(+1);
        if (Input.GetKeyDown(KeyCode.Return))     ConfirmSelection();
    }

    private IEnumerator FlipAndChangePreview(Sprite newSprite)
    {
        Vector3 original = previewRenderer.transform.localEulerAngles;
        float elapsed    = 0f;
        bool swapped     = false;

        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t     = Mathf.Clamp01(elapsed / transitionTime);
            float angle = Mathf.Lerp(0f, 360f, t);

            if (!swapped && angle >= 180f)
            {
                previewRenderer.sprite = newSprite;
                swapped = true;
            }

            previewRenderer.transform.localEulerAngles = new Vector3(
                original.x,
                original.y + angle,
                original.z
            );

            yield return null;
        }

        previewRenderer.transform.localEulerAngles = original + new Vector3(0f, 360f, 0f);
    }
}
