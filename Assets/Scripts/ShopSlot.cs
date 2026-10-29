using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI priceText;
    public GameObject itemVisualObject; // GameObject con hiển thị hình ảnh item có gắn script GridDragAndDrop

    [HideInInspector] public ItemData assignedItem;

    public void Setup(ItemData item)
    {
        assignedItem = item;

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = item.Icon;
        }

        if (priceText != null)
        {
            priceText.gameObject.SetActive(true);
            priceText.text = item.price.ToString() + " Gold";
        }

        // Truyền dữ liệu sang component kéo thả của item visual
        if (itemVisualObject != null)
        {
            itemVisualObject.SetActive(true);
            GridDragAndDrop dragScript = itemVisualObject.GetComponent<GridDragAndDrop>();
            if (dragScript != null)
            {
                dragScript.itemData = item;
                dragScript.sourceShopSlot = this;
            }
        }
    }

    // Làm sạch slot khi mua thành công hoặc làm mới cửa hàng
    public void ClearShopSlot()
    {
        assignedItem = null;
        if (iconImage != null) iconImage.gameObject.SetActive(false);
        if (priceText != null) priceText.gameObject.SetActive(false);
        if (itemVisualObject != null) itemVisualObject.SetActive(false);
    }
}