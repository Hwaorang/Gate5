using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Soldier GameObject를 미리 생성해두고 재사용하는 Object Pool.
///
/// 주요 역할
/// - 게임 시작 시 병사를 미리 생성
/// - 필요할 때 비활성 병사를 꺼내서 사용
/// - 사용이 끝난 병사를 다시 Pool에 반환
///
/// Instantiate / Destroy 반복을 줄여
/// 병사 수가 많을 때 성능 부담을 줄인다.
/// </summary>
public class SoldierPool : MonoBehaviour
{
    [Header("병사 풀 설정")]

    // Pool에서 사용할 Soldier Prefab
    [SerializeField] private GameObject soldierPrefab;

    // 게임 시작 시 미리 생성할 병사 수
    [SerializeField] private int initialSize = 20;


    // 현재 사용하지 않는 병사들을 보관하는 Queue
    private readonly Queue<GameObject> pool =
        new Queue<GameObject>();


    private void Awake()
    {
        CreatePool();
    }


    /// <summary>
    /// 게임 시작 시 initialSize만큼
    /// 병사를 미리 생성해서 Pool에 넣는다.
    /// </summary>
    private void CreatePool()
    {
        if (soldierPrefab == null)
        {
            Debug.LogWarning(
                "[SoldierPool] SoldierPrefab이 연결되지 않았습니다."
            );

            return;
        }

        // 음수 값 방지
        int createCount =
            Mathf.Max(
                0,
                initialSize
            );

        for (int i = 0; i < createCount; i++)
        {
            CreateNewSoldier();
        }
    }


    /// <summary>
    /// 새로운 Soldier를 생성해서
    /// 비활성 상태로 Pool에 보관한다.
    /// </summary>
    private GameObject CreateNewSoldier()
    {
        if (soldierPrefab == null)
        {
            return null;
        }

        GameObject soldier =
            Instantiate(
                soldierPrefab,
                transform
            );

        // Pool 내부에서는 사용하지 않는 상태이므로 비활성화
        soldier.SetActive(false);

        // Queue에 보관
        pool.Enqueue(
            soldier
        );

        return soldier;
    }


    /// <summary>
    /// Pool에서 Soldier 하나를 가져온다.
    ///
    /// Pool이 비어 있으면 새로운 Soldier를 생성한 뒤 사용한다.
    /// </summary>
    public GameObject GetSoldier(
        Transform parent)
    {
        // 사용할 병사가 없다면 하나 추가 생성
        if (pool.Count <= 0)
        {
            CreateNewSoldier();
        }


        // 생성 실패 등으로 여전히 비어 있다면 종료
        if (pool.Count <= 0)
        {
            return null;
        }


        GameObject soldier =
            pool.Dequeue();


        // 현재 SquadManager 아래로 이동
        soldier.transform.SetParent(
            parent,
            false
        );


        // 사용 가능한 상태로 활성화
        soldier.SetActive(true);


        return soldier;
    }


    /// <summary>
    /// 사용이 끝난 Soldier를 Pool에 반환한다.
    ///
    /// 직접 Destroy하지 않고 비활성화해서
    /// 이후 다시 재사용한다.
    /// </summary>
    public void ReturnSoldier(
        GameObject soldier)
    {
        if (soldier == null)
        {
            return;
        }


        // 이미 Pool에 들어가 있는 비활성 Soldier가
        // 중복 반환되는 상황을 방지
        if (!soldier.activeSelf &&
            soldier.transform.parent == transform)
        {
            return;
        }


        // 사용 중지
        soldier.SetActive(false);


        // Pool 오브젝트 아래로 다시 이동
        soldier.transform.SetParent(
            transform,
            false
        );


        // 다시 Queue에 보관
        pool.Enqueue(
            soldier
        );
    }
}