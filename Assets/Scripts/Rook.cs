using UnityEngine;

public class Rook : ChessPiece
{
    // Added to support castling logic.
    public bool hasMoved = false;

    public override bool IsValidMove(string targetCell)
    {
        if (targetCell[0] == CurrentCell[0] || targetCell[1] == CurrentCell[1]) // Straight-line movement
        {
            if (IsPathBlocked(CurrentCell, targetCell))
                return false; // Path is blocked

            ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);
            if (targetPiece != null && targetPiece.isWhite == this.isWhite)
                return false; // Same color piece at target

            return true; // Valid move
        }

        return false; // Not a valid move for a Rook
    }
}
