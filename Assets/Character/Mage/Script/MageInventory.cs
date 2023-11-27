using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageInventory : MonoBehaviour
{
    [SerializeField] ParticleSystem healParticle;
    [SerializeField] ParticleSystem speedParticle;
    [SerializeField] ParticleSystem coolDownParticle;

    Character_Info characterInfo;
    [SerializeField] UnityEngine.AI.NavMeshAgent agent;
    MageCharacterSkill mage_Skill;
    Inventory inventory = new Inventory();
    ItemType Item;
    [SerializeField] GameObject[] slotUI;

    private float originalSpeed;
    private bool isSpeedIncreased = false;

    // Start is called before the first frame update
    void Start()
    {
        characterInfo = GetComponent<Character_Info>();
        inventory.setSlot(slotUI);
        mage_Skill = GetComponent<MageCharacterSkill>();
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
                characterInfo.TakeHeal(20);
                PlayParticle(healParticle);
                StartCoroutine(StopParticleAfterDuration(healParticle, 1f)); 
                break;
            case ItemType.Speed:
                if (!isSpeedIncreased)
                {
                    agent.speed = agent.speed * 1.5f;
                    isSpeedIncreased = true;
                    PlayParticle(speedParticle);
                    StartCoroutine(StopParticleAfterDuration(speedParticle, 5f)); 
                    StartCoroutine(ResetSpeedAfterDuration(5f)); 
                }
                break;
            case ItemType.CoolDown:
                mage_Skill.ResetCoolDown();
                PlayParticle(coolDownParticle);
                StartCoroutine(StopParticleAfterDuration(coolDownParticle, 1f)); 
                break;
            default:
                break;
        }
    }

    void PlayParticle(ParticleSystem particle)
    {
        if (particle != null)
        {
            particle.Play();
        }
    }

    IEnumerator StopParticleAfterDuration(ParticleSystem particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (particle != null)
        {
            particle.Stop();
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
