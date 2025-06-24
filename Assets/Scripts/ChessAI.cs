using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;


public class ChessAI : MonoBehaviour
{
    private StockfishInterface stockfish;
    private ChessManager chessManager;

    public bool isInitialized = false;
    [SerializeField] private int stockfishDepth = 3;

    public void Initialize()
    {
        stockfish = new StockfishInterface();
        stockfish.StartEngine();

        chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
        {
            Debug.LogError("ChessManager não encontrado na cena!");
            return;
        }

        isInitialized = true;
        Debug.Log("IA Stockfish pronta.");
    }

    public void MakeMove(Action onMoveCompleted = null)
    {
    if (!isInitialized || chessManager == null)
        return;

    string fen = FenUtility.GenerateFEN(chessManager);
    Debug.Log("♟️ FEN gerado: " + fen);
    string move = stockfish.GetBestMove(fen, stockfishDepth);

    if (string.IsNullOrEmpty(move))
    {
        Debug.Log("❌ Stockfish não retornou movimento.");
        return;
    }

    string from = move.Substring(0, 2);
    string to = move.Substring(2, 2);

    ChessPiece piece = chessManager.FindPieceAtCell(from);
        if (piece != null)
        {
            chessManager.MovePiece(piece, to);
            Debug.Log($"🤖 Stockfish moveu {piece.name} de {from} para {to}");

            // Aguarda até o movimento terminar visualmente
            chessManager.StartCoroutine(MakeMoveAndWait(piece, to, onMoveCompleted));
        }
        else
        {
            Debug.LogError("Peça não encontrada para o movimento sugerido.");
        }
    }
    private IEnumerator MakeMoveAndWait(ChessPiece piece, string to, Action callback)
    {
        chessManager.MovePiece(piece, to);

        // Espera até que o movimento gráfico tenha tempo de se completar
        yield return new WaitForSeconds(0.5f);

        // Só então chama o callback (que por sua vez invoca CheckOpponentStatus + EndTurn)
        callback?.Invoke();
    }

    private IEnumerator WaitAndInvoke(Action callback)
    {
        yield return new WaitForSeconds(0.4f); // espera o MovePieceRoutine (300ms)
        callback?.Invoke();
    }

}
