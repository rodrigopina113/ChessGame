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

    public bool isInitialized = false;
    
    public void Initialize()
    {
        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
        {
            Debug.LogError("ChessManager não encontrado na cena!");
            return;
        }

        board = chessManager.chessboard;
        if (board == null)
        {
            Debug.LogError("Tabuleiro não está pronto ainda!");
            return;
        }

        tiles = board.tiles;
        isInitialized = true; // Set flag when initialization is complete
        Debug.Log("IA pronta para fazer uma jogada...");
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
        // Add this check at the start
        if (!isInitialized)
        {
            Debug.LogWarning("AI not initialized yet!");
            return;
        }
        /*
        if (chessManager == null || board == null || tiles == null)
        {
            Debug.LogError("AI: MakeMove chamado antes de inicializar corretamente!");
            return;
        }*/

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

        chessManager.MovePiece(bestMove.piece, bestMove.targetCell);
        Debug.Log($"AI moved {bestMove.piece.name} to {bestMove.targetCell} (score {bestScore})");

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

        if (chessManager == null || board == null || tiles == null)
        {
            Debug.LogWarning("AI: ChessManager, board ou tiles não estão inicializados.");
            return moves;
        }

        var pieces = chessManager.GetAllPieces(forWhite);

        foreach (var piece in pieces)
        {
            foreach (var tile in tiles)
            {
                if (piece.IsValidMove(tile.name))
                {
                    ChessPiece targetPiece = chessManager.FindPieceAtCell(tile.name);
                    if (targetPiece == null || targetPiece.isWhite != piece.isWhite)
                    {
                        if (forWhite)
                        {
                            string sig = piece.CurrentCell + "-" + tile.name;
                            if (IsMoveRepetitive(sig)) continue;
                        }

                        moves.Add(new Move
                        {
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
