using TMPro;
using UnityEngine;

public class TooltipPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI damageHealthBonusText;
    [SerializeField] private TextMeshProUGUI healingAmountText;
    [SerializeField] private TextMeshProUGUI cooldownTimeText;
    [SerializeField] private TextMeshProUGUI itemDimensionText;
    [SerializeField] private float offsetX = 50f;
    [SerializeField] private float offsetY = 50f;

    private void Update()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;
        GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);
        float x = GetComponent<RectTransform>().sizeDelta.x / 2;
        if (position.x > (Screen.width / 2))
        {
            transform.position = position + new Vector2(-x - offsetX, -offsetY);
        }
        else
        {
            transform.position = position + new Vector2(x + offsetX, -offsetY);
        }
    }

    public void SetText(ItemData data)
    {
        itemNameText.text = data.ItemName;
        itemTypeText.text = "Type: " + data.ItemType.ToString();
        switch (data.ItemType)
        {
            case ItemType.Weapon:
                damageHealthBonusText.enabled = true;
                damageHealthBonusText.text = "Damage: " + data.Damage.ToString();
                healingAmountText.enabled = false;
                cooldownTimeText.enabled = true;
                cooldownTimeText.text = "Cooldown: " + data.AttackRate.ToString() + " sec";
                break;
            case ItemType.Armor:
                damageHealthBonusText.enabled = true;
                damageHealthBonusText.text = "Health Bonus: " + data.HealthBonus.ToString();
                healingAmountText.enabled = false;
                cooldownTimeText.enabled = false;
                break;
            case ItemType.Accessory:
                damageHealthBonusText.enabled = false;
                damageHealthBonusText.text = "Health Bonus: " + data.HealthBonus.ToString();
                healingAmountText.enabled = true;
                healingAmountText.text = "Heal: " + data.HealAmount.ToString();
                cooldownTimeText.enabled = true;
                cooldownTimeText.text = "Cooldown: " + data.AttackRate.ToString() + " sec";
                break;
            case ItemType.Potion:
                damageHealthBonusText.enabled = false;
                healingAmountText.enabled = true;
                healingAmountText.text = "Heal: " + data.HealAmount.ToString();
                cooldownTimeText.enabled = false;
                break;
        }
        itemDimensionText.text = "Dimensions: " + data.ItemWidth.ToString() + " x " + data.ItemHeight.ToString();
    }
}
