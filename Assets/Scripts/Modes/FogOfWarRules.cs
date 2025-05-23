using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Fog of War", fileName = "FogOfWarRules")]
public class FogOfWarRules : ScriptableObject, IChessRules
{
    [Header("Base rules to use under the hood")]
    public StandardChessRules baseRules;

    [Header("Fog settings")]
    public GameObject fogPrefab;

    // Change from List to Dictionary so we can track per‐tile
    private readonly Dictionary<string, GameObject> fogByTile = new Dictionary<string, GameObject>();

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        baseRules.InitializeBoard(manager, board);
        UpdateFog(manager, board);
        manager.FinishSetup();
    }

    public IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board)
        => baseRules.GetValidMoves(piece, board);

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board)
        => baseRules.IsMoveValid(piece, target, board);

    /// <summary>
    /// Incrementally add/remove fog: 
    /// - Destroy fog on newly visible tiles 
    /// - Instantiate fog on newly hidden tiles
    /// </summary>
    public void UpdateFog(ChessManager manager, Chessboard board)
    {
        // 1) Compute set of currently visible tile-names for the human (black) side
        var visible = new HashSet<string>();
        foreach (var p in manager.GetAllPieces(false))  // always black side
        {
            visible.Add(p.CurrentCell);
            foreach (var mv in GetValidMoves(p, board))
                visible.Add($"{(char)('a' + mv.x)}{mv.y + 1}");
        }

        // 2) Remove fog from tiles that have become visible
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

        // 3) Add fog to tiles that are unseen and not yet fogged
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

        // ── Scale white pieces based on fog coverage
        const float baseScale = 20f;                // same as in PlacePiece:contentReference[oaicite:1]{index=1}
        const float hiddenFactor = 0.2f;
        foreach (var wp in manager.GetAllPieces(true))
        {
            float scale = fogByTile.ContainsKey(wp.CurrentCell)
                ? baseScale * hiddenFactor  // hidden
                : baseScale;               // visible
            wp.transform.localScale = Vector3.one * scale;
        }
    }
}
