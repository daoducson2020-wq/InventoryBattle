using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector] private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set 
        {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    [SerializeField] private List<ItemData> items;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform canvasTransform;

    private InventoryItem selectedItem;
    private InventoryItem overlapItem;
    private InventoryItem itemToHighlight;
    private InventoryHighlight inventoryHighlight;
    private RectTransform rectTransform; //item image
    private Vector2Int oldPosition;
    
    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

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
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int tileGridPosition = GetSelectedPosition();

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
        if (selectedItem != null && Input.GetMouseButtonDown(1))
        {
            RotateItem();
        }
    }

    private void RotateItem()
    {
        selectedItem.Rotate();
    }
    
    private Vector2Int GetSelectedPosition()
    {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.Width - 1) * ItemGrid.TILE_SIZE / 2;
            position.y += (selectedItem.Height - 1) * ItemGrid.TILE_SIZE / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetSelectedPosition();
        if (oldPosition == positionOnGrid)
        {
            return;
        }
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if (inventoryHighlight != null && itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize();
                inventoryHighlight.SetPosition(GetSelectedPosition());
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.IsInsideGrid(
                                                    positionOnGrid.x,
                                                    positionOnGrid.y,
                                                    selectedItem.Width,
                                                    selectedItem.Height));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
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
