using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

public class Mon_Ctrl : MonoBehaviour
{
    [SerializeField]Mon_Data data;
    Transform target;
    Vector3 targetPos;
    //[SerializeField] Transform testTargetPos;
    //State Machine

    NavMeshAgent agent;
    bool arrive = false;

    string objname;
    float hp;

    public float MaxHP
    {
        get {  return hp; } 
    }
    void Start()
    {
        
    }

    void SetMon()
    {
        objname = data.monName;
        

        agent = GetComponent<NavMeshAgent>();   

        agent.speed = data.walkSpeed;

    }
    private void OnEnable()
    {
        SetMon();
        SetTarget();

        if (targetPos != null)
        {
            StartCoroutine(Move());   
        }
    }
    public void SetTarget()
    {
        target = FindFirstObjectByType<PlayerController>().transform;


        if (targetPos != null)
        {
            //Debug.Log("target_Set");
            targetPos = new Vector3(this.transform.position.x, target.position.y, target.position.z);
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
            Debug.Log("Damage Call");
            //
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject);
        }
    }

    public void TakeDamage(float _damage)
    {
        hp -= _damage;

        if(hp < 0)
        {
            Debug.Log(name + "mon dead");
            MonSpawn_Mgr.instance.ReturnObject(objname, this.gameObject);
        }
    }
}
