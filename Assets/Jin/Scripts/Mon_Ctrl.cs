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

    string objname;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();        
    }

    private void OnEnable()
    {
        SetTarget();

        if (targetPos != null || testTargetPos != null)
        {
            StartCoroutine(Move());   
        }
    }
    public void SetTarget()
    {
        targetPos = FindFirstObjectByType<PlayerController>().transform;

        if (targetPos != null)
        {
            Debug.Log("target_Set");
            targetPos.position = new Vector3(targetPos.position.x, targetPos.position.y, this.transform.position.z);
        }
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
            //Damage 
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject);
        }
    }
}
