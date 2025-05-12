using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel; // Assign PauseMenuPanel here
    public Button pauseButton; // Assign PauseButton here

    [Header("Game Manager")]
    public ChessManager chessManager; // Assign your ChessManager here

    private bool isPaused = false;

    void Awake()
    {
        // Ensure timescale is normal at start
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);

        // Fallbacks using the new API:
        if (chessManager == null)
            chessManager = Object.FindFirstObjectByType<ChessManager>();
        if (pauseButton == null)
            pauseButton = Object.FindFirstObjectByType<Button>();

        pauseButton.onClick.AddListener(Pause);
    }

    void Update()
    {
        // Optional: toggle pause with Escape key
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
        // Unpause so that ResetGame logic (and any coroutines) run normally
        Resume();
        chessManager.ResetGame();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        // e.g. UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
