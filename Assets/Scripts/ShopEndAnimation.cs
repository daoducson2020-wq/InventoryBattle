using UnityEngine;

public class ShopEndAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator1;

    public void Buy()
    {
        animator1.SetBool("isBuying", false);
    }
    public void Sell()
    {
        animator1.SetBool("isBurn", false);
    }
    public void Attack()
    {
        animator1.SetBool("isAttacking", false);
    }
    public void GetHit()
    {
        animator1.SetBool("isAttacked",false);
    }
}
