using UnityEngine;
public enum ItemType{Weapon, Armor, Accessory, Potion, Core}
[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private ItemType itemType;
    [SerializeField] private int damage;
    [SerializeField] private int healthBonus;
    [SerializeField] private int healAmount;
    [SerializeField] private float attackRate;
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite iconWithFrame;
    [SerializeField] private int itemWidth = 1;
    [SerializeField] private int itemHeight = 1;
    [SerializeField] public int price;

    public string ItemName => itemName;
    public ItemType ItemType => itemType;
    public int Damage => damage;
    public int HealthBonus => healthBonus;
    public int HealAmount => healAmount;
    public float AttackRate => attackRate;
    public Sprite Icon => icon;
    public Sprite IconWithFrame => iconWithFrame;
    public int ItemWidth => itemWidth;
    public int ItemHeight => itemHeight;
}
