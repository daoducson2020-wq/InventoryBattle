using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float TILE_SIZE = 64;

    [SerializeField] private int gridSizeWidth = 20;
    [SerializeField] private int gridSizeHeight = 10;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private ItemCollections collections;
    private RectTransform inventoryPlaceholderImage;
    private Vector2 positionOnGrid = new();
    private Vector2Int tileGridPosition = new();
    private InventoryItem[,] inventoryItemSlot;
    public InventoryItem[,] Inventory => inventoryItemSlot;


    private void Awake()
    {
        inventoryPlaceholderImage = GetComponent<RectTransform>();
        if (rootCanvas == null)
        {
            // Fallback when the inspector reference is missing
            Canvas parentCanvas = GetComponentInParent<Canvas>();
            rootCanvas = parentCanvas != null ? parentCanvas.rootCanvas : FindAnyObjectByType<Canvas>();
        }
        InitInventorySlot(gridSizeWidth, gridSizeHeight);
    }

    private void Start()
    {
        GameSession gameSession = GameSession.Instance;
        if (gameSession != null
            && gameSession.PlayerInventoryDatas.Count > 0
            && transform.CompareTag("Player"))
        {
            LoadItem(gameSession.PlayerInventoryDatas);
        }
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnGrid.x = mousePosition.x - inventoryPlaceholderImage.position.x;
        positionOnGrid.y = inventoryPlaceholderImage.position.y - mousePosition.y;
        tileGridPosition.x = (int)(positionOnGrid.x / (TILE_SIZE * rootCanvas.scaleFactor));
        tileGridPosition.y = (int)(positionOnGrid.y / (TILE_SIZE * rootCanvas.scaleFactor));

        return tileGridPosition;
    }

    public bool IsInsideGrid(int posX, int posY, int itemWidth, int itemHeight)
    {
        if (posX < 0 || posY < 0)
        {
            return false;
        }
        if (posX + itemWidth > gridSizeWidth || posY + itemHeight > gridSizeHeight)
        {
            return false;
        }
        return true;
    }

    public InventoryItem GetItem(int x, int y)
    {
        if (x >= gridSizeWidth || y >= gridSizeHeight)
        {
            return null;
        }
        return inventoryItemSlot[x, y];
    }

    public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (!IsInsideGrid(posX, posY, item.Width, item.Height))
        {
            return false;
        }

        if (!IsOccupied(posX, posY, item.Width, item.Height, ref overlapItem))
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            UpdateGrid(overlapItem);
        }

        RectTransform rect = item.GetComponent<RectTransform>();
        rect.SetParent(inventoryPlaceholderImage);
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = item;
            }
        }
        item.onGridPositionX = posX;
        item.onGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(item, posX, posY);
        rect.localPosition = position;
        NotifyHealthChanged();
        return true;
    }

    public bool TryPlaceNewItemAtScreenPosition(InventoryItem item, Vector2 screenPosition)
    {
        if (item == null || item.ItemData == null)
        {
            Debug.LogError("Cannot place an inventory item without item data.", this);
            return false;
        }

        Vector2 position = screenPosition;
        position.x -= (item.Width - 1) * TILE_SIZE / 2;
        position.y += (item.Height - 1) * TILE_SIZE / 2;
        Vector2Int tileGridPosition = GetTileGridPosition(position);

        if (!IsInsideGrid(
                tileGridPosition.x,
                tileGridPosition.y,
                item.Width,
                item.Height))
        {
            return false;
        }

        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                if (inventoryItemSlot[tileGridPosition.x + x, tileGridPosition.y + y] != null)
                {
                    return false;
                }
            }
        }

        InventoryItem overlapItem = null;
        return PlaceItem(item, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem item, int posX, int posY)
    {
        return new Vector2(posX * TILE_SIZE + TILE_SIZE * item.Width / 2,
                    -(posY * TILE_SIZE + TILE_SIZE * item.Height / 2));
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem pickItemAt = inventoryItemSlot[x, y];
        if (pickItemAt == null)
        {
            return null;
        }
        UpdateGrid(pickItemAt);
        return pickItemAt;
    }

    public bool TryRemoveItem(InventoryItem item)
    {
        if (item == null || item.ItemData == null || inventoryItemSlot == null)
        {
            return false;
        }

        int posX = item.onGridPositionX;
        int posY = item.onGridPositionY;
        if (!IsInsideGrid(posX, posY, item.Width, item.Height)
            || inventoryItemSlot[posX, posY] != item)
        {
            return false;
        }

        UpdateGrid(item);
        return true;
    }

    public void LoadItem(List<InventorySaveData> inventoryDatas)
    {
        if (inventoryDatas == null)
        {
            Debug.LogError("Cannot load inventory because the save data is null.", this);
            return;
        }

        foreach (InventoryItem existingItem in GetComponentsInChildren<InventoryItem>(true))
        {
            existingItem.gameObject.SetActive(false);
            Destroy(existingItem.gameObject);
        }
        inventoryItemSlot = new InventoryItem[gridSizeWidth, gridSizeHeight];

        HashSet<string> loadedItemIds = new();
        for (int i = 0; i < inventoryDatas.Count; i++)
        {
            InventorySaveData data = inventoryDatas[i];
            if (data == null)
            {
                Debug.LogWarning("Skipping an empty inventory save entry.", this);
                continue;
            }

            if (!string.IsNullOrEmpty(data.id) && loadedItemIds.Contains(data.id))
            {
                Debug.LogWarning($"Skipping duplicate inventory item ID '{data.id}'.", this);
                continue;
            }

            if (PutItem(data.itemName, data.startX, data.startY, data.rotateAngle, data.rotated))
            {
                if (!string.IsNullOrEmpty(data.id))
                {
                    loadedItemIds.Add(data.id);
                }
            }
            else
            {
                Debug.LogWarning($"Skipping invalid inventory item at ({data.startX}, {data.startY}).", this);
            }
        }
        NotifyHealthChanged();
    }

    private bool PutItem(string itemName, int posX, int posY, float rotation, bool rotated)
    {
        ItemData data = collections.GetDataInCollections(itemName);
        
        if (data == null || itemPrefab == null)
        {
            return false;
        }

        GameObject itemObject = Instantiate(itemPrefab, inventoryPlaceholderImage);
        InventoryItem item = itemObject.GetComponent<InventoryItem>();
        if (item == null)
        {
            Debug.LogError("The inventory item prefab does not have an InventoryItem component.", itemPrefab);
            Destroy(itemObject);
            return false;
        }

        item.Set(data);
        item.rotated = rotated;
        item.itemRotation = rotation;
        if (!IsInsideGrid(posX, posY, item.Width, item.Height))
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject);
            return false;
        }

        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    item.gameObject.SetActive(false);
                    Destroy(item.gameObject);
                    return false;
                }
            }
        }

        RectTransform rect = item.GetComponent<RectTransform>();
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = item;
            }
        }
        item.onGridPositionX = posX;
        item.onGridPositionY = posY;
        Damage itemDmg = item.GetComponent<Damage>();
        if (itemDmg != null)
        {
            itemDmg.SetWeapon(item.ItemData);
        }
        Vector2 position = CalculatePositionOnGrid(item, posX, posY);
        rect.localPosition = position;
        rect.rotation = Quaternion.Euler(0f, 0f, rotation);
        return true;
    }

    private void InitInventorySlot(int x, int y)
    {
        inventoryItemSlot = new InventoryItem[x, y];
        Vector2 size = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
        inventoryPlaceholderImage.sizeDelta = size;
    }


    private void UpdateGrid(InventoryItem item)
    {
        for (int x = 0; x < gridSizeWidth; x++)
        {
            for (int y = 0; y < gridSizeHeight; y++)
            {
                if (inventoryItemSlot[x, y] == item)
                {
                    inventoryItemSlot[x, y] = null;
                }
            }
        }
        NotifyHealthChanged();
    }

    public int GetTotalArmorHealthBonus()
    {
        if (inventoryItemSlot == null) return 0;

        int totalBonus = 0;
        HashSet<InventoryItem> countedItems = new();

        for (int x = 0; x < gridSizeWidth; x++)
        {
            for (int y = 0; y < gridSizeHeight; y++)
            {
                InventoryItem item = inventoryItemSlot[x, y];
                if (item != null
                    && item.ItemData != null
                    && (item.ItemData.ItemType == ItemType.Armor
                        || item.ItemData.ItemType == ItemType.Accessory)
                    && countedItems.Add(item))
                {
                    totalBonus += item.ItemData.HealthBonus;
                }
            }
        }

        return totalBonus;
    }

    /// <summary>
    /// Finds the next health potion in use order (top row first, then left to right),
    /// removes it from the grid and destroys it. Returns its heal amount, or 0 if none.
    /// </summary>
    public int ConsumeNextHealthPotion()
    {
        if (inventoryItemSlot == null) return 0;

        for (int y = 0; y < gridSizeHeight; y++)
        {
            for (int x = 0; x < gridSizeWidth; x++)
            {
                InventoryItem item = inventoryItemSlot[x, y];
                if (item == null || item.ItemData == null
                    || item.ItemData.ItemType != ItemType.Potion
                    || item.ItemData.HealAmount <= 0)
                {
                    continue;
                }

                int healAmount = item.ItemData.HealAmount;
                UpdateGrid(item);
                item.gameObject.SetActive(false);
                Destroy(item.gameObject);
                return healAmount;
            }
        }
        return 0;
    }

    private void NotifyHealthChanged()
    {
        if (TryGetComponent<Health>(out Health health))
        {
            health.UpdateMaxHealthFromInventory(GetTotalArmorHealthBonus());
        }
    }

    private bool IsOccupied(int posX, int posY, int itemWidth, int itemHeight, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < itemWidth; x++)
        {
            for (int y = 0; y < itemHeight; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else
                    {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    
}
