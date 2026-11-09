using TMPro;
using UnityEngine;

public class BattleStartTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI battleStartText;
    [SerializeField] private Animator animator;

    private void Start()
    {
        battleStartText.text = "Ready!";
    }


    public void PlayAnim()
    {
        battleStartText.text = "Fight!";
        animator.SetBool("Start", true);
    }
    
    private void EndBattleTextAnim()
    {
        animator.SetBool("Start", false);
        battleStartText.enabled = false;
    }
}
