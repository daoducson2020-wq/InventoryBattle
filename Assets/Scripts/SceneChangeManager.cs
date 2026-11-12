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
            }
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

        Time.timeScale = 1f;
        SceneManager.LoadScene("MapScene");
    }

}
