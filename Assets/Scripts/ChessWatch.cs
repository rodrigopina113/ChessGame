using UnityEngine;
using TMPro;

public class ChessWatch : MonoBehaviour
{
    [Header("Tempo total por jogador (em minutos)")]
    [Range(1, 60)]
    public float totalMinutes = 10f;

    [Header("Referências de UI")]
    public TextMeshProUGUI whiteTimerText;
    public TextMeshProUGUI blackTimerText;
    public GameObject winPanel;
    public bool showWhiteWinsOnTimeout = false;

    private float whiteTimeRemaining;
    private float blackTimeRemaining;

    private bool isWhiteTurn = true;
    private bool isCounting = false;

    void Start()
    {
        whiteTimeRemaining = blackTimeRemaining = totalMinutes * 60f;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isCounting) return;

        if (isWhiteTurn)
        {
            whiteTimeRemaining -= Time.deltaTime;
            if (whiteTimeRemaining <= 0)
            {
                whiteTimeRemaining = 0;
                EndGame(false); // preto venceu
            }
        }
        else
        {
            blackTimeRemaining -= Time.deltaTime;
            if (blackTimeRemaining <= 0)
            {
                blackTimeRemaining = 0;
                EndGame(true); // branco venceu
            }
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (whiteTimerText != null)
            whiteTimerText.text = FormatTime(whiteTimeRemaining);
        if (blackTimerText != null)
            blackTimerText.text = FormatTime(blackTimeRemaining);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    /// <summary>
    /// Deve ser chamado no início do jogo.
    /// </summary>
    public void StartTimers()
    {
        isCounting = true;
    }

    /// <summary>
    /// Deve ser chamado no fim do turno, após mover a peça.
    /// </summary>
    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }

    /// <summary>
    /// Para temporariamente a contagem de tempo (ex: pausa).
    /// </summary>
    public void PauseTimers()
    {
        isCounting = false;
    }

    /// <summary>
    /// Retoma a contagem de tempo após pausa.
    /// </summary>
    public void ResumeTimers()
    {
        isCounting = true;
    }

    private void EndGame(bool whiteWins)
    {
        isCounting = false;
        if (winPanel != null)
            winPanel.SetActive(true);

        Debug.Log(whiteWins ? "White wins on time!" : "Black wins on time!");
    }
}
