using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public ItemType type;
    public Sprite image;
    public int quantity; 

    public void setItem(ItemType type, Sprite itemImage) {
        this.type = type;
        this.image = itemImage;
        quantity = 1;
    }
}