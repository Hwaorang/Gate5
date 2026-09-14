using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mon_Ctrl : MonoBehaviour
{
    [SerializeField] Mon_Data data;
    Transform target;
    Vector3 targetPos;
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

        //SetTarget();
        
        if (!isDead)
        {
            StartCoroutine(Move());   
        }
    }
    public void SetTarget(Transform _target)
    {
        //target = FindFirstObjectByType<PlayerController>().transform;

        if (_target != null)
        {
            //targetPos = _target.position;
            
            targetPos = new Vector3(this.transform.position.x, _target.position.y, _target.position.z);
            Debug.Log("target_Set : " + targetPos);
        }
    }

    IEnumerator Move()
    {
        while(true)
        {
            agent.SetDestination(targetPos);
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
        Debug.Log("Take Damage");
        if (curHP <= 0 && !isDead)
        {
            if(!isDead)
                isDead = true;
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject, exp);
        }
    }
}
