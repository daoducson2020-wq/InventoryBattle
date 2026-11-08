using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MobType {mob, miniBoss}

public class MobController : MonoBehaviour
{
    [SerializeField] private int mobID;
    [SerializeField] private string mobName;
    [SerializeField] private int atkDamage;
    [SerializeField] private float atkCooldown;
    private float nextATKTime;

    [SerializeField] private Health playerHealth;


    private void Update()
    {
        if(Time.time >= nextATKTime)
        {
            playerHealth.TakeDamage(atkDamage);
            nextATKTime = Time.time + atkCooldown;
        }
    }


}
