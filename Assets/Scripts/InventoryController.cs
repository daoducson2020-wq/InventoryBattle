using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] public ItemGrid selectedItemGrid;
    [SerializeField] private List<ItemData> items;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform canvasTransform;

    private InventoryItem selectedItem;
    private InventoryItem overlapItem;
    private RectTransform rectTransform; //item image

    private void Update()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateRandomItem();
        }

        if (selectedItemGrid == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 position = Input.mousePosition;
            if (selectedItem != null)
            {
                position.x -= (selectedItem.ItemData.ItemWidth - 1) * ItemGrid.TILE_SIZE / 2;
                position.y += (selectedItem.ItemData.ItemHeight - 1) * ItemGrid.TILE_SIZE / 2;
            }
            Vector2Int tileGridPosition = selectedItemGrid.GetTileGridPosition(position);

            if (selectedItem == null)
            {
                selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
                if (selectedItem != null)
                {
                    rectTransform = selectedItem.GetComponent<RectTransform>();
                }
            }
            else
            {
                bool placeSuccess = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
                if (placeSuccess)
                {
                    selectedItem = null;
                    if (overlapItem != null)
                    {
                        selectedItem = overlapItem;
                        overlapItem = null;
                        rectTransform = selectedItem.GetComponent<RectTransform>();
                    }
                }
            }
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem randomItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = randomItem;
        rectTransform = randomItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        int selectedItemID = Random.Range(0, items.Count);
        randomItem.Set(items[selectedItemID]);
    }
}
