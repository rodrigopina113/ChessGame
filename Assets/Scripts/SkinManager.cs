using UnityEngine;
using System;

public class SkinManager : MonoBehaviour
{
    [Serializable]
    public struct SkinOption
    {
        public string name;                     // Nome da skin (ex: "Clássica", "Moderna")
        public GameObject[] whitePiecePrefabs;  // Array de 6 prefabs: R, N, B, Q, K, P
    }

    [Header("Opções de Skin Branca")]
    public SkinOption[] skins;                  // Configure no Inspector

    [HideInInspector] public int currentSkinIndex;
    [HideInInspector] public IChessRules currentRules;
    [HideInInspector] public ChessManager chessManager;

    /// <summary>
    /// Atualiza apenas as peças brancas de acordo com skinIndex e re-instancia no tabuleiro.
    /// </summary>
    public void ApplyWhiteSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= skins.Length) return;
        currentSkinIndex = skinIndex;
        var skin = skins[skinIndex];

        // Atualiza só o array whitePiecePrefabs na Variante ativa:
        switch (currentRules)
        {
            case StandardChessRules std:
                std.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
            case Chess960Rules c960:
                c960.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
            case FogOfWarRules fow:
                fow.baseRules.whitePiecePrefabs = skin.whitePiecePrefabs; // No Fog, braço base
                break;
            case RacingKingsRules rk:
                rk.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
        }

        // Recarrega as peças brancas no tabuleiro em execução:
        chessManager.ReplaceWhitePieces();
    }
}
