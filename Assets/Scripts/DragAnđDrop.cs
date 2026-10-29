using UnityEngine;
using UnityEngine.EventSystems;

public class GridDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private ShopSlot shopSlot;
    private InventoryItem draggedInventoryItem;
    private RectTransform draggedItemRectTransform;
    private CanvasGroup draggedItemCanvasGroup;
    private bool isShopSlot;
    private Transform originalParent;
    private Vector3 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        shopSlot = GetComponent<ShopSlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (shopSlot != null)
        {
            isShopSlot = true;
            InventoryController inventoryController = FindAnyObjectByType<InventoryController>();
            if (inventoryController == null)
            {
                Debug.LogError("A shop item cannot be dragged without an InventoryController.", this);
                return;
            }

            draggedInventoryItem = inventoryController.CreateInventoryItem(shopSlot.assignedItem, eventData.position);
            if (draggedInventoryItem == null) return;

            draggedItemRectTransform = draggedInventoryItem.GetComponent<RectTransform>();
            draggedItemCanvasGroup = draggedInventoryItem.GetComponent<CanvasGroup>();
            if (draggedItemCanvasGroup == null)
            {
                draggedItemCanvasGroup = draggedInventoryItem.gameObject.AddComponent<CanvasGroup>();
            }
            draggedItemCanvasGroup.blocksRaycasts = false;
            draggedItemCanvasGroup.alpha = 0.7f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.7f;
            return;
        }

        originalParent = transform.parent;
        originalPosition = rectTransform.localPosition;

        // Đưa item lên Canvas chính để không bị che khuất khi kéo qua các UI khác
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false; // Cho phép raycast xuyên qua để kiểm tra điểm thả
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isShopSlot)
        {
            if (draggedItemRectTransform != null)
            {
                draggedItemRectTransform.position = eventData.position;
            }
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isShopSlot)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            GameObject shopDropTarget = eventData.pointerCurrentRaycast.gameObject;
            ItemGrid itemGrid = shopDropTarget != null ? shopDropTarget.GetComponentInParent<ItemGrid>() : null;
            ItemData itemData = shopSlot.assignedItem;
            PlayerWallet wallet = PlayerWallet.Instance;
            bool purchaseSucceeded = false;

            if (draggedInventoryItem != null && itemData != null && itemGrid != null)
            {
                if (itemData.price < 0)
                {
                    Debug.LogError("Cannot purchase an item with a negative price.", shopSlot);
                }
                else if (wallet == null)
                {
                    Debug.LogError("A shop item cannot be purchased without a PlayerWallet.", this);
                }
                else if (!wallet.CanAffordGold(itemData.price))
                {
                    Debug.Log("Not Enough Gold!");
                }
                else if (itemGrid.TryPlaceNewItemAtScreenPosition(draggedInventoryItem, eventData.position))
                {
                    purchaseSucceeded = wallet.SpendGold(itemData.price);
                    if (!purchaseSucceeded)
                    {
                        itemGrid.PickUpItem(
                            draggedInventoryItem.onGridPositionX,
                            draggedInventoryItem.onGridPositionY);
                    }
                }
            }

            if (draggedInventoryItem != null && !purchaseSucceeded)
            {
                Destroy(draggedInventoryItem.gameObject);
            }
            else if (purchaseSucceeded && draggedItemCanvasGroup != null)
            {
                draggedItemCanvasGroup.blocksRaycasts = true;
                draggedItemCanvasGroup.alpha = 1f;
            }

            if (purchaseSucceeded)
            {
                Destroy(gameObject);
            }

            draggedInventoryItem = null;
            draggedItemRectTransform = null;
            draggedItemCanvasGroup = null;
            isShopSlot = false;
            return;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Kiểm tra xem người chơi có thả vào một ô Slot hợp lệ nào không
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (dropTarget != null && dropTarget.CompareTag("InventorySlot"))
        {
            // Nếu thả vào slot kho đồ: Chuyển parent của item vào slot đó và reset vị trí về tâm slot
            transform.SetParent(dropTarget.transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            // Nếu thả ra ngoài không gian trống: Trả về vị trí cũ trong cửa hàng
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}
