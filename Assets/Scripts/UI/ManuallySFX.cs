using UnityEngine;
using UnityEngine.UI;

public class ManuallySFX : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private void Start()
    {
        foreach (var button in buttons)
        {
            if (button.CompareTag("EnemyInventory"))
            {
                button.onClick.AddListener(SoundManager.Instance.PlayInventoryOpen);
            }
            else
            {
                button.onClick.AddListener(SoundManager.Instance.PlayButtonPressed);
            }
        }
    }
}
