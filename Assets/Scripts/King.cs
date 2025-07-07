using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public bool hasMoved = false;

    public override bool IsValidMove(string targetCell)
    {
        int rowDifference = Mathf.Abs(targetCell[1] - CurrentCell[1]);
        int colDifference = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        if (rowDifference == 0 && colDifference == 2)
        {
            if (hasMoved)
            {
                Debug.Log("Castling not allowed: King has already moved.");
                return false;
            }

            char currentFile = CurrentCell[0];
            int currentRank = int.Parse(CurrentCell[1].ToString());
            string rookCell = "";
            if (targetCell[0] > currentFile)
            {
                rookCell = $"h{currentRank}";
            }
            else
            {
                rookCell = $"a{currentRank}";
            }

            ChessPiece rookPiece = chessManager.FindPieceAtCell(rookCell);
            if (rookPiece == null || !(rookPiece is Rook) || ((Rook)rookPiece).hasMoved)
            {
                Debug.Log("Castling not allowed: Rook missing or has moved.");
                return false;
            }

            List<string> betweenCells = chessManager.GetCellsBetween(CurrentCell, rookCell);
            foreach (var cell in betweenCells)
            {
                if (chessManager.FindPieceAtCell(cell) != null)
                {
                    Debug.Log("Castling not allowed: Pieces between king and rook.");
                    return false;
                }
            }

            if (IsKingInCheck())
            {
                Debug.Log("Castling not allowed: King is currently in check.");
                return false;
            }

            char midFile = (char)((CurrentCell[0] + targetCell[0]) / 2);
            string intermediateCell = $"{midFile}{currentRank}";

            string originalCell = CurrentCell;
            CurrentCell = intermediateCell;
            bool safeIntermediate = !IsKingInCheck();
            CurrentCell = originalCell;

            if (!safeIntermediate)
            {
                Debug.Log("Castling not allowed: King would pass through check.");
                return false;
            }

            CurrentCell = targetCell;
            bool safeTarget = !IsKingInCheck();
            CurrentCell = originalCell;

            if (!safeTarget)
            {
                Debug.Log("Castling not allowed: King would land in check.");
                return false;
            }

            return true;
        }

        if (rowDifference <= 1 && colDifference <= 1)
        {
                ChessPiece targetPiece = chessManager.FindPieceAtCell(targetCell);

                if (targetPiece != null && targetPiece.isWhite == this.isWhite)
                    return false;

                string originalCell = CurrentCell;
                List<ChessPiece> allPieces = new List<ChessPiece>(
                    Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
                );

                // Simular remoção da peça capturada
                if (targetPiece != null)
                    allPieces.Remove(targetPiece);

                // Simular movimento
                CurrentCell = targetCell;

                // Avaliar se rei está em perigo, com base em peças simuladas
                bool inCheck = false;
                foreach (var enemy in allPieces)
                {
                    if (enemy.isWhite == this.isWhite)
                        continue;

                    if (enemy.IsValidMove(CurrentCell))
                    {
                        if (enemy is Knight || chessManager.IsPathClear(enemy.CurrentCell, CurrentCell, enemy))
                        {
                            inCheck = true;
                            break;
                        }
                    }
                }

                // Reverter estado
                CurrentCell = originalCell;

                return !inCheck;
        }




        return false;
    }

    public void MoveTo(string targetCell)
    {
        string originalCell = CurrentCell;
        int colDiff = Mathf.Abs(targetCell[0] - CurrentCell[0]);

        if (colDiff == 2)
        {
            int currentRank = int.Parse(CurrentCell[1].ToString());
            if (targetCell[0] > CurrentCell[0])
            {
                ChessPiece rook = chessManager.FindPieceAtCell($"h{currentRank}");
                if (rook != null)
                {
                    chessManager.MovePiece(rook, $"f{currentRank}");
                    ((Rook)rook).hasMoved = true;
                }
            }
            else
            {
                ChessPiece rook = chessManager.FindPieceAtCell($"a{currentRank}");
                if (rook != null)
                {
                    chessManager.MovePiece(rook, $"d{currentRank}");
                    ((Rook)rook).hasMoved = true;
                }
            }
        }

        chessManager.MovePiece(this, targetCell);
        hasMoved = true;
        chessManager.EndTurn();
    }

    public bool IsKingInCheck()
    {
        string kingPosition = CurrentCell;
        List<ChessPiece> opponentPieces = chessManager.GetAllPieces(!isWhite);

        foreach (var piece in opponentPieces)
        {
            if (piece.IsValidMove(kingPosition))
            {
                if (IsPathClear(piece.CurrentCell, kingPosition, piece))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsPathClear(string startCell, string endCell, ChessPiece piece)
    {
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

        foreach (var tile in chessManager.chessboard.tiles)
        {
            string targetCell = tile.name;
            if (IsValidMove(targetCell))
            {
                validMoves.Add(targetCell);
            }
        }
        return validMoves;
    }
}
