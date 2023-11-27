using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Info : MonoBehaviour
{
    [SerializeField] public float Max_HP;
    [SerializeField] public float HP;

    [SerializeField] private int AD;

    [SerializeField] private float armor;

    [SerializeField] private GameObject hitEffectPrefab;

    public bool Immotal;

    void Start()
    {
        HP=Max_HP;
    }

    // Update is called once per frame
    void Update()
    {
        Immotal=false;
    }

    //보스에게 근접 공격 피격시 작동하는 이펙트
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("BossAttack")&&!Immotal)
        {
            //Vector3 characterCenter = transform.position; // 캐릭터 중심점
            //Vector3 BossCenter = other.gameObject.transform.position; // 적의 중심점

            // 타격 위치를 평균 위치로 설정
            //Vector3 hitPoint = (characterCenter + BossCenter) / 2;

            // 타격 이펙트를 생성하거나 처리
            //GameObject spawnedHit =Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);

            //TakeDamage(20);
        }
    }


    public virtual void TakeDamage(float damage)
    {
        //Debug.Log("Take Damage");
        if (damage - armor > 0 && !Immotal)
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
        if (HP+Heal >= Max_HP)
        {
            HP=Max_HP;
        }
        else{
            HP += Heal;
        }
    }

    private void Death()
    {

    }

    public float GetHP()
    {
        return HP;
    }

    public void SetHP(float HP) 
    {
        this.HP = HP;
    }

    public float GetArmor() 
    {
        return armor;
    }
}
