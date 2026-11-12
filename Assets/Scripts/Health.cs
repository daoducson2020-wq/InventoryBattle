using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameManager gameManager;
    private int maxHealth;
    
    private EnemyReward enemyReward;
    private int currentHealth;
    private ItemGrid itemGrid;
    private bool isInitialized;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public void Awake()
    {
        enemyReward = GetComponent<EnemyReward>();
        itemGrid = GetComponent<ItemGrid>();
        maxHealth = baseHealth;
    }

    public void Start()
    {
        EnsureInitialized();

        if (itemGrid != null)
        {
            UpdateMaxHealthFromInventory(itemGrid.GetTotalArmorHealthBonus());
        }
        else
        {
            SaveHealth();
            RefreshHealthUI();
        }
    }



    private void EnsureInitialized()
    {
        if (isInitialized) return;
        isInitialized = true;

        if (GameSession.Instance != null && GameSession.Instance.HasSavedHealth && CompareTag("Player"))
        {
            maxHealth = GameSession.Instance.CurrentMaxHealth > 0
                ? GameSession.Instance.CurrentMaxHealth
                : baseHealth;
            currentHealth = Mathf.Clamp(GameSession.Instance.CurrentHealth, 0, maxHealth);
        }
        else
        {
            maxHealth = baseHealth;
            currentHealth = baseHealth;
        }
    }

    public void UpdateMaxHealthFromInventory(int bonusHealth)
    {
        EnsureInitialized();

        int oldMaxHealth = maxHealth > 0 ? maxHealth : baseHealth;
        maxHealth = baseHealth + Mathf.Max(0, bonusHealth);

        int delta = maxHealth - oldMaxHealth;
        if (delta > 0)
        {
            currentHealth += delta;
        }
        else if (delta < 0)
        {
            currentHealth = Mathf.Max(1, currentHealth + delta);
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SaveHealth();
        RefreshHealthUI();
    }

    public void SaveHealth()
    {
        if (CompareTag("Player") && GameSession.Instance != null)
            GameSession.Instance.SaveHealth(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            Debug.Log("Heal Amount Cannot be Negative Number!");
            return;
        }
        currentHealth = Math.Min(currentHealth + amount, maxHealth);
        SaveHealth();
        RefreshHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        SaveHealth();
        RefreshHealthUI();
        if (currentHealth == 0) Die();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " Die!");
        if (CompareTag("Player") && gameManager != null)
            gameManager.GameOver();
        else if (CompareTag("Enemy") && gameManager != null)
        {
            enemyReward.Reward();
            gameManager.WinStage();
        }
    }

    private void RefreshHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.wholeNumbers = true;
            healthBarUI.maxValue = maxHealth;
            healthBarUI.value = currentHealth;
        }
        UpdateHealthText(currentHealth);
    }

    private void UpdateHealthText(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.SetText("{0:0} / {1:0}", (float)currentHealth, (float)maxHealth);
        }
    }
}
