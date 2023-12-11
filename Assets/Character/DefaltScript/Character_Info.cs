using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Info : MonoBehaviour
{
    [SerializeField] public float Max_HP;
    [SerializeField] public float HP;

    [SerializeField] private int AD;
    public bool preventHpDrop;
    [SerializeField] private float armor;

    [SerializeField] private GameObject hitEffectPrefab;

    public Image HealthGlobe;

    public bool Immotal;

    public GameObject Grave;

    public CapsuleCollider capsuleCollider;

    void Start()
    {
        HP=Max_HP;
        Immotal=false;
        if(preventHpDrop)
        {
            StartCoroutine(PreventHpDropCoroutine(5f));
        }
        capsuleCollider=GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(HealthGlobe!=null)
        {
        float ratio = HP / Max_HP;
		HealthGlobe.rectTransform.localPosition = new Vector3(0, HealthGlobe.rectTransform.rect.height * ratio - HealthGlobe.rectTransform.rect.height, 0);
        }
        if(preventHpDrop)
        {
            StartCoroutine(PreventHpDropCoroutine(5f));
        }
        capsuleCollider.enabled = true;
    }

    private IEnumerator PreventHpDropCoroutine(float duration)
    {
        // 5초 동안 HP가 1 이하로 내려가지 않도록 함
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            if (HP <= 1f)
            {
                HP = Mathf.Max(1f, HP); // HP가 1 이하로 내려가지 않도록 함
            }
            preventHpDrop=false;
            yield return null;
        }
    }
    //보스에게 근접 공격 피격시 작동하는 이펙트
    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("BossAttack") && !Immotal)
        {
            //Vector3 characterCenter = transform.position; // 캐릭터 중심점
            //Vector3 BossCenter = other.gameObject.transform.position; // 적의 중심점
            TakeDamage(20);
        }

        if (other.gameObject.CompareTag("Heal"))
        {
            if (other.gameObject.GetComponent<SkillHealAmount>())
            {
                TakeHeal((int)(other.gameObject.GetComponent<SkillHealAmount>().HealAmount));
            }
                else
            {
                TakeHeal(20);
                other.enabled = false;
            }
            other.enabled = false;
        }
 

        if(other.gameObject.CompareTag("Unbeatable"))
        {
            Immotal=true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Heal"))
        {
            if (other.gameObject.GetComponent<SkillHealAmount>())
            {
                TakeHeal((int)(other.gameObject.GetComponent<SkillHealAmount>().HealAmount));
                other.enabled = false;
            }
            else
            {
                TakeHeal(20);
                other.enabled = false;
            }
        }


        if(other.gameObject.CompareTag("Unbeatable"))
        {
            Immotal=true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Unbeatable"))
        {
            Immotal=false;
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

    public void SetArmor(int value) 
    {
        armor+=value;
    }

    public void IncreaseArmor(float value)
    {
        Debug.Log("잉크리즈 아머 "+value);
        StartCoroutine(ArmorPlusTemporary(value));
    }

    IEnumerator ArmorPlusTemporary(float delay)
    {
        SetArmor(10);
        yield return new WaitForSeconds(10);
        SetArmor(-10);
        yield break;
    }

    private void Death()
    {
        /*GameObject CharacterGrave = Instantiate(Grave, this.transform.position, Quaternion.identity);
        CharacterGrave.GetComponent<RivivalCharacter>().GameCharacter = this.gameObject;
        this.gameObject.SetActive(false);*/
    }
}
