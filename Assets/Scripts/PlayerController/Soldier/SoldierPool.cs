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
    [SerializeField]
    private GameObject soldierPrefab;

    // 게임 시작 시 미리 생성할 병사 수
    [SerializeField]
    private int initialSize = 20;


    // PlayerCharacterVisual이
    // Soldier와 동일한 Skin 원본을 사용할 수 있도록 제공한다.
    public GameObject SoldierPrefab =>
        soldierPrefab;


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

        int createCount =
            Mathf.Max(
                0,
                initialSize
            );

        for (int i = 0;
             i < createCount;
             i++)
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

        soldier.SetActive(false);

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
        if (pool.Count <= 0)
        {
            CreateNewSoldier();
        }


        if (pool.Count <= 0)
        {
            return null;
        }


        GameObject soldier =
            pool.Dequeue();


        soldier.transform.SetParent(
            parent,
            false
        );


        // SoldierVisualController.OnEnable()에서
        // 현재 SaveManager의 selectedSkin을 자동 적용한다.
        soldier.SetActive(true);


        return soldier;
    }


    /// <summary>
    /// 사용이 끝난 Soldier를 Pool에 반환한다.
    /// </summary>
    public void ReturnSoldier(
        GameObject soldier)
    {
        if (soldier == null)
        {
            return;
        }


        if (!soldier.activeSelf &&
            soldier.transform.parent == transform)
        {
            return;
        }


        soldier.SetActive(false);


        soldier.transform.SetParent(
            transform,
            false
        );


        pool.Enqueue(
            soldier
        );
    }
}
