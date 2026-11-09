using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyEntry
{
    public string key;
    public List<InventorySaveData> value;
}

[Serializable]
public class EnemySaveData
{
    public List<EnemyEntry> enemyDatas = new();

    public List<InventorySaveData> GetValue(string key)
    {
        foreach (var item in enemyDatas)
        {
            if (item.key == key)
            {
                return item.value;
            }
        }
        return null;
    }

    public void AddData(string key, List<InventorySaveData> value)
    {
        EnemyEntry newEntry = new()
        {
            key = key,
            value = value
        };
        enemyDatas.Add(newEntry);
    }
}