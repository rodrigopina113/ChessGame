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
    public Button skinButton;          // “Skins…” opener
    public GameObject skinPanel;       // The full-screen SkinPanel
    public Image previewImage;         // Shows the current skin thumbnail
    public Button nextSkinButton;      // “>” arrow button
    public Button applySkinButton;     // “Apply” button

    [Header("Skins")]
    public SkinOption[] skins;

    [Header("Game Manager")]
    public ChessManager chessManager;

    private int currentIndex = 0;
    private int appliedIndex = 0; // Tracks which skin is currently applied

    void Awake()
    {
        skinPanel.SetActive(false);

        // Initialize appliedIndex → assume default (index 0) is already in use at startup
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
        // Whenever the panel opens, sync preview to the applied skin
        currentIndex = appliedIndex;
        UpdatePreview();
        skinPanel.SetActive(true);
    }

    void CycleNextSkin()
    {
        // Move to next index in a circular fashion
        currentIndex = (currentIndex + 1) % skins.Length;
        UpdatePreview();
    }

    void UpdatePreview()
    {
        previewImage.sprite = skins[currentIndex].previewSprite;

        if (currentIndex == appliedIndex)
        {
            // The user is looking at the skin that is already applied:
            // ♦ Dim the preview to show "already selected"
            var c = previewImage.color;
            previewImage.color = new Color(c.r, c.g, c.b, 0.5f);

            // Disable Apply button
            applySkinButton.interactable = false;
        }
        else
        {
            // Full opacity and allow “Apply” if it’s not already selected
            var c = previewImage.color;
            previewImage.color = new Color(c.r, c.g, c.b, 1f);

            applySkinButton.interactable = true;
        }
    }

    void ApplyCurrentSkin()
    {
        // 1) Update manager’s prefab list
        chessManager.blackPiecePrefabs = skins[currentIndex].blackPiecePrefabs;

        // 2) Tell the manager to replace all on-board black pieces
        chessManager.ReplaceBlackPieces();

        // 3) Record which index is now applied
        appliedIndex = currentIndex;

        // 4) Close the panel
        skinPanel.SetActive(false);
    }
}
