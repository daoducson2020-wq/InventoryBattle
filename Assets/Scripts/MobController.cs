using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MobType { mob, miniBoss }

public class MobController : MonoBehaviour
{
    [SerializeField] private int mobID;
    [SerializeField] private string mobName;
    [SerializeField] private int atkDamage;
    [SerializeField] private float atkCooldown;
    private float nextATKTime;
    private float waitTime;
    private float startCombat = 3f;

    [SerializeField] private Health playerHealth;


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
        waitTime += Time.deltaTime;
    }


}
