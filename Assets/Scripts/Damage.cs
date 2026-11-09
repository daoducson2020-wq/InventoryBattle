using UnityEngine.SceneManagement;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    private Health target;
    private float nextAtkTime;
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
        if (waitTime >= startCombat)
        {
            if (Time.time >= nextAtkTime)
            {
                target.TakeDamage(itemData.Damage);
                nextAtkTime = Time.time + itemData.AttackRate;
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
        self = transform.parent.tag;
        switch (self)
        {
            case "Player":
                target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Health>();
                break;
            case "Enemy":
                target = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
                break;
        }
    }
}
