using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class MapRoom : MonoBehaviour
{
    // Sự kiện báo cho MapView biết khi người chơi click chọn phòng này
    public event Action<Room> OnSelected;

    [Header("Visual References")]
    [SerializeField] private SpriteRenderer iconSprite;
    [SerializeField] private GameObject selectedOutline; // Vòng tròn đánh dấu đã đi qua

    [Header("Room Icons")]
    [SerializeField] private Sprite monsterIcon;
    [SerializeField] private Sprite treasureIcon;
    [SerializeField] private Sprite campfireIcon;
    [SerializeField] private Sprite shopIcon;
    [SerializeField] private Sprite miniBossIcon;
    [SerializeField] private Sprite bossIcon;

    public Room RoomData { get; private set; }
    
    private bool isAvailable = false;
    private Vector3 baseScale;

    public bool Available
    {
        get => isAvailable;
        set
        {
            isAvailable = value;
            if (!isAvailable)
            {
                // Trả về kích thước gốc khi không còn được phép chọn
                transform.localScale = baseScale;
            }
        }
    }

    private void Awake()
    {
        baseScale = transform.localScale;
        if (selectedOutline != null)
            selectedOutline.SetActive(false);
    }

    private void Update()
    {
        // Hiệu ứng phóng to - thu nhỏ (Pulse Animation) khi phòng đang mở khóa chờ chọn
        if (isAvailable)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.15f;
            transform.localScale = baseScale * pulse;
        }
    }

    public void Setup(Room room)
    {
        RoomData = room;
        transform.localPosition = room.position;

        // Gán Icon và kích thước tương ứng theo loại phòng
        switch (room.type)
        {
            case RoomType.Monster:
                iconSprite.sprite = monsterIcon;
                break;
            case RoomType.Treasure:
                iconSprite.sprite = treasureIcon;
                break;
            case RoomType.Campfire:
                iconSprite.sprite = campfireIcon;
                break;
            case RoomType.Shop:
                iconSprite.sprite = shopIcon;
                break;
            case RoomType.MiniBoss:
                iconSprite.sprite = miniBossIcon;
                break;
            case RoomType.Boss:
                iconSprite.sprite = bossIcon;
                baseScale = Vector3.one * 1.5f; // Phóng to Icon Boss gấp rưỡi
                transform.localScale = baseScale;
                break;
        }
    }

    public void ShowSelected()
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(true);
    }

    private void OnMouseDown()
    {
        // Chỉ nhận click khi phòng đang ở trạng thái Available (mở khóa)
        if (!isAvailable) return;

        RoomData.selected = true;
        Available = false;
        ShowSelected();

        // Kích hoạt sự kiện chuyển cảnh / khóa các phòng cùng tầng
        OnSelected?.Invoke(RoomData);
    }
}