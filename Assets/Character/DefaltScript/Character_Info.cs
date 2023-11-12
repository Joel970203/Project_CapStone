using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Info : MonoBehaviour
{
    [SerializeField]
    private int Max_HP;
    public int HP;

    [SerializeField]
    private int AD;

    [SerializeField]
    private int armor;

    [SerializeField]
    private GameObject hitEffectPrefab;

    public bool Immotal;

    [SerializeField]
    private GameObject Grave;

    void Start()
    {
        HP = Max_HP;
        Immotal = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Death();
        }
    }

    //보스에게 근접 공격 피격시 작동하는 이펙트
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("BossAttack") && !Immotal)
        {
            //Vector3 characterCenter = transform.position; // 캐릭터 중심점
            //Vector3 BossCenter = other.gameObject.transform.position; // 적의 중심점

            // 타격 위치를 평균 위치로 설정
            //Vector3 hitPoint = (characterCenter + BossCenter) / 2;

            // 타격 이펙트를 생성하거나 처리
            //GameObject spawnedHit =Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

            TakeDamage(20);
        }

        if (other.gameObject.CompareTag("Heal"))
        {
            if (other.gameObject.GetComponent<SkillHealAmount>())
            {
                TakeHeal((int)(other.gameObject.GetComponent<SkillHealAmount>().HealAmount));
            }
        }
        else
        {
            TakeHeal(20);
        }
    }


    public virtual void TakeDamage(int damage)
    {
        //Debug.Log("Take Damage");
        if (damage - armor > 0)
        {
            HP -= damage - armor;
            if (HP <= 0)
            {
                Death();
            }
        }
    }

    public virtual void TakeHeal(int Heal)
    {
        Debug.Log(Heal);
        if (HP + Heal >= Max_HP)
        {
            HP = Max_HP;
        }
        else
        {
            HP += Heal;
        }
    }

    private void Death()
    {
        GameObject CharacterGrave = Instantiate(Grave, this.transform.position, Quaternion.identity);
        CharacterGrave.GetComponent<RivivalCharacter>().GameCharacter = this.gameObject;
        this.gameObject.SetActive(false);
    }
}
