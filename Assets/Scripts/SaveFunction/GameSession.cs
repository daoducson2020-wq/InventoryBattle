using UnityEngine;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set;}
    public int CurrentStage { get; set;}
    public int CurrentFloor { get; private set; }
    public int CurrentHealth { get; private set; }
    public int CurrentMaxHealth { get; private set; }
    public bool HasSavedHealth { get; private set; }
    public int CurrentGold { get; private set;}
    public bool HasSaveGold { get; private set;}
    public bool HasSavedMap { get; private set;}

    private List<InventorySaveData> playerInventoryDatas = new();
    public List<InventorySaveData> PlayerInventoryDatas => playerInventoryDatas;

    private List<List<Room>> savedMapData = new();
    public List<List<Room>> SavedMapData => savedMapData;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveItemInInventory(List<InventorySaveData> datas)
    {
        playerInventoryDatas.Clear();
        playerInventoryDatas = datas;
    }

    public void SaveGold(int gold)
    {
        CurrentGold = gold;
        HasSaveGold = true;
    }
    
    public void SaveHealth(int health, int maxHealth = 0)
    {
        CurrentHealth = health;
        if (maxHealth > 0)
        {
            CurrentMaxHealth = maxHealth;
        }
        HasSavedHealth = true;
    }

    public void SaveMapData(List<List<Room>> mapData, int floor)
    {
        savedMapData = mapData ?? new List<List<Room>>();
        CurrentFloor = floor;
        HasSavedMap = savedMapData.Count > 0;
    }

    public void ClearPendingMapLoad()
    {
        HasSavedMap = false;
    }

    public void ResetSession()
    {
        CurrentGold = 0;
        HasSaveGold = false;
        CurrentHealth = 0;
        CurrentMaxHealth = 0;
        HasSavedHealth = false;
        CurrentStage = 0;
        CurrentFloor = 0;
        HasSavedMap = false;
        savedMapData.Clear();
        playerInventoryDatas.Clear();
        if (MapView.Instance != null)
        {
            MapView.Instance.ResetMap();
        }
    }
}
