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
    private ItemData draggedItemPreview;
    private InventoryHighlight inventoryHighlight;
    private RectTransform rectTransform; //item image
    
    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;

            if (Input.GetMouseButtonUp(0) && TrySellSelectedItem())
            {
                return;
            }
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
            Vector2Int tileGridPosition = GetSelectedPosition(
                selectedItem != null ? selectedItem.ItemData : null);

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

    private bool TrySellSelectedItem()
    {
        ShopManager shopManager = FindAnyObjectByType<ShopManager>();
        if (shopManager == null || !shopManager.IsPointerOverShop(Input.mousePosition))
        {
            return false;
        }

        if (selectedItem.ItemData.price < 0)
        {
            Debug.LogError("Cannot sell an item with a negative price.", selectedItem);
            return false;
        }

        PlayerWallet wallet = PlayerWallet.Instance;
        if (wallet == null)
        {
            Debug.LogError("An inventory item cannot be sold without a PlayerWallet.", this);
            return false;
        }

        if (!wallet.TryAddGold(selectedItem.ItemData.price))
        {
            Debug.LogError("Cannot add the sale price to the player's gold.", wallet);
            return false;
        }

        Destroy(selectedItem.gameObject);
        selectedItem = null;
        rectTransform = null;
        return true;
    }

    private Vector2Int GetSelectedPosition(ItemData itemData)
    {
        Vector2 position = Input.mousePosition;
        if (itemData != null)
        {
            position.x -= (itemData.ItemWidth - 1) * ItemGrid.TILE_SIZE / 2;
            position.y += (itemData.ItemHeight - 1) * ItemGrid.TILE_SIZE / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void HandleHighlight()
    {
        ItemData activeItemData = draggedItemPreview != null
            ? draggedItemPreview
            : selectedItem != null ? selectedItem.ItemData : null;
        if (activeItemData != null)
        {
            Vector2Int positionOnGrid = GetSelectedPosition(activeItemData);
            inventoryHighlight.Show(selectedItemGrid.IsInsideGrid(
                positionOnGrid.x,
                positionOnGrid.y,
                activeItemData.ItemWidth,
                activeItemData.ItemHeight));
            inventoryHighlight.SetSize(activeItemData);
            inventoryHighlight.SetPosition(
                selectedItemGrid,
                activeItemData,
                positionOnGrid.x,
                positionOnGrid.y);
        }
        else
        {
            Vector2Int positionOnGrid = GetSelectedPosition(null);
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
                inventoryHighlight.SetPosition(positionOnGrid);
            }
        }
    }

    public void SetDraggedItemPreview(ItemData itemData)
    {
        draggedItemPreview = itemData;
    }

    public bool TryDetachItemForDrag(InventoryItem item, ItemGrid itemGrid)
    {
        if (item == null || itemGrid == null)
        {
            return false;
        }

        if (selectedItem == item)
        {
            selectedItem = null;
            rectTransform = null;
            return true;
        }

        return itemGrid.TryRemoveItem(item);
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

    public InventoryItem CreateInventoryItem(ItemData itemData, Vector2 screenPosition)
    {
        if (itemPrefab == null || canvasTransform == null || itemData == null)
        {
            Debug.LogError("Cannot create an inventory item because its prefab, canvas, or item data is missing.", this);
            return null;
        }

        GameObject itemObject = Instantiate(itemPrefab, canvasTransform);
        InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
        if (inventoryItem == null)
        {
            Debug.LogError("The inventory item prefab does not have an InventoryItem component.", itemObject);
            Destroy(itemObject);
            return null;
        }

        inventoryItem.Set(itemData);
        itemObject.GetComponent<RectTransform>().position = screenPosition;
        return inventoryItem;
    }
}
