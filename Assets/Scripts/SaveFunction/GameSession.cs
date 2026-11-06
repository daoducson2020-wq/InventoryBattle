using UnityEngine;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }
    public int CurrentStage { get; set; }
    public int CurrentGold { get; private set; }
    public bool HasSaveGold { get; private set; }
    private List<InventorySaveData> playerInventoryDatas = new();
    public List<InventorySaveData> PlayerInventoryDatas => playerInventoryDatas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveItemInInventory(List<InventorySaveData> datas)
    {
        playerInventoryDatas = datas;
    }
    
    public void SaveGold(int gold)
    {
        CurrentGold = gold;
        HasSaveGold = true;
    }

    public void SaveStage(int stage)
    {
        CurrentStage = stage;
    }

    public void ResetSession()
    {
        CurrentGold = 0;
        HasSaveGold = false;
        CurrentStage = 0;
    }
}
