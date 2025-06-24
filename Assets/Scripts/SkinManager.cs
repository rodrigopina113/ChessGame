using UnityEngine;
using System;

public class SkinManager : MonoBehaviour
{
    [Serializable]
    public struct SkinOption
    {
        public string name;
        public GameObject[] whitePiecePrefabs;
    }

    [Header("Opções de Skin Branca")]
    public SkinOption[] skins;

    [HideInInspector] public int currentSkinIndex;
    [HideInInspector] public IChessRules currentRules;
    [HideInInspector] public ChessManager chessManager;

    public void ApplyWhiteSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= skins.Length) return;
        currentSkinIndex = skinIndex;
        var skin = skins[skinIndex];

        switch (currentRules)
        {
            case StandardChessRules std:
                std.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
            case Chess960Rules c960:
                c960.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
            case FogOfWarRules fow:
                fow.baseRules.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
            case RacingKingsRules rk:
                rk.whitePiecePrefabs = skin.whitePiecePrefabs;
                break;
        }
        chessManager.ReplaceWhitePieces();
    }
}
