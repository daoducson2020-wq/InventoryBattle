using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BigEnemyManager : MonoBehaviour
{
    [SerializeField] private ItemGrid enemyGrid;
    [SerializeField] private GameObject enemyInventory;
    [SerializeField] private Image enemyImage;
    [SerializeField] private Sprite[] enemyImages;

    private bool isShow = false;
    private string filePath = Application.dataPath + "/EnemyConfig/enemies.json";

    private void Start()
    {
        Load();
        SetAlpha(0f);
    }

    [ContextMenu("Save Enemy Data")]
    public void SaveEnemyData()
    {
        List<InventorySaveData> datas = new();
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
        if (!File.Exists(filePath)) return null;

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<EnemySaveData>(json);
    }

    [ContextMenu("Load Enemy Data")]
    public void Load()
    {
        EnemySaveData data = LoadEnemyData();
        LoadBossType(data);
    }

    public void ShowEnemyInventory()
    {
        if (!isShow)
        {
            isShow = true;
            SetAlpha(1f);
        }
        else
        {
            isShow = false;
            SetAlpha(0f);
        }
    }

    private void SetAlpha(float value)
    {
        enemyInventory.GetComponent<CanvasGroup>().alpha = value;
    }

    private void LoadBossType(EnemySaveData data)
    {
        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            enemyGrid.LoadItem(data.enemyDatas[data.enemyDatas.Count - 1].value);
        }
        else if (SceneManager.GetActiveScene().name == "MiniBossScene")
        {
            if (MapView.Instance.FloorsClimbed < 5)
            {
                int rnd = Random.Range(0, 3);
                Debug.Log("loadout: " + rnd);
                enemyGrid.LoadItem(data.enemyDatas[rnd].value);
                enemyImage.GetComponent<Image>().sprite = enemyImages[0];
            }
            else
            {
                int rnd = Random.Range(3, data.enemyDatas.Count - 1);
                enemyGrid.LoadItem(data.enemyDatas[rnd].value);
                enemyImage.GetComponent<Image>().sprite = enemyImages[1];
            }
        }
    }
}
