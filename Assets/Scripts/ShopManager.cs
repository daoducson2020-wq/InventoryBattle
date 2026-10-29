using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private const int ShopColumns = 3;
    private const int ShopRows = 2;

    [Header("Shop Settings")]
    public List<ItemData> allAvailableItems;
    public Transform shopGridPanel;
    public GameObject shopSlotPrefab;
    public int totalGridSlots = ShopColumns * ShopRows;

    private List<GameObject> spawnedSlots = new List<GameObject>();

    void Start()
    {
        ConfigureGridLayout();
        OpenShop();
    }

    public void OpenShop()
    {
        GenerateRandomShopItems();
    }

    public void RefreshShop()
    {
        // Tùy chọn: Có thể trừ một ít tiền vàng khi bấm làm mới cửa hàng ở đây nếu muốn
        ClearShop();
        GenerateRandomShopItems();
    }

    public bool IsPointerOverShop(Vector2 screenPosition)
    {
        RectTransform shopRect = shopGridPanel as RectTransform;
        if (shopRect == null)
        {
            return false;
        }

        Canvas canvas = shopGridPanel.GetComponentInParent<Canvas>();
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(
            shopRect,
            screenPosition,
            eventCamera);
    }

    private void GenerateRandomShopItems()
    {
        ClearShop();

        if (allAvailableItems == null || shopGridPanel == null || shopSlotPrefab == null)
        {
            Debug.LogError("ShopManager requires an item list, shop grid, and slot prefab.", this);
            return;
        }

        List<ItemData> availableList = new List<ItemData>(allAvailableItems);
        availableList.RemoveAll(item => item == null);
        int slotsToFill = Mathf.Min(
            Mathf.Min(totalGridSlots, ShopColumns * ShopRows),
            availableList.Count);

        for (int i = 0; i < slotsToFill; i++)
        {
            int randomIndex = Random.Range(0, availableList.Count);
            ItemData selectedItem = availableList[randomIndex];
            availableList.RemoveAt(randomIndex);

            GameObject slotObj = Instantiate(shopSlotPrefab, shopGridPanel);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            if (slot != null)
            {
                slot.Setup(selectedItem);
            }
            spawnedSlots.Add(slotObj);
        }
    }

    private void ConfigureGridLayout()
    {
        if (shopGridPanel == null)
        {
            Debug.LogError("ShopManager requires a shop grid.", this);
            return;
        }

        GridLayoutGroup layout = shopGridPanel.GetComponent<GridLayoutGroup>();
        if (layout == null)
        {
            layout = shopGridPanel.gameObject.AddComponent<GridLayoutGroup>();
        }

        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = ShopColumns;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.spacing = new Vector2(layout.spacing.x, 30f);
    }

    private void ClearShop()
    {
        foreach (GameObject slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot);
        }
        spawnedSlots.Clear();
    }
}