using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    // Tracks whether the King has moved (affects castling).
    public bool hasMoved = false;

    public override bool IsValidMove(string targetCell)
    {
        int rowDifference = Mathf.Abs(targetCell[1] - CurrentCell[1]);
        int colDifference = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        // ----- CASTLING LOGIC -----
        // If the king is moving exactly two squares horizontally (and no vertical change)
        if (rowDifference == 0 && colDifference == 2)
        {
            // Castling can only occur if the king hasn't moved.
            if (hasMoved)
            {
                Debug.Log("Castling not allowed: King has already moved.");
                return false;
            }

            // Determine the king's current rank and file.
            char currentFile = CurrentCell[0];
            int currentRank = int.Parse(CurrentCell[1].ToString());
            string rookCell = "";
            // Determine castling side: target cell to the right means kingside; to the left means queenside.
            if (targetCell[0] > currentFile)
            {
                rookCell = $"h{currentRank}";
            }
            else
            {
                rookCell = $"a{currentRank}";
            }

            // Retrieve the rook from the board.
            ChessPiece rookPiece = chessManager.FindPieceAtCell(rookCell);
            if (rookPiece == null || !(rookPiece is Rook) || ((Rook)rookPiece).hasMoved)
            {
                Debug.Log("Castling not allowed: Rook missing or has moved.");
                return false;
            }

            // Verify that all cells between the king and rook are empty.
            List<string> betweenCells = chessManager.GetCellsBetween(CurrentCell, rookCell);
            foreach (var cell in betweenCells)
            {
                if (chessManager.FindPieceAtCell(cell) != null)
                {
                    Debug.Log("Castling not allowed: Pieces between king and rook.");
                    return false;
                }
            }

            // The king must not be in check, and the squares it passes through must be safe.
            if (IsKingInCheck())
            {
                Debug.Log("Castling not allowed: King is currently in check.");
                return false;
            }

            // Determine the intermediate cell (the square the king passes over).
            char midFile = (char)((CurrentCell[0] + targetCell[0]) / 2);
            string intermediateCell = $"{midFile}{currentRank}";

            // Simulate moving the king to the intermediate cell.
            string originalCell = CurrentCell;
            CurrentCell = intermediateCell;
            bool safeIntermediate = !IsKingInCheck();
            CurrentCell = originalCell; // Restore original position.

            if (!safeIntermediate)
            {
                Debug.Log("Castling not allowed: King would pass through check.");
                return false;
            }

            // Simulate moving the king to the target cell.
            CurrentCell = targetCell;
            bool safeTarget = !IsKingInCheck();
            CurrentCell = originalCell; // Restore original position.

            if (!safeTarget)
            {
                Debug.Log("Castling not allowed: King would land in check.");
                return false;
            }

            // All castling conditions are met.
            return true;
        }

        // ----- NORMAL KING MOVEMENT (one square in any direction) -----
        if (rowDifference <= 1 && colDifference <= 1)
        {
            ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);
            // If the target cell is occupied by a friendly piece, the move is invalid.
            if (targetPiece != null && targetPiece.isWhite == this.isWhite)
            {
                Debug.Log($"Invalid move: Target cell {targetCell} is occupied by your own piece.");
                return false;
            }

            // Simulation: Temporarily perform the move.
            string originalCell = CurrentCell;
            Vector3 originalPosition = transform.position;
            bool captured = false;
            GameObject capturedObj = null;

            if (targetPiece != null)
            {
                // Temporarily remove the enemy piece to simulate capturing it.
                captured = true;
                capturedObj = targetPiece.gameObject;
                capturedObj.SetActive(false);
            }

            CurrentCell = targetCell;
            transform.position = chessManager.chessboard.GetCellPosition(targetCell);
            bool safe = !IsKingInCheck();

            // Undo simulation.
            CurrentCell = originalCell;
            transform.position = originalPosition;
            if (captured)
            {
                capturedObj.SetActive(true);
            }

            if (!safe)
            {
                Debug.Log($"Invalid move: Moving to {targetCell} leaves the King in check.");
                return false;
            }
            return true;
        }

        return false;
    }

    // Optionally, add a helper method to execute the move.
    // This handles castling by also moving the rook.
    public void MoveTo(string targetCell)
    {
        string originalCell = CurrentCell;
        int colDiff = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        // Check for castling move.
        if (colDiff == 2)
        {
            int currentRank = int.Parse(CurrentCell[1].ToString());
            // For kingside castling, move the rook from h{rank} to f{rank}.
            if (targetCell[0] > CurrentCell[0])
            {
                ChessPiece rook = chessManager.FindPieceAtCell($"h{currentRank}");
                if (rook != null)
                {
                    chessManager.MovePiece(rook, $"f{currentRank}");
                    ((Rook)rook).hasMoved = true;
                }
            }
            else // Queenside castling: move the rook from a{rank} to d{rank}.
            {
                ChessPiece rook = chessManager.FindPieceAtCell($"a{currentRank}");
                if (rook != null)
                {
                    chessManager.MovePiece(rook, $"d{currentRank}");
                    ((Rook)rook).hasMoved = true;
                }
            }
        }

        // Now move the king.
        chessManager.MovePiece(this, targetCell);
        hasMoved = true;
    }

    public bool IsKingInCheck()
    {
        string kingPosition = CurrentCell;
        // Get all opponent pieces.
        List<ChessPiece> opponentPieces = chessManager.GetAllPieces(!isWhite);

        foreach (var piece in opponentPieces)
        {
            if (piece.IsValidMove(kingPosition))
            {
                // Check if the path between the piece and the king is clear.
                if (IsPathClear(piece.CurrentCell, kingPosition, piece))
                {
                    Debug.Log($"King is in check by {piece.name} at {piece.CurrentCell}");
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsPathClear(string startCell, string endCell, ChessPiece piece)
    {
        // For diagonal moves (Bishop or Queen).
        if ((piece is Bishop || piece is Queen) && IsDiagonalMove(startCell, endCell))
        {
            List<string> cellsInPath = chessManager.GetCellsBetween(startCell, endCell);
            foreach (var cell in cellsInPath)
            {
                if (chessManager.FindPieceAtCell(cell) != null)
                    return false;
            }
            return true;
        }
        // For straight-line moves (Rook or Queen).
        if ((piece is Rook || piece is Queen) && IsStraightLineMove(startCell, endCell))
        {
            List<string> cellsInPath = chessManager.GetCellsBetween(startCell, endCell);
            foreach (var cell in cellsInPath)
            {
                if (chessManager.FindPieceAtCell(cell) != null)
                    return false;
            }
            return true;
        }
        // Knights bypass intervening cells.
        if (piece is Knight)
            return true;

        return false;
    }

    private bool IsStraightLineMove(string startCell, string endCell)
    {
        int startRow = startCell[1] - '1';
        int startCol = startCell[0] - 'a';
        int endRow = endCell[1] - '1';
        int endCol = endCell[0] - 'a';
        return startRow == endRow || startCol == endCol;
    }

    private bool IsDiagonalMove(string startCell, string endCell)
    {
        int startRow = startCell[1] - '1';
        int startCol = startCell[0] - 'a';
        int endRow = endCell[1] - '1';
        int endCol = endCell[0] - 'a';
        return Mathf.Abs(startRow - endRow) == Mathf.Abs(startCol - endCol);
    }

    public List<string> GetValidMoves()
    {
        List<string> validMoves = new List<string>();

        // Iterate over every cell defined on the chessboard.
        foreach (var tile in chessManager.chessboard.tiles)
        {
            string targetCell = tile.name;
            if (IsValidMove(targetCell))
            {
                validMoves.Add(targetCell);
            }
        }
        Debug.Log($"Valid moves for {name} at {CurrentCell}: {string.Join(", ", validMoves)}");
        return validMoves;
    }
}
