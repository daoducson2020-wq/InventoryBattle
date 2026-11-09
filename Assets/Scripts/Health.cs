using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameManager gameManager;
    private EnemyReward enemyReward;
    private int currentHealth;
    public void Awake()
    {
        enemyReward = GetComponent<EnemyReward>();
    }
    public void Start()
    {

        currentHealth = maxHealth;
        healthBarUI.maxValue = maxHealth;
        healthBarUI.value = currentHealth;
        healthBarUI.wholeNumbers = true;
        UpdateHealthText(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        healthBarUI.value = currentHealth;

        UpdateHealthText(currentHealth);

        if (currentHealth == 0) Die();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " Die!");
        if (gameObject.tag == "Player" && gameManager != null)
            gameManager.GameOver();
        else if (gameObject.tag == "Enemy" && gameManager != null)
        {
            enemyReward.Reward();
            gameManager.WinStage();
        }
    }

    private void UpdateHealthText(int currentHealth)
    {
        healthText.SetText("{0:0} / {1:0}", (float)currentHealth, (float)maxHealth);
    }
}
