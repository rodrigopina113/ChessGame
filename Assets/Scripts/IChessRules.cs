// IChessRules.cs
using System.Collections.Generic;
using UnityEngine;

public interface IChessRules
{
    /// <summary>
    /// Clears the board and places pieces in their starting positions.
    /// </summary>
    void InitializeBoard(ChessManager manager, Chessboard board);

    /// <summary>
    /// Returns all legal destinations for 'piece' under this variant's rules.
    /// </summary>
    IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board);

    /// <summary>
    /// Quick check: is moving 'piece' to 'target' legal?
    /// </summary>
    bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board);

    // Optional hooks you could add:
    // bool CanCastle(King king, Vector2Int rookTarget, Chessboard board);
    // Vector2Int? EnPassantCapture(Pawn pawn, Chessboard board);
}
