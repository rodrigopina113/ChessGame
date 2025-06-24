using UnityEngine;

public class Pawn : ChessPiece
{
    public override bool IsValidMove(string targetCell)
    {
        int currentRow = int.Parse(CurrentCell[1].ToString());
        int targetRow = int.Parse(targetCell[1].ToString());
        int colDifference = targetCell[0] - CurrentCell[0];

        ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);

        if (isWhite)
        {
            if (colDifference == 0 && targetRow == currentRow + 1 && targetPiece == null)
                return true;
            if (
                colDifference == 0
                && currentRow == 2
                && targetRow == currentRow + 2
                && targetPiece == null
            )
                return true;
            if (
                Mathf.Abs(colDifference) == 1
                && targetRow == currentRow + 1
                && targetPiece != null
                && !targetPiece.isWhite
            )
                return true;
        }
        else
        {
            if (colDifference == 0 && targetRow == currentRow - 1 && targetPiece == null)
                return true;
            if (
                colDifference == 0
                && currentRow == 7
                && targetRow == currentRow - 2
                && targetPiece == null
            )
                return true;
            if (
                Mathf.Abs(colDifference) == 1
                && targetRow == currentRow - 1
                && targetPiece != null
                && targetPiece.isWhite
            )
                return true;
        }

        return false;
    }
}
