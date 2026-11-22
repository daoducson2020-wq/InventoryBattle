using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private Animator transitionScene;
    public Animator TransitionScene => transitionScene;

    private int currentStage = 1;
    public int CurrentStage => currentStage;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        if (GameSession.Instance != null)
        {
            if (GameSession.Instance.CurrentStage > 0)
            {
                currentStage = GameSession.Instance.CurrentStage;
            }
        }
        if (transitionScene == null)
        {
            transitionScene = GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>();
        }
    }

    public void ReturnToMap()
    {
        if (SceneManager.GetActiveScene().name != "MainMenuScene"
            && saveLoadUI != null
            && GameSession.Instance != null)
        {
            GameSession.Instance.SaveItemInInventory(saveLoadUI.SaveInventoryItem());
        }
        //saveLoadUI.SaveGame();
        Time.timeScale = 1f;
        transitionScene.Play("FadeToBlack");
        if (MapView.Instance != null && MapView.Instance.FloorsClimbed == 15)
        {
            SceneManager.LoadScene("MainMenuScene");
            return;
        }
        if (MapView.Instance != null && MapView.Instance.FloorsClimbed == 14)
        {
            MusicManager.Instance.PlayBossMusic();
        }
        else
        {
            MusicManager.Instance.PlayMapMusic();
        }
        SceneManager.LoadScene("MapScene");
    }
}
