using System.Collections;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private SceneChangeManager sceneChangeManager;
    private static SaveData pendingLoad;

    private IEnumerator Start()
    {
        if(pendingLoad == null) yield break;
        yield return null;

        SaveData data = pendingLoad;

        if(playerWallet == null || sceneChangeManager == null)
        {
            Debug.LogError("Mising Restore references!");
            yield break;
        }

        playerWallet.SetGold(data.wallet);
        sceneChangeManager.SetStage(data.stage);
        pendingLoad = null;
        Debug.Log("Game Loaded Successfully!");
    }

    public void SaveGame()
    {   
        if(saveSystem == null || playerWallet == null)
        {
            Debug.LogWarning("Missing Save References!");
            return;
        }

        SaveData data = new()
        {
            wallet = playerWallet.CurrentGold,
            stage = sceneChangeManager.CurrentStage - 1
        };

        saveSystem.SaveGame(data);
    }

    public void LoadGame()
    {
        if(saveSystem == null)
        {
            Debug.LogWarning("Missing Load References!");
            return;
        }

        SaveData data = saveSystem.LoadGame();

        if(data == null)
        {
            Debug.Log("No Save file Found!");
            return;
        }
        pendingLoad = data;
        Time.timeScale = 1f;
        SceneManager.LoadScene("ShopScene");
    }
}
