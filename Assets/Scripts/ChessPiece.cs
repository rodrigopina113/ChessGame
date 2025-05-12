using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    public bool isWhite; // Set in Inspector
    private string currentCell; // Managed by ChessManager
    public ChessManager chessManager; // Reference to the ChessManager

    public string CurrentCell
    {
        get => currentCell;
        set => currentCell = value;
    }

    public abstract bool IsValidMove(string targetCell);

    protected bool IsPathBlocked(string startCell, string endCell)
    {
        var cellsBetween = chessManager.GetCellsBetween(startCell, endCell);

        foreach (var cell in cellsBetween)
        {
            ChessPiece piece = chessManager.FindPieceAtCell(cell);
            if (piece != null) // If a piece is found in the path
            {
                return true; // Path is blocked
            }
        }

        return false; // Path is clear
    }

    public void AdjustSizeBasedOnRow()
    {
        if (chessManager != null && !string.IsNullOrEmpty(CurrentCell))
        {
            int row = int.Parse(CurrentCell[1].ToString());
            float scaleFactor = 40.0f - (8 - row) * 2.5f;
            transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }

    private void Start()
    {
        if (chessManager == null)
        {
            chessManager = Object.FindFirstObjectByType<ChessManager>();
            if (chessManager == null)
                Debug.LogError("No ChessManager found in the Scene!");
        }
    }
}
