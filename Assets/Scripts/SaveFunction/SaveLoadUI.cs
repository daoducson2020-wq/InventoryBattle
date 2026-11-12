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
        if (pendingLoad == null) yield break;
        yield return null;

        SaveData data = pendingLoad;

        if (playerWallet == null || sceneChangeManager == null || gridItem == null)
        {
            Debug.LogError("Missing restore references.", this);
            yield break;
        }

        playerWallet.SetGold(data.wallet);
        gridItem.LoadItem(data.inventoryDatas);
        List<List<Room>> loadedMap = MapView.ConvertFromSaveMapData(data.mapData);
        if (GameSession.Instance != null)
        {
            GameSession.Instance.SaveItemInInventory(SaveInventoryItem());
            GameSession.Instance.SaveMapData(loadedMap, data.floor);
        }
        if (MapView.Instance != null && loadedMap.Count > 0)
        {
            MapView.Instance.LoadMap(loadedMap, data.floor);
        }
        pendingLoad = null;
        Debug.Log("Game Loaded Successfully!");
    }

    public void SaveGame()
    {
        if (saveSystem == null || playerWallet == null)
        {
            Debug.LogWarning("Missing Save References!");
            return;
        }

        int currentFloor = 0;
        List<List<Room>> currentMap = null;

        if (MapView.Instance != null && MapView.Instance.MapData.Count > 0)
        {
            currentMap = MapView.Instance.MapData;
            currentFloor = MapView.Instance.FloorsClimbed;
        }
        else if (GameSession.Instance != null)
        {
            currentMap = GameSession.Instance.SavedMapData;
            currentFloor = GameSession.Instance.CurrentFloor;
        }

        SaveData data = new()
        {
            floor = currentFloor,
            wallet = playerWallet.CurrentGold,
            inventoryDatas = SaveInventoryItem(),
            mapData = MapView.ConvertToSaveMapData(currentMap)
        };

        saveSystem.SaveGame(data);
    }

    public void LoadGame()
    {
        if (saveSystem == null)
        {
            Debug.LogWarning("Missing Load References!");
            return;
        }

        SaveData data = saveSystem.LoadGame();

        if (data == null)
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
        List<InventorySaveData> datas = new();
        HashSet<string> savedItemIds = new();
        for (int x = 0; x < gridItem.Inventory.GetLength(0); x++)
        {
            for (int y = 0; y < gridItem.Inventory.GetLength(1); y++)
            {
                InventoryItem item = gridItem.Inventory[x, y];
                if (item != null && savedItemIds.Add(item.ItemID))
                {
                    InventorySaveData data = new()
                    {
                        id = item.ItemID,
                        itemName = item.ItemName,
                        startX = item.onGridPositionX,
                        startY = item.onGridPositionY,
                        rotateAngle = item.itemRotation,
                        rotated = item.rotated
                    };
                    datas.Add(data);
                }
            }
        }
        return datas;
    }
}
