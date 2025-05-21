using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Racing Kings", fileName = "RacingKingsRules")]
public class RacingKingsRules : ScriptableObject, IChessRules
{
    [Header("Piece Prefabs (R, N, B, Q, K) — no pawns")]
    public GameObject[] whitePiecePrefabs; // 0=R, 1=N, 2=B, 3=Q, 4=K
    public GameObject[] blackPiecePrefabs;

    private Vector2Int CellToCoord(string cell) => new Vector2Int(cell[0] - 'a', cell[1] - '1');

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        manager.ClearAllPieces();

        float baseDelay = 0.05f;
        int dropCounter = 0;

        // Black’s “back ranks” on 8+7:
        int[] blackRank1 = { 3, 0, 2, 1 }; // a8=Q, b8=R, c8=B, d8=N
        int[] blackRank2 = { 4, 0, 2, 1 }; // a7=K, b7=R, c7=B, d7=N

        // White mirrored on files e–h:
        int[] whiteRank1 = { 1, 2, 0, 3 }; // e8=N, f8=B, g8=R, h8=Q
        int[] whiteRank2 = { 1, 2, 0, 4 }; // e7=N, f7=B, g7=R, h7=K

        // Place rank 8
        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();
            string cell = $"{file}8";

            if (i < 4)
                manager.PlacePiece(
                    blackPiecePrefabs[blackRank1[i]],
                    cell,
                    baseDelay * dropCounter++
                );
            else
                manager.PlacePiece(
                    whitePiecePrefabs[whiteRank1[i - 4]],
                    cell,
                    baseDelay * dropCounter++
                );
        }

        // Place rank 7
        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();
            string cell = $"{file}7";

            if (i < 4)
                manager.PlacePiece(
                    blackPiecePrefabs[blackRank2[i]],
                    cell,
                    baseDelay * dropCounter++
                );
            else
                manager.PlacePiece(
                    whitePiecePrefabs[whiteRank2[i - 4]],
                    cell,
                    baseDelay * dropCounter++
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

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board) =>
        GetValidMoves(piece, board).Contains(target);
}
