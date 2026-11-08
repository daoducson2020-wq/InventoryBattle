using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MobType {mob, miniBoss}

public class MobController : MonoBehaviour
{
    [SerializeField] private int mobID;
    [SerializeField] private int baseHealth;
    [SerializeField] private int atkDamage;
    [SerializeField] private float atkCooldown;
    [SerializeField] private int goldReward;
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerWallet playerWallet;
    private float nextATKTime;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public int GoldReward => goldReward;
    [SerializeField] private Health playerHealth;

    public void Start()
    {
        currentHealth = baseHealth;
        healthBarUI.maxValue = baseHealth;
        healthBarUI.value = currentHealth;
        healthBarUI.wholeNumbers = true;
        UpdateHealthText(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if(damage <= 0 || currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        healthBarUI.value = currentHealth;

        UpdateHealthText(currentHealth);

        if(currentHealth == 0) Die();
    }

    private void Update()
    {
        if(Time.time >= nextATKTime)
        {
            playerHealth.TakeDamage(atkDamage);
            nextATKTime = Time.time + atkCooldown;
        }
    }

    public void Die()
    {
        if(gameManager == null || playerWallet == null) return;
        Debug.Log(gameObject.name + " Die");
        playerWallet.AddGold(goldReward);
        gameManager.WinStage();
    }

    private void UpdateHealthText(int currentHealth)
    {
        healthText.SetText("HP: {0:0} / {1:0}",(float)currentHealth, (float)baseHealth);
    }

}
