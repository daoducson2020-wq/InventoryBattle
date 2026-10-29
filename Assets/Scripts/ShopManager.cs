using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private const int ShopColumns = 3;

    [Header("Grid Settings")]
    public List<ItemData> allAvailableItems; // Danh sách tổng vật phẩm
    public Transform shopGridPanel;          // Panel chứa lưới cửa hàng
    public GameObject shopSlotPrefab;        // Prefab của Shop Slot dạng lưới
    public int totalGridSlots = 6;

    private List<GameObject> spawnedSlots = new List<GameObject>();
    private Transform itemsGrid;

    public bool IsPointerOverShop(Vector2 screenPosition)
    {
        if (itemsGrid == null) return false;

        Canvas canvas = itemsGrid.GetComponentInParent<Canvas>();
        Camera eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera
            : null;

        return RectTransformUtility.RectangleContainsScreenPoint(
            itemsGrid as RectTransform,
            screenPosition,
            eventCamera);
    }

    void Start()
    {
        ConfigureItemsGrid();
        OpenShop();
    }

    public void OpenShop()
    {
        GenerateRandomShopItems();
    }

    public void RefreshShop()
    {
        ClearShop();
        GenerateRandomShopItems();
    }

    private void GenerateRandomShopItems()
    {
        ClearShop();

        if (allAvailableItems == null)
        {
            Debug.LogError("ShopManager requires an item list.", this);
            return;
        }

        if (itemsGrid == null || shopSlotPrefab == null)
        {
            Debug.LogError("ShopManager requires a shop grid and slot prefab.", this);
            return;
        }

        if (allAvailableItems.Count == 0) return;

        List<ItemData> availableList = new List<ItemData>(allAvailableItems);
        availableList.RemoveAll(item => item == null);
        if (availableList.Count == 0) return;

        for (int i = 0; i < totalGridSlots; i++)
        {
            ItemData selectedItem = availableList[Random.Range(0, availableList.Count)];

            GameObject slotObj = Instantiate(shopSlotPrefab, itemsGrid);
            ShopSlot slot = slotObj.GetComponent<ShopSlot>();
            if (slot != null)
            {
                slot.Setup(selectedItem);
            }
            else
            {
                Debug.LogError("Shop slot prefab is missing the ShopSlot component.", slotObj);
            }
            spawnedSlots.Add(slotObj);
        }
    }

    private void ConfigureItemsGrid()
    {
        if (shopGridPanel == null)
        {
            Debug.LogError("ShopManager requires a shop panel.", this);
            return;
        }

        GridLayoutGroup layout = shopGridPanel.GetComponent<GridLayoutGroup>();
        if (layout != null)
        {
            itemsGrid = shopGridPanel;
        }
        else
        {
            Transform existingGrid = shopGridPanel.Find("ShopItemsGrid");
            if (existingGrid == null)
            {
                GameObject gridObject = new GameObject("ShopItemsGrid", typeof(RectTransform));
                existingGrid = gridObject.transform;
                existingGrid.SetParent(shopGridPanel, false);

                RectTransform gridRect = (RectTransform)existingGrid;
                gridRect.anchorMin = Vector2.zero;
                gridRect.anchorMax = Vector2.one;
                gridRect.offsetMin = new Vector2(20f, 58f);
                gridRect.offsetMax = new Vector2(-20f, -20f);
            }

            itemsGrid = existingGrid;
            layout = itemsGrid.GetComponent<GridLayoutGroup>();
            if (layout == null)
            {
                layout = itemsGrid.gameObject.AddComponent<GridLayoutGroup>();
            }
        }

        layout.cellSize = new Vector2(136f, 120f);
        layout.spacing = new Vector2(12f, 12f);
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = ShopColumns;
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
