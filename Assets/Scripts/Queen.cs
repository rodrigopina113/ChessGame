using UnityEngine;

public class Queen : ChessPiece
{
    public override bool IsValidMove(string targetCell)
    {
        int rowDifference = Mathf.Abs(targetCell[1] - CurrentCell[1]);
        int colDifference = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        if (
            rowDifference == colDifference
            || targetCell[0] == CurrentCell[0]
            || targetCell[1] == CurrentCell[1]
        )
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
