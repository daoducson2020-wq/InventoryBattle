using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState { Normal, Combat}
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winStagePanel;
    [SerializeField] private BattleStartTextUI battleStartTextUI;
    [SerializeField] private float battleStartTimer = 2f;

    private GameState currentState = GameState.Normal;
    public GameState CurrentState => currentState;
    private bool isGameOver;
    private bool isPaused;
    private bool isWin;
    private float counterTimer;
    
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

    public void Update()
    {
        if (isGameOver) return;
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();

        UpdateState();
        switch (currentState)
        {
            case GameState.Normal:
                break;
            case GameState.Combat:
                CombatState();
                break;
            default:
                break;
        }
        Debug.Log("current state: " + currentState);
    }

    private void UpdateState()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene" ||
            SceneManager.GetActiveScene().name == "BossScene" ||
            SceneManager.GetActiveScene().name == "MiniBossScene")
        {
            ChangeState(GameState.Combat);
        }
        else
        {
            ChangeState(GameState.Normal);
        }
    }

    private void ChangeState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }
        currentState = newState;
    }

    private void CombatState()
    {
        if (counterTimer >= battleStartTimer)
        {
            if (battleStartTextUI != null)
            {
                battleStartTextUI.PlayAnim();
            }
        }
        counterTimer += Time.deltaTime;
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
        if (GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
        }
        SceneManager.LoadScene("ShopScene");
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
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }

    public void WinStage()
    {
        if(isGameOver || isPaused) return;
        isWin = true;
        Time.timeScale = 0f;
        winStagePanel.SetActive(true);
    }
}
