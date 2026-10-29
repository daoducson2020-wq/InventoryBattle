using System;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float TILE_SIZE = 32;

    [SerializeField] private int gridSizeWidth = 20;
    [SerializeField] private int gridSizeHeight = 10;

    private RectTransform rectTransform; //inventory image size
    private Vector2 positionOnGrid = new();
    private Vector2Int tileGridPosition = new();
    private InventoryItem[,] inventoryItemSlot;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitInventorySlot(gridSizeWidth, gridSizeHeight);
    }

    private void InitInventorySlot(int x, int y)
    {
        inventoryItemSlot = new InventoryItem[x, y];
        Vector2 size = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
        rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnGrid.y = rectTransform.position.y - mousePosition.y;
        tileGridPosition.x = Mathf.FloorToInt(positionOnGrid.x / TILE_SIZE);
        tileGridPosition.y = Mathf.FloorToInt(positionOnGrid.y / TILE_SIZE);

        return tileGridPosition;
    }

    private bool IsInsideGrid(int posX, int posY, int itemWidth, int itemHeight)
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

    public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (item == null || item.ItemData == null)
        {
            return false;
        }

        if (!IsInsideGrid(posX, posY, item.ItemData.ItemWidth, item.ItemData.ItemHeight))
        {
            return false;
        }

        if (!IsOccupied(posX, posY, item.ItemData.ItemWidth, item.ItemData.ItemHeight, ref overlapItem))
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            UpdateGrid(overlapItem);
        }
        
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.SetParent(rectTransform);
        for (int x = 0; x < item.ItemData.ItemWidth; x++)
        {
            for (int y = 0; y < item.ItemData.ItemHeight; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = item;
            }
        }
        item.onGridPositionX = posX;
        item.onGridPositionY = posY;
        Vector2 position = new Vector2(
                    posX * TILE_SIZE + TILE_SIZE * item.ItemData.ItemWidth / 2,
                    -(posY * TILE_SIZE + TILE_SIZE * item.ItemData.ItemHeight / 2)
                        );
        rect.localPosition = position;
        return true;
    }

    public bool TryPlaceNewItemAtScreenPosition(InventoryItem item, Vector2 screenPosition)
    {
        if (item == null || item.ItemData == null)
        {
            return false;
        }

        Vector2 itemOrigin = screenPosition;
        itemOrigin.x -= (item.ItemData.ItemWidth - 1) * TILE_SIZE / 2f;
        itemOrigin.y += (item.ItemData.ItemHeight - 1) * TILE_SIZE / 2f;
        Vector2Int gridPosition = GetTileGridPosition(itemOrigin);

        if (!IsInsideGrid(gridPosition.x, gridPosition.y, item.ItemData.ItemWidth, item.ItemData.ItemHeight))
        {
            return false;
        }

        InventoryItem occupiedItem = null;
        if (!IsOccupied(gridPosition.x, gridPosition.y, item.ItemData.ItemWidth, item.ItemData.ItemHeight, ref occupiedItem)
            || occupiedItem != null)
        {
            return false;
        }

        InventoryItem noOverlap = null;
        return PlaceItem(item, gridPosition.x, gridPosition.y, ref noOverlap);
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

    private void UpdateGrid(InventoryItem item)
    {
        for (int i = 0; i < item.ItemData.ItemWidth; i++)
        {
            for (int j = 0; j < item.ItemData.ItemHeight; j++)
            {
                inventoryItemSlot[item.onGridPositionX + i, item.onGridPositionY + j] = null;
            }
        }
    }
}
