using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public const int X_DIST = 2;
    public const int Y_DIST = 2;
    public const int PLACEMENT_RANDOMNESS = 0;
    public const int FLOORS = 15;
    public const int MAP_WIDTH = 7;
    public const int PATHS = 6;

    [Header("Room Weights")]
    public const float MONSTER_ROOM_WEIGHT = 10.0f;
    public const float SHOP_ROOM_WEIGHT = 3.0f;
    public const float CAMPFIRE_ROOM_WEIGHT = 2.5f;
    public const float TREASURE_ROOM_WEIGHT = 2.5f;

    private Dictionary<RoomType, float> randomRoomTypeWeights = new Dictionary<RoomType, float>()
    {
        { RoomType.Monster, 0.0f },
        { RoomType.Campfire, 0.0f },
        { RoomType.Shop, 0.0f }
    };

    private float randomRoomTypeTotalWeight = 0;
    private List<List<Room>> mapData = new List<List<Room>>();

    // private void Start()
    // {
    //     GenerateMap();
    // }

    public List<List<Room>> GenerateMap()
    {
        // 1. Khởi tạo lưới 15x7 trống [00:19:15]
        mapData = GenerateInitialGrid();

        // 2. Lấy 6 điểm xuất phát ở tầng 0 (ít nhất 2 điểm khác nhau) [00:30:05]
        List<int> startingPoints = GetRandomStartingPoints();

        // 3. Từ mỗi điểm xuất phát, nối đường đi từ tầng 0 lên tầng 13 [00:34:50]
        foreach (int j in startingPoints)
        {
            int currentJ = j;
            for (int i = 0; i < FLOORS - 1; i++)
            {
                currentJ = SetupConnection(i, currentJ);
            }
        }

        // 4. Gán phòng Boss và phân bổ loại phòng [00:51:50]
        SetupBossRoom();
        SetupRandomRoomWeights();
        SetupRoomTypes();

        // Mở comment dòng dưới nếu muốn kiểm tra kết quả trên Console của Unity [01:06:30]
        //PrintMapToConsole();

        return mapData;
    }

    // --- BƯỚC 1: KHỞI TẠO LƯỚI [00:19:25] ---
    private List<List<Room>> GenerateInitialGrid()
    {
        List<List<Room>> result = new List<List<Room>>();

        for (int i = 0; i < FLOORS; i++)
        {
            List<Room> adjacentRooms = new List<Room>();

            for (int j = 0; j < MAP_WIDTH; j++)
            {
                Room currentRoom = new Room();
                Vector2 offset = new Vector2(Random.value, Random.value) * PLACEMENT_RANDOMNESS;
                
                // Trong Unity trục Y hướng lên nên dùng Y_DIST dương
                currentRoom.position = new Vector2(j * X_DIST, i * Y_DIST) + offset;
                currentRoom.row = i;
                currentRoom.column = j;
                currentRoom.nextRooms = new List<Room>();

                // Phòng Boss ở tầng cuối không bị lệch Y và đẩy cao gấp đôi khoảng cách [00:22:45]
                if (i == FLOORS - 1)
                {
                    currentRoom.position.y = (i + 1) * Y_DIST;
                }

                adjacentRooms.Add(currentRoom);
            }

            result.Add(adjacentRooms);
        }

        return result;
    }

    // --- BƯỚC 2: CHỌN ĐIỂM XUẤT PHÁT [00:30:15] ---
    private List<int> GetRandomStartingPoints()
    {
        List<int> yCoordinates = new List<int>();
        int uniquePoints = 0;

        while (uniquePoints < 2)
        {
            uniquePoints = 0;
            yCoordinates.Clear();

            for (int i = 0; i < PATHS; i++)
            {
                int startingPoint = Random.Range(0, MAP_WIDTH);
                if (!yCoordinates.Contains(startingPoint))
                {
                    uniquePoints++;
                }
                yCoordinates.Add(startingPoint);
            }
        }

        return yCoordinates;
    }

    // --- BƯỚC 3: NỐI ĐƯỜNG & CHỐNG CẮT CHÉO [00:38:35] ---
    private int SetupConnection(int i, int j)
    {
        Room nextRoom = null;
        Room currentRoom = mapData[i][j];

        // Tiếp tục tìm ứng viên ở tầng i + 1 cho đến khi không bị cắt chéo [00:39:15]
        while (nextRoom == null || WouldCrossExistingPath(i, j, nextRoom))
        {
            int randomJ = Mathf.Clamp(Random.Range(j - 1, j + 2), 0, MAP_WIDTH - 1);
            nextRoom = mapData[i + 1][randomJ];
        }

        // Chỉ thêm vào danh sách nếu kết nối này chưa tồn tại
        if (!currentRoom.nextRooms.Contains(nextRoom))
        {
            currentRoom.nextRooms.Add(nextRoom);
        }

        return nextRoom.column;
    }

    private bool WouldCrossExistingPath(int i, int j, Room room)
    {
        Room leftNeighbour = null;
        Room rightNeighbour = null;

        // Lấy hàng xóm bên trái và bên phải cùng tầng i [00:45:25]
        if (j > 0) leftNeighbour = mapData[i][j - 1];
        if (j < MAP_WIDTH - 1) rightNeighbour = mapData[i][j + 1];

        // 1. Nếu đang định đi chéo sang PHẢI (room.column > j), kiểm tra xem hàng xóm bên phải có đi chéo sang TRÁI không [00:46:25]
        if (rightNeighbour != null && room.column > j)
        {
            foreach (Room nextRoom in rightNeighbour.nextRooms)
            {
                if (nextRoom.column < room.column)
                    return true;
            }
        }

        // 2. Nếu đang định đi chéo sang TRÁI (room.column < j), kiểm tra xem hàng xóm bên trái có đi chéo sang PHẢI không [00:47:20]
        if (leftNeighbour != null && room.column < j)
        {
            foreach (Room nextRoom in leftNeighbour.nextRooms)
            {
                if (nextRoom.column > room.column)
                    return true;
            }
        }

        return false;
    }

    // --- BƯỚC 4: THIẾT LẬP BOSS & LOẠI PHÒNG [00:52:25] ---
    private void SetupBossRoom()
    {
        int middle = Mathf.FloorToInt(MAP_WIDTH * 0.5f);
        Room bossRoom = mapData[FLOORS - 1][middle];

        // Mọi phòng có đường đi ở tầng áp chót (FLOORS - 2) đều nối thẳng vào Boss [00:53:30]
        for (int j = 0; j < MAP_WIDTH; j++)
        {
            Room currentRoom = mapData[FLOORS - 2][j];
            if (currentRoom.nextRooms.Count > 0)
            {
                currentRoom.nextRooms.Clear();
                currentRoom.nextRooms.Add(bossRoom);
            }
        }

        bossRoom.type = RoomType.Boss;
    }

    private void SetupRandomRoomWeights()
    {
        // Tính trọng số cộng dồn (Accumulated Weights) [00:54:40]
        randomRoomTypeWeights[RoomType.Monster] = MONSTER_ROOM_WEIGHT;
        randomRoomTypeWeights[RoomType.Campfire] = MONSTER_ROOM_WEIGHT + CAMPFIRE_ROOM_WEIGHT;
        randomRoomTypeWeights[RoomType.Shop] = MONSTER_ROOM_WEIGHT + CAMPFIRE_ROOM_WEIGHT + SHOP_ROOM_WEIGHT;
        randomRoomTypeWeights[RoomType.Treasure] = MONSTER_ROOM_WEIGHT + CAMPFIRE_ROOM_WEIGHT + SHOP_ROOM_WEIGHT + TREASURE_ROOM_WEIGHT;

        randomRoomTypeTotalWeight = randomRoomTypeWeights[RoomType.Treasure];
    }

    private void SetupRoomTypes()
    {
        // Luật 1: Tầng 0 luôn là Monster [00:55:45]
        foreach (Room room in mapData[0])
        {
            if (room.nextRooms.Count > 0)
                room.type = RoomType.Monster;
        }

        // Luật 2: Tầng 8 (Floor thứ 9) luôn là Treasure [00:56:15]
        foreach (Room room in mapData[8])
        {
            if (room.nextRooms.Count > 0)
                room.type = RoomType.MiniBoss;
        }

        // Luật 3: Tầng áp chót (Tầng 13) trước Boss luôn là Campfire [00:56:35]
        foreach (Room room in mapData[FLOORS - 2])
        {
            if (room.nextRooms.Count > 0)
                room.type = RoomType.Campfire;
        }

        // Luật 4: Các phòng còn lại nằm trên đường đi sẽ được gán ngẫu nhiên có điều kiện [00:57:40]
        foreach (List<Room> floor in mapData)
        {
            foreach (Room room in floor)
            {
                foreach (Room nextRoom in room.nextRooms)
                {
                    if (nextRoom.type == RoomType.NotAssigned)
                    {
                        SetRoomRandomly(nextRoom);
                    }
                }
            }
        }
    }

    private void SetRoomRandomly(Room roomToSet)
    {
        bool campfireBelow4 = true;
        bool consecutiveCampfire = true;
        bool consecutiveShop = true;
        bool campfireOn13 = true;

        RoomType typeCandidate = RoomType.NotAssigned;

        // Lặp cho đến khi tìm được loại phòng không vi phạm bất kỳ luật nào [00:59:05]
        while (campfireBelow4 || consecutiveCampfire || consecutiveShop || campfireOn13)
        {
            typeCandidate = GetRandomRoomTypeByWeight();

            bool isCampfire = typeCandidate == RoomType.Campfire;
            bool hasCampfireParent = RoomHasParentOfType(roomToSet, RoomType.Campfire);
            bool isShop = typeCandidate == RoomType.Shop;
            bool hasShopParent = RoomHasParentOfType(roomToSet, RoomType.Shop);

            campfireBelow4 = isCampfire && roomToSet.row < 3;
            consecutiveCampfire = isCampfire && hasCampfireParent;
            consecutiveShop = isShop && hasShopParent;
            campfireOn13 = isCampfire && roomToSet.row == 12; // Tầng 12 không được có trại vì tầng 13 đã có
        }

        roomToSet.type = typeCandidate;
    }

    private bool RoomHasParentOfType(Room room, RoomType type)
    {
        List<Room> parents = new List<Room>();

        // Kiểm tra 3 ứng viên cha ở tầng dưới (row - 1) xem có nối lên phòng hiện tại không [01:02:25]
        if (room.row > 0)
        {
            // Cha bên trái
            if (room.column > 0)
            {
                Room leftParent = mapData[room.row - 1][room.column - 1];
                if (leftParent.nextRooms.Contains(room)) parents.Add(leftParent);
            }
            // Cha ngay bên dưới
            Room belowParent = mapData[room.row - 1][room.column];
            if (belowParent.nextRooms.Contains(room)) parents.Add(belowParent);

            // Cha bên phải
            if (room.column < MAP_WIDTH - 1)
            {
                Room rightParent = mapData[room.row - 1][room.column + 1];
                if (rightParent.nextRooms.Contains(room)) parents.Add(rightParent);
            }
        }

        foreach (Room parent in parents)
        {
            if (parent.type == type)
                return true;
        }

        return false;
    }

    private RoomType GetRandomRoomTypeByWeight()
    {
        float roll = Random.Range(0.0f, randomRoomTypeTotalWeight);

        foreach (var pair in randomRoomTypeWeights)
        {
            if (pair.Value > roll)
                return pair.Key;
        }

        return RoomType.Monster;
    }

    // Hàm in ra Console để kiểm tra giống trong video [00:49:10]
    private void PrintMapToConsole()
    {
        for (int i = 0; i < mapData.Count; i++)
        {
            var usedRooms = mapData[i].Where(r => r.nextRooms.Count > 0 || r.type == RoomType.Boss).ToList();
            string floorStr = string.Join("\t", usedRooms.Select(r => r.ToString()));
            Debug.Log($"Floor {i}:\t{floorStr}");
        }
    }
}