using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button deleteSaveButton;

    public void Start()
    {
        UpdateButton();
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
        if(GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
        }
        
        SceneChangeManager.Instance.LoadStage();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
