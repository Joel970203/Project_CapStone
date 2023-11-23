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

            collision.gameObject.GetComponent<CharacterInventory>().CollectItem(item);
            Destroy(gameObject);
        }
    }
}