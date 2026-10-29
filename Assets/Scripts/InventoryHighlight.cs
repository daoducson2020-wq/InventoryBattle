using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] private RectTransform highlight;

    public void Show(bool isShow)
    {
        highlight.gameObject.SetActive(isShow);
    }

    public void SetSize(InventoryItem item)
    {
        if (item == null)
        {
            return;
        }
        Vector2 size = new Vector2(item.ItemData.ItemWidth * ItemGrid.TILE_SIZE,
                                item.ItemData.ItemHeight * ItemGrid.TILE_SIZE);
        highlight.sizeDelta = size;
    }

    public void SetSize()
    {
        Vector2 size = new Vector2(ItemGrid.TILE_SIZE, ItemGrid.TILE_SIZE);
        highlight.sizeDelta = size;
    }
    
    public void SetParent(ItemGrid grid)
    {
        if (grid == null)
        {
            return;
        }
        highlight.SetParent(grid.GetComponent<RectTransform>());
    }

    public void SetPosition(ItemGrid grid, InventoryItem item)
    {
        if (grid == null || item == null)
        {
            return;
        }
        Vector2 position = grid.CalculatePositionOnGrid(item, item.onGridPositionX, item.onGridPositionY);
        highlight.localPosition = position;
    }

    public void SetPosition(Vector2Int position)
    {
        Vector2 pos = new Vector2(position.x * ItemGrid.TILE_SIZE + ItemGrid.TILE_SIZE / 2,
                                -(position.y * ItemGrid.TILE_SIZE + ItemGrid.TILE_SIZE / 2));
        highlight.localPosition = pos;
    }

    public void SetPosition(ItemGrid grid, InventoryItem item, int posX, int posY)
    {
        Vector2 position = grid.CalculatePositionOnGrid(item, posX, posY);
        highlight.localPosition = position;
    }
}
