using System;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float TILE_SIZE = 64;

    [SerializeField] private int gridSizeWidth = 20;
    [SerializeField] private int gridSizeHeight = 10;
    [SerializeField] private Canvas rootCanvas;
    private RectTransform rectTransform; //inventory image size
    private Vector2 positionOnGrid = new();
    private Vector2Int tileGridPosition = new();
    private InventoryItem[,] inventoryItemSlot;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitInventorySlot(gridSizeWidth, gridSizeHeight);
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnGrid.y = rectTransform.position.y - mousePosition.y;
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
        rect.SetParent(rectTransform);
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

    private void InitInventorySlot(int x, int y)
    {
        inventoryItemSlot = new InventoryItem[x, y];
        Vector2 size = new Vector2(x * TILE_SIZE, y * TILE_SIZE);
        rectTransform.sizeDelta = size;
    }


    private void UpdateGrid(InventoryItem item)
    {
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                inventoryItemSlot[item.onGridPositionX + i, item.onGridPositionY + j] = null;
            }
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
