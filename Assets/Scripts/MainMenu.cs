using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button deleteSaveButton;
    [SerializeField] private GameObject warningPopup;
    
    public void Start()
    {
        UpdateButton();
        warningPopup.SetActive(false);
    }

    public void UpdateButton()
    {
        bool hasSave = saveSystem != null && saveSystem.HasSaveFile();

        if(continueButton != null)
            continueButton.interactable = hasSave;
        
        if(deleteSaveButton != null)
            deleteSaveButton.interactable = hasSave;
    } 

    public void ContinueGame()
    {
        if(saveSystem == null || !saveSystem.HasSaveFile())
        {
            Debug.LogWarning("No Save File Available!");
            UpdateButton();
            return;
        }

        saveLoadUI.LoadGame();
    }

    public void DeleteSave()
    {
        saveSystem.DeleteSave();
        UpdateButton();
    }

    public void StartGame()
    {
        if(saveSystem.HasSaveFile())
        {
            warningPopup.SetActive(true);
            return;
        }
        NewGame();
    }
    
    public void Yes()
    {
        DeleteSave();
        NewGame();
    }
    
    public void No()
    {
        warningPopup.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void NewGame()
    {
        if(GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
        }
        
        SceneManager.LoadScene("ShopScene");
    }
}
