

using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Drag your ScriptableObject IChessRules variants here")]
    [SerializeField]
    private List<ScriptableObject> variantAssets;

    [Header("Default Settings")]
    [Tooltip("Índice da variante padrão (ex: Standard, 960, etc.)")]
    [SerializeField]
    private int defaultVariant = 0;

    [Tooltip("Índice da skin branca padrão (configurada no SkinManager)")]
    [SerializeField]
    private int defaultWhiteSkinIndex = 0;

    [Header("Multiplayer Settings")]
    public bool isLocalMultiplayer = false;

    private IChessRules activeRules;
    private ChessManager chessManager;
    private SkinManager skinManager;

    private void Awake()
    {
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
            Debug.LogError("No ChessManager found in scene!");

        skinManager = Object.FindFirstObjectByType<SkinManager>();
        if (skinManager == null)
            Debug.LogError("No SkinManager found in scene!");
    }

    private void Start()
    {
        chessManager.StopAllCoroutines();

        SelectVariant(defaultVariant, defaultWhiteSkinIndex);

        SwitchCameraAtStart();
    }

    private void SwitchCameraAtStart()
    {
        if (CameraSwitcher.Instance != null)
            CameraSwitcher.Instance.SwitchCamera(true);
    }


    public void FinishTurn()
    {
        chessManager.EndTurn();
    }

    /// <param name="variantIndex">Índice da variante (Standard, 960, etc.)</param>
    /// <param name="skinIndex">Índice da skin branca configurada no SkinManager</param>
    public void SelectVariant(int variantIndex, int skinIndex)
    {
        var soc = variantAssets[variantIndex] as IChessRules;
        if (soc == null)
        {
            Debug.LogError($"Variant at {variantIndex} does not implement IChessRules");
            return;
        }
        activeRules = soc;

        chessManager.SetRules(activeRules);
        chessManager.isLocalMultiplayer = isLocalMultiplayer;

        skinManager.currentRules = activeRules;
        skinManager.chessManager = chessManager;
        skinManager.ApplyWhiteSkin(skinIndex);

        // ← NOW add this:
        skinManager.ApplyBlackSkin(skinManager.currentBlackSkinIndex);

        chessManager.StartGame();
    }

    public void SwitchVariant(int newVariantIndex)
    {
        SelectVariant(newVariantIndex, skinManager.currentSkinIndex);
    }

    public void OnVariantDropdownChanged(int variantIndex)
    {
        SwitchVariant(variantIndex);
    }

    public void OnSkinDropdownChanged(int skinIndex)
    {
        SelectVariant(defaultVariant, skinIndex);
    }
}
