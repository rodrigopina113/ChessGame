using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessAI : MonoBehaviour
{
    private ChessManager chessManager;

    // Set search depth: a depth of 2–3 gives medium difficulty.
    public int searchDepth = 2;

    // Track recent moves by signature ("fromCell-targetCell")
    private List<string> aiMoveHistory = new List<string>();

    private void Start()
    {
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
        {
            Debug.LogError("ChessManager not found in the scene!");
        }
    }

    // Struct to represent a move for simulation.
    public struct Move
    {
        public ChessPiece piece;
        public string fromCell;
        public string targetCell;
        public ChessPiece capturedPiece; // null if none
    }

    // Entry point: choose the best move using minimax search.
    public void MakeMove()
    {
        if (chessManager == null)
            return;

        List<Move> moves = GenerateAllMoves(true); // AI is white
        if (moves.Count == 0)
        {
            Debug.Log("AI has no valid moves!");
            return;
        }

        Move bestMove = moves[0];
        int bestScore = int.MinValue;

        foreach (var move in moves)
        {
            ApplyMove(move);
            int score = Minimax(searchDepth - 1, false);
            UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        // Make the chosen move in the real game.
        chessManager.MovePiece(bestMove.piece, bestMove.targetCell);
        Debug.Log($"AI moved {bestMove.piece.name} to {bestMove.targetCell} (score {bestScore})");

        // Record the move signature and maintain a history of the last 10 moves.
        string moveSignature = bestMove.fromCell + "-" + bestMove.targetCell;
        aiMoveHistory.Add(moveSignature);
        if (aiMoveHistory.Count > 10)
            aiMoveHistory.RemoveAt(0);
    }

    // Minimax search with alternating maximizing (AI) and minimizing (opponent) moves.
    private int Minimax(int depth, bool maximizingPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard();
        }

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            List<Move> moves = GenerateAllMoves(true);
            if (moves.Count == 0)
                return EvaluateBoard();

            foreach (var move in moves)
            {
                ApplyMove(move);
                int eval = Minimax(depth - 1, false);
                UndoMove(move);
                maxEval = Mathf.Max(maxEval, eval);
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            List<Move> moves = GenerateAllMoves(false);
            if (moves.Count == 0)
                return EvaluateBoard();

            foreach (var move in moves)
            {
                ApplyMove(move);
                int eval = Minimax(depth - 1, true);
                UndoMove(move);
                minEval = Mathf.Min(minEval, eval);
            }
            return minEval;
        }
    }

    // Generate all possible moves for pieces of the given color.
    private List<Move> GenerateAllMoves(bool forWhite)
    {
        List<Move> moves = new List<Move>();
        List<ChessPiece> pieces = chessManager.GetAllPieces(forWhite);

        foreach (var piece in pieces)
        {
            foreach (var tile in chessManager.chessboard.tiles)
            {
                if (piece.IsValidMove(tile.name))
                {
                    ChessPiece targetPiece = chessManager.FindPieceAtCell(tile.name);
                    // Only add move if target cell is empty or holds an opponent piece.
                    if (targetPiece == null || targetPiece.isWhite != piece.isWhite)
                    {
                        // Build a candidate move signature.
                        string candidateSignature = piece.CurrentCell + "-" + tile.name;
                        // If playing as white, filter out moves that would repeat three times in a row.
                        if (forWhite && IsMoveRepetitive(candidateSignature))
                        {
                            continue;
                        }

                        Move move = new Move
                        {
                            piece = piece,
                            fromCell = piece.CurrentCell,
                            targetCell = tile.name,
                            capturedPiece = targetPiece,
                        };
                        moves.Add(move);
                    }
                }
            }
        }
        return moves;
    }

    // Check if the candidate move signature has been repeated in the last two moves.
    private bool IsMoveRepetitive(string candidateSignature)
    {
        if (aiMoveHistory.Count >= 2)
        {
            string last = aiMoveHistory[aiMoveHistory.Count - 1];
            string secondLast = aiMoveHistory[aiMoveHistory.Count - 2];
            if (last == candidateSignature && secondLast == candidateSignature)
                return true;
        }
        return false;
    }

    // Apply a move on the board (simulate it).
    private void ApplyMove(Move move)
    {
        move.piece.CurrentCell = move.targetCell;
        move.piece.transform.position = chessManager.chessboard.GetCellPosition(move.targetCell);
        if (move.capturedPiece != null)
        {
            move.capturedPiece.gameObject.SetActive(false);
        }
    }

    // Undo a previously simulated move.
    private void UndoMove(Move move)
    {
        move.piece.CurrentCell = move.fromCell;
        move.piece.transform.position = chessManager.chessboard.GetCellPosition(move.fromCell);
        if (move.capturedPiece != null)
        {
            move.capturedPiece.gameObject.SetActive(true);
        }
    }

    // Evaluate the board by summing up material values: positive values favor white.
    private int EvaluateBoard()
    {
        int score = 0;
        // Get all active pieces (inactive ones are considered captured)
        ChessPiece[] allPieces = Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None);
        foreach (var piece in allPieces)
        {
            if (!piece.gameObject.activeSelf)
                continue;

            int value = GetPieceValue(piece);
            score += piece.isWhite ? value : -value;
        }
        return score;
    }

    // Basic piece values (you can tweak these as needed)
    private int GetPieceValue(ChessPiece piece)
    {
        if (piece is Pawn)
            return 100;
        if (piece is Knight)
            return 320;
        if (piece is Bishop)
            return 330;
        if (piece is Rook)
            return 500;
        if (piece is Queen)
            return 900;
        if (piece is King)
            return 20000;
        return 0;
    }
}
