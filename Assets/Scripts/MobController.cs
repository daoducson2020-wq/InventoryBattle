using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MobType { begin, middle, high }

public class MobController : MonoBehaviour
{
    public int mobID;
    public string mobName;
    public MobType mobType;
    [SerializeField] private int atkDamage;
    [SerializeField] private float atkCooldown;
    private float nextATKTime;
    private float waitTime;
    private float startCombat = 3f;

    [SerializeField] private Health playerHealth;

    public void Start()
    {   
        if (playerHealth != null) return;
        foreach (Health h in FindObjectsByType<Health>())
        {
            if (h.CompareTag("Player")) { playerHealth = h; break; }
        }
    }

    private void Update()
    {
        if (waitTime >= startCombat)
        {
            if (Time.time >= nextATKTime)
            {
                playerHealth.TakeDamage(atkDamage);
                nextATKTime = Time.time + atkCooldown;
            }
        }
        else
        {
            nextATKTime = Time.time + atkCooldown;
        }
        waitTime += Time.deltaTime;
    }


}
