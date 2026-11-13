using System;
using System.Collections.Generic;

[Serializable]
public class MapFloorSaveData
{
    public List<Room> rooms = new();
}

[Serializable]
public class SaveData
{
    public int floor;
    public int wallet;
    public int currentHealth;
    public int maxHealth; // 0 = no health saved
    public List<InventorySaveData> inventoryDatas;
    public List<MapFloorSaveData> mapData;
}