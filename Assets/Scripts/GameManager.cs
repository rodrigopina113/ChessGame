// **** Integração completa no GameManager para seleção de variante e skin branca ****

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

    // Referências internas
    private IChessRules activeRules;
    private ChessManager chessManager;
    private SkinManager skinManager;

    private void Awake()
    {
        // Busca referências de manager
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
            Debug.LogError("No ChessManager found in scene!");

        skinManager = Object.FindFirstObjectByType<SkinManager>();
        if (skinManager == null)
            Debug.LogError("No SkinManager found in scene!");
    }

    private void Start()
    {
        // Cancela coroutines pendentes
        chessManager.StopAllCoroutines();

        // Seleciona variante + skin branca padrão
        SelectVariant(defaultVariant, defaultWhiteSkinIndex);

        // Ajusta câmera inicial para as brancas
        SwitchCameraAtStart();
    }

    /// <summary>
    /// Garante que a câmera comece focada nas peças brancas.
    /// </summary>
    private void SwitchCameraAtStart()
    {
        if (CameraSwitcher.Instance != null)
            CameraSwitcher.Instance.SwitchCamera(true);
    }


    public void FinishTurn()
    {
        // Garantir que o turno está sendo trocado corretamente no GameManager também
        chessManager.EndTurn(); // Chama o método de alternância de turno no ChessManager
    }

    
    /// <summary>
    /// Seleciona variante de jogo e aplica skin branca antes de iniciar a partida.
    /// </summary>
    /// <param name="variantIndex">Índice da variante (Standard, 960, etc.)</param>
    /// <param name="skinIndex">Índice da skin branca configurada no SkinManager</param>
    public void SelectVariant(int variantIndex, int skinIndex)
    {
        // 1) Recupera e valida regras da variante
        var soc = variantAssets[variantIndex] as IChessRules;
        if (soc == null)
        {
            Debug.LogError($"Variant at {variantIndex} does not implement IChessRules");
            return;
        }
        activeRules = soc;

        // 2) Configura ChessManager
        chessManager.SetRules(activeRules);
        chessManager.isLocalMultiplayer = isLocalMultiplayer;

        // 3) Aplica skin branca via SkinManager
        skinManager.currentRules     = activeRules;
        skinManager.chessManager     = chessManager;
        skinManager.ApplyWhiteSkin(skinIndex);

        // 4) Inicia o jogo
        chessManager.StartGame();
    }

    /// <summary>
    /// Exemplo de trocar variante em tempo de jogo, mantendo skin atual.
    /// </summary>
    public void SwitchVariant(int newVariantIndex)
    {
        SelectVariant(newVariantIndex, skinManager.currentSkinIndex);
    }

    // Métodos hook para UI Dropdowns

    /// <summary>
    /// Chamado pelo dropdown de variante: troca variante e mantém skin atual.
    /// </summary>
    public void OnVariantDropdownChanged(int variantIndex)
    {
        SwitchVariant(variantIndex);
    }

    /// <summary>
    /// Chamado pelo dropdown de skin: troca apenas a skin branca na mesma variante.
    /// </summary>
    public void OnSkinDropdownChanged(int skinIndex)
    {
        SelectVariant(defaultVariant, skinIndex);
    }
}
