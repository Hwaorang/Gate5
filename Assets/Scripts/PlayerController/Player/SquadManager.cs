using System;
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

    [Header("대형 설정")]
    // 병사 간격
    [SerializeField] private float spacing = 1.2f;
    [SerializeField] private int maxColumnCount = 7;

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

    // 현재 투사체 강화 단계
    // 0 = 기본 상태
    private int projectileUpgradeLevel = 0;

    //공격타이머
    [SerializeField] private int shotsPerFrame = 20;
    private float attackTimer;
    private bool isFiring;
    private int fireIndex;

    [SerializeField]
    private BulletPool bulletPool;

    private readonly List<SoldierAttack> soldierAttacks = new();

    public IReadOnlyList<GameObject> Soldiers => soldiers;

    public event Action<int> OnSoldierCountChanged;

    private void Start()
    {
        // PlayerStats에 설정된 시작 병사 수만큼 생성
        AddUnit(playerStats.StartSoldierCount);
    }

    private void Update()
    {
        HandleAttackTimer();
        HandleDistributedFire();

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

            SoldierUnit unit =
                soldier.GetComponent<SoldierUnit>();

            if (unit != null)
            {
                unit.Init(this);
            }

            SoldierAttack attack =
                soldier.GetComponent<SoldierAttack>();

            if (attack != null)
            {
                attack.SetDamageMultiplier(
                    damageMultiplier
                );

                attack.SetAttackSpeedMultiplier(
                    attackSpeedMultiplier
                );

                attack.SetProjectileUpgradeLevel(
                    projectileUpgradeLevel
                );

                attack.SetBulletPool(bulletPool);

                // 처음 한 번만 저장
                soldierAttacks.Add(attack);
            }
        }

        UpdateFormation();

        OnSoldierCountChanged?.Invoke(CurrentCount);
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

        GameObject soldierObject =
            soldier.gameObject;

        bool removed =
            soldiers.Remove(soldierObject);

        if (!removed)
        {
            return;
        }

        SoldierAttack attack =
            soldierObject.GetComponent<SoldierAttack>();

        if (attack != null)
        {
            soldierAttacks.Remove(attack);
        }

        soldierPool.ReturnSoldier(soldierObject);

        UpdateFormation();

        OnSoldierCountChanged?.Invoke(CurrentCount);

        CheckGameOver();
    }

    /// <summary>
    /// 현재 병사들을 3열 형태로 정렬한다.
    /// </summary>
    private void UpdateFormation()
    {
        if (soldiers.Count == 0)
        {
            return;
        }

        int columnCount =
            Mathf.CeilToInt(Mathf.Sqrt(soldiers.Count));

        columnCount =
            Mathf.Min(columnCount, maxColumnCount);

        for (int i = 0; i < soldiers.Count; i++)
        {
            int row = i / columnCount;
            int column = i % columnCount;

            int rowStartIndex =
                row * columnCount;

            int remainingSoldiers =
                soldiers.Count - rowStartIndex;

            int soldiersInThisRow =
                Mathf.Min(
                    columnCount,
                    remainingSoldiers
                );

            float xOffset =
                (soldiersInThisRow - 1)
                * spacing
                * 0.5f;

            float x =
                column * spacing - xOffset;

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
#if UNITY_EDITOR
        Debug.Log(
            $"현재 공격속도 배율 : {attackSpeedMultiplier}"
        );
#endif
    }

    public void UpgradeAllSoldierProjectile()
    {
        projectileUpgradeLevel++;

        foreach (GameObject soldierObject in soldiers)
        {
            if (soldierObject == null)
            {
                continue;
            }

            SoldierAttack soldierAttack =
                soldierObject.GetComponent<SoldierAttack>();

            if (soldierAttack != null)
            {
                soldierAttack.SetProjectileUpgradeLevel(
                    projectileUpgradeLevel
                );
            }
        }
    }

    public void RemoveUnits(int amount)
    {
        // 현재 병사 수보다 많이 제거하려고 해도
        // 실제 존재하는 병사까지만 제거
        int removeCount =
            Mathf.Min(amount, soldiers.Count);

        for (int i = 0; i < removeCount; i++)
        {
            // 뒤쪽 병사부터 제거
            SoldierUnit soldier =
                soldiers[soldiers.Count - 1]
                .GetComponent<SoldierUnit>();

            if (soldier != null)
            {
                RemoveUnit(soldier);
            }
        }
    }

    /// <summary>
    /// 현재 병사 중 한 명을 제거한다.
    /// </summary>
    public void RemoveOneSoldier()
    {
        if (soldiers.Count <= 0)
        {
            return;
        }

        GameObject soldierObject =
            soldiers[soldiers.Count - 1];

        SoldierUnit soldier =
            soldierObject.GetComponent<SoldierUnit>();

        if (soldier != null)
        {
            RemoveUnit(soldier);
        }
    }

    private void HandleAttackTimer()
    {
        if (soldierAttacks.Count == 0)
        {
            return;
        }

        // 이미 발사 중이면
        // 다음 공격 타이머를 시작하지 않음
        if (isFiring)
        {
            return;
        }

        attackTimer += Time.deltaTime;

        float attackDelay =
            soldierAttacks[0].AttackDelay;

        if (attackTimer < attackDelay)
        {
            return;
        }

        attackTimer = 0f;

        // 분산 발사 시작
        isFiring = true;
        fireIndex = 0;
    }

    private void HandleDistributedFire()
    {
        if (!isFiring)
        {
            return;
        }

        int firedThisFrame = 0;

        while (
            fireIndex < soldierAttacks.Count &&
            firedThisFrame < shotsPerFrame
        )
        {
            SoldierAttack attack =
                soldierAttacks[fireIndex];

            if (attack != null)
            {
                attack.Fire();
            }

            fireIndex++;
            firedThisFrame++;
        }

        // 모든 병사가 발사를 끝냈다면
        if (fireIndex >= soldierAttacks.Count)
        {
            isFiring = false;
        }
    }
}