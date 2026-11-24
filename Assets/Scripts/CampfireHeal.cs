using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CampfireHeal : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Animator animator;
    [SerializeField] private Button healButton;
    [SerializeField] private float clickDelay = 2f;

    private bool hasHealed = false;
    private bool canClick = false;

    public void Awake()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        hasHealed = false;
        canClick = false;
        if (healButton != null)
        {
            healButton.interactable = false;
        }
        StartCoroutine(EnableClickAfterDelay());
    }

    private IEnumerator EnableClickAfterDelay()
    {
        yield return new WaitForSeconds(clickDelay);
        canClick = true;
        if (healButton != null && !hasHealed)
        {
            healButton.interactable = true;
        }
    }

    public void HealCampfire()
    {
        if (!canClick || hasHealed || playerHealth == null) return;
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
