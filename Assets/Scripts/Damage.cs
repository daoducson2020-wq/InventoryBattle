using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Material cooldownAnimation;

    private Health target;
    private Health selfHealth;
    private PlayerWallet playerWallet;
    private float waitTime;
    private float cooldownTime;
    private float startCombat = 3f;
    private string self;
    private Material itemMaterial;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "ShopScene" || SceneManager.GetActiveScene().name != "TreasureScene")
        {
            IdentifySelfTag();
        }
        CheckScene();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene" || SceneManager.GetActiveScene().name == "TreasureScene")
        {
            return;
        }
        if (itemData == null || itemData.AttackRate <= 0f)
        {
            return;
        }
        if (waitTime >= startCombat)
        {
            if (itemData.Damage > 0 && target != null && cooldownTime >= itemData.AttackRate)
            {
                ResetFloat();
                if (itemData.ItemType == ItemType.Weapon)
                {
                    SoundManager.Instance.PlaySound(itemData.AttackSound.ToString());
                }
                target.TakeDamage(itemData.Damage);
            }
            if (itemData.HealAmount > 0 && selfHealth != null && cooldownTime >= itemData.AttackRate)
            {
                ResetFloat();
                if (itemData.ItemType == ItemType.Accessory)
                {
                    SoundManager.Instance.PlaySound(itemData.AttackSound.ToString());
                }
                selfHealth.Heal(itemData.HealAmount);
            }
            if (itemData.GoldAmount > 0 && playerWallet != null && cooldownTime >= itemData.AttackRate)
            {
                ResetFloat();
                playerWallet.AddGold(itemData.GoldAmount);
            }
            cooldownTime += Time.deltaTime;
            float time = cooldownTime / itemData.AttackRate;
            if (itemMaterial != null)
                itemMaterial.SetFloat("_Cooldown", time);
        }
        waitTime += Time.deltaTime;
    }

    public void SetWeapon(ItemData item)
    {
        itemData = item;
        switch (itemData.ItemType)
        {
            case ItemType.Weapon:
            case ItemType.Accessory:
            case ItemType.Core:
                itemMaterial = new Material(cooldownAnimation);
                Texture tex = ConvertSpriteToTexture(itemData.IconWithFrame);
                itemMaterial.SetTexture("_ItemTexture", tex);
                CheckScene();
                GetComponent<Image>().material = itemMaterial;
                break;
        }
    }

    public void IdentifySelfTag()
    {
        if (transform.parent == null) return;
        self = transform.parent.tag;
        switch (self)
        {
            case "Player":
                target = GameObject.FindGameObjectWithTag("Enemy")?.GetComponent<Health>();
                selfHealth = GetComponentInParent<Health>();
                playerWallet = PlayerWallet.Instance != null
                    ? PlayerWallet.Instance
                    : transform.parent.GetComponentInChildren<PlayerWallet>();
                break;
            case "Enemy":
                target = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>();
                selfHealth = GetComponentInParent<Health>();
                break;
        }
    }

    private void ResetFloat()
    {
        if (itemMaterial != null)
            itemMaterial.SetFloat("_Cooldown", 0f);
        cooldownTime = 0f;
    }

    private void CheckScene()
    {
        if ((SceneManager.GetActiveScene().name == "ShopScene" ||
            SceneManager.GetActiveScene().name == "TreasureScene") && itemMaterial != null)
        {
            itemMaterial.SetFloat("_Cooldown", 1f);
        }
        else if ((SceneManager.GetActiveScene().name != "ShopScene" ||
                SceneManager.GetActiveScene().name != "TreasureScene") && itemMaterial != null)
        {
            itemMaterial.SetFloat("_Cooldown", 0f);
        }
    }

    private Texture2D ConvertSpriteToTexture(Sprite sprite)
    {
        try
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] colors = newText.GetPixels();
                Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                             (int)System.Math.Ceiling(sprite.textureRect.y),
                                                             (int)System.Math.Ceiling(sprite.textureRect.width),
                                                             (int)System.Math.Ceiling(sprite.textureRect.height));
                Debug.Log(colors.Length + "_" + newColors.Length);
                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }
        catch
        {
            return sprite.texture;
        }
    }
}
