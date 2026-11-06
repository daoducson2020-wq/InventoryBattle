using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    [HideInInspector] public int onGridPositionX;
    [HideInInspector] public int onGridPositionY;
    [HideInInspector] public bool rotated = false;

    private string itemID;
    public string ItemID => itemID;
    private float itemRotation = 0f;
    public float ItemRotation => itemRotation;
    private static int itemCreated = 0;
    public int ItemCreated => itemCreated;
    public int Height
    {
        get
        {
            if (rotated == false)
            {
                return itemData.ItemHeight;
            }
            return itemData.ItemWidth;
        }
    }
    public int Width
    {
        get
        {
            if (rotated == false)
            {
                return itemData.ItemWidth;
            }
            return itemData.ItemHeight;
        }
    }

    public void Set(ItemData item)
    {
        itemCreated += 1;
        itemData = item;
        itemID = RandomString();
        GetComponent<Image>().sprite = itemData.Icon;
        Vector2 size = new Vector2(itemData.ItemWidth * ItemGrid.TILE_SIZE,
                                    itemData.ItemHeight * ItemGrid.TILE_SIZE);
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotate()
    {
        rotated = !rotated;
        itemRotation += 90f;
        RectTransform itemIcon = GetComponent<RectTransform>();
        itemIcon.rotation = Quaternion.Euler(0, 0, itemRotation);
    }

    private string RandomString()
    {
        string uniqueString = System.Guid.NewGuid().ToString();
        return uniqueString;
    }
}
