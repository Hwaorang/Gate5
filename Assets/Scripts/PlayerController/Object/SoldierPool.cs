using System.Collections.Generic;
using UnityEngine;

public class SoldierPool : MonoBehaviour
{
    [Header("병사 풀 설정")]
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private int initialSize = 20;

    // 사용하지 않는 병사들을 보관
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        CreatePool();
    }

    /// <summary>
    /// 게임 시작 시 병사를 미리 생성해둔다.
    /// </summary>
    private void CreatePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewSoldier();
        }
    }

    /// <summary>
    /// 새로운 병사를 생성해서 풀에 넣는다.
    /// </summary>
    private void CreateNewSoldier()
    {
        GameObject soldier = Instantiate(
            soldierPrefab,
            transform
        );

        soldier.SetActive(false);

        pool.Enqueue(soldier);
    }

    /// <summary>
    /// 풀에서 병사를 하나 가져온다.
    /// </summary>
    public GameObject GetSoldier(Transform parent)
    {
        // 풀이 비어있으면 새 병사 생성
        if (pool.Count <= 0)
        {
            CreateNewSoldier();
        }

        GameObject soldier = pool.Dequeue();

        // SquadManager 아래로 이동
        soldier.transform.SetParent(parent);

        soldier.SetActive(true);

        return soldier;
    }

    /// <summary>
    /// 사용이 끝난 병사를 풀에 반환한다.
    /// </summary>
    public void ReturnSoldier(GameObject soldier)
    {
        soldier.SetActive(false);

        // 다시 Pool 오브젝트 아래로 이동
        soldier.transform.SetParent(transform);

        pool.Enqueue(soldier);
    }
}