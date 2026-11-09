using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadEnemyData : MonoBehaviour
{
    [SerializeField] private ItemGrid enemyGrid;
    
    private string filePath = Application.dataPath + "/EnemyConfig/enemies.json";

    private void Start()
    {
        Load();
    }

    [ContextMenu("Save Enemy Data")]
    public void SaveEnemyData()
    {
        List<InventorySaveData> datas = new ();
        string tempID = null;
        for (int x = 0; x < enemyGrid.Inventory.GetLength(0); x++)
        {
            for (int y = 0; y < enemyGrid.Inventory.GetLength(1); y++)
            {
                if (enemyGrid.Inventory[x, y] != null)
                {
                    if (tempID == enemyGrid.Inventory[x, y].ItemID)
                    {
                        continue;
                    }
                    if (tempID == null || tempID != enemyGrid.Inventory[x, y].ItemID)
                    {
                        tempID = enemyGrid.Inventory[x, y].ItemID;
                    }
                    InventorySaveData data = new()
                    {
                        id = enemyGrid.Inventory[x, y].ItemID,
                        itemdata = enemyGrid.Inventory[x, y].ItemData,
                        startX = enemyGrid.Inventory[x, y].onGridPositionX,
                        startY = enemyGrid.Inventory[x, y].onGridPositionY,
                        rotateAngle = enemyGrid.Inventory[x, y].itemRotation,
                        rotated = enemyGrid.Inventory[x, y].rotated
                    };
                    datas.Add(data);
                }
            }
        }

        EnemySaveData enemySaveData = new();
        enemySaveData.AddData("Barbarian", datas);
        string json = JsonUtility.ToJson(enemySaveData, true);

        File.WriteAllText(filePath, json);
        Debug.Log("Data Saved");
    }

    public EnemySaveData LoadEnemyData()
    {
        if(!File.Exists(filePath)) return null;

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<EnemySaveData>(json);
    }

    [ContextMenu("Load Enemy Data")]
    public void Load()
    {
        EnemySaveData data = LoadEnemyData();
        enemyGrid.LoadItem(data.GetValue("Barbarian"));
    }
}
