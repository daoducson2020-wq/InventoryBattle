using System;
using UnityEngine;

[Serializable]
public class InventorySaveData
{
    public string id;
    public ItemData itemdata;
    public int startX;
    public int startY;
    public float rotateAngle;
    public bool rotated;

    // public InventorySaveData(string id, ItemData itemdata, int startx, int starty, int height, int width, float rotation, bool rotated)
    // {
    //     this.id = id;
    //     this.itemdata = itemdata;
    //     startX = startx;
    //     startY = starty;
    //     this.height = height;
    //     this.width = width;
    //     rotateAngle = rotation;
    //     this.rotated = rotated;
    // }
}
