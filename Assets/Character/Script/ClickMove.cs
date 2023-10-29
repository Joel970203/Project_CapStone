using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClickMove : MonoBehaviour
{

    NavMeshAgent agent;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        agent=GetComponent<NavMeshAgent>();
        anim=GetComponent<Animator>();

        agent.speed = 16.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray,out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
                anim.SetBool("Walk",true);
            }
        }
        else if(agent.remainingDistance<0.1f)
        {
            anim.SetBool("Walk",false);
        }
    }
}
