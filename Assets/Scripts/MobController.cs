using Unity.VisualScripting;
using UnityEngine;

public enum MobType { begin, middle, high }

public class MobController : MonoBehaviour
{
    public int mobID;
    public string mobName;
    public MobType mobType;
    [SerializeField] private int atkDamage;
    [SerializeField] private float atkCooldown;
    [SerializeField] private Animator animator;
    private float nextATKTime;
    private float waitTime;
    private float startCombat = 3f;

    [SerializeField] private Health playerHealth;

    private Health selfHealth;

    public void Start()
    {
        selfHealth = GetComponentInParent<Health>();
        if (playerHealth != null) return;
        foreach (Health h in FindObjectsByType<Health>())
        {
            if (h.CompareTag("Player")) { playerHealth = h; break; }
        }
    }

    private void Update()
    {
        if ((selfHealth != null && selfHealth.CurrentHealth <= 0) ||
            (playerHealth != null && playerHealth.CurrentHealth <= 0))
        {
            return;
        }
        if (waitTime >= startCombat)
        {
            if (Time.time >= nextATKTime)
            {
                nextATKTime = Time.time + atkCooldown;
                animator.SetTrigger("Attack");
                animator.SetBool("isAttacking", true);
            }
        }
        else
        {
            nextATKTime = Time.time + atkCooldown;
        }
        waitTime += Time.deltaTime;
    }

    public void DealDamage()
    {
        if (selfHealth != null && selfHealth.CurrentHealth <= 0)
        {
            return;
        }
        playerHealth.TakeDamage(atkDamage);
    }

    public void EndAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }
}
