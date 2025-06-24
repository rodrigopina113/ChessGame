using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
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
        bool engineOk = stockfish.StartEngine();

        if (!engineOk)
        {
            Debug.LogWarning("⚠️ Stockfish local indisponível. Apenas API online será usada.");
        }


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

        string move = stockfish != null ? stockfish.GetBestMove(fen, stockfishDepth) : null;

        if (!string.IsNullOrEmpty(move))
        {
            Debug.Log("✅ Movimento obtido via Stockfish local.");
            ExecuteMove(move, onMoveCompleted);
        }
        else
        {
            Debug.LogWarning("⚠️ Stockfish local falhou. Usando fallback online...");
            StartCoroutine(GetMoveFromWebAPI(fen, onMoveCompleted));
        }
    }

    private void ExecuteMove(string move, Action onMoveCompleted)
    {
        if (string.IsNullOrEmpty(move) || move.Length < 4)
        {
            Debug.LogError("Movimento inválido recebido: " + move);
            return;
        }

        string from = move.Substring(0, 2);
        string to = move.Substring(2, 2);

        ChessPiece piece = chessManager.FindPieceAtCell(from);
        if (piece != null)
        {
            chessManager.MovePiece(piece, to);
            Debug.Log($"🤖 Move: {piece.name} de {from} para {to}");
            chessManager.StartCoroutine(MakeMoveAndWait(piece, to, onMoveCompleted));
        }
        else
        {
            Debug.LogError("Peça não encontrada para o movimento sugerido.");
        }
    }

    private IEnumerator GetMoveFromWebAPI(string fen, Action onMoveCompleted)
    {
        string url = "https://stockfish.online/api/s/v2.php?fen=" + UnityWebRequest.EscapeURL(fen) + $"&depth={stockfishDepth}";
        using UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Erro na API online: " + www.error);
            yield break;
        }

        try
        {
            string json = www.downloadHandler.text;
            OnlineStockfishResponse response = JsonUtility.FromJson<OnlineStockfishResponse>(json);
            if (response != null && response.status == "ok" && !string.IsNullOrEmpty(response.move))
            {
                Debug.Log("✅ Movimento via API online.");
                ExecuteMove(response.move, onMoveCompleted);
            }
            else
            {
                Debug.LogError("❌ Resposta inválida da API: " + json);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Erro ao interpretar resposta: " + ex.Message);
        }
    }

    private IEnumerator MakeMoveAndWait(ChessPiece piece, string to, Action callback)
    {
        chessManager.MovePiece(piece, to);
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }

    [Serializable]
    private class OnlineStockfishResponse
    {
        public string status;
        public string move;
    }
}
