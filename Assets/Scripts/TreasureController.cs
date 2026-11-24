using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class TreasureController : MonoBehaviour
{
    [SerializeField] private GameObject lifePanel;
    [SerializeField] private GameObject lifeOption1;
    [SerializeField] private GameObject lifeOption2;
    [SerializeField] private GameObject powerPanel;
    [SerializeField] private GameObject p_Opt1;
    [SerializeField] private GameObject p_Opt2;
    [SerializeField] private GameObject p_Opt3;
    [SerializeField] private GameObject sacreficePanel;
    [SerializeField] private GameObject s_Opt1;
    [SerializeField] private GameObject s_Opt2;
    [SerializeField] private GameObject goldPanel;
    [SerializeField] private GameObject g_Opt1;
    [SerializeField] private GameObject g_Opt2;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerWallet playerWallet;

    public void Start()
    {
        playerPanel.SetActive(false);
        powerPanel.SetActive(false);
        sacreficePanel.SetActive(false);
        goldPanel.SetActive(false);
        lifePanel.SetActive(false);
        mainPanel.SetActive(false);
    }

    public void ChooseLife()
    {
        lifeOption1.SetActive(false);
        lifeOption2.SetActive(false);
        playerPanel.SetActive(true);
        lifePanel.SetActive(true);
    }

    public void ChooseLifeOpt1()
    {
        lifeOption1.SetActive(true);
        playerHealth.Heal(playerHealth.MaxHealth);
    }

    public void ChooseLifeOpt2()
    {
        lifeOption2.SetActive(true);
    }

    public void ChoosePower()
    {
        p_Opt1.SetActive(false);
        p_Opt2.SetActive(false);
        p_Opt3.SetActive(false);
        powerPanel.SetActive(true);
        playerPanel.SetActive(true);
    }

    public void P_OPT1()
    {
        p_Opt1.SetActive(true);
    }
    public void P_OPT2()
    {
        p_Opt2.SetActive(true);
    }
    public void P_OPT3()
    {
        p_Opt3.SetActive(true);
    }

    public void ChooseSacrefice()
    {
        s_Opt1.SetActive(false);
        s_Opt2.SetActive(false);
        sacreficePanel.SetActive(true);
        playerPanel.SetActive(true);
    }
    public void Sacrefice_OPT1()
    {
        int requires_health = Mathf.RoundToInt(playerHealth.MaxHealth * 25 / 100);
        if (playerHealth.CurrentHealth < requires_health || playerWallet.CurrentGold < 100)
        {
            s_Opt2.SetActive(true);
        }
        else
        {
            playerHealth.TakeDamage(requires_health);
            playerWallet.SpendGold(100);
            s_Opt1.SetActive(true);
        }
    }
    public void Sacrefice_OPT2()
    {
        s_Opt2.SetActive(true);
    }

    public void ChooseGold()
    {
        g_Opt1.SetActive(false);
        g_Opt2.SetActive(false);
        goldPanel.SetActive(true);
        playerPanel.SetActive(true);
    }
    public void G_OPT1()
    {
        g_Opt1.SetActive(true);
    }
    public void G_OPT2()
    {
        playerWallet.AddGold(150);
        g_Opt2.SetActive(true);
    }

    public void ChooseMain()
    {
        playerPanel.SetActive(true);
        mainPanel.SetActive(true);
    }
}
