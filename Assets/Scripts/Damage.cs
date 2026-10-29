using UnityEngine;

public class Damage : MonoBehaviour
{
    private Health target;
    private float nextAtkTime;
    [SerializeField]
    private ItemData ItemData;


    private void Awake()
    {
        string self = transform.parent.tag;
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

    private void Update()
    {
        if(Time.time >= nextAtkTime)
        {
            target.TakeDamage(ItemData.Damage);
            nextAtkTime = Time.time + ItemData.AttackRate;
        }
    }
}
