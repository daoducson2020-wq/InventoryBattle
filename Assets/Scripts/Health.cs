using UnityEngine.UI;
using UnityEngine;
using TMPro;
using NUnit.Framework.Constraints;
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
    public int CurrentHealth => currentHealth;
    public void Awake()
    {
        enemyReward = GetComponent<EnemyReward>();
    }
    public void Start()
    {
        if (GameSession.Instance != null && GameSession.Instance.HasSavedHealth && gameObject.tag == "Player")
            currentHealth = GameSession.Instance.CurrentHealth;
        else
        {
            currentHealth = baseHealth;
            SaveHealth();
        }    
        healthBarUI.maxValue = baseHealth;
        healthBarUI.value = currentHealth;
        healthBarUI.wholeNumbers = true;
        UpdateHealthText(currentHealth);
    }

    public void SaveHealth()
    {
        if (gameObject.tag == "Player" && GameSession.Instance != null)
            GameSession.Instance.SaveHealth(currentHealth);
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
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        SaveHealth();
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
        healthText.SetText("{0:0} / {1:0}", (float)currentHealth, (float)baseHealth);
    }
}
