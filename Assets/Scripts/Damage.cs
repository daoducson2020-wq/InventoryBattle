using UnityEngine.SceneManagement;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    private Health target;
    private Health selfHealth;
    private float nextAtkTime;
    private float nextHealTime;
    private float waitTime;
    private float startCombat = 3f;
    private string self;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "ShopScene")
        {
            IdentifySelfTag();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene")
        {
            return;
        }
        if (itemData == null || itemData.AttackRate <= 0f)
        {
            return;
        }
        if (waitTime >= startCombat)
        {
            if (itemData.Damage > 0 && target != null && Time.time >= nextAtkTime)
            {
                target.TakeDamage(itemData.Damage);
                nextAtkTime = Time.time + itemData.AttackRate;
            }
            if (itemData.HealAmount > 0 && selfHealth != null && Time.time >= nextHealTime)
            {
                selfHealth.Heal(itemData.HealAmount);
                nextHealTime = Time.time + itemData.AttackRate;
            }
        }
        waitTime += Time.deltaTime;
    }

    public void SetWeapon(ItemData item)
    {
        itemData = item;
    }

    public void IdentifySelfTag()
    {
        if (transform.parent == null) return;
        self = transform.parent.tag;
        switch (self)
        {
            case "Player":
                target = GameObject.FindGameObjectWithTag("Enemy")?.GetComponent<Health>();
                selfHealth = GetComponentInParent<Health>();
                break;
            case "Enemy":
                target = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>();
                selfHealth = GetComponentInParent<Health>();
                break;
        }
    }
}
