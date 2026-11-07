using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private SaveSystem saveSystem;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private ItemGrid gridItem;
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
        gridItem.LoadItem(data.inventoryDatas);
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

    public List<InventorySaveData> SaveInventoryItem()
    {
        List<InventorySaveData> datas = new ();
        string tempID = null;
        for (int x = 0; x < gridItem.Inventory.GetLength(0); x++)
        {
            for (int y = 0; y < gridItem.Inventory.GetLength(1); y++)
            {
                if (gridItem.Inventory[x, y] != null)
                {
                    if (tempID == gridItem.Inventory[x, y].ItemID)
                    {
                        continue;
                    }
                    if (tempID == null || tempID != gridItem.Inventory[x, y].ItemID)
                    {
                        tempID = gridItem.Inventory[x, y].ItemID;
                    }
                    InventorySaveData data = new()
                    {
                        id = gridItem.Inventory[x, y].ItemID,
                        itemdata = gridItem.Inventory[x, y].ItemData,
                        startX = gridItem.Inventory[x, y].onGridPositionX,
                        startY = gridItem.Inventory[x, y].onGridPositionY,
                        rotateAngle = gridItem.Inventory[x, y].itemRotation,
                        rotated = gridItem.Inventory[x, y].rotated
                    };
                    datas.Add(data);
                }
            }
        }
        return datas;
    }
}
