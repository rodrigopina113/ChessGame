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
    public TextMeshProUGUI currentPlayerText;
    public GameObject winPanel;
    public bool showWhiteWinsOnTimeout = false;

    private float whiteTimeRemaining;
    private float blackTimeRemaining;

    private bool isWhiteTurn = true;
    private bool isCounting = false;



    void Start()
    {
        whiteTimeRemaining = blackTimeRemaining = totalMinutes * 60f;
        UpdateTurnText();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!isCounting) return;

        if (whiteTimeRemaining > 0 || blackTimeRemaining > 0)
            Debug.Log("⏱️ Atualizando relógio. Turno branco: " + isWhiteTurn);

        if (isWhiteTurn)
        {
            whiteTimeRemaining -= Time.deltaTime;
            if (whiteTimeRemaining <= 0)
            {
                whiteTimeRemaining = 0;
                EndGame(false);
            }
        }
        else
        {
            blackTimeRemaining -= Time.deltaTime;
            if (blackTimeRemaining <= 0)
            {
                blackTimeRemaining = 0;
                EndGame(true);
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


    public void StartTimers()
    {
        isCounting = true;
        Debug.Log("✔️ StartTimers chamado: isCounting = " + isCounting);
    }

 
    public void SwitchTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (currentPlayerText != null)
            currentPlayerText.text = isWhiteTurn ? "Brancas a jogar" : "Pretas a jogar";
    }

 
    public void PauseTimers()
    {
        isCounting = false;
    }

  
    public void ResumeTimers()
    {
        isCounting = true;
    }

    private void EndGame(bool whiteWins)
    {
        isCounting = false;

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f; // ⏸️ Pausar o jogo completamente

        Debug.Log(whiteWins ? "White wins on time!" : "Black wins on time!");
    }

}
