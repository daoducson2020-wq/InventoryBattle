using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Material cooldownAnimation;

    private Health target;
    private Health selfHealth;
    private float waitTime;
    private float cooldownTime;
    private float startCombat = 3f;
    private string self;
    private Material itemMaterial;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "ShopScene")
        {
            IdentifySelfTag();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene")
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
                target.TakeDamage(itemData.Damage);
            }
            if (itemData.HealAmount > 0 && selfHealth != null && cooldownTime >= itemData.AttackRate)
            {
                ResetFloat();
                selfHealth.Heal(itemData.HealAmount);
            }
            cooldownTime += Time.deltaTime;
            float time = cooldownTime / itemData.AttackRate;
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
                itemMaterial = new Material(cooldownAnimation);
                Texture tex = ConvertSpriteToTexture(itemData.IconWithFrame);
                itemMaterial.SetTexture("_ItemTexture", tex);
                itemMaterial.SetFloat("_Cooldown", 0f);
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
                break;
            case "Enemy":
                target = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>();
                selfHealth = GetComponentInParent<Health>();
                break;
        }
    }

    private void ResetFloat()
    {
        itemMaterial.SetFloat("_Cooldown", 0f);
        cooldownTime = 0f;
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
