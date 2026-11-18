using UnityEngine;
using UnityEngine.UI;

public class CampfireHeal : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Animator animator;
    [SerializeField] private Button healButton;

    private bool hasHealed = false;

    public void Awake()
    {
        gameObject.SetActive(true);
    }

    public void HealCampfire()
    {
        if (hasHealed || playerHealth == null) return;
        hasHealed = true;

        if (healButton != null)
        {
            healButton.interactable = false;
        }
        if (animator != null)
        {
            animator.SetTrigger("Heal");
            animator.SetBool("isHealed", true);
        }
        playerHealth.Heal(playerHealth.MaxHealth * 40 / 100);
    }

    public void EndHealAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isHealed", false);
        }
        gameObject.SetActive(false);
    }
}
