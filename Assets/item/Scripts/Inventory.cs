using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

using Image = UnityEngine.UI.Image;

public class Inventory
{
    List<Item> items = new List<Item>();
    GameObject[] slotUI;
    bool dupFlag = false; //중복됐는지를 확인하는 플레그

    public void setSlotUI(GameObject[] slotUI)
    {
       this.slotUI = slotUI;
    }

    public void AddItem(Item newItem)
    {
        int index = 0; //i랑 따로 둔 이유는 나중에 자리 위치 바꿀 것 고려
        int itemsCount = items.Count;
        for (int i = 0; i < itemsCount; i++)
        {
            Item existItem = items[i];

            if (existItem == null)
            {
                itemsCount++;
                continue;
            }

            if (existItem.type == newItem.type)
            {
                existItem.quantity++;

                TextMeshProUGUI quantityText = slotUI[i].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
                quantityText.text = existItem.quantity.ToString();

                dupFlag = true;
                break;
            }

            index++; //슬롯에 아이템이 없거나 중복된 아이템이 아닐때만 index를 더한다.
        }

        if (!dupFlag)
        {
            slotUI[index].GetComponent<Image>().sprite = newItem.image;
            items.Add(newItem);

            TextMeshProUGUI quantityText = slotUI[index].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
            quantityText.text = "1";
        }

        dupFlag = false;
    }

    public ItemType UseItem(int index)
    {
        try
        {
            if (items[index] == null) return ItemType.Nothing;


            items[index].quantity--;
            TextMeshProUGUI quantityText = slotUI[index].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
            quantityText.text = items[index].quantity.ToString();

            if (items[index].quantity == 0)
            {
                slotUI[index].GetComponent<Image>().sprite = null;
                items.RemoveAt(index);
            }

            return items[index].type;
        }
        catch
        {
            Debug.LogError("out of range");
            return ItemType.Nothing;
        }
    }

    public void print()
    {
        foreach (Item existItem in items)
        {
            Debug.Log("Item Type: " + existItem.type);
            Debug.Log("Item quantity: " + existItem.quantity);
        }
    }
}