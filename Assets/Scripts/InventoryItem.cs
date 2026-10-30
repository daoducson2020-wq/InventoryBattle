using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;

    public int onGridPositionX;
    public int onGridPositionY;
    private bool rotated = false;
    private float angleRotation = 0f;
    public int Height
    {
        get
        {
            return rotated == false ? itemData.ItemHeight : itemData.ItemWidth;
        }
    }
    public int Width
    {
        get
        {
            return rotated == false ? itemData.ItemWidth : itemData.ItemHeight;
        }
    }

    public void Set(ItemData item)
    {
        itemData = item;
        GetComponent<Image>().sprite = itemData.Icon;
        Vector2 size = new Vector2(itemData.ItemWidth * ItemGrid.TILE_SIZE,
                                    itemData.ItemHeight * ItemGrid.TILE_SIZE);
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotate()
    {
        rotated = !rotated;
        angleRotation += 90f;
        RectTransform itemIcon = GetComponent<RectTransform>();
        itemIcon.rotation = Quaternion.Euler(0, 0, angleRotation);
    }
}
