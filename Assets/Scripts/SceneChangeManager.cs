using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance;
<<<<<<< Updated upstream
=======
    [SerializeField] private SaveLoadUI saveLoadUI;

>>>>>>> Stashed changes
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

    public void LoadStage()
    {
<<<<<<< Updated upstream
=======
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameSession.Instance.SaveItemInInventory(saveLoadUI.SaveInventoryItem());
        }
>>>>>>> Stashed changes
        if (SceneManager.GetActiveScene().name == "ShopScene")
        {
            currentStage++;
            SaveStage();
            Debug.Log("Index: " + currentStage);
            SceneManager.LoadScene(currentStage);
        }
        else
        {
            Debug.Log("Load: ShopScene");
            SceneManager.LoadScene("ShopScene");
        }
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
