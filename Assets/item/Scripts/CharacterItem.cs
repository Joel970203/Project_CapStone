using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Inventory2 inventory = new Inventory2();
    [SerializeField] GameObject[] slotUI;
    // Start is called before the first frame update
    void Start() {
        inventory.setSlotUI(slotUI);
    }

    void Update() {
        inventory.print();

        ItemController();
    }

    void ItemController() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.UseItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.UseItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.UseItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) inventory.UseItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) inventory.UseItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) inventory.UseItem(5);
    }

    public void CollectItem(Item item){
        inventory.AddItem(item);
    }
}
