using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemType;
    [SerializeField] private int damage;
    [SerializeField] private float attackRate;
    [SerializeField] private Sprite icon;
    [SerializeField] private Sprite iconWithFrame;
    [SerializeField] private int itemWidth = 1;
    [SerializeField] private int itemHeight = 1;
    [SerializeField] public int price;

    public string ItemName => itemName;
    public int Damage => damage;
    public float AttackRate => attackRate;
    public Sprite Icon => icon;
    public Sprite IconWithFrame => iconWithFrame;
    public int ItemWidth => itemWidth;
    public int ItemHeight => itemHeight;
}
