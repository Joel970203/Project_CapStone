using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] Sprite sprite;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Item item = new Item();
            item.setItem(itemType, sprite);

            other.gameObject.GetComponent<CharacterInventory>().CollectItem(item);
            Destroy(gameObject);
        }
    }
}