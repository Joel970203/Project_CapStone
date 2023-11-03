using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Explosion;
    public float ExplosionTime;

    private bool flag=false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position=Vector3.Lerp(this.transform.position,this.transform.parent.position,Time.deltaTime*4f);

        ExplosionTime -= Time.deltaTime;

        if(ExplosionTime<=0&&!flag)
        {
            GameObject CastingEffet1 = Instantiate(Explosion, this.transform.position, Quaternion.identity);
            CastingEffet1.transform.parent=this.gameObject.transform;
            GameObject CastingEffet2 = Instantiate(Explosion, this.transform.position, Quaternion.identity);
            CastingEffet2.transform.parent=this.gameObject.transform;
            GameObject CastingEffet3 = Instantiate(Explosion, this.transform.position, Quaternion.identity);
            CastingEffet3.transform.parent=this.gameObject.transform;
            GameObject CastingEffet4 = Instantiate(Explosion, this.transform.position, Quaternion.identity);
            CastingEffet4.transform.parent=this.gameObject.transform;
            GameObject CastingEffet5 = Instantiate(Explosion, this.transform.position, Quaternion.identity);
            CastingEffet5.transform.parent=this.gameObject.transform;
            flag=true;
        }
    }
}
