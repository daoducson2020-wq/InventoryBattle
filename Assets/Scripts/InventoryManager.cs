using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    private InventoryItem[,] playerInventory = new InventoryItem[10, 10];
    public InventoryItem[,] PlayerInventory => playerInventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    
    
    // public void SetItemSlotNULL(int posX, int posY)
    // {
    //     PlayerInventory[posX, posY] = null;
    // }

    // public void SaveItem(InventoryItem item, int posX, int posY)
    // {
    //     for (int x = 0; x < item.Width; x++)
    //     {
    //         for (int y = 0; y < item.Height; y++)
    //         {
    //             PlayerInventory[posX + x, posY + y] = item;
    //         }
    //     }
    // }

    // [ContextMenu("Print Inventory from instance")]
    // private void PrintInven()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         for (int j = 0; j < 10; j++)
    //         {
    //             if (InventoryManager.Instance.PlayerInventory[i,j] == null)
    //             {
    //                 Debug.Log("(null)");
    //             }
    //             else
    //             {
    //                 Debug.Log(InventoryManager.Instance.PlayerInventory[i,j].ItemData.ItemName);
    //             }
    //         }
    //     }
    // }
    
    
    // public void LoadItem()
    // {
    //     if (!InventoryHaveItem())
    //     {
    //         return;
    //     }
    //     for (int i = 0; i < playerInventory.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < playerInventory.GetLength(1); j++)
    //         {
    //             if (playerInventory[i, j] != null)
    //             {
    //                 gridItem.inventoryItemSlot[i, j] = playerInventory[i, j];
    //             }
    //         }
    //     }
    // }

    // private bool InventoryHaveItem()
    // {
    //     for (int i = 0; i < playerInventory.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < playerInventory.GetLength(1); j++)
    //         {
    //             if (playerInventory[i, j] != null)
    //             {
    //                 return true;
    //             }
    //         }
    //     }
    //     return false;
    // }
}
