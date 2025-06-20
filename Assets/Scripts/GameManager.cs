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

    public bool isLocalMultiplayer = false;

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

        // Agora chamamos SwitchCamera com a variável isWhiteTurn para garantir que, no início, a câmera das brancas será visível
        SwitchCameraAtStart();
    }

    /// <summary>
    /// Configura a câmera corretamente no início do jogo para as brancas (isWhiteTurn).
    /// </summary>
    private void SwitchCameraAtStart()
    {
        if (CameraSwitcher.Instance != null)
        {
            // Chama imediatamente a câmera para as brancas
            CameraSwitcher.Instance.SwitchCamera(true);
        }
    }

    public void FinishTurn()
    {
        // Garantir que o turno está sendo trocado corretamente no GameManager também
        chessManager.FinishTurn(); // Chama o método de alternância de turno no ChessManager
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
        chessManager.isLocalMultiplayer = isLocalMultiplayer;
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
