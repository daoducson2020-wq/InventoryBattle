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

    [ContextMenu("DevFeature | Save Enemy Data")]
    public void SaveEnemyData()
    {
        List<InventorySaveData> datas = new ();
        HashSet<string> savedItemIds = new();
        for (int x = 0; x < enemyGrid.Inventory.GetLength(0); x++)
        {
            for (int y = 0; y < enemyGrid.Inventory.GetLength(1); y++)
            {
                InventoryItem item = enemyGrid.Inventory[x, y];
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

        EnemySaveData enemySaveData = new();
        enemySaveData.AddData("Chad Cheems", datas);
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
        int rnd = Random.Range(0, data.enemyDatas.Count);
        enemyGrid.LoadItem(data.enemyDatas[rnd].value);
    }
}
