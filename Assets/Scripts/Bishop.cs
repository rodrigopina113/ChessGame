using UnityEngine;

public class Bishop : ChessPiece
{
    public override bool IsValidMove(string targetCell)
    {
        int rowDifference = Mathf.Abs(targetCell[1] - CurrentCell[1]);
        int colDifference = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        if (rowDifference == colDifference) // Diagonal movement
        {
            if (IsPathBlocked(CurrentCell, targetCell))
                return false; // Path is blocked

            ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);
            if (targetPiece != null && targetPiece.isWhite == this.isWhite)
                return false; // Same color piece at target

            return true; // Valid move
        }

        return false; // Not a valid move for a Bishop
    }
}
