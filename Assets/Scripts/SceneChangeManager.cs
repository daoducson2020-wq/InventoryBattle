using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;
    [SerializeField] private SaveLoadUI saveLoadUI;

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
                SaveStage();
            }
            else
            {
                SaveStage();
            }
        }
    }

    public void ReturnToMap()
    {
        SceneManager.LoadScene("MapScene");
    }

    public void SaveStage()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.SaveStage(currentStage);
        }
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
        SaveStage();
    }
}
