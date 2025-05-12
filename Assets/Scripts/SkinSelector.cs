using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct SkinOption
{
    public string name;
    public Sprite previewSprite;
    public GameObject[] blackPiecePrefabs; // Rook, Knight, Bishop, Queen, King, Pawn
}

public class SkinSelector : MonoBehaviour
{
    [Header("UI References")]
    public Button skinButton; // “Skins…” opener
    public GameObject skinPanel; // The full-screen SkinPanel
    public Image previewImage; // Shows the current skin thumbnail
    public Button nextSkinButton; // “>” arrow button
    public Button applySkinButton; // ← new Apply button

    [Header("Skins")]
    public SkinOption[] skins;

    [Header("Game Manager")]
    public ChessManager chessManager;

    private int currentIndex = 0;

    void Awake()
    {
        skinPanel.SetActive(false);

        skinButton.onClick.AddListener(OpenSkinPanel);
        nextSkinButton.onClick.AddListener(CycleNextSkin);
        applySkinButton.onClick.AddListener(ApplyCurrentSkin);

        if (chessManager == null)
            chessManager = UnityEngine.Object.FindFirstObjectByType<ChessManager>();
    }

    void OpenSkinPanel()
    {
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
    }

    void ApplyCurrentSkin()
    {
        // 1) Update manager’s prefab list
        chessManager.blackPiecePrefabs = skins[currentIndex].blackPiecePrefabs;

        // 2) Tell the manager to replace all on-board black pieces
        chessManager.ReplaceBlackPieces();

        // 3) Close the panel
        skinPanel.SetActive(false);
    }
}
