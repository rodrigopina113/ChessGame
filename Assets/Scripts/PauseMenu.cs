using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button pauseButton;

    [Header("Game Manager")]
    public ChessManager chessManager;

    private bool isPaused = false;

    void Awake()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);

        if (chessManager == null)
            chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (pauseButton == null)
            pauseButton = Object.FindFirstObjectByType<Button>();

        pauseButton.onClick.AddListener(TogglePause);
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
   
        Resume();
        chessManager.ResetGame();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
    }
}
