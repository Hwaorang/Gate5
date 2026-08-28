using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquadManager : MonoBehaviour
{
    [Header("병사 설정")]

    // 병사를 생성/반환할 오브젝트 풀
    [SerializeField] private SoldierPool soldierPool;

    // 플레이어 기본 스탯 정보
    [SerializeField] private PlayerStats playerStats;

    // 병사 간격
    [SerializeField] private float spacing = 1.2f;

    // 현재 사용 중인 병사 목록
    private List<GameObject> soldiers = new List<GameObject>();

    // 현재 병사 수
    public int CurrentCount => soldiers.Count;

    // 현재 공격력 배율
    // 1.0 = 기본 공격력
    private float damageMultiplier = 1f;

    // 현재 공격속도 배율
    // 1.0 = 기본 공격속도
    private float attackSpeedMultiplier = 1f;

    private void Start()
    {
        // PlayerStats에 설정된 시작 병사 수만큼 생성
        AddUnit(playerStats.StartSoldierCount);
    }

    private void Update()
    {
        // 테스트용 병사 추가
        // 나중에 제거 가능
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AddUnit(1);
        }
    }

    /// <summary>
    /// 풀에서 병사를 가져와 현재 분대에 추가한다.
    /// </summary>
    public void AddUnit(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject soldier =
                soldierPool.GetSoldier(transform);

            soldiers.Add(soldier);

            // Soldier가 자신을 관리하는 SquadManager를 알도록 설정
            SoldierUnit soldierUnit =
                soldier.GetComponent<SoldierUnit>();

            if (soldierUnit != null)
            {
                soldierUnit.Init(this);
            }

            // 현재 강화 상태를 새 Soldier에게도 적용
            SoldierAttack soldierAttack =
                soldier.GetComponent<SoldierAttack>();

            if (soldierAttack != null)
            {
                soldierAttack.SetDamageMultiplier(
                    damageMultiplier
                );

                soldierAttack.SetAttackSpeedMultiplier(
                    attackSpeedMultiplier
                );
            }
        }

        UpdateFormation();
    }

    /// <summary>
    /// 지정된 병사를 현재 분대에서 제거하고 풀에 반환한다.
    /// </summary>
    public void RemoveUnit(SoldierUnit soldier)
    {
        if (soldier == null)
        {
            return;
        }

        GameObject soldierObject = soldier.gameObject;

        // 리스트에 존재하는 Soldier인지 확인 후 제거
        bool removed = soldiers.Remove(soldierObject);

        if (!removed)
        {
            Debug.LogWarning(
                $"{soldierObject.name}을 병사 리스트에서 찾지 못했습니다."
            );

            return;
        }

        // Destroy 대신 오브젝트 풀로 반환
        soldierPool.ReturnSoldier(soldierObject);

        // 남아있는 병사 재정렬
        UpdateFormation();

        CheckGameOver();
    }

    /// <summary>
    /// 현재 병사들을 3열 형태로 정렬한다.
    /// </summary>
    private void UpdateFormation()
    {
        int columnCount = 3;

        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / columnCount;
            int column = i % columnCount;

            float x =
                (column - 1) * spacing;

            float z =
                -row * spacing;

            soldiers[i].transform.localPosition =
                new Vector3(x, 0f, z);
        }
    }

    /// <summary>
    /// 병사가 모두 사망했는지 확인한다.
    /// </summary>
    private void CheckGameOver()
    {
        if (soldiers.Count <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    /// <summary>
    /// 모든 병사의 공격력을 증가시킨다.
    /// </summary>
    public void UpgradeAllSoldierDamage(float percent)
    {
        // 예:
        // 10% 증가
        // 1.0 -> 1.1 -> 1.2
        damageMultiplier += percent;

        foreach (GameObject soldierObject in soldiers)
        {
            SoldierAttack soldierAttack =
                soldierObject.GetComponent<SoldierAttack>();

            if (soldierAttack != null)
            {
                soldierAttack.SetDamageMultiplier(
                    damageMultiplier
                );
            }
        }

        Debug.Log(
            $"현재 공격력 배율 : {damageMultiplier}"
        );
    }

    /// <summary>
    /// 모든 병사의 공격속도를 증가시킨다.
    /// </summary>
    public void UpgradeAllSoldierAttackSpeed(float percent)
    {
        attackSpeedMultiplier += percent;

        foreach (GameObject soldierObject in soldiers)
        {
            SoldierAttack soldierAttack =
                soldierObject.GetComponent<SoldierAttack>();

            if (soldierAttack != null)
            {
                soldierAttack.SetAttackSpeedMultiplier(
                    attackSpeedMultiplier
                );
            }
        }

        Debug.Log(
            $"현재 공격속도 배율 : {attackSpeedMultiplier}"
        );
    }
}