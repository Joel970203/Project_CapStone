using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    Inventory inventory = new Inventory();
    [SerializeField] GameObject[] slotUI;
    // Start is called before the first frame update
    void Start() {
        inventory.setSlot(slotUI);
    }

    void Update() {
        ItemController();
    }

    void ItemController() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.UseItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.UseItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.UseItem(2);
    }

    public void CollectItem(Item item){
        inventory.AddItem(item);
    }
}
