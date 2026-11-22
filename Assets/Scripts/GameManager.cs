using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState { Normal, Combat }
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winStagePanel;
    [SerializeField] private BattleStartTextUI battleStartTextUI;
    [SerializeField] private GameObject informationView;
    [SerializeField] private GameObject inforBTN;
    [SerializeField] private GameObject speedControl;
    [SerializeField] private TMP_Text speedText;
    private int speed = 1;
    private bool isOpenView;
    private NetworkManager networkManager;
    private Health playerHealth;
    private ItemGrid playerItemGrid;
    [SerializeField] private SaveSystem saveSystem;

    [SerializeField] private float battleStartTimer = 2f;

    private GameState currentState = GameState.Normal;
    public GameState CurrentState => currentState;
    private bool isGameOver;
    private bool isPaused;
    private bool isWin;
    private bool hasSavedRecord;
    private float counterTimer;
    public float CounterTimer => counterTimer;

    public void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isPaused = false;
        isWin = false;
        hasSavedRecord = false;
        isOpenView = false;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (winStagePanel != null)
        {
            winStagePanel.SetActive(false);
        }
        if (informationView != null)
            informationView.SetActive(false);
        if (inforBTN != null)
            inforBTN.SetActive(true);
        if (speedText != null)
            speedText.text = "Speed X1";
        SceneChangeManager.Instance.TransitionScene.Play("BackToNormal");
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
        //Debug.Log("current state: " + currentState);
    }

    public void OpenInformationView()
    {
        if (isOpenView) return;
        isOpenView = true;
        inforBTN.SetActive(false);
        informationView.SetActive(true);
    }

    public void GameSpeedControl()
    {
        speed++;
        switch (speed)
        {
            case 2:
                Time.timeScale = 2f;
                speedText.text = "Speed X2";
                break;
            case 3:
                Time.timeScale = 3f;
                speedText.text = "Speed X3";
                break;
            case 4:
                Time.timeScale = 1f;
                speedText.text = "Speed X1";
                speed = 1;
                break;
        }
    }

    public void CloseInformationView()
    {
        if (!isOpenView) return;
        isOpenView = false;
        informationView.SetActive(false);
        inforBTN.SetActive(true);
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        SavePlayerRecord();
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (winStagePanel != null) winStagePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        saveSystem.DeleteSave();
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
        if (isGameOver || isPaused) return;
        isWin = true;
        StartCoroutine(PlayVictoryMusic());
        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            SavePlayerRecord();
        }
        // Time.timeScale = 0f;
        winStagePanel.SetActive(true);
    }

    public void SavePlayerRecord()
    {
        if (hasSavedRecord) return;
        hasSavedRecord = true;

        if (networkManager == null)
        {
            networkManager = NetworkManager.Instance != null
                ? NetworkManager.Instance
                : FindAnyObjectByType<NetworkManager>();
        }
        if (networkManager == null)
        {
            networkManager = new GameObject("NetworkManager").AddComponent<NetworkManager>();
        }

        ItemGrid grid = ResolvePlayerItemGrid();
        int inventoryValue = grid != null ? grid.GetTotalInventoryValue() : 0;

        int floorsClimbed = MapView.Instance != null
            ? MapView.Instance.FloorsClimbed
            : (GameSession.Instance != null ? GameSession.Instance.CurrentFloor : 0);

        string playerName = (GameSession.Instance != null && !string.IsNullOrWhiteSpace(GameSession.Instance.PlayerName))
            ? GameSession.Instance.PlayerName
            : "Hero";

        networkManager.SendRecord(playerName, inventoryValue, floorsClimbed);
    }

    private ItemGrid ResolvePlayerItemGrid()
    {
        if (playerItemGrid != null) return playerItemGrid;

        if (playerHealth != null && playerHealth.TryGetComponent(out ItemGrid healthGrid))
        {
            playerItemGrid = healthGrid;
            return playerItemGrid;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.TryGetComponent(out ItemGrid taggedGrid))
        {
            playerItemGrid = taggedGrid;
            return playerItemGrid;
        }

        foreach (ItemGrid grid in FindObjectsByType<ItemGrid>())
        {
            if (grid.CompareTag("Player"))
            {
                playerItemGrid = grid;
                return playerItemGrid;
            }
        }

        return null;
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
                BeginBattle();
            }
        }
        counterTimer += Time.deltaTime;
    }

    private void BeginBattle()
    {
        if (counterTimer >= 2.01f && counterTimer <= 2.03f)
        {
            SoundManager.Instance.PlayBeginBattle();
        }
    }

    IEnumerator PlayVictoryMusic()
    {
        MusicManager.Instance.StopPlayMusic();
        yield return new WaitForSeconds(.2f);
        SoundManager.Instance.PlayVictory();
        yield return new WaitForSeconds(.1f);
        Time.timeScale = 0f;
    }
}
