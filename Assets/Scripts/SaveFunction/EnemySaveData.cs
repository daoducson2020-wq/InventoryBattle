using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySaveData
{
    [SerializeField] public Dictionary<string, List<InventorySaveData>> enemyDatas;
}
