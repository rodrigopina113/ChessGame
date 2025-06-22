using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Video;


public class LevelSelectorController : MonoBehaviour
{
    [Header("Data")]
    public List<LevelData> levels;

    [Header("UI Elements")]
    public Slider progressBar;    // just fill
    public TMP_Text levelNameText;
    [Header("Preview Display")]
    public SpriteRenderer previewRenderer;


    [Header("Planet Display")]
    public Transform planetHolder;   // parent for instantiated planets
    private GameObject currentPlanet;

    [Header("Transition Settings")]
    [Tooltip("World-units to offset planets on entry/exit")]
    public float planetOffset = 10f;
    [Tooltip("Seconds for the slide animation")]
    public float transitionTime = 0.5f;

    private int currentIndex = 0;
    private int maxUnlocked;
    private bool isTransitioning = false;

    public SliderTickMarks tickMarks;


    void Start()
    {
        // 1) Load unlocked
        maxUnlocked = LevelProgressManager.Instance.GetHighestUnlocked();

        // 2) Prevent selection beyond unlocked
        currentIndex = Mathf.Clamp(currentIndex, 0, maxUnlocked);

        tickMarks.RefreshTicks(currentIndex, maxUnlocked);

        // 3) Draw initial UI & listen for unlocks
        UpdateProgressUI();
        UpdateUIInstant();
        LevelProgressManager.Instance.OnProgressUnlocked += OnUnlocked;
    }

    /// <summary>
    /// Called by your Left/Right UI buttons or keyboard arrows.
    /// </summary>
    void OnUnlocked(int newMax)
    {
        maxUnlocked = newMax;
        UpdateProgressUI();
        tickMarks.RefreshTicks(currentIndex, maxUnlocked);
    }

    void UpdateProgressUI()
    {
        // Fill = unlocked / (count-1)
        progressBar.value = (float)maxUnlocked / (levels.Count - 1);
    }
    public void ChangeIndex(int delta)
    {
        if (isTransitioning) return;
        int target = Mathf.Clamp(currentIndex + delta, 0, maxUnlocked);
        if (target != currentIndex)
            StartCoroutine(AnimatePlanetTransition(target, delta));
    }

    public void OnLevelComplete()
    {
        // Call this when a level is beaten:
        LevelProgressManager.Instance.UnlockLevel(currentIndex + 1);
    }

    private IEnumerator AnimatePlanetTransition(int newIndex, int direction)
    {
        isTransitioning = true;

        // 1) Cache old
        GameObject oldPlanet = currentPlanet;

        // 2) Prepare new
        var newData = levels[newIndex];
        GameObject newPlanet = Instantiate(newData.planetPrefab, planetHolder);
        // place off to the right (direction=+1) or left (direction=-1)
        newPlanet.transform.localPosition = Vector3.right * planetOffset * direction;
        newPlanet.transform.localRotation = Quaternion.identity;
        newPlanet.transform.localScale = newData.planetPrefab.transform.localScale;

        // 3) Immediately update UI text/preview/slider
        currentIndex = newIndex;
        progressBar.value = (float)newIndex / (levels.Count - 1);
        levelNameText.text = newData.levelName;
        StartCoroutine(FlipAndChangePreview(newData.previewSprite));

        if (tickMarks != null)
            tickMarks.RefreshTicks(currentIndex, maxUnlocked);


        // 4) Animate slide
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionTime);

            // old → offscreen
            if (oldPlanet != null)
                oldPlanet.transform.localPosition = Vector3.Lerp(
                    Vector3.zero,
                    Vector3.right * -planetOffset * direction,
                    t
                );

            // new → center
            newPlanet.transform.localPosition = Vector3.Lerp(
                Vector3.right * planetOffset * direction,
                Vector3.zero,
                t
            );

            yield return null;
        }

        // 5) Cleanup
        if (oldPlanet != null) Destroy(oldPlanet);
        newPlanet.transform.localPosition = Vector3.zero;
        currentPlanet = newPlanet;

        isTransitioning = false;
    }

    /// <summary>
    /// For first-time setup (no animation).
    /// </summary>
    private void UpdateUIInstant()
    {
        var data = levels[currentIndex];

        // UI
        progressBar.value = (float)currentIndex / (levels.Count - 1);
        levelNameText.text = data.levelName;
        previewRenderer.sprite = data.previewSprite;

        // Planet
        if (currentPlanet != null)
            Destroy(currentPlanet);
        currentPlanet = Instantiate(data.planetPrefab, planetHolder);
        currentPlanet.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Called by PlanetRotator.OnPointerClick and Return key.
    /// </summary>
    public void ConfirmSelection()
    {
        var data = levels[currentIndex];

        // 1) Guarda qual cena de jogo vamos carregar depois da cutscene
        NextLevelLoader.sceneName    = data.sceneName;
        // 2) Guarda o VideoClip que vamos tocar
        NextLevelLoader.cutsceneClip = data.cutsceneClip;
        // 3) Carrega a única cena de cutscene
        SceneManager.LoadScene("CutScene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangeIndex(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeIndex(+1);
        if (Input.GetKeyDown(KeyCode.Return)) ConfirmSelection();
    }

    private IEnumerator FlipAndChangePreview(Sprite newSprite)
    {
        // 1) Grab the sprite’s current rotation (X = –6.22, Y = –16.28 in your case)
        Vector3 originalEuler = previewRenderer.transform.localEulerAngles;

        float elapsed = 0f;
        bool hasSwapped = false;
        float duration = transitionTime; // e.g. 0.5f

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float spinAngle = Mathf.Lerp(0f, 360f, t);

            // 2) Swap the Sprite at halfway
            if (!hasSwapped && spinAngle >= 180f)
            {
                previewRenderer.sprite = newSprite;
                hasSwapped = true;
            }

            // 3) Apply spin + original X/Y
            float y = originalEuler.y + spinAngle;
            previewRenderer.transform.localEulerAngles = new Vector3(
                originalEuler.x,
                y,
                originalEuler.z
            );

            yield return null;
        }

        // 4) Snap exactly to original.X and original.Y + 360
        previewRenderer.transform.localEulerAngles = new Vector3(
            originalEuler.x,
            originalEuler.y + 360f,
            originalEuler.z
        );
    }



}
