using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class MonSpawn_Mgr : MonoBehaviour
{
    public static MonSpawn_Mgr instance;

    [SerializeField] Renderer field;
    float fieldSize;

    [SerializeField] List<GameObject> objList = new List<GameObject>();

    Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    int poolSize = 50;

    Vector3 curPos;

    Vector3 targetPos;

    float spawnNum;

    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }
    void Start()
    {
        curPos = transform.position;

        targetPos = FindFirstObjectByType<PlayerController>().transform.position;

        foreach (GameObject obj in objList)
        {
            pools[obj.name] = new Queue<GameObject>();

            GameObject parentPool = new GameObject($"{obj.name}_Pool");
            parentPool.transform.SetParent(this.transform);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = Instantiate(obj, parentPool.transform);
                go.SetActive(false);
                pools[obj.name].Enqueue(go);
            }
        }

        fieldSize = field.bounds.size.x;
        float fieldSizeZ = field.bounds.size.z;
        this.transform.position = new Vector3(-(fieldSize / 2)+0.5f, 2.5f, -(fieldSizeZ/2)+5);
        Debug.Log("field" + fieldSize);
        StartCoroutine(SpawnMon(0));
    }

    public GameObject GetObject(string name, Vector3 _pos)
    {
        if (!pools.ContainsKey(name))
        {
            Debug.Log("null");
            return null;
        }

        if (pools[name].Count > 0)
        {
            //Debug.Log($"name : {name}");
            GameObject go = pools[name].Dequeue();

            go.transform.position = _pos;
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject go = Instantiate(objList.Find(obj => obj.name == name));
            go.transform.position = _pos;
            return go;
        }

        
    }

    public void ReturnObject(string name, GameObject go)
    {
        Debug.Log("Return");
        if (!pools.ContainsKey(name))
        {
            Destroy(go);
            return;
        }
        go.SetActive(false);
        pools[name].Enqueue(go);
    }

    IEnumerator SpawnMon(int _num)
    {
        BoxCollider moncoll = objList[_num].GetComponent<BoxCollider>();
        if(moncoll == null)
            Debug.Log("null");  
        float monSize = moncoll.size.x * Mathf.Abs(objList[_num].transform.lossyScale.x);

        Debug.Log("Monsize : " + monSize + ", Moncoll : " + moncoll.size.x);
        int spawnCount = Mathf.FloorToInt(fieldSize / (monSize * 1.5f)) -1;

        WaitForSeconds wait = new WaitForSeconds(1.5f);

        while (true)
        {
            yield return wait;

            Debug.Log("why2" + spawnCount);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos = new Vector3(curPos.x + i * (monSize * 1.5f),curPos.y,curPos.z);
                GameObject zombie = GetObject("zombie", spawnPos);

                if (zombie == null)
                    continue;

                

                //zombie.transform.position = spawnPos;
            }
        }
    }
}
