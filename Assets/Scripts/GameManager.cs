using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winStagePanel;

    private bool isGameOver;
    private bool isPaused;
    private bool isWin;

    public void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
        isWin = false;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if(winStagePanel != null)
        {
            winStagePanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (winStagePanel != null) winStagePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if(GameSession.Instance != null)
        {
            GameSession.Instance.CurrentStage = 1;
            SceneChangeManager.Instance.LoadStage();
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void PauseGame()
    {
        if (isGameOver) return;
        if (isWin) return;
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }

    public void Update()
    {
        if (isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
    }

    public void WinStage()
    {
        if(isGameOver || isPaused) return;
        isWin = true;
        Time.timeScale = 0f;
        winStagePanel.SetActive(true);
    }

    
}
