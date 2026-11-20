using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipPanel;
    public TooltipPanelUI tooltipPanelUI;
    private ShopSlot itemData;

    private void Start()
    {
        itemData = GetComponent<ShopSlot>();
        tooltipPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData.assignedItem != null)
        {
            tooltipPanel.SetActive(true);
            tooltipPanelUI.SetText(itemData.assignedItem);

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }
}
