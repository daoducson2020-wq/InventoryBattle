using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthBarUI;
    private int currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;
        healthBarUI.maxValue = maxHealth;
        healthBarUI.value = currentHealth;
        healthBarUI.wholeNumbers = true;
    }

    public void TakeDamage(int damage)
    {
        if(damage <= 0 || currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        healthBarUI.value = currentHealth;

        if(currentHealth == 0) Die();
    }

    public void Die()
    {
        Debug.Log(gameObject.name + "Lose");
    }
}
