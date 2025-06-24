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


    private Vector2Int CellNameToCoord(string cell) => new Vector2Int(cell[0] - 'a', cell[1] - '1');

    public void InitializeBoard(ChessManager manager, Chessboard board)
    {
        float baseDelay = 0.05f;
        int dropCounter = 0;

        manager.ClearAllPieces();


        int[] backRank = GenerateChess960BackRank();


        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();


            manager.PlacePiece(
                whitePiecePrefabs[backRank[i]],
                $"{file}1",
                baseDelay * dropCounter++,
                true
            );


            manager.PlacePiece(whitePiecePrefabs[5], $"{file}2", baseDelay * dropCounter++, true);
        }


        for (int i = 0; i < 8; i++)
        {
            string file = ((char)('a' + i)).ToString();

            manager.PlacePiece(
                blackPiecePrefabs[backRank[i]],
                $"{file}8",
                baseDelay * dropCounter++,
                false
            );

            manager.PlacePiece(blackPiecePrefabs[5], $"{file}7", baseDelay * dropCounter++, false);
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
    public bool IsKingInCheck(bool isWhiteTurn)
    {
        var kings = UnityEngine.Object.FindObjectsByType<King>(FindObjectsSortMode.None);
        var king = kings.FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck();
    }

    public bool IsCheckmate(bool isWhiteTurn)
    {
        var kings = UnityEngine.Object.FindObjectsByType<King>(FindObjectsSortMode.None);
        var king = kings.FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck() && !king.GetValidMoves().Any();
    }

    public bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board) =>
        GetValidMoves(piece, board).Contains(target);


    private static int[] GenerateChess960BackRank()
    {
        int[] slots = Enumerable.Repeat(-1, 8).ToArray();
        var rnd = new System.Random();


        int[] darkSquares = { 0, 2, 4, 6 };
        int[] lightSquares = { 1, 3, 5, 7 };
        slots[darkSquares[rnd.Next(darkSquares.Length)]] = 2;
        slots[lightSquares[rnd.Next(lightSquares.Length)]] = 2;


        var empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).ToList();
        int qPos = empty[rnd.Next(empty.Count)];
        slots[qPos] = 3;


        empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).ToList();
        int n1 = empty[rnd.Next(empty.Count)];
        slots[n1] = 1; 
        empty.Remove(n1);
        int n2 = empty[rnd.Next(empty.Count)];
        slots[n2] = 1; 


        empty = slots.Select((v, i) => v < 0 ? i : -1).Where(i => i >= 0).OrderBy(i => i).ToList();

        slots[empty[0]] = 0; 
        slots[empty[1]] = 4;
        slots[empty[2]] = 0;

        return slots;
    }

   

}
