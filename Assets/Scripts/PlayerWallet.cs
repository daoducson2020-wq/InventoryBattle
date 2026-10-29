using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance;

    [Header("Currency Settings")]
    public int currentGold = 1000; // Số vàng khởi điểm của người chơi
    public TextMeshProUGUI goldText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateGoldUI();
    }

    // Kiểm tra và trừ tiền khi mua hàng
    public bool SpendGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Cannot spend a negative amount of gold.", this);
            return false;
        }

        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldUI();
            return true;
        }
        
        Debug.Log("Not Enough Gold!");
        return false;
    }

    public bool CanAffordGold(int amount)
    {
        return amount >= 0 && currentGold >= amount;
    }

    // Cộng thêm tiền (nếu cần bán đồ hoặc nhận thưởng)
    public void AddGold(int amount)
    {
        if (!TryAddGold(amount))
        {
            Debug.LogError("Cannot add an invalid amount of gold.", this);
        }
    }

    public bool TryAddGold(int amount)
    {
        if (amount < 0 || currentGold > int.MaxValue - amount)
        {
            return false;
        }

        currentGold += amount;
        UpdateGoldUI();
        return true;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + currentGold.ToString("N0"); // Hiển thị định dạng số đẹp (vd: 1,000)
        }
    }
}
