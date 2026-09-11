using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mon_Ctrl : MonoBehaviour
{
    [SerializeField] Mon_Data data;
    Transform targetPos;
    //[SerializeField] Transform testTargetPos;
    //State Machine

    NavMeshAgent agent;
    bool arrive = false;

    string objname;
    float curHP;
    float damage;
    int exp;

    bool isDead = false;

    
    void Start()
    {
               
    }

    void SetState()
    {
        curHP = data.maxHp;
        agent.speed = data.walkSpeed;
        damage = data.damage;
        objname = data.monName;
        exp = data.exp;
        isDead = false;
    }
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>(); 
        SetState();

        SetTarget();
        
        if (targetPos != null)
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
            targetPos.position = new Vector3(this.transform.position.x, targetPos.position.y, targetPos.position.z);
        }
    }

    IEnumerator Move()
    {
        while(true)
        {
            agent.SetDestination(targetPos.position);
            //agent.SetDestination(testTargetPos.position);

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
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject, 0);
        }
    }

    public void TakeDamage(float _damage)
    {
        curHP -= _damage;

        if (curHP <= 0 && !isDead)
        {
            if(!isDead)
                isDead = true;
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject, exp);
        }
    }
}
