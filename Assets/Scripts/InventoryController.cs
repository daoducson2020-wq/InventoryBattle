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
    [SerializeField] private Animator animator;

    private InventoryItem selectedItem;
    private InventoryItem overlapItem;
    private InventoryItem itemToHighlight;
    private InventoryItem draggedItemPreview;
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
            PlaceItem();
        }
        if (selectedItem != null && Input.GetMouseButtonDown(1))
        {
            RotateItem();
        }
    }

    private void PlaceItem()
    {
        // Vector2Int tileGridPosition = GetSelectedPosition(
        //     selectedItem != null ? selectedItem.ItemData : null);
        Vector2Int tileGridPosition = GetSelectedPosition();

        if (selectedItem == null)
        {
            selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
            if (selectedItem != null)
            {
                selectedItem.ShowIcon();
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform, true);
            }
        }
        else
        {
            bool placeSuccess = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
            if (placeSuccess)
            {
                PutItemInInventory(selectedItem);
                selectedItem.ShowIconFrame();
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

    private void RotateItem()
    {
        selectedItem.Rotate();
    }

    private bool TrySellSelectedItem()
    {
        SellPoint sellPoint = FindAnyObjectByType<SellPoint>();
        if (sellPoint == null || !sellPoint.IsPointerOverSellPoint(Input.mousePosition))
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

        if (!wallet.TryAddGold(selectedItem.ItemData.price - (selectedItem.ItemData.price*20/100)))
        {
            Debug.LogError("Cannot add the sale price to the player's gold.", wallet);
            return false;
        }
        if(animator != null)
        {
            animator.SetBool("isBurn", true);
            animator.SetTrigger("Sell");
        }
        Destroy(selectedItem.gameObject);
        selectedItem = null;
        rectTransform = null;
        return true;
    }

    // private Vector2Int GetSelectedPosition(ItemData itemData)
    // {
    //     Vector2 position = Input.mousePosition;
    //     if (itemData != null)
    //     {
    //         position.x -= (itemData.ItemWidth - 1) * ItemGrid.TILE_SIZE / 2;
    //         position.y += (itemData.ItemHeight - 1) * ItemGrid.TILE_SIZE / 2;
    //     }
    //     return selectedItemGrid.GetTileGridPosition(position);
    // }

    private Vector2Int GetSelectedPosition()
    {
        Vector2 position = Input.mousePosition;
        InventoryItem activeItem = selectedItem != null ? selectedItem : draggedItemPreview;
        if (activeItem != null)
        {
            position.x -= (activeItem.Width - 1) * ItemGrid.TILE_SIZE / 2;
            position.y += (activeItem.Height - 1) * ItemGrid.TILE_SIZE / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetSelectedPosition();
        InventoryItem activeItem = selectedItem != null ? selectedItem : draggedItemPreview;
        if (activeItem != null)
        {
            inventoryHighlight.Show(selectedItemGrid.IsInsideGrid(
                positionOnGrid.x,
                positionOnGrid.y,
                activeItem.Width,
                activeItem.Height));
            inventoryHighlight.SetSize(activeItem);
            inventoryHighlight.SetPosition(selectedItemGrid, activeItem, positionOnGrid.x, positionOnGrid.y);
            return;
        }

        itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
        if (itemToHighlight != null)
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

    public void SetDraggedItemPreview(InventoryItem item)
    {
        draggedItemPreview = item;
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

    private void PutItemInInventory(InventoryItem item)
    {
        Damage itemDmg = item.GetComponent<Damage>();
        itemDmg.SetWeapon(item.ItemData);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShopScene")
        {
            itemDmg.IdentifySelfTag();
        }
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
