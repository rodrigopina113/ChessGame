using UnityEngine;

public class Knight : ChessPiece
{
    public override bool IsValidMove(string targetCell)
    {
        int rowDifference = Mathf.Abs(targetCell[1] - CurrentCell[1]);
        int colDifference = Mathf.Abs(targetCell[0] - CurrentCell[0]);
        return (rowDifference == 2 && colDifference == 1)
            || (rowDifference == 1 && colDifference == 2);
    }
}
