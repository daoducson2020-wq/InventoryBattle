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
    private bool selectedItemFromGrid;

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

        if (selectedItem != null && selectedItemFromGrid && Input.GetMouseButtonUp(0))
        {
            TrySellSelectedItem();
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
                    selectedItemFromGrid = true;
                }
            }
            else
            {
                bool placeSuccess = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
                if (placeSuccess)
                {
                    selectedItem = null;
                    selectedItemFromGrid = false;
                    if (overlapItem != null)
                    {
                        selectedItem = overlapItem;
                        overlapItem = null;
                        rectTransform = selectedItem.GetComponent<RectTransform>();
                        selectedItemFromGrid = true;
                    }
                }
            }
        }
    }

    private void TrySellSelectedItem()
    {
        ShopManager shopManager = FindAnyObjectByType<ShopManager>();
        if (shopManager == null || !shopManager.IsPointerOverShop(Input.mousePosition))
        {
            return;
        }

        if (selectedItem.ItemData == null || selectedItem.ItemData.price < 0)
        {
            Debug.LogError("Cannot sell an inventory item with invalid item data or price.", selectedItem);
            return;
        }

        PlayerWallet wallet = PlayerWallet.Instance;
        if (wallet == null)
        {
            Debug.LogError("Cannot sell an inventory item without a PlayerWallet.", this);
            return;
        }

        int sellPrice = Mathf.FloorToInt(selectedItem.ItemData.price * 0.8f);
        if (!wallet.TryAddGold(sellPrice))
        {
            Debug.LogError("The wallet could not accept the sale proceeds.", wallet);
            return;
        }

        Destroy(selectedItem.gameObject);
        selectedItem = null;
        rectTransform = null;
        selectedItemFromGrid = false;
    }

    private void CreateRandomItem()
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogError("InventoryController requires at least one item to create a random item.", this);
            return;
        }

        InventoryItem randomItem = CreateInventoryItem(items[Random.Range(0, items.Count)], Input.mousePosition);
        if (randomItem == null) return;

        selectedItem = randomItem;
        rectTransform = randomItem.GetComponent<RectTransform>();
        selectedItemFromGrid = false;
    }

    public InventoryItem CreateInventoryItem(ItemData item, Vector2 screenPosition)
    {
        if (item == null)
        {
            Debug.LogError("Cannot create an inventory item without item data.", this);
            return null;
        }

        if (itemPrefab == null || canvasTransform == null)
        {
            Debug.LogError("InventoryController requires an item prefab and canvas transform.", this);
            return null;
        }

        GameObject itemObject = Instantiate(itemPrefab, canvasTransform);
        InventoryItem inventoryItem = itemObject.GetComponent<InventoryItem>();
        if (inventoryItem == null)
        {
            Debug.LogError("Inventory item prefab is missing the InventoryItem component.", itemObject);
            Destroy(itemObject);
            return null;
        }

        inventoryItem.Set(item);
        inventoryItem.GetComponent<RectTransform>().position = screenPosition;
        return inventoryItem;
    }
}
