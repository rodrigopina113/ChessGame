using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessAI : MonoBehaviour
{
    private ChessManager chessManager;
    // Cached references for performance
    private Chessboard board;
    private Chessboard.Tile[] tiles;

    // Set search depth: a depth of 2–3 gives medium difficulty.
    public int searchDepth = 2;

    // Track recent moves by signature ("fromCell-targetCell")
    private List<string> aiMoveHistory = new List<string>();

    private void Start()
    {
        // 1) Find and cache the ChessManager
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
        {
            Debug.LogError("ChessManager not found in the scene!");
            return;
        }

        // 2) Cache the board & its tiles array for fast access
        board = chessManager.chessboard;
        tiles = board.tiles;
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

        // Execute the chosen move
        chessManager.MovePiece(bestMove.piece, bestMove.targetCell);
        Debug.Log($"AI moved {bestMove.piece.name} to {bestMove.targetCell} (score {bestScore})");

        // Record the move signature
        string moveSignature = bestMove.fromCell + "-" + bestMove.targetCell;
        aiMoveHistory.Add(moveSignature);
        if (aiMoveHistory.Count > 10)
            aiMoveHistory.RemoveAt(0);
    }

    // Minimax search with alternating max/min players.
    private int Minimax(int depth, bool maximizingPlayer)
    {
        if (depth == 0)
            return EvaluateBoard();

        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            var moves = GenerateAllMoves(true);
            if (moves.Count == 0) return EvaluateBoard();

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
            var moves = GenerateAllMoves(false);
            if (moves.Count == 0) return EvaluateBoard();

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
        var moves = new List<Move>();
        var pieces = chessManager.GetAllPieces(forWhite);

        foreach (var piece in pieces)
        {
            foreach (var tile in tiles)  // use cached tiles[]
            {
                if (piece.IsValidMove(tile.name))
                {
                    ChessPiece targetPiece = chessManager.FindPieceAtCell(tile.name);
                    if (targetPiece == null || targetPiece.isWhite != piece.isWhite)
                    {
                        // Avoid threefold repetition on white side
                        if (forWhite)
                        {
                            string sig = piece.CurrentCell + "-" + tile.name;
                            if (IsMoveRepetitive(sig)) continue;
                        }

                        moves.Add(new Move {
                            piece = piece,
                            fromCell = piece.CurrentCell,
                            targetCell = tile.name,
                            capturedPiece = targetPiece
                        });
                    }
                }
            }
        }

        return moves;
    }

    // Check for repeating the same move twice in a row.
    private bool IsMoveRepetitive(string candidateSignature)
    {
        if (aiMoveHistory.Count >= 2)
        {
            int last = aiMoveHistory.Count - 1;
            return aiMoveHistory[last] == candidateSignature
                && aiMoveHistory[last - 1] == candidateSignature;
        }
        return false;
    }

    // Apply a move on the board (simulate it).
    private void ApplyMove(Move move)
    {
        move.piece.CurrentCell = move.targetCell;
        move.piece.transform.position = board.GetCellPosition(move.targetCell);
        if (move.capturedPiece != null)
            move.capturedPiece.gameObject.SetActive(false);
    }

    // Undo a previously simulated move.
    private void UndoMove(Move move)
    {
        move.piece.CurrentCell = move.fromCell;
        move.piece.transform.position = board.GetCellPosition(move.fromCell);
        if (move.capturedPiece != null)
            move.capturedPiece.gameObject.SetActive(true);
    }

    // Evaluate the board by summing material: positive favors white.
    private int EvaluateBoard()
    {
        int score = 0;

        // Sum white piece values
        var whitePieces = chessManager.GetAllPieces(true);
        foreach (var p in whitePieces)
            if (p.gameObject.activeSelf)
                score += GetPieceValue(p);

        // Subtract black piece values
        var blackPieces = chessManager.GetAllPieces(false);
        foreach (var p in blackPieces)
            if (p.gameObject.activeSelf)
                score -= GetPieceValue(p);

        return score;
    }

    // Standard piece values.
    private int GetPieceValue(ChessPiece piece)
    {
        if (piece is Pawn)   return 100;
        if (piece is Knight) return 320;
        if (piece is Bishop) return 330;
        if (piece is Rook)   return 500;
        if (piece is Queen)  return 900;
        if (piece is King)   return 20000;
        return 0;
    }
}
