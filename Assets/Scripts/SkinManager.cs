using UnityEngine;
using System;

public class SkinManager : MonoBehaviour
{
    [Serializable]
    public struct SkinOption
    {
        public string name;
        public GameObject[] whitePiecePrefabs;
        public GameObject[] blackPiecePrefabs;
    }

    [Header("Opções de Skin Branca")]
    public SkinOption[] skins;

    [HideInInspector] public int currentSkinIndex;

    [HideInInspector] public int currentBlackSkinIndex;
    [HideInInspector] public IChessRules currentRules;
    [HideInInspector] public ChessManager chessManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentBlackSkinIndex = PlayerPrefs.GetInt("SelectedBlackSkin", 0);
        currentBlackSkinIndex = Mathf.Clamp(currentBlackSkinIndex, 0, skins.Length - 1);
    }

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
    
    public void ApplyBlackSkin(int skinIndex)
   {
       if (skinIndex < 0 || skinIndex >= skins.Length) return;
       currentSkinIndex = skinIndex;
       var skin = skins[skinIndex];

       switch (currentRules)
       {
           case StandardChessRules std:
               std.blackPiecePrefabs = skin.blackPiecePrefabs;
               break;
           case Chess960Rules c960:
               c960.blackPiecePrefabs = skin.blackPiecePrefabs;
               break;
           case FogOfWarRules fow:
               fow.baseRules.blackPiecePrefabs = skin.blackPiecePrefabs;
               break;
           case RacingKingsRules rk:
               rk.blackPiecePrefabs = skin.blackPiecePrefabs;
               break;
       }
       chessManager.ReplaceBlackPieces();
   }
}
