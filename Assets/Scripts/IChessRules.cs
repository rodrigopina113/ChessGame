// IChessRules.cs
using System.Collections.Generic;
using UnityEngine;

public interface IChessRules
{
    void InitializeBoard(ChessManager manager, Chessboard board);
    IEnumerable<Vector2Int> GetValidMoves(ChessPiece piece, Chessboard board);
    bool IsMoveValid(ChessPiece piece, Vector2Int target, Chessboard board);

    bool IsKingInCheck(bool isWhiteTurn);
    bool IsCheckmate(bool isWhiteTurn);
}

