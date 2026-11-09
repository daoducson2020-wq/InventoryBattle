using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCollections", menuName = "Scriptable Objects/Item Collections")]
public class ItemCollections : ScriptableObject
{
    public List<ItemData> itemCollections = new();

    public ItemData GetDataInCollections(string name)
    {
        ItemData data = itemCollections.Find(x => x.ItemName == name);
        return data;
    }
}
