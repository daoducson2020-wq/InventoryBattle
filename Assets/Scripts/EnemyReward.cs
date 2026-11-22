using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    [SerializeField] private int goldReward;
    [SerializeField] private PlayerWallet playerWallet;
    public int GoldReward => goldReward;
            
    public void Reward()
    {
        if (playerWallet == null) playerWallet = FindAnyObjectByType<PlayerWallet>();
        playerWallet.AddGold(goldReward);
    }
}
