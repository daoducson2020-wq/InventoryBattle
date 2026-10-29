using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI priceText;
    [HideInInspector] public ItemData assignedItem;

    public void Setup(ItemData item)
    {
        assignedItem = item;
        if (iconImage != null)
        {
            iconImage.sprite = item.Icon;
            iconImage.preserveAspect = true;
        }
        if (priceText != null) priceText.text = item.price + " Gold";
    }
}
