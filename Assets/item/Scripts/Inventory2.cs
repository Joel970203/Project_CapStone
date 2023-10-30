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

public class Inventory2
{
    List<Item> itemList = new List<Item>();
    int[] itemCountArr = new int[6];
    GameObject[] slotUI;
    bool dupFlag = false; //중복됐는지를 확인하는 플레그

    public void setSlotUI(GameObject[] slotUI)
    {
        this.slotUI = slotUI;
    }

    public void AddItem(Item newItem)
    {
        int index = 0;
        int itemListCount = itemList.Count;

        for (int i = 0; i < itemListCount; i++)
        {
            if (i > 5) break;
            
            if (itemCountArr[i] == 0)
            {
                itemListCount++;
                continue;
            }

            Item existItem = itemList[i];

            if (existItem.type == newItem.type)
            {
                itemCountArr[i]++;
                //existItem.quantity++;

                TextMeshProUGUI quantityText = slotUI[i].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
                quantityText.text = itemCountArr[i].ToString();

                dupFlag = true;
                break;
            }

            index++;
        }

        if (!dupFlag)
        {
            slotUI[index].GetComponent<Image>().sprite = newItem.image;
            itemList.Add(newItem);
            itemCountArr[index]++;

            TextMeshProUGUI quantityText = slotUI[index].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
            quantityText.text = "1";
        }

        dupFlag = false;
    }

    public ItemType UseItem(int index)
    {
        if (itemCountArr[index] == 0) return ItemType.Nothing;

        itemCountArr[index]--;
        TextMeshProUGUI quantityText = slotUI[index].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
        quantityText.text = itemCountArr[index].ToString();

        if (itemCountArr[index] == 0)
        {
            slotUI[index].GetComponent<Image>().sprite = null;
            itemList.RemoveAt(index);
        }

        return itemList[index].type;
    }

    public void print()
    {
        foreach (Item existItem in itemList)
        {
            Debug.Log("Item Type: " + existItem.type);
            Debug.Log("Item quantity: " + existItem.quantity);
        }
    }
}