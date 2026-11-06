using System.Collections.Generic;
using System;

[Serializable]
public class SaveData
{
    public int stage;
    public int wallet;
    public List<InventorySaveData> inventoryDatas;
}