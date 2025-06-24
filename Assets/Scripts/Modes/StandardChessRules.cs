// StandardChessRules.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Variants/Standard", fileName = "StandardChessRules")]
public class StandardChessRules : ScriptableObject, IChessRules
{
    [Header("Piece Prefabs (R,N,B,Q,K,P)")]
    public GameObject[] whitePiecePrefabs;
    public GameObject[] blackPiecePrefabs;

    private Vector2Int CellNameToCoord(string cell) => new Vector2Int(cell[0] - 'a', cell[1] - '1');

    public void InitializeBoard(ChessManager manager, Chessboard board)
{
    float baseDelay = 0.05f;
    int counter = 0;
    int counter2 = 0;
    manager.ClearAllPieces();


    for (int i = 0; i < 8; i++)
    {
        string file = ((char)('a' + i)).ToString();
        int prefabIndex = i == 1 || i == 6 ? 1
                         : i == 2 || i == 5 ? 2
                         : i == 3 ? 3
                         : i == 4 ? 4
                         : 0;

        manager.PlacePiece(whitePiecePrefabs[prefabIndex], $"{file}1", baseDelay * counter++, true);
        manager.PlacePiece(whitePiecePrefabs[5], $"{file}2", baseDelay * counter++, true); // Pawns
    }


    for (int i = 0; i < 8; i++)
    {
        string file = ((char)('a' + i)).ToString();
        int prefabIndex = i == 1 || i == 6 ? 1
                         : i == 2 || i == 5 ? 2
                         : i == 3 ? 3
                         : i == 4 ? 4
                         : 0;

        manager.PlacePiece(blackPiecePrefabs[prefabIndex], $"{file}8", baseDelay * counter2++, false);
        manager.PlacePiece(blackPiecePrefabs[5], $"{file}7", baseDelay * counter2++, false); // Pawns
    }

    manager.FinishSetup();
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
}
