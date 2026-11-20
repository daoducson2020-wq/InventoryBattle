using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string soundName, float volume = 1f)
    {
        sfx2DSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName), volume);
    }

    public void PlayPurchaseItem()
    {
        PlaySound("Purchase");
    }

    public void PlayButtonPressed()
    {
        PlaySound("ButtonPress", .8f);
    }

    public void PlaySellItem()
    {
        PlaySound("SellItem");
    }

    public void PlayRefreshShop()
    {
        PlaySound("RefreshShop");
    }

    public void PlayItemPickup()
    {
        PlaySound("ItemPickup");
    }

    public void PlayDrinkPotion()
    {
        PlaySound("DrinkPotion");
    }

    public void PlayInventoryOpen()
    {
        PlaySound("InventoryOpen");
    }

    public void PlayDie()
    {
        PlaySound("Die");
    }

    public void PlayBeginBattle()
    {
        PlaySound("BeginBattle", .25f);
    }

    public void PlayGetHit()
    {
        PlaySound("GetHit");
    }
    
    public void PlayVictory()
    {
        PlaySound("Victory");
    }

    public void PlayCancel()
    {
        PlaySound("Cancel");
    }

    public void PlayPlayerGetHit()
    {
        PlaySound("PlayerGetHit");
    }

    public void PlayMobGetHit()
    {
        PlaySound("MobGetHit");
    }
    
    public void PlayMiniBossGetHit()
    {
        PlaySound("MiniBossGetHit");
    }

    public void PlayBossGetHit()
    {
        PlaySound("BossGetHit");
    }
}