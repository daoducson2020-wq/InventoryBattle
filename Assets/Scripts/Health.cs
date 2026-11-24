using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class Health : MonoBehaviour
{
    [SerializeField] private int baseHealth = 300;
    [SerializeField] private Slider healthBarUI;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private MobSpawner mobSpawner;
    [SerializeField] private HitResponseUI hitResponse;
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
        if (CompareTag("Player") && baseHealth < 300)
        {
            baseHealth = 300;
        }
        maxHealth = baseHealth;
        mobSpawner = FindAnyObjectByType<MobSpawner>();
    }

    public void Start()
    {
        EnsureInitialized();
        InitializeUnitName();

        if (itemGrid != null)
        {
            // If health was restored from GameSession and the grid has not loaded its items yet,
            // the armor bonus reads as 0 here and would wrongly shrink max/current health.
            // ItemGrid.LoadItem -> NotifyHealthChanged will sync it right after loading items.
            bool gridItemsPending = CompareTag("Player")
                && GameSession.Instance != null
                && GameSession.Instance.HasSavedHealth
                && GameSession.Instance.PlayerInventoryDatas.Count > 0
                && itemGrid.GetTotalArmorHealthBonus() == 0;

            if (gridItemsPending)
            {
                RefreshHealthUI();
            }
            else
            {
                UpdateMaxHealthFromInventory(itemGrid.GetTotalArmorHealthBonus());
            }
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
            // Losing max HP from gear must not kill, but must never revive a dead unit either
            currentHealth = currentHealth > 0 ? Mathf.Max(1, currentHealth + delta) : 0;
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
        if (currentHealth <= 0) return;
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
        SoundManager.Instance.PlayGetHit();
        hitResponse.SpriteChange();
        switch (transform.name)
        {
            case "Player":
                int rnd = UnityEngine.Random.Range(0, 2);
                if (rnd == 0)
                {
                    SoundManager.Instance.PlayPlayerGetHit();
                }
                break;
            case "Mob":
                int rng = UnityEngine.Random.Range(0, 2);
                if (rng == 0)
                {
                    PlaySfx(mobSpawner.EnemyName.text);
                }
                break;
            case "Enemy":
                int vng = UnityEngine.Random.Range(0, 2);
                if (vng == 0)
                {
                    PlaySfx(nameText.text);
                }
                break;
        }
        SaveHealth();
        RefreshHealthUI();
        if (currentHealth == 0) Die();
        else AutoUseHealthPotions();
    }

    // Player only: while HP < 50% of max, consume potions one by one (grid order).
    private void AutoUseHealthPotions()
    {
        if (!CompareTag("Player") || itemGrid == null) return;

        while (currentHealth * 2 < maxHealth)
        {
            int healAmount = itemGrid.ConsumeNextHealthPotion();
            if (healAmount <= 0) break;
            SoundManager.Instance.PlayDrinkPotion();
            Heal(healAmount);
        }
    }

    public void Die()
    {
        Debug.Log(gameObject.name + " Die!");
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Health.Die: no GameManager found, GameOver/WinStage not called.", this);
            return;
        }
        if (!CompareTag("Player") && !CompareTag("Enemy"))
        {
            Debug.LogWarning($"Health.Die: '{name}' has tag '{tag}', expected Player or Enemy.", this);
            return;
        }
        if (CompareTag("Player"))
        {
            MusicManager.Instance.StopPlayMusic();
            SoundManager.Instance.PlayDie();
            gameManager.GameOver();
        }
        else if (CompareTag("Enemy"))
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

    private void InitializeUnitName()
    {
        if (CompareTag("Player"))
        {
            string playerName = (GameSession.Instance != null && !string.IsNullOrEmpty(GameSession.Instance.PlayerName))
                ? GameSession.Instance.PlayerName
                : "Player";
            SetUnitName(playerName);
        }
        else if (CompareTag("Enemy"))
        {
            MobController mob = GetComponent<MobController>();
            if (mob != null && !string.IsNullOrEmpty(mob.mobName))
            {
                SetUnitName(mob.mobName);
            }
        }
    }

    // Hàm public để BigEnemyManager (MiniBoss / Boss) hoặc các script khác gán tên trực tiếp
    public void SetUnitName(string unitName)
    {
        if (nameText != null)
        {
            nameText.text = unitName;
        }
    }

    private void PlaySfx(string name)
    {
        switch (name)
        {
            case "Archer":
            case "Dark Triad":
            case "Armored General":
            case "Fallen Angel":
            case "Fighter Cheems":
            case "Guardian":
                SoundManager.Instance.PlayMobGetHit();
                break;
            case "Chad of Cheems":
                SoundManager.Instance.PlayBossGetHit();
                break;
            default:
                SoundManager.Instance.PlayMiniBossGetHit();
                break;
        }
    }
}
