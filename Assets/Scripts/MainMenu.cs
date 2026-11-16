using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Character Name Input")]
    [SerializeField] private GameObject nameInputPopup;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text nameErrorText;
    [SerializeField] private SaveLoadUI saveLoadUI;
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button deleteSaveButton;
    [SerializeField] private GameObject warningPopup;


    public void Start()
    {
        UpdateButton();
        warningPopup.SetActive(false);
        nameInputPopup.SetActive(false);
    }

    public void UpdateButton()
    {
        bool hasSave = saveSystem != null && saveSystem.HasSaveFile();

        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (deleteSaveButton != null)
            deleteSaveButton.interactable = hasSave;
    }

    public void ContinueGame()
    {
        if (saveSystem == null || !saveSystem.HasSaveFile())
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
        if (saveSystem.HasSaveFile())
        {
            warningPopup.SetActive(true);
            return;
        }
        NewGame();
    }

    public void Yes()
    {
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
        if (GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
        }
        warningPopup.SetActive(false);
        if (nameInputPopup != null)
        {
            nameInputPopup.SetActive(true);
            if (nameInputField != null)
            {
                nameInputField.text = "";
                nameInputField.characterLimit = 16;
                nameInputField.Select();
            }
            if (nameErrorText != null) nameErrorText.text = "";
        }
    }

    public void ConfirmCharacterName()
    {
        string rawName = nameInputField != null ? nameInputField.text.Trim() : "";
        if (string.IsNullOrEmpty(rawName) || rawName.Length < 2)
        {
            if (nameErrorText != null)
                nameErrorText.text = "Character must have at least 2 letters";
            return;
        }
        if (GameSession.Instance == null)
        {
            new GameObject("GameSession").AddComponent<GameSession>();
        }
        GameSession.Instance.ResetSession();
        GameSession.Instance.SetPlayerProfile(rawName);
        if(saveSystem.HasSaveFile())
            DeleteSave();
        SceneManager.LoadScene("ShopScene");
    }

    public void CancelNameInput()
    {
        if (nameInputPopup != null)
            nameInputPopup.SetActive(false);
    }
}
