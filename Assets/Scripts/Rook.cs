using UnityEngine;

public class Rook : ChessPiece
{
    public bool hasMoved = false;

    public override bool IsValidMove(string targetCell)
    {
        if (targetCell[0] == CurrentCell[0] || targetCell[1] == CurrentCell[1])
        {
            if (IsPathBlocked(CurrentCell, targetCell))
                return false;

            ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);
            if (targetPiece != null && targetPiece.isWhite == this.isWhite)
                return false;

            return true;
        }

        return false;
    }
}
