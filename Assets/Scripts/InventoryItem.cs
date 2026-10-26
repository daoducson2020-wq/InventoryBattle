using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;

    public int onGridPositionX;
    public int onGridPositionY;
    
    public void Set(ItemData item)
    {
        itemData = item;
        GetComponent<Image>().sprite = itemData.Icon;
        Vector2 size = new Vector2(itemData.ItemWidth * ItemGrid.TILE_SIZE,
                                    itemData.ItemHeight * ItemGrid.TILE_SIZE);
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
