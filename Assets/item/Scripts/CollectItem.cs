using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] Sprite sprite;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Item item = new Item();
            item.setItem(itemType, sprite);

            NinjaInventory ninjaInventory = collision.gameObject.GetComponent<NinjaInventory>();
            MageInventory mageInventory = collision.gameObject.GetComponent<MageInventory>();
            PaladinInventory paladinInventory = collision.gameObject.GetComponent<PaladinInventory>();
            HealerInventory healerInventory = collision.gameObject.GetComponent<HealerInventory>();
            //ArcherInventory archerInventory = collision.gameObject.GetComponent<ArcherInventory>();
            WarriorInventory warriorInventory = collision.gameObject.GetComponent<WarriorInventory>();

            if (ninjaInventory != null)
            {
                ninjaInventory.CollectItem(item); // 닌자의 인벤토리에 아이템 전달
            }
            else if (mageInventory != null)
            {
                mageInventory.CollectItem(item); // 메이지의 인벤토리에 아이템 전달
            }
            else if (paladinInventory != null)
            {
                paladinInventory.CollectItem(item); // 팔라딘의 인벤토리에 아이템 전달
            }
            else if (healerInventory != null)
            {
                healerInventory.CollectItem(item); // 힐러의 인벤토리에 아이템 전달
            }
            /*
            else if (archerInventory != null)
            {
                archerInventory.CollectItem(item); // 아처의 인벤토리에 아이템 전달
            }*/
            else if (warriorInventory != null)
            {
                warriorInventory.CollectItem(item); // 워리어의 인벤토리에 아이템 전달
            }
            Destroy(gameObject);
        }
    }
}

