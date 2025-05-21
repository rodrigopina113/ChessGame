using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Chess960", fileName = "Chess960Rules")]
public class Chess960Rules : ScriptableObject, IChessRules
{
    [Header("Piece Prefabs (R,N,B,Q,K,P)")]
    public GameObject[] whitePiecePrefabs;
    public GameObject[] blackPiecePrefabs;

    // Helper to convert "e4" → Vector2Int(4,3)
    private Vector2Int CellNameToCoord(string cell) => new Vector2Int(cell[0] - 'a', cell[1] - '1');

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        float baseDelay = 0.05f;
        int dropCounter = 0;

        manager.ClearAllPieces();

        // Generate a legal Chess960 back‐rank permutation
        int[] backRank = GenerateChess960BackRank();

        // White back‐rank + pawns
        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();

            // Place piece on rank 1
            manager.PlacePiece(
                whitePiecePrefabs[backRank[i]],
                $"{file}1",
                baseDelay * dropCounter++
            );
            // Place pawn on rank 2
            manager.PlacePiece(whitePiecePrefabs[5], $"{file}2", baseDelay * dropCounter++);
        }

        // Black back‐rank + pawns
        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();
            manager.PlacePiece(
                blackPiecePrefabs[backRank[i]],
                $"{file}8",
                baseDelay * dropCounter++
            );
            manager.PlacePiece(blackPiecePrefabs[5], $"{file}7", baseDelay * dropCounter++);
        }

        manager.FinishSetup();
    }

    public IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board)
    {
        var moves = new List<Vector2Int>();
        foreach (var tile in board.tiles)
        {
            if (piece.IsValidMove(tile.name))
                moves.Add(CellNameToCoord(tile.name));
        }
        return moves;
    }

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board) =>
        GetValidMoves(piece, board).Contains(target);

    /// <summary>
    /// Generates a Chess960‐legal back‐rank:
    /// bishops on opposite colors, king between rooks.
    /// </summary>
    private static int[] GenerateChess960BackRank()
    {
        int[] slots = Enumerable.Repeat(-1, 8).ToArray();
        var rnd = new System.Random();

        // 1) Place bishops on opposite-colored squares
        int[] darkSquares = { 0, 2, 4, 6 };
        int[] lightSquares = { 1, 3, 5, 7 };
        slots[darkSquares[rnd.Next(darkSquares.Length)]] = 2; // Bishop
        slots[lightSquares[rnd.Next(lightSquares.Length)]] = 2; // Bishop

        // 2) Place the queen
        var empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).ToList();
        int qPos = empty[rnd.Next(empty.Count)];
        slots[qPos] = 3; // Queen

        // 3) Place two knights
        empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).ToList();
        int n1 = empty[rnd.Next(empty.Count)];
        slots[n1] = 1; // Knight
        empty.Remove(n1);
        int n2 = empty[rnd.Next(empty.Count)];
        slots[n2] = 1; // Knight

        // 4) Remaining three slots: rook, king, rook (king must be between rooks)
        empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).OrderBy(i => i).ToList();
        // empty[0], empty[1], empty[2]
        slots[empty[0]] = 0; // Rook
        slots[empty[1]] = 4; // King
        slots[empty[2]] = 0; // Rook

        return slots;
    }
}
