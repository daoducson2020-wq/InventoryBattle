using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BigEnemyManager : MonoBehaviour
{
    [SerializeField] private ItemGrid enemyGrid;
    [SerializeField] private Health enemyHealth;
    [SerializeField] private GameObject enemyInventory;
    [SerializeField] private Image enemyImage;
    [SerializeField] private TextAsset enemyConfigJson;
    [SerializeField] private Sprite[] enemyImages;

    private bool isShow = false;

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

#if UNITY_EDITOR
        string path = enemyConfigJson != null
            ? UnityEditor.AssetDatabase.GetAssetPath(enemyConfigJson)
            : Path.Combine(Application.dataPath, "EnemyConfig/enemies.json");
        File.WriteAllText(path, json);
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Enemy data saved to: " + path);
#endif
    }

    public EnemySaveData LoadEnemyData()
    {
        if (enemyConfigJson != null)
        {
            return JsonUtility.FromJson<EnemySaveData>(enemyConfigJson.text);
        }

        string fallbackPath = Path.Combine(Application.dataPath, "EnemyConfig/enemies.json");
        if (File.Exists(fallbackPath))
        {
            return JsonUtility.FromJson<EnemySaveData>(File.ReadAllText(fallbackPath));
        }

        Debug.LogWarning("Enemy config TextAsset is not assigned!");
        return null;
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
        enemyGrid.GetComponent<Image>().raycastTarget = value == 0f ? false : true;
    }

    private void LoadBossType(EnemySaveData data)
    {
        if (data == null || data.enemyDatas == null || data.enemyDatas.Count == 0) return;

        EnemyEntry selectedEnemy = null;
        int targetGoldReward = 0;

        if (SceneManager.GetActiveScene().name == "BossScene")
        {
            selectedEnemy = data.enemyDatas[data.enemyDatas.Count - 1]; // "Chad Cheems"
            targetGoldReward = 100;
        }
        else if (SceneManager.GetActiveScene().name == "MiniBossScene")
        {
            if (MapView.Instance != null && MapView.Instance.FloorsClimbed < 5)
            {
                int rnd = Random.Range(0, 3); // 0: Assassin, 1: Ranger, 2: Grunt
                Debug.Log("loadout: " + rnd);
                selectedEnemy = data.enemyDatas[rnd];
                enemyImage.GetComponent<Image>().sprite = enemyImages[0];
                targetGoldReward = 55;
            }
            else
            {
                int rnd = Random.Range(3, data.enemyDatas.Count - 1); // 3: Paladin, 4: Hunter, 5: Marksman, 6: Berserker
                selectedEnemy = data.enemyDatas[rnd];
                enemyImage.GetComponent<Image>().sprite = enemyImages[1];
                targetGoldReward = 80;
            }
        }

        if (selectedEnemy != null)
        {
            // 1. Load trang bị của MiniBoss/Boss lên Grid
            enemyGrid.LoadItem(selectedEnemy.value);

            // 2. Hiển thị tên lấy từ trường "key" trong enemies.json lên thanh máu
            if (enemyHealth == null && enemyGrid != null)
            {
                enemyHealth = enemyGrid.GetComponent<Health>();
            }
            if (enemyHealth != null)
            {
                enemyHealth.SetUnitName(selectedEnemy.key);
            }

            // 3. Cập nhật tiền thưởng theo phân tầng độ khó (MiniBoss < 5: 40G, MiniBoss >= 5: 65G, Boss: 100G)
            EnemyReward reward = enemyGrid != null ? enemyGrid.GetComponent<EnemyReward>() : null;
            if (reward == null && enemyHealth != null)
            {
                reward = enemyHealth.GetComponent<EnemyReward>();
            }
            if (reward != null && targetGoldReward > 0)
            {
                reward.SetGoldReward(targetGoldReward);
            }
        }
    }
}
