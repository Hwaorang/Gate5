using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonSpawn_Mgr : MonoBehaviour
{
    public static MonSpawn_Mgr instance;

    [SerializeField] List<GameObject> objList = new List<GameObject>();

    Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    int poolSize = 10;

    Vector3 curPos;

    Vector3 targetPos;
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
    }

    public GameObject GetObject(string name)
    {
        if (!pools.ContainsKey(name))
        {
            Debug.Log("null");
            return null;
        }

        if (pools[name].Count > 0)
        {
            Debug.Log($"name : {name}");
            GameObject go = pools[name].Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject go = Instantiate(objList.Find(obj => obj.name == name));
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

    IEnumerator SpawnMon()
    {
        while (true)
        {

            yield return null;
        }
    }
}
