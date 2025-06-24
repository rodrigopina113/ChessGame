using System.Diagnostics;
using System.IO;
using UnityEngine;


public class StockfishInterface
{
    private Process engine;

    public void StartEngine()
    {
        string enginePath = Path.Combine(Application.streamingAssetsPath, "Engine/stockfish-windows-x86-64-avx2.exe");

        engine = new Process();
        engine.StartInfo.FileName = enginePath;
        engine.StartInfo.RedirectStandardInput = true;
        engine.StartInfo.RedirectStandardOutput = true;
        engine.StartInfo.UseShellExecute = false;
        engine.StartInfo.CreateNoWindow = true;
        engine.Start();

        SendCommand("uci");
        SendCommand("isready");
        WaitForReady();
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
        engine.StandardInput.WriteLine(command);
        engine.StandardInput.Flush();
    }

    public string GetBestMove(string fen, int depth = 2)
    {
        SendCommand("position fen " + fen);
        SendCommand("go depth " + depth);

        string line;
        while ((line = engine.StandardOutput.ReadLine()) != null)
        {
            if (line.StartsWith("bestmove"))
            {
                string move = line.Split(' ')[1];
                return move;
            }
        }

        return null;
    }

    public void Stop()
    {
        SendCommand("quit");
        if (!engine.HasExited)
            engine.Kill();
    }
}
