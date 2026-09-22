using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonSpawn_Mgr : MonoBehaviour
{
    public static MonSpawn_Mgr instance;

    [Header("Field")]
    [SerializeField] private Renderer field;

    [Header("Monster Prefabs")]
    [SerializeField]
    private List<GameObject> objList =
        new List<GameObject>();

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 50;

    private readonly Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private readonly Dictionary<string, Transform> poolParents = new Dictionary<string, Transform>();

    private PlayerController player;
    private PlayerExperience playerExp;
    private Transform target;

    private float fieldSize;
    private Vector3 spawnStartPosition;

    // 프리팹 생성 중 OnEnable 실행을 막기 위한 비활성 부모
    private Transform inactiveCreateRoot;

    float timeLv = 1;
    WaitForSeconds hpTimer = new WaitForSeconds(30);
    int prefabLv = 0;

    float spawnTimer = 1.5f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private IEnumerator Start()
    {
        if (field == null)
        {
            Debug.LogError("MonSpawn_Mgr에 field Renderer가 할당되지 않았어.",this);

            yield break;
        }

        if (objList == null || objList.Count == 0)
        {
            Debug.LogError(
                "objList에 몬스터 프리팹이 없어.",
                this
            );

            yield break;
        }

        // 동적으로 생성되는 플레이어를 기다림
        yield return FindPlayer();

        if (player == null)
        {
            Debug.LogError(
                "PlayerController를 찾지 못했어.",
                this
            );

            yield break;
        }

        target = player.transform;

        playerExp =
            player.GetComponent<PlayerExperience>();

        if (playerExp == null)
        {
            playerExp =
                FindFirstObjectByType<PlayerExperience>();
        }

        if (playerExp == null)
        {
            Debug.LogError(
                "PlayerExperience를 찾지 못했어.",
                this
            );

            yield break;
        }

        InitializeField();
        InitializePools();


        StartCoroutine(SpawnMon(0));
        StartCoroutine(CheckTime());
    }

    private IEnumerator FindPlayer()
    {
        WaitForSeconds wait =
            new WaitForSeconds(0.1f);

        while (player == null)
        {
            player =
                FindFirstObjectByType<PlayerController>();

            if (player == null)
            {
                yield return wait;
            }
        }

        Debug.Log(
            $"플레이어 탐색 완료: {player.name}",
            player
        );
    }

    private void InitializeField()
    {
        Bounds fieldBounds = field.bounds;

        fieldSize = fieldBounds.size.x;

        // 필드의 왼쪽 바깥쪽부터 몬스터가 생성되도록 설정
        spawnStartPosition = new Vector3(
            fieldBounds.min.x,
            2.5f,
            fieldBounds.max.z + 5f
        );

        transform.position = spawnStartPosition;

        Debug.Log(
            $"fieldSize: {fieldSize}, " +
            $"spawnStartPosition: {spawnStartPosition}"
        );
    }

    private void InitializePools()
    {
        // 생성 중 OnEnable 방지용 비활성 오브젝트
        GameObject inactiveRootObject =
            new GameObject("Inactive_Create_Root");

        inactiveRootObject.transform.SetParent(transform);
        inactiveRootObject.SetActive(false);

        inactiveCreateRoot =
            inactiveRootObject.transform;

        foreach (GameObject prefab in objList)
        {
            if (prefab == null)
            {
                Debug.LogError(
                    "objList에 null 프리팹이 있어.",
                    this
                );

                continue;
            }

            string poolName = prefab.name;

            if (pools.ContainsKey(poolName))
            {
                Debug.LogWarning(
                    $"중복된 프리팹 이름이야: {poolName}",
                    prefab
                );

                continue;
            }

            pools.Add(
                poolName,
                new Queue<GameObject>()
            );

            GameObject parentPool =
                new GameObject($"{poolName}_Pool");

            parentPool.transform.SetParent(transform);

            poolParents.Add(
                poolName,
                parentPool.transform
            );

            for (int i = 0; i < poolSize; i++)
            {
                GameObject go =
                    CreatePoolObject(
                        poolName,
                        prefab
                    );

                pools[poolName].Enqueue(go);
            }
        }
    }

    IEnumerator CheckTime()
    {
        while (true)
        {
            yield return hpTimer;

            if (timeLv <= 5.0f)
            {
                timeLv += 0.1f;
                spawnTimer -= 0.025f;
            }
            else
            {
                break;
            }
        }
    }
    private GameObject CreatePoolObject(string poolName,GameObject prefab)
    {
        /*
         * 비활성 부모 밑에서 생성하기 때문에
         * Instantiate 순간 OnEnable이 호출되지 않음
         */
        GameObject go =
            Instantiate(
                prefab,
                inactiveCreateRoot
            );

        // (Clone)을 제거하고 풀 키와 이름을 통일
        go.name = poolName;

        go.SetActive(false);

        if (poolParents.TryGetValue(
                poolName,
                out Transform poolParent))
        {
            go.transform.SetParent(poolParent);
        }

        Mon_Ctrl monCtrl =
            go.GetComponent<Mon_Ctrl>();

        if (monCtrl == null)
        {
            Debug.LogError(
                $"{poolName} 프리팹에 Mon_Ctrl이 없어.",
                go
            );
        }

        return go;
    }

    public GameObject GetObject(
        string poolName,
        Vector3 spawnPosition)
    {
        if (!pools.TryGetValue(
                poolName,
                out Queue<GameObject> pool))
        {
            Debug.LogError(
                $"존재하지 않는 풀 이름이야: {poolName}",
                this
            );

            return null;
        }

        GameObject go;

        if (pool.Count > 0)
        {
            go = pool.Dequeue();
        }
        else
        {
            GameObject prefab =
                objList.Find(
                    obj => obj != null &&
                           obj.name == poolName
                );

            if (prefab == null)
            {
                Debug.LogError(
                    $"{poolName} 프리팹을 찾지 못했어.",
                    this
                );

                return null;
            }

            go = CreatePoolObject(
                poolName,
                prefab
            );
        }

        Mon_Ctrl monCtrl =
            go.GetComponent<Mon_Ctrl>();

        if (monCtrl == null)
        {
            Debug.LogError(
                $"{go.name}에 Mon_Ctrl이 없어.",
                go
            );

            go.SetActive(false);
            pool.Enqueue(go);

            return null;
        }

        // 순서가 중요함
        go.transform.position = spawnPosition;
        monCtrl.SetTarget(target);
        
        go.SetActive(true);
        
        monCtrl.SetHP(timeLv);

        return go;
    }

    public void ReturnObject(
        string poolName,
        GameObject go,
        int exp)
    {
        if (go == null)
            return;

        if (playerExp != null)
        {
            playerExp.AddExp(exp);
        }
        else
        {
            Debug.LogWarning(
                "PlayerExperience가 null이라 경험치를 지급하지 못했어.",
                this
            );
        }

        if (!pools.TryGetValue(
                poolName,
                out Queue<GameObject> pool))
        {
            Debug.LogError(
                $"반환할 풀이 존재하지 않아: {poolName}",
                go
            );

            Destroy(go);
            return;
        }

        go.SetActive(false);

        if (poolParents.TryGetValue(
                poolName,
                out Transform poolParent))
        {
            go.transform.SetParent(poolParent);
        }

        pool.Enqueue(go);
    }

    private IEnumerator SpawnMon(int prefabIndex)
    {
        if (prefabIndex < 0 ||
            prefabIndex >= objList.Count)
        {
            Debug.LogError(
                $"잘못된 objList 인덱스: {prefabIndex}",
                this
            );

            yield break;
        }

        GameObject monsterPrefab =
            objList[prefabIndex];

        if (monsterPrefab == null)
        {
            Debug.LogError(
                "몬스터 프리팹이 null이야.",
                this
            );

            yield break;
        }

        BoxCollider monsterCollider =
            monsterPrefab.GetComponent<BoxCollider>();

        if (monsterCollider == null)
        {
            Debug.LogError(
                $"{monsterPrefab.name}에 BoxCollider가 없어.",
                monsterPrefab
            );

            yield break;
        }

        float monsterSize =
            monsterCollider.size.x *
            Mathf.Abs(
                monsterPrefab.transform.lossyScale.x
            );

        if (monsterSize <= 0f)
        {
            Debug.LogError(
                $"몬스터 크기가 잘못됐어: {monsterSize}",
                monsterPrefab
            );

            yield break;
        }

        float spawnSpacing =
            monsterSize * 1.5f;

        int spawnCount =
            Mathf.FloorToInt(
                fieldSize / spawnSpacing
            );

        if (spawnCount <= 0)
        {
            Debug.LogError(
                $"spawnCount가 0 이하야. " +
                $"fieldSize: {fieldSize}, " +
                $"monsterSize: {monsterSize}",
                this
            );

            yield break;
        }

        Debug.Log(
            $"monsterSize: {monsterSize}, " +
            $"spawnSpacing: {spawnSpacing}, " +
            $"spawnCount: {spawnCount}"
        );

        WaitForSeconds wait =
            new WaitForSeconds(spawnTimer);

        while (true)
        {
            yield return wait;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition =
                    new Vector3(
                        spawnStartPosition.x +
                        i * spawnSpacing,

                        spawnStartPosition.y,
                        spawnStartPosition.z
                    );

                GameObject monster =
                    GetObject(
                        monsterPrefab.name,
                        spawnPosition
                    );

                if (monster == null)
                {
                    Debug.LogWarning(
                        $"{monsterPrefab.name} 생성 실패",
                        this
                    );
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}