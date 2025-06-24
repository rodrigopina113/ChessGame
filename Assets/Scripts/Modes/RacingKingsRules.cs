using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Racing Kings", fileName = "RacingKingsRules")]
public class RacingKingsRules : ScriptableObject, IChessRules
{
    [Header("Piece Prefabs (R, N, B, Q, K) — no pawns")]
    public GameObject[] whitePiecePrefabs;
    public GameObject[] blackPiecePrefabs;

    private Vector2Int CellToCoord(string cell) => new Vector2Int(cell[0] - 'a', cell[1] - '1');

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        manager.ClearAllPieces();

        float baseDelay = 0.05f;
        int dropCounter = 0;


        int[] blackRank1 = { 3, 0, 2, 1 };
        int[] blackRank2 = { 4, 0, 2, 1 };


        int[] whiteRank1 = { 1, 2, 0, 3 };
        int[] whiteRank2 = { 1, 2, 0, 4 };


        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();
            string cell = $"{file}8";

            if (i < 4)
            manager.PlacePiece(
                blackPiecePrefabs[blackRank1[i]],
                cell,
                baseDelay * dropCounter++,
                false
            );
            else
                manager.PlacePiece(
                    whitePiecePrefabs[whiteRank1[i - 4]],
                    cell,
                    baseDelay * dropCounter++,
                    true
                );

        }


        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();
            string cell = $"{file}7";

            if (i < 4)
                manager.PlacePiece(
                    blackPiecePrefabs[blackRank2[i]],
                    cell,
                    baseDelay * dropCounter++,
                    false
                );
            else
                manager.PlacePiece(
                    whitePiecePrefabs[whiteRank2[i - 4]],
                    cell,
                    baseDelay * dropCounter++,
                    true
                );

        }

        manager.FinishSetup();
    }

    public IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board)
    {
        var moves = new List<Vector2Int>();
        foreach (var tile in board.tiles)
            if (piece.IsValidMove(tile.name))
                moves.Add(CellToCoord(tile.name));
        return moves;
    }

    public bool IsKingInCheck(bool isWhiteTurn)
    {
        var kings = Object.FindObjectsByType<King>(FindObjectsSortMode.None);
        var king = kings.FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck();
    }

    public bool IsCheckmate(bool isWhiteTurn)
    {
        var kings = Object.FindObjectsByType<King>(FindObjectsSortMode.None);
        var king = kings.FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck() && !king.GetValidMoves().Any();
    }

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board) =>
        GetValidMoves(piece, board).Contains(target);
}
