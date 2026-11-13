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

    private void Start()
    {
        // Everything (map, gold, inventory, health) is restored through GameSession / MapView.
        // Do NOT re-apply the map here: it would re-flag GameSession.HasSavedMap and reset progress.
        pendingLoad = null;
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
            currentHealth = GameSession.Instance != null ? GameSession.Instance.CurrentHealth : 0,
            maxHealth = GameSession.Instance != null && GameSession.Instance.HasSavedHealth ? GameSession.Instance.CurrentMaxHealth : 0,
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
        // MapView applies this when MapScene loads, so no SaveLoadUI is needed there
        List<List<Room>> mapToLoad = MapView.ConvertFromSaveMapData(data.mapData);
        MapView.SetPendingMap(mapToLoad, data.floor);
        if (GameSession.Instance == null)
        {
            // Persistent holder so gold/inventory/health/map survive into scenes without wallet/grid
            new GameObject("GameSession").AddComponent<GameSession>();
        }
        GameSession.Instance.SaveGold(data.wallet);
        GameSession.Instance.SaveItemInInventory(data.inventoryDatas ?? new List<InventorySaveData>());
        if (data.maxHealth > 0)
        {
            GameSession.Instance.SaveHealth(data.currentHealth, data.maxHealth);
        }
        GameSession.Instance.SaveMapData(mapToLoad, data.floor);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapScene");
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
