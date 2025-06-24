using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Fog of War", fileName = "FogOfWarRules")]
public class FogOfWarRules : ScriptableObject, IChessRules
{
    [Header("Base rules to use under the hood")]
    public StandardChessRules baseRules;

    [Header("Fog settings")]
    public GameObject fogPrefab;


    private readonly Dictionary<string, GameObject> fogByTile = new Dictionary<string, GameObject>();

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        baseRules.InitializeBoard(manager, board);
        UpdateFog(manager, board);
        manager.FinishSetup();
    }

    public bool IsKingInCheck(bool isWhiteTurn)
    {
        return baseRules.IsKingInCheck(isWhiteTurn);
    }

    public bool IsCheckmate(bool isWhiteTurn)
    {
        return baseRules.IsCheckmate(isWhiteTurn);
    }

    public IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board)
        => baseRules.GetValidMoves(piece, board);

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board)
        => baseRules.IsMoveValid(piece, target, board);


    public void UpdateFog(ChessManager manager, Chessboard board)
    {

        var visible = new HashSet<string>();
        foreach (var p in manager.GetAllPieces(false))
        {
            visible.Add(p.CurrentCell);
            foreach (var mv in GetValidMoves(p, board))
                visible.Add($"{(char)('a' + mv.x)}{mv.y + 1}");
        }


        var toRemove = new List<string>();
        foreach (var kv in fogByTile)
        {
            if (visible.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }
        foreach (var cell in toRemove)
        {
            Destroy(fogByTile[cell]);
            fogByTile.Remove(cell);
        }


        foreach (var tile in board.tiles)
        {
            if (!visible.Contains(tile.name) && !fogByTile.ContainsKey(tile.name))
            {
                var fog = Instantiate(
                    fogPrefab,
                    tile.GetCenter(),
                    Quaternion.Euler(-90f, 0f, 0f),
                    board.transform
                );
                fogByTile[tile.name] = fog;
            }
        }


        const float baseScale = 20f;
        const float hiddenFactor = 0.2f;
        foreach (var wp in manager.GetAllPieces(true))
        {
            float scale = fogByTile.ContainsKey(wp.CurrentCell)
                ? baseScale * hiddenFactor
                : baseScale;
            wp.transform.localScale = Vector3.one * scale;
        }
    }
}
