using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int goldReward;
    [SerializeField] private PlayerWallet playerWallet;

    public void Reward()
    {
        if(playerWallet == null) return;
        playerWallet.AddGold(goldReward);
    }
}
