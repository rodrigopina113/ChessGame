using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SkinOption
{
    public string name;
    public Sprite previewSprite;
    public GameObject[] blackPiecePrefabs;
}

public class SkinSelector : MonoBehaviour
{
    [Header("UI References")]
    public Button skinButton;
    public GameObject skinPanel;
    public Image previewImage;
    public Button nextSkinButton;
    public Button prevSkinButton;
    public Button applySkinButton;

    [Header("Skins")]
    public SkinOption[] skins;

    [Header("Game Manager")]
    public ChessManager chessManager;

    private int currentIndex;
    private int appliedIndex;

    int maxUnlockedSkin;

    void Awake()
    {
        maxUnlockedSkin = LevelProgressManager.Instance.GetHighestUnlockedSkin();

        appliedIndex = PlayerPrefs.GetInt("SelectedBlackSkin", 0);
        appliedIndex = Mathf.Clamp(appliedIndex, 0, skins.Length - 1);

        currentIndex = appliedIndex;

        skinPanel.SetActive(false);

        skinButton.onClick.AddListener(OpenSkinPanel);
        nextSkinButton.onClick.AddListener(CycleNextSkin);
        prevSkinButton.onClick.AddListener(CyclePrevSkin);
        applySkinButton.onClick.AddListener(ApplyCurrentSkin);

        if (chessManager == null)
            chessManager = UnityEngine.Object.FindFirstObjectByType<ChessManager>();
    }

    void OnEnable()
    {
        maxUnlockedSkin = LevelProgressManager.Instance.GetHighestUnlockedSkin();
        LevelProgressManager.Instance.OnSkinUnlocked += OnSkinUnlocked;

        // clamp our indices
        appliedIndex  = Mathf.Min(appliedIndex, maxUnlockedSkin);
        currentIndex  = appliedIndex;
        UpdatePreview();
        StartCoroutine(DelayedInitialPreview());
    }

    void OnDisable()
    {
        LevelProgressManager.Instance.OnSkinUnlocked -= OnSkinUnlocked;
    }

    private IEnumerator DelayedInitialPreview()
    {
        // wait one frame so ChessManager.StartGame (or Awake/Start)
        // has already laid out the pieces in this preview scene
        yield return null;

        // now swap in the saved skin
        appliedIndex = PlayerPrefs.GetInt("SelectedBlackSkin", 0);
        appliedIndex = Mathf.Clamp(appliedIndex, 0, skins.Length - 1);
        currentIndex = appliedIndex;

        PreviewBlackPieces(appliedIndex);
        UpdatePreview();   // refresh your UI sprite + apply‐button state
    }

    private void OnSkinUnlocked(int newMax)
    {
        maxUnlockedSkin = newMax;
        // enable Next button if this unlocks further skins
        nextSkinButton.interactable = (currentIndex < maxUnlockedSkin);
    }

    void Start()
    {
        // Now that ChessManager has built the board, apply your saved skin
        PreviewBlackPieces(appliedIndex);
        // Also update the previewImage/button state so it matches
        currentIndex = appliedIndex;
        UpdatePreview();
    }
    void Update()
    {
        // Only navigate when the panel is open
        if (!skinPanel.activeSelf) return;

        // Prev: LeftArrow, A, Comma (<)
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.A))
        {
            CyclePrevSkin();
        }
        // Next: RightArrow, D, Period (>)
        else if (Input.GetKeyDown(KeyCode.RightArrow) ||
                 Input.GetKeyDown(KeyCode.D))
        {
            CycleNextSkin();
        }
    }


    void OpenSkinPanel()
    {
        appliedIndex = PlayerPrefs.GetInt("SelectedBlackSkin", 0);
        appliedIndex = Mathf.Clamp(appliedIndex, 0, skins.Length - 1);
        currentIndex = appliedIndex;
        UpdatePreview();
        PreviewBlackPieces(currentIndex);
        skinPanel.SetActive(true);
    }

    void CycleNextSkin()
    {
        currentIndex = Mathf.Min(currentIndex + 1, maxUnlockedSkin);
        UpdatePreview();
        PreviewBlackPieces(currentIndex);
    }

    void CyclePrevSkin()
    {
        currentIndex = Mathf.Max(currentIndex - 1, 0);
        UpdatePreview();
        PreviewBlackPieces(currentIndex);
    }

    private void PreviewBlackPieces(int index)
    {
        chessManager.blackPiecePrefabs = skins[index].blackPiecePrefabs;
        chessManager.ReplaceBlackPieces();
    }

    void UpdatePreview()
    {
        previewImage.sprite = skins[currentIndex].previewSprite;

        if (currentIndex > maxUnlockedSkin)
        {
            var c = previewImage.color;
            previewImage.color = new Color(c.r, c.g, c.b, 0.3f);
            applySkinButton.interactable = false;
        }
        else if (currentIndex == appliedIndex)
        {
            var c = previewImage.color;
            previewImage.color = new Color(c.r, c.g, c.b, 0.5f);
            applySkinButton.interactable = false;
        }
        else
        {
            var c = previewImage.color;
            previewImage.color = new Color(c.r, c.g, c.b, 1f);
            applySkinButton.interactable = true;
        }

        prevSkinButton.interactable = (currentIndex > 0);
        nextSkinButton.interactable = (currentIndex < maxUnlockedSkin);
    }

    void ApplyCurrentSkin()
    {
        // 1) Apply immediately in this scene
        chessManager.blackPiecePrefabs = skins[currentIndex].blackPiecePrefabs;
        chessManager.ReplaceBlackPieces();

        // 2) Persist choice for other scenes
        PlayerPrefs.SetInt("SelectedBlackSkin", currentIndex);
        PlayerPrefs.Save();

        if (SkinManager.Instance != null)
       {
           SkinManager.Instance.currentBlackSkinIndex = currentIndex;
       }

        // 3) Remember and close
        appliedIndex = currentIndex;
        skinPanel.SetActive(false);
    }
}
