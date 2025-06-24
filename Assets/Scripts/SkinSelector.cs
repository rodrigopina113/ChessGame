using System;
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
    public Button applySkinButton;

    [Header("Skins")]
    public SkinOption[] skins;

    [Header("Game Manager")]
    public ChessManager chessManager;

    private int currentIndex = 0;
    private int appliedIndex = 0;

    void Awake()
    {
        skinPanel.SetActive(false);

        appliedIndex = 0;
        currentIndex = appliedIndex;

        skinButton.onClick.AddListener(OpenSkinPanel);
        nextSkinButton.onClick.AddListener(CycleNextSkin);
        applySkinButton.onClick.AddListener(ApplyCurrentSkin);

        if (chessManager == null)
            chessManager = UnityEngine.Object.FindFirstObjectByType<ChessManager>();
    }

    void OpenSkinPanel()
    {
        currentIndex = appliedIndex;
        UpdatePreview();
        skinPanel.SetActive(true);
    }

    void CycleNextSkin()
    {
        currentIndex = (currentIndex + 1) % skins.Length;
        UpdatePreview();
    }

    void UpdatePreview()
    {
        previewImage.sprite = skins[currentIndex].previewSprite;

        if (currentIndex == appliedIndex)
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
    }

    void ApplyCurrentSkin()
    {

        chessManager.blackPiecePrefabs = skins[currentIndex].blackPiecePrefabs;

        chessManager.ReplaceBlackPieces();

        appliedIndex = currentIndex;

        skinPanel.SetActive(false);
    }
}
