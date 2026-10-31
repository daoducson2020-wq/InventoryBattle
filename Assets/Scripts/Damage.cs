using UnityEngine;

public class Damage : MonoBehaviour
{
    private Health target;
    private float nextAtkTime;
    [SerializeField] private ItemData itemData;

    private string self;
    
    // private void Awake()
    // {
    //     string self = transform.parent.tag;
    //     switch (self)
    //     {
    //         case "Player":
    //             target = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Health>();
    //             break;
    //         case "Enemy":
    //             target = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    //             break;
    //     }
    // }
    
    private void Update()
    {
        if (self == null)
        {
            return;    
        }
        if(Time.time >= nextAtkTime)
        {
            target.TakeDamage(itemData.Damage);
            nextAtkTime = Time.time + itemData.AttackRate;
        }
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
