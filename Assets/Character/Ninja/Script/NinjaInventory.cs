
/*
public class NinjaInventory : MonoBehaviour
{
    Character_Info characterInfo;
    [SerializeField]
    UnityEngine.AI.NavMeshAgent agent;
    Ninja_Skill ninja_Skill;
    Inventory inventory = new Inventory();
    ItemType Item;
    [SerializeField] GameObject[] slotUI;
    // Start is called before the first frame update
    void Start() {
        inventory.setSlot(slotUI);
        ninja_Skill = GetComponent<Ninja_Skill>();
    }

    void Update() {
        ItemController();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaInventory : MonoBehaviour
{
    Character_Info characterInfo;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    Ninja_Skill ninja_Skill;
    Inventory inventory = new Inventory();
    ItemType Item;
    [SerializeField] GameObject[] slotUI;

    private float originalSpeed;
    private bool isSpeedIncreased = false;

    // Start is called before the first frame update
    void Start()
    {
        inventory.setSlot(slotUI);
        ninja_Skill = GetComponent<Ninja_Skill>();
        originalSpeed = agent.speed; // 초기 속도 저장
    }

    void Update()
    {
        ItemController();
    }

    void ItemController()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Item = inventory.UseItem(0);
            UseItemEffect(Item);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Item = inventory.UseItem(1);
            UseItemEffect(Item);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Item = inventory.UseItem(2);
            UseItemEffect(Item);
        }
    }

    // 아이템 사용 효과 적용
    void UseItemEffect(ItemType item)
    {
        switch (item)
        {
            case ItemType.Hp:
                characterInfo.HP += 20;
                break;
            case ItemType.Speed:
                if (!isSpeedIncreased)
                {
                    agent.speed = agent.speed * 1.5f;
                    isSpeedIncreased = true;
                    StartCoroutine(ResetSpeedAfterDuration(5f)); // 5초 후에 속도 원래대로 복구
                }
                break;
            case ItemType.CoolDown:
                ninja_Skill.ResetCoolDown();
                break;
            default:
                break;
        }
    }

    // 일정 시간이 지난 후에 속도 원래대로 복구하는 코루틴
    IEnumerator ResetSpeedAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        agent.speed = originalSpeed; // 원래 속도로 복구
        isSpeedIncreased = false; // 상태 초기화
    }

    public void CollectItem(Item item)
    {
        inventory.AddItem(item);
    }
}
