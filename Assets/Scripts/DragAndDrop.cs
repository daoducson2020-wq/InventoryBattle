using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform draggedItemRectTransform;
    private InventoryItem draggedInventoryItem;
    private InventoryController dragPreviewController;
    private float originalAlpha;
    private bool originalBlocksRaycasts;

    [HideInInspector] public ItemData itemData;       // Dữ liệu vật phẩm được gán từ Shop Slot
    [HideInInspector] public bool isFree;             // Vật phẩm nhặt miễn phí (không trừ vàng)
    [HideInInspector] public System.Action onClaimed; // Gọi khi vật phẩm được đặt vào kho
    [HideInInspector] public ShopSlot sourceShopSlot; // Tham chiếu đến ô cửa hàng chứa vật phẩm này

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (itemData == null || canvas == null)
        {
            Debug.LogError("DragAndDrop cannot start: itemData=" + itemData + ", canvas=" + canvas, this);
            return;
        }

        dragPreviewController = FindAnyObjectByType<InventoryController>();
        if (dragPreviewController == null)
        {
            Debug.LogError("A shop item cannot be dragged without an InventoryController.", this);
            return;
        }

        draggedInventoryItem = dragPreviewController.CreateInventoryItem(itemData, eventData.position);
        if (draggedInventoryItem == null)
        {
            return;
        }

        draggedInventoryItem.ShowIcon();
        SoundManager.Instance.PlayItemPickup();
        draggedItemRectTransform = draggedInventoryItem.GetComponent<RectTransform>();
        originalAlpha = canvasGroup.alpha;
        originalBlocksRaycasts = canvasGroup.blocksRaycasts;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        dragPreviewController.SetDraggedItemPreview(draggedInventoryItem);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedInventoryItem == null)
        {
            return;
        }
        draggedItemRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private void Update()
    {
        if (draggedInventoryItem != null && Input.GetMouseButtonDown(1))
        {
            draggedInventoryItem.Rotate();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedInventoryItem == null)
        {
            ClearDragPreview();
            return;
        }

        ItemGrid itemGrid = FindGridUnderPointer(eventData);

        if (itemGrid != null && TryPurchaseIntoGrid(itemGrid, eventData.position))
        {
            draggedInventoryItem.ShowIconFrame();
            SoundManager.Instance.PlayPurchaseItem();
            sourceShopSlot?.ClearShopSlot();
            onClaimed?.Invoke();
            Destroy(gameObject);
            draggedInventoryItem = null;
            draggedItemRectTransform = null;
            ClearDragPreview();
            return;
        }

        Destroy(draggedInventoryItem.gameObject);
        draggedInventoryItem = null;
        draggedItemRectTransform = null;
        canvasGroup.blocksRaycasts = originalBlocksRaycasts;
        canvasGroup.alpha = originalAlpha;
        ClearDragPreview();
    }

    // Quét mọi UI dưới con trỏ để tìm ItemGrid, không bị che bởi panel/icon đang kéo
    private ItemGrid FindGridUnderPointer(PointerEventData eventData)
    {
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult r in results)
        {
            ItemGrid grid = r.gameObject.GetComponentInParent<ItemGrid>();
            if (grid != null) return grid;
        }
        return null;
    }

    private void ClearDragPreview()
    {
        if (dragPreviewController != null)
        {
            dragPreviewController.SetDraggedItemPreview(null);
            dragPreviewController = null;
        }
    }

    private bool TryPurchaseIntoGrid(ItemGrid itemGrid, Vector2 screenPosition)
    {
        if (isFree)
        {
            return itemGrid.TryPlaceNewItemAtScreenPosition(draggedInventoryItem, screenPosition);
        }

        if (itemData.price < 0)
        {
            Debug.LogError("Cannot purchase an item with a negative price.", this);
            return false;
        }

        PlayerWallet wallet = PlayerWallet.Instance;
        if (wallet == null)
        {
            Debug.LogError("A shop item cannot be purchased without a PlayerWallet.", this);
            return false;
        }

        if (!wallet.CanAffordGold(itemData.price))
        {
            Debug.Log("Not Enough Gold!");
            SoundManager.Instance.PlayCancel();
            return false;
        }

        InventoryItem inventoryItem = draggedInventoryItem;
        if (!itemGrid.TryPlaceNewItemAtScreenPosition(inventoryItem, screenPosition))
        {
            return false;
        }

        if (!wallet.SpendGold(itemData.price))
        {
            itemGrid.TryRemoveItem(inventoryItem);
            return false;
        }

        return true;
    }
}