using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameObject goldRewardPanel;
    [SerializeField] private TextMeshProUGUI currentGoldText;
    [SerializeField] private TextMeshProUGUI rewardGoldText;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private GameObject finalResultPanel;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI inventoryValue;
    [SerializeField] private TextMeshProUGUI floorsClimbed;
    [SerializeField] private TextMeshProUGUI nextStageText;
    [SerializeField] private Button returnButton;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private GameManager gameManager;
    private EnemyReward enemyReward;

    private void Start()
    {
        if (goldRewardPanel == null || finalResultPanel == null)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name != "BossScene")
        {
            SetPanel(true, false);
        }
        else
        {
            SetPanel(false, true);
        }
    }

    public void SetResultGoldText()
    {
        SetPanel(true, false);
        enemyReward = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyReward>();
        currentGoldText.SetText((playerWallet.CurrentGold - enemyReward.GoldReward).ToString() + " G");
        rewardGoldText.SetText(enemyReward.GoldReward.ToString() + " G");
        totalGoldText.SetText(playerWallet.CurrentGold.ToString() + " G");
    }

    public void FinalResultText()
    {
        SetPanel(false, true);
        playerName.SetText(GameSession.Instance.PlayerName);
        ItemGrid grid = gameManager.ResolvePlayerItemGrid();
        int value = grid != null ? grid.GetTotalInventoryValue() : 0;
        inventoryValue.SetText(value.ToString() + " G");
        nextStageText.SetText("Finish Run");
        returnButton.gameObject.SetActive(false);
    }

    public void DeathResultText()
    {
        playerName.SetText(GameSession.Instance.PlayerName);
        ItemGrid grid = gameManager.ResolvePlayerItemGrid();
        int value = grid != null ? grid.GetTotalInventoryValue() : 0;
        inventoryValue.SetText(value.ToString() + " G");
        floorsClimbed.SetText(MapView.Instance.FloorsClimbed.ToString());
    }

    private void SetPanel(bool goldPanel, bool finalPanel)
    {
        goldRewardPanel.SetActive(goldPanel);
        finalResultPanel.SetActive(finalPanel);
    }
}
