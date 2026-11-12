using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 1. Thêm thư viện quản lý Scene

public class MapView : MonoBehaviour
{
    public static MapView Instance { get; private set; } // Singleton giữ bản đồ xuyên Scene

    [Header("Scene Names (Scene Name in Build Settings)")]
    [SerializeField] private string mapSceneName = "MapScene";
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private string shopSceneName = "ShopScene";
    [SerializeField] private string campfireSceneName = "CampfireScene";
    [SerializeField] private string treasureSceneName = "TreasureScene";
    [SerializeField] private string miniBossSceneName = "MiniBossScene";
    [SerializeField] private string bossSceneName = "BossScene";

    [Header("Dependencies")]
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private Camera mapCamera;

    [Header("Containers (Empty GameObjects)")]
    [SerializeField] private Transform visualsContainer;
    [SerializeField] private Transform roomsContainer;
    [SerializeField] private Transform linesContainer;

    [Header("Prefabs")]
    [SerializeField] private MapRoom mapRoomPrefab;
    [SerializeField] private LineRenderer mapLinePrefab;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 15f;

    private List<List<Room>> mapData = new List<List<Room>>();
    private List<MapRoom> spawnedRooms = new List<MapRoom>();

    public List<List<Room>> MapData => mapData;
    public int FloorsClimbed { get; private set; } = 0;
    public Room LastRoom { get; private set; } = null;

    private float minCameraY = 0f;
    private float maxCameraY = 0f;
    private float savedCameraY = 0f; // Lưu vị trí cuộn Camera trước khi chuyển Scene

    private void Awake()
    {
        // 2. Giữ MapSystem không bị hủy khi chuyển sang Scene chiến đấu
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        maxCameraY = MapGenerator.Y_DIST * (MapGenerator.FLOORS - 1);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (GameSession.Instance != null && GameSession.Instance.HasSavedMap)
        {
            LoadMap(GameSession.Instance.SavedMapData, GameSession.Instance.CurrentFloor);
            GameSession.Instance.ClearPendingMapLoad();
            return;
        }

        // Chỉ sinh bản đồ mới hoàn toàn ở lần đầu tiên vào game
        if (mapData.Count == 0)
        {
            GenerateNewMap();
            UnlockFloor(0);
        }
    }

    // 3. Tự động xử lý khi chuyển qua lại giữa MapScene và các Scene màn chơi
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mapSceneName)
        {
            mapCamera = Camera.main;

            if (GameSession.Instance != null && GameSession.Instance.HasSavedMap)
            {
                LoadMap(GameSession.Instance.SavedMapData, GameSession.Instance.CurrentFloor);
                GameSession.Instance.ClearPendingMapLoad();
            }
            else if (mapData.Count == 0)
            {
                GenerateNewMap();
                UnlockFloor(0);
            }

            if (mapCamera != null)
            {
                Vector3 camPos = mapCamera.transform.position;
                camPos.y = savedCameraY; // Khôi phục đúng vị trí cuộn trước đó
                mapCamera.transform.position = camPos;
            }

            // Hiện lại bản đồ và mở khóa các Node nối tiếp từ phòng vừa hoàn thành
            visualsContainer.gameObject.SetActive(true);
            if (LastRoom != null)
            {
                UnlockNextRooms();
            }
            else if (FloorsClimbed == 0)
            {
                UnlockFloor(0);
            }
        }
        else
        {
            // Khi đang ở trong BattleScene / ShopScene: Ẩn hình ảnh bản đồ đi
            visualsContainer.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (visualsContainer.gameObject.activeInHierarchy)
        {
            HandleCameraScroll();
        }
    }

    private void HandleCameraScroll()
    {
        if (mapCamera == null) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.001f)
        {
            Vector3 camPos = mapCamera.transform.position;
            camPos.y += scrollInput * scrollSpeed * 10f;
            camPos.y = Mathf.Clamp(camPos.y, minCameraY, maxCameraY);
            mapCamera.transform.position = camPos;
        }
    }

    // --- KHI NGƯỜI CHƠI CLICK CHỌN 1 NODE ---
    private void OnMapRoomSelected(Room room)
    {
        // Khóa toàn bộ các Node cùng tầng
        foreach (MapRoom mapRoom in spawnedRooms)
        {
            if (mapRoom.RoomData.row == room.row)
            {
                mapRoom.Available = false;
            }
        }

        LastRoom = room;
        FloorsClimbed++;

        // Lưu lại độ cao Camera đang cuộn trước khi rời Scene
        if (mapCamera != null)
        {
            savedCameraY = mapCamera.transform.position.y;
        }

        // Chuyển sang Scene tương ứng với loại Node vừa chọn
        LoadSceneForRoom(room.type);
    }

    private void LoadSceneForRoom(RoomType type)
    {
        switch (type)
        {
            case RoomType.Monster:
                SceneManager.LoadScene(battleSceneName);
                break;
            case RoomType.Shop:
                SceneManager.LoadScene(shopSceneName);
                break;
            case RoomType.Campfire:
                SceneManager.LoadScene(campfireSceneName);
                break;
            case RoomType.Treasure:
                SceneManager.LoadScene(treasureSceneName);
                break;
            case RoomType.MiniBoss:
                SceneManager.LoadScene(miniBossSceneName);
                break;
            case RoomType.Boss:
                SceneManager.LoadScene(bossSceneName);
                break;
        }
    }

    // --- 1. KHỞI TẠO VÀ VẼ BẢN ĐỒ ---
    public void GenerateNewMap()
    {
        FloorsClimbed = 0;
        LastRoom = null;
        savedCameraY = 0f;

        mapData = mapGenerator.GenerateMap();
        CreateMapVisuals();
    }

    public void ResetMap()
    {
        FloorsClimbed = 0;
        LastRoom = null;
        savedCameraY = 0f;
        mapData.Clear();
        ClearExistingVisuals();
    }

    public void LoadMap(List<List<Room>> loadedMapData, int loadedFloor)
    {
        if (loadedMapData == null || loadedMapData.Count == 0)
        {
            GenerateNewMap();
            UnlockFloor(0);
            return;
        }

        mapData = loadedMapData;
        FloorsClimbed = Mathf.Max(0, loadedFloor);
        LastRoom = null;

        foreach (List<Room> floorRooms in mapData)
        {
            foreach (Room room in floorRooms)
            {
                if (room.selected && room.row < FloorsClimbed)
                {
                    if (LastRoom == null || room.row >= LastRoom.row)
                    {
                        LastRoom = room;
                    }
                }
            }
        }

        CreateMapVisuals();

        if (FloorsClimbed == 0 || LastRoom == null)
        {
            UnlockFloor(0);
            savedCameraY = 0f;
        }
        else
        {
            UnlockNextRooms();
            savedCameraY = Mathf.Clamp(LastRoom.position.y, minCameraY, maxCameraY);
        }

        if (mapCamera != null)
        {
            Vector3 camPos = mapCamera.transform.position;
            camPos.y = savedCameraY;
            mapCamera.transform.position = camPos;
        }
    }

    public static List<MapFloorSaveData> ConvertToSaveMapData(List<List<Room>> sourceMap)
    {
        List<MapFloorSaveData> result = new List<MapFloorSaveData>();
        if (sourceMap == null) return result;

        foreach (List<Room> floor in sourceMap)
        {
            MapFloorSaveData floorSave = new MapFloorSaveData();
            foreach (Room room in floor)
            {
                room.nextRoomColumns.Clear();
                if (room.nextRooms != null)
                {
                    foreach (Room next in room.nextRooms)
                    {
                        room.nextRoomColumns.Add(next.column);
                    }
                }
                floorSave.rooms.Add(room);
            }
            result.Add(floorSave);
        }

        return result;
    }

    public static List<List<Room>> ConvertFromSaveMapData(List<MapFloorSaveData> savedFloors)
    {
        List<List<Room>> result = new List<List<Room>>();
        if (savedFloors == null) return result;

        foreach (MapFloorSaveData floorSave in savedFloors)
        {
            result.Add(new List<Room>(floorSave.rooms));
        }

        for (int i = 0; i < result.Count; i++)
        {
            foreach (Room room in result[i])
            {
                room.nextRooms = new List<Room>();
                if (room.nextRoomColumns == null || i + 1 >= result.Count) continue;

                foreach (int col in room.nextRoomColumns)
                {
                    if (col >= 0 && col < result[i + 1].Count)
                    {
                        room.nextRooms.Add(result[i + 1][col]);
                    }
                }
            }
        }

        return result;
    }

    private void CreateMapVisuals()
    {
        ClearExistingVisuals();

        // Duyệt qua toàn bộ lưới và chỉ sinh những phòng có đường nối (nextRooms.Count > 0)
        foreach (List<Room> floor in mapData)
        {
            foreach (Room room in floor)
            {
                if (room.nextRooms.Count > 0)
                {
                    SpawnRoom(room);
                }
            }
        }

        // Phòng Boss ở tầng cuối cùng không có nextRooms nên phải được gọi sinh riêng
        int middle = Mathf.FloorToInt(MapGenerator.MAP_WIDTH * 0.5f);
        Room bossRoom = mapData[MapGenerator.FLOORS - 1][middle];
        SpawnRoom(bossRoom);

        // Căn giữa toàn bộ cụm Visuals theo chiều ngang (X = 0)
        float mapWidthPixels = (MapGenerator.MAP_WIDTH - 1) * MapGenerator.X_DIST;
        if (visualsContainer != null)
        {
            visualsContainer.localPosition = new Vector3(-mapWidthPixels * 0.5f, 0f, 0f);
        }
    }

    private void SpawnRoom(Room room)
    {
        MapRoom newMapRoom = Instantiate(mapRoomPrefab, roomsContainer);
        newMapRoom.Setup(room);
        newMapRoom.OnSelected += OnMapRoomSelected;
        spawnedRooms.Add(newMapRoom);

        // Vẽ các đường nối từ phòng này lên tầng tiếp theo
        ConnectLines(room);

        // Hiển thị lại vòng tròn đánh dấu nếu tải lại từ file Save
        if (room.selected && room.row < FloorsClimbed)
        {
            newMapRoom.ShowSelected();
        }
    }

    private void ConnectLines(Room room)
    {
        if (room.nextRooms.Count == 0) return;

        foreach (Room next in room.nextRooms)
        {
            LineRenderer newLine = Instantiate(mapLinePrefab, linesContainer);
            // Quan trọng: Tắt Use World Space trên LineRenderer để tọa độ đi theo Container
            newLine.useWorldSpace = false;
            newLine.positionCount = 2;
            newLine.SetPosition(0, room.position);
            newLine.SetPosition(1, next.position);
        }
    }

    private void ClearExistingVisuals()
    {
        foreach (MapRoom mr in spawnedRooms)
        {
            if (mr != null)
            {
                mr.OnSelected -= OnMapRoomSelected;
                Destroy(mr.gameObject);
            }
        }
        spawnedRooms.Clear();

        foreach (Transform child in linesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    // --- 2. LOGIC MỞ KHÓA VÀ CHỌN ĐƯỜNG ĐI ---
    public void UnlockFloor(int whichFloor = 0)
    {
        foreach (MapRoom mapRoom in spawnedRooms)
        {
            if (mapRoom.RoomData.row == whichFloor)
            {
                mapRoom.Available = true;
            }
        }
    }

    // Gọi hàm này sau khi người chơi thắng trận đấu / rời khỏi Shop và quay lại bản đồ
    public void UnlockNextRooms()
    {
        if (LastRoom == null) return;

        foreach (MapRoom mapRoom in spawnedRooms)
        {
            if (LastRoom.nextRooms.Contains(mapRoom.RoomData))
            {
                mapRoom.Available = true;
            }
        }
    }
}