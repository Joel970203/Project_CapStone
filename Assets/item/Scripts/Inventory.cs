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
    Dictionary<ItemType, Item> itemList = new Dictionary<ItemType, Item>();
    ItemType[] slotArr = new ItemType[6]; //슬롯 번호에 해당되는 아이템 타입의 정보가 들어간다.

    GameObject[] slotUI;
    bool dupFlag = false; //중복됐는지를 확인하는 플레그
    bool notDupFlag = false; //중복되지 않는 아이템의 경우 슬롯의 index가 낮을 수록 우선순위를 부여하기 위한 flag

    public void setSlot(GameObject[] slotUI)
    {
        this.slotUI = slotUI;
        for (int i = 0; i < 6; i++)
        {
            slotArr[i] = ItemType.Nothing;
        }
    }

    public void AddItem(Item newItem)
    {
        int notDupIndex = 0; //인벤토리 내에 추가되는 아이템과 중복되는 아이템이 없을 경우 사용하는 index
        int itemListCount = itemList.Count;

        ItemType newItemType = newItem.type;

        for (int i = 0; i < 6; i++)
        {
            if (i < itemListCount)
            {
                if (slotArr[i] == ItemType.Nothing)
                {
                    if (!notDupFlag)
                    {  //for문은 돌수록 index 번호가 커지기에 처음 빈 슬롯을 찾으면 만일 중복되지 않을 시에 슬롯을 해당 index 번호로 정해두기 위해 flag 설정 (우선순위는 index 번호가 낮은 것)
                        notDupIndex = i;
                        notDupFlag = true;
                    }

                    itemListCount++;
                    continue;
                }

                ItemType existItemType = slotArr[i];

                if (existItemType == newItemType)
                {
                    itemList[existItemType].quantity++;

                    TextMeshProUGUI quantityText = slotUI[i].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
                    quantityText.text = itemList[existItemType].quantity.ToString();

                    dupFlag = true;

                    break;
                }
            }
            else
            {
                if (!notDupFlag)
                {   
                    notDupIndex = i;
                    break;
                }
            }
        }

        if (!dupFlag)
        {
            slotUI[notDupIndex].GetComponent<Image>().sprite = newItem.image;
            slotArr[notDupIndex] = newItemType;
            itemList.Add(newItemType, newItem);
            itemListCount++;

            TextMeshProUGUI quantityText = slotUI[notDupIndex].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
            quantityText.text = "1";
        }

        notDupFlag = false;
        dupFlag = false;
    }

    public ItemType UseItem(int index)
    {
        if (slotArr[index] == ItemType.Nothing) return ItemType.Nothing;

        ItemType useItemType = slotArr[index];
        itemList[useItemType].quantity--;

        TextMeshProUGUI quantityText = slotUI[index].transform.Find("QuantityText").GetComponentInChildren<TextMeshProUGUI>();
        quantityText.text = itemList[useItemType].quantity.ToString();

        if (itemList[useItemType].quantity == 0)
        {
            itemList.Remove(useItemType);
            slotUI[index].GetComponent<Image>().sprite = null;
            slotArr[index] = ItemType.Nothing;
        }

        return useItemType;
    }
}
