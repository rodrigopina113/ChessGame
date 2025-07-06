using System.Diagnostics;
using System.IO;
using UnityEngine;

using System;


public class StockfishInterface
{
    private Process engine;

   public void StartEngine()
    {
        string enginePath = Path.Combine(Application.streamingAssetsPath, "Engine/stockfish-windows-x86-64-avx2.exe");

        if (!File.Exists(enginePath))
        {
            UnityEngine.Debug.LogError("❌ Stockfish não encontrado: " + enginePath);
            return;
        }

        engine = new Process();
        engine.StartInfo.FileName = enginePath;
        engine.StartInfo.RedirectStandardInput = true;
        engine.StartInfo.RedirectStandardOutput = true;
        engine.StartInfo.UseShellExecute = false;
        engine.StartInfo.CreateNoWindow = true;

        try
        {
            engine.Start();
            UnityEngine.Debug.Log("✅ Stockfish iniciado.");

            if (engine.HasExited)
            {
                UnityEngine.Debug.LogError("❌ Stockfish fechou imediatamente após iniciar.");
                string output = engine.StandardOutput.ReadToEnd();
                UnityEngine.Debug.LogError("🧾 Output capturado antes de fechar: " + output);
                return;
            }

            SendCommand("uci");
            SendCommand("isready");
            WaitForReady();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("❌ Erro ao iniciar Stockfish: " + ex.Message);
        }
    }


    private void WaitForReady()
    {
        string line;
        while ((line = engine.StandardOutput.ReadLine()) != null)
        {
            if (line == "readyok")
                break;
        }
    }

    public void SendCommand(string command)
    {
        if (engine == null || engine.HasExited)
        {
            UnityEngine.Debug.LogError("❌ Tentativa de enviar comando, mas Stockfish já terminou.");
            return;
        }

        try
        {
            engine.StandardInput.WriteLine(command);
            engine.StandardInput.Flush();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"❌ Falha ao enviar comando para Stockfish: {command}\n{ex.Message}");
        }
    }

public string GetBestMove(string fen, int depth = 2)
{
    if (engine == null || engine.HasExited)
    {
        UnityEngine.Debug.LogError("❌ Stockfish não está ativo. Abortando GetBestMove.");
        return null;
    }

    UnityEngine.Debug.Log("Enviando FEN para Stockfish: " + fen);
    SendCommand("position fen " + fen);
    SendCommand("go depth " + depth);

    DateTime start = DateTime.Now;
    TimeSpan timeout = TimeSpan.FromSeconds(10);
    string fullLog = "";

    string line;
    while ((line = engine.StandardOutput.ReadLine()) != null)
    {
        fullLog += line + "\n";

        if (line.StartsWith("bestmove"))
        {
            string[] parts = line.Split(' ');
            if (parts.Length > 1 && parts[1] != "(none)")
                return parts[1];
            else
            {
                UnityEngine.Debug.LogError("❌ Stockfish retornou 'bestmove (none)'.\nLog:\n" + fullLog);
                return null;
            }
        }

        if ((DateTime.Now - start) > timeout)
        {
            UnityEngine.Debug.LogError("⏱ Timeout à espera de resposta do Stockfish.\nLog parcial:\n" + fullLog);
            return null;
        }
    }

    UnityEngine.Debug.LogError("❌ Stockfish terminou sem enviar bestmove.\nÚltimo log:\n" + fullLog);
    return null;
}



    public void Stop()
    {
        SendCommand("quit");
        if (!engine.HasExited)
            engine.Kill();
    }
}
