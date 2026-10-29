using UnityEngine;
using UnityEngine.EventSystems;

public class GridDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;
    private InventoryController dragPreviewController;

    [HideInInspector] public ItemData itemData;       // Dữ liệu vật phẩm được gán từ Shop Slot
    [HideInInspector] public ShopSlot sourceShopSlot; // Tham chiếu đến ô cửa hàng chứa vật phẩm này

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null) return;

        dragPreviewController = FindAnyObjectByType<InventoryController>();
        if (dragPreviewController != null)
        {
            dragPreviewController.SetDraggedItemPreview(itemData);
        }

        originalParent = transform.parent;
        originalPosition = rectTransform.localPosition;

        // Đưa item lên Canvas chính để hiển thị nổi lên trên cùng khi kéo
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false; // Tắt raycast để dò tìm điểm thả phía dưới
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemData == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemData == null)
        {
            ClearDragPreview();
            return;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
        ItemGrid itemGrid = dropTarget != null
            ? dropTarget.GetComponentInParent<ItemGrid>()
            : null;

        if (itemGrid != null && TryPurchaseIntoGrid(itemGrid, eventData.position))
        {
            ClearDragPreview();
            return;
        }

        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        ClearDragPreview();
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
        if (itemData.price < 0)
        {
            Debug.LogError("Cannot purchase an item with a negative price.", this);
            return false;
        }

        InventoryController inventoryController = FindAnyObjectByType<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogError("A shop item cannot be purchased without an InventoryController.", this);
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
            return false;
        }

        InventoryItem inventoryItem = inventoryController.CreateInventoryItem(itemData, screenPosition);
        if (inventoryItem == null)
        {
            return false;
        }

        if (!itemGrid.TryPlaceNewItemAtScreenPosition(inventoryItem, screenPosition))
        {
            Destroy(inventoryItem.gameObject);
            return false;
        }

        if (!wallet.SpendGold(itemData.price))
        {
            itemGrid.TryRemoveItem(inventoryItem);
            Destroy(inventoryItem.gameObject);
            return false;
        }

        if (sourceShopSlot != null)
        {
            sourceShopSlot.ClearShopSlot();
        }

        return true;
    }
}