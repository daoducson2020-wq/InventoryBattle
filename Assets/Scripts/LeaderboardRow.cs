using UnityEngine;
using TMPro;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text inventoryValueText;
    [SerializeField] private TMP_Text floorsClimbedText;

    public void SetData(int rank, LeaderboardEntry entry)
    {
        rankText.text = rank.ToString();
        playerNameText.text = entry.playerName;
        inventoryValueText.text = entry.inventoryValue.ToString();
        floorsClimbedText.text = entry.floorsClimbed.ToString();
    }
}
