using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mon_Ctrl : MonoBehaviour
{
    Mon_Data data;
    Transform targetPos;
    [SerializeField] Transform testTargetPos;
    //State Machine

    NavMeshAgent agent;
    bool arrive = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        SetTarget();

        if (targetPos != null || testTargetPos != null)
        {
            StartCoroutine(Move());   
        }
    }


    public void SetTarget()
    {
        targetPos = FindFirstObjectByType<PlayerController>().transform;
    }

    IEnumerator Move()
    {
        while(true)
        {
            //agent.SetDestination(targetPos.position);
            agent.SetDestination(testTargetPos.position);

            if(arrive)
                yield break;

            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Goal"))
        {
            arrive = true;

            MonSpawn_Mgr.instance.ReturnObject(this.gameObject);

        }
    }
}
