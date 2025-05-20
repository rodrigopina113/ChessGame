// GameManager.cs
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Drag your ScriptableObject IChessRules variants here")]
    [SerializeField]
    private List<ScriptableObject> variantAssets;

    private IChessRules activeRules;
    private ChessManager chessManager;

    [Header("Default Variant Index")]
    [SerializeField]
    private int defaultVariant = 0;

    private void Awake()
    {
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
            Debug.LogError("No ChessManager found in scene!");
    }

    private void Start()
    {
        // Cancel any leftover coroutines on the manager
        chessManager.StopAllCoroutines();

        // Now inject and start the default variant
        SelectVariant(defaultVariant);
    }

    /// <summary>
    /// Injects and starts the chosen variant.
    /// </summary>
    public void SelectVariant(int index)
    {
        var soc = variantAssets[index] as IChessRules;
        if (soc == null)
        {
            Debug.LogError($"Variant at {index} does not implement IChessRules");
            return;
        }
        activeRules = soc;
        chessManager.SetRules(activeRules);
        chessManager.StartGame();
    }

    /// <summary>
    /// Example: switch mid-game to another variant.
    /// </summary>
    public void SwitchVariant(int newIndex)
    {
        SelectVariant(newIndex);
    }

    // Hook this up to a UI dropdown:
    public void OnVariantDropdownChanged(int idx) => SwitchVariant(idx);
}
