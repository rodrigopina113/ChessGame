using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using System;

public class StockfishInterface
{
    public string GetBestMove(string fen, int depth = 4)
    {
        string move = null;

        Task task = Task.Run(async () =>
        {
            move = await GetBestMoveOnline(fen, depth);
        });
        task.Wait();

        return move;
    }

    private async Task<string> GetBestMoveOnline(string fen, int depth)
    {
        string url = "https://stockfish.online/api/s/v2.php";
        string jsonBody = $"{{\"fen\": \"{fen}\", \"depth\": {depth}}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Falha de rede: " + request.error);
                return null;
            }

            string response = request.downloadHandler.text;
            Debug.Log("📨 Resposta: " + response);

            try
            {
                var parsed = JsonUtility.FromJson<StockfishResponse>(response);
                if (string.IsNullOrEmpty(parsed.bestmove) || parsed.bestmove == "(none)")
                {
                    Debug.LogError("❌ bestmove inválido: " + parsed.bestmove);
                    return null;
                }

                return parsed.bestmove;
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Falha ao analisar resposta JSON: " + e.Message);
                return null;
            }
        }
    }

    [Serializable]
    private class StockfishResponse
    {
        public string bestmove;
    }

    public void StartEngine() => Debug.Log("✅ Modo Online — engine local ignorada.");
    public void Stop() { }
    public void SendCommand(string _) { }
}
