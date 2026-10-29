using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Image itemImage;

    public ItemData ItemData => itemData;
    public int ItemWidth => itemData.ItemWidth;
    public int ItemHeight => itemData.ItemHeight;

    private int gridX;
    public int GridX => gridX;
    private int gridY;
    public int GridY => gridY;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (itemData == null || itemImage == null)
        {
            return;
        }
        itemImage.sprite = itemData.Icon;
        // itemImage.preserveAspect = true;
    }
    
    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
}
