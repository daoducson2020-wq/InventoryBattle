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
            SetText(itemData.assignedItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipPanel.SetActive(false);
    }

    private void SetText(ItemData data)
    {
        tooltipPanelUI.itemNameText.text = data.ItemName;
        tooltipPanelUI.itemTypeText.text = "Type: " + data.ItemType.ToString();
        switch (data.ItemType)
        {
            case ItemType.Weapon:
                tooltipPanelUI.damageHealthBonusText.enabled = true;
                tooltipPanelUI.damageHealthBonusText.text = "Damage: " + data.Damage.ToString();
                tooltipPanelUI.healingAmountText.enabled = false;
                tooltipPanelUI.cooldownTimeText.enabled = true;
                tooltipPanelUI.cooldownTimeText.text = "Cooldown: " + data.AttackRate.ToString() + " sec";
                break;
            case ItemType.Armor:
                tooltipPanelUI.damageHealthBonusText.enabled = true;
                tooltipPanelUI.damageHealthBonusText.text = "Health Bonus: " + data.HealthBonus.ToString();
                tooltipPanelUI.healingAmountText.enabled = false;
                tooltipPanelUI.cooldownTimeText.enabled = false;
                break;
            case ItemType.Accessory:
                tooltipPanelUI.damageHealthBonusText.enabled = false;
                tooltipPanelUI.damageHealthBonusText.text = "Health Bonus: " + data.HealthBonus.ToString();
                tooltipPanelUI.healingAmountText.enabled = true;
                tooltipPanelUI.healingAmountText.text = "Heal: " + data.HealAmount.ToString();
                tooltipPanelUI.cooldownTimeText.enabled = true;
                tooltipPanelUI.cooldownTimeText.text = "Cooldown: " + data.AttackRate.ToString() + " sec";
                break;
            case ItemType.Potion:
                tooltipPanelUI.damageHealthBonusText.enabled = false;
                tooltipPanelUI.healingAmountText.enabled = true;
                tooltipPanelUI.healingAmountText.text = "Heal: " + data.HealAmount.ToString();
                tooltipPanelUI.cooldownTimeText.enabled = false;
                break;
        }
        tooltipPanelUI.itemDimensionText.text = "Dimensions: " + data.ItemWidth.ToString() + " x " + data.ItemHeight.ToString();
    }
}
