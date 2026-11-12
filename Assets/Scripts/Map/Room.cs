using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    NotAssigned,
    Monster,
    Treasure,
    Campfire,
    MiniBoss,
    Shop,
    Boss
}

[System.Serializable]
public class Room
{
    public RoomType type = RoomType.NotAssigned;
    public int row;    // Chỉ số tầng (i): 0 -> 14
    public int column; // Chỉ số cột (j): 0 -> 6
    public Vector2 position; // Tọa độ thực tế trên màn hình (đã cộng độ lệch ngẫu nhiên)
    [System.NonSerialized]
    public List<Room> nextRooms = new List<Room>(); // Các phòng có đường nối lên tầng trên (i + 1)
    public List<int> nextRoomColumns = new List<int>(); // Lưu chỉ số cột của nextRooms để serialize JSON
    public bool selected = false;

    // Ghi đè ToString() để dễ dàng in ra Console kiểm tra (như ở [00:07:20])
    public override string ToString()
    {
        return $"{column} ({type.ToString()})";
    }
}