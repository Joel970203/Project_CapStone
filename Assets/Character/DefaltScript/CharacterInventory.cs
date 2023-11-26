using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterInventory : MonoBehaviour
{}
/*{
    Character_Info characterInfo;
    [SerializeField]
    UnityEngine.AI.NavMeshAgent agent;
    Ninja_Skill ninja_Skill;
    Inventory inventory = new Inventory();
    ItemType Item;
    [SerializeField] GameObject[] slotUI;
    [SerializeField] Sprite sprite;
    // Start is called before the first frame update
    void Start() {
        inventory.setSlot(slotUI);
        ninja_Skill = GetComponent<Ninja_Skill>();
    }

    void Update() {
        ItemController();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Item item = new Item();
            item.setItem(itemType, sprite);

            collision.gameObject.GetComponent<NinjaInventory>().CollectItem(item);
            Destroy(gameObject);
        }
    }


    void ItemController() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            Item = inventory.UseItem(0);
            if(Item == ItemType.Hp)
            {
                characterInfo.HP += 20;
            }
            else if(Item == ItemType.Speed)
            {
                agent.speed = agent.speed * 1.5f;
            }
            else if(Item == ItemType.CoolDown)
            {
                ninja_Skill.ResetCoolDown();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Item = inventory.UseItem(1);
            {
                if(Item == ItemType.Hp)
                {
                    characterInfo.HP += 20;
                }
                else if(Item == ItemType.Speed)
                {
                    agent.speed = agent.speed * 1.5f;
                }
                else if(Item == ItemType.CoolDown)
                {
                    ninja_Skill.ResetCoolDown();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) 
        {
            Item = inventory.UseItem(2);
            {
                if(Item == ItemType.Hp)
                {
                    characterInfo.HP += 20;
                }
                else if(Item == ItemType.Speed)
                {
                    agent.speed = agent.speed * 1.5f;
                }
                else if(Item == ItemType.CoolDown)
                {
                    ninja_Skill.ResetCoolDown();
                }
            }
        }
    }
    public void CollectItem(Item item){
        inventory.AddItem(item);
    }
}
*/