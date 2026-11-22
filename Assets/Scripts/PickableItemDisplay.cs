using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hiển thị các vật phẩm chọn sẵn trong Inspector giống ShopManager:
/// sinh ShopSlot prefab làm con của "Slots Parent" (vị trí đặt trước trên Canvas).
/// Người chơi kéo item vào kho đồ như item trong shop, nhưng miễn phí.
/// </summary>
public class PickableItemDisplay : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    [Header("Hiển thị (giống Shop)")]
    [Tooltip("Object trên Canvas làm vị trí đặt trước. Các slot sẽ được sinh làm con của nó. Có thể gắn GridLayoutGroup/HorizontalLayoutGroup để tự sắp xếp.")]
    [SerializeField] private Transform slotsParent;
    [Tooltip("Prefab ShopSlot (có thể là bản copy đã bỏ giá).")]
    [SerializeField] private GameObject slotPrefab;
    [Tooltip("Khoảng cách giữa các slot (px), xếp ngang quanh tâm Slots Parent.")]
    [SerializeField] private float spacing = 20f;
    [SerializeField] private bool showOnStart = true;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TooltipPanelUI tooltipPanelUI;

    private readonly List<GameObject> spawnedSlots = new List<GameObject>();

    private void Start()
    {
        Debug.Log("PickableItemDisplay.Start (showOnStart=" + showOnStart + ")", this);
        if (showOnStart) Show();
    }

    public void Show()
    {
        Clear();

        if (slotsParent == null) slotsParent = transform;
        if (slotPrefab == null)
        {
            Debug.LogError("PickableItemDisplay requires a slot prefab.", this);
            return;
        }

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("PickableItemDisplay: danh sách Items đang trống.", this);
            return;
        }

        int count = 0;
        foreach (ItemData item in items) if (item != null) count++;

        int index = 0;
        foreach (ItemData item in items)
        {
            if (item == null) continue;
            SpawnSlot(item, index++, count);
        }
    }

    public void Clear()
    {
        foreach (GameObject slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot);
        }
        spawnedSlots.Clear();
    }

    private void SpawnSlot(ItemData item, int index, int count)
    {
        GameObject slotObj = Instantiate(slotPrefab, slotsParent, false);
        slotObj.SetActive(true);

        // Đặt slot đúng tâm của Slots Parent và hiển thị trên cùng
        RectTransform slotRect = slotObj.transform as RectTransform;
        if (slotRect != null)
        {
            slotRect.anchorMin = slotRect.anchorMax = new Vector2(0.5f, 0.5f);
            slotRect.pivot = new Vector2(0.5f, 0.5f);
            slotRect.anchoredPosition = Vector2.zero;
            slotRect.localRotation = Quaternion.identity;

            // Xếp ngang quanh tâm, cách nhau "spacing" (bỏ qua nếu parent có LayoutGroup)
            if (slotsParent.GetComponent<UnityEngine.UI.LayoutGroup>() == null)
            {
                float step = slotRect.rect.width * slotRect.localScale.x + spacing;
                float x = (index - (count - 1) * 0.5f) * step;
                slotRect.anchoredPosition = new Vector2(x, 0f);
            }
        }
        slotObj.transform.SetAsLastSibling();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        if (slot == null)
        {
            Debug.LogError("Slot prefab must have a ShopSlot component.", slotObj);
            Destroy(slotObj);
            return;
        }

        ItemTooltip tip = slotObj.GetComponent<ItemTooltip>();
        if (tip != null)
        {
            tip.tooltipPanel = tooltipPanel;
            tip.tooltipPanelUI = tooltipPanelUI;
        }

        slot.Setup(item);
        if (slot.priceText != null) slot.priceText.gameObject.SetActive(false);
        Debug.Log("PickableItemDisplay: spawned '" + item.name + "' under '" + slotsParent.name
            + "' (activeInHierarchy=" + slotObj.activeInHierarchy + ")", slotObj);

        DragAndDrop drag = slot.itemVisualObject != null
            ? slot.itemVisualObject.GetComponent<DragAndDrop>()
            : slotObj.GetComponentInChildren<DragAndDrop>(true);
        if (drag == null)
        {
            Debug.LogError("Slot prefab needs a DragAndDrop on its item visual.", slotObj);
            Destroy(slotObj);
            return;
        }

        drag.itemData = item;
        drag.sourceShopSlot = slot;
        drag.isFree = true;
        drag.onClaimed = () =>
        {
            spawnedSlots.Remove(slotObj);
            if (slotObj != null) Destroy(slotObj);
        };

        spawnedSlots.Add(slotObj);
    }
}
