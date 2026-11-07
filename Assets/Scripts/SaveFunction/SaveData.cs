using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int stage;
    public int wallet;
    public List<InventorySaveData> inventoryDatas;
}