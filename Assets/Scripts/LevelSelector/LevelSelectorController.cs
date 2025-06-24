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
    public Slider progressBar;
    public TMP_Text levelNameText;
    [Header("Preview Display")]
    public SpriteRenderer previewRenderer;


    [Header("Planet Display")]
    public Transform planetHolder;
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

        maxUnlocked = LevelProgressManager.Instance.GetHighestUnlocked();

        maxUnlocked = Mathf.Clamp(maxUnlocked, 0, levels.Count);


        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Min(maxUnlocked, levels.Count - 1));

        tickMarks.RefreshTicks(currentIndex, maxUnlocked);


        UpdateProgressUI();
        UpdateUIInstant();
        LevelProgressManager.Instance.OnProgressUnlocked += OnUnlocked;
    }


    void OnUnlocked(int newMax)
    {
        maxUnlocked = Mathf.Clamp(newMax, 0, levels.Count);
        UpdateProgressUI();
        tickMarks.RefreshTicks(currentIndex, maxUnlocked);
    }

    void UpdateProgressUI()
    {

        progressBar.value = (float)maxUnlocked / (levels.Count - 1);
    }
    public void ChangeIndex(int delta)
    {
        if (isTransitioning) return;
        int maxSelectable = Mathf.Min(maxUnlocked, levels.Count - 1);
        int target = Mathf.Clamp(currentIndex + delta, 0, maxSelectable);
        if (target != currentIndex)
            StartCoroutine(AnimatePlanetTransition(target, delta));
    }

    public void OnLevelComplete()
    {

        LevelProgressManager.Instance.UnlockLevel(currentIndex + 1);
    }

    private IEnumerator AnimatePlanetTransition(int newIndex, int direction)
    {
        isTransitioning = true;


        GameObject oldPlanet = currentPlanet;


        var newData = levels[newIndex];
        GameObject newPlanet = Instantiate(newData.planetPrefab, planetHolder);

        newPlanet.transform.localPosition = Vector3.right * planetOffset * direction;
        newPlanet.transform.localRotation = Quaternion.identity;
        newPlanet.transform.localScale = newData.planetPrefab.transform.localScale;


        currentIndex = newIndex;
        progressBar.value = (float)newIndex / (levels.Count - 1);
        levelNameText.text = newData.levelName;
        StartCoroutine(FlipAndChangePreview(newData.previewSprite));

        if (tickMarks != null)
            tickMarks.RefreshTicks(currentIndex, maxUnlocked);


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
        var data = levels[currentIndex];


        progressBar.value = (float)currentIndex / (levels.Count - 1);
        levelNameText.text = data.levelName;
        previewRenderer.sprite = data.previewSprite;


        if (currentPlanet != null)
            Destroy(currentPlanet);
        currentPlanet = Instantiate(data.planetPrefab, planetHolder);
        currentPlanet.transform.localPosition = Vector3.zero;
    }

   
    public void ConfirmSelection()
    {
        var data = levels[currentIndex];

 
        NextLevelLoader.sceneName = data.sceneName;

        NextLevelLoader.cutsceneClip = data.cutsceneClip;

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

        Vector3 originalEuler = previewRenderer.transform.localEulerAngles;

        float elapsed = 0f;
        bool hasSwapped = false;
        float duration = transitionTime;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float spinAngle = Mathf.Lerp(0f, 360f, t);


            if (!hasSwapped && spinAngle >= 180f)
            {
                previewRenderer.sprite = newSprite;
                hasSwapped = true;
            }


            float y = originalEuler.y + spinAngle;
            previewRenderer.transform.localEulerAngles = new Vector3(
                originalEuler.x,
                y,
                originalEuler.z
            );

            yield return null;
        }


        previewRenderer.transform.localEulerAngles = new Vector3(
            originalEuler.x,
            originalEuler.y + 360f,
            originalEuler.z
        );
    }



}
