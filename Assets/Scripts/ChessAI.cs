using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Text;

public class ChessAI : MonoBehaviour
{
    private ChessManager chessManager;

    public bool isInitialized = false;
    [SerializeField] private int depth = 10;

    public void Initialize()
    {
        chessManager = UnityEngine.Object.FindFirstObjectByType<ChessManager>();
        if (chessManager == null)
        {
            Debug.LogError("ChessManager não encontrado na cena!");
            return;
        }

        isInitialized = true;
        Debug.Log("IA ChessAPI Online pronta.");
    }

    public void MakeMove(Action onMoveCompleted = null)
    {
        if (!isInitialized || chessManager == null)
            return;

        string fen = FenUtility.GenerateFEN(chessManager).Trim().Replace("  ", " ");
        Debug.Log("♟️ FEN gerado: " + fen);
        StartCoroutine(RequestBestMoveFromChessAPI(fen, depth, onMoveCompleted));
    }

    private IEnumerator RequestBestMoveFromChessAPI(string fen, int depth, Action callback)
    {
        string fenEscaped = fen.Replace("\\", "\\\\").Replace("\"", "\\\"");
        string jsonBody = "{\"fen\":\"" + fenEscaped + "\",\"depth\":" + depth + ",\"variants\":1}";
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest("https://chess-api.com/v1", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Falha na ligação: " + request.error);
            yield break;
        }

        string response = request.downloadHandler.text;
        Debug.Log("📨 Resposta recebida: " + response);

        ChessApiResponse result;
        try
        {
            result = JsonUtility.FromJson<ChessApiResponse>(response);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Erro ao interpretar resposta JSON: " + e.Message);
            yield break;
        }

        if (string.IsNullOrEmpty(result.move) || result.move.Length < 4)
        {
            Debug.LogError("❌ Movimento inválido: " + result.move);
            yield break;
        }

        string from = result.move.Substring(0, 2);
        string to = result.move.Substring(2, 2);

        if (!chessManager.IsValidAIMove(from, to))
        {
            Debug.LogError($"❌ Movimento inválido: Peça em {from} não existe ou pertence ao jogador errado.");
            yield break;
        }

        ChessPiece piece = chessManager.FindPieceAtCell(from);
        chessManager.MovePiece(piece, to);
        Debug.Log($"🤖 ChessAPI moveu {piece.name} de {from} para {to}");

        yield return new WaitForSeconds(0.4f);
        callback?.Invoke();
    }

    [Serializable]
    private class ChessApiResponse
    {
        public string move;
        public string status;
        public float eval;
    }
}
