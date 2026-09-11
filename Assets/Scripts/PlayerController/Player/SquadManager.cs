using System;
using System.Collections.Generic;
using UnityEngine;
using GptAsset.HyperCasualBulletFX;

/// <summary>
/// 플레이어가 보유한 병사 분대를 전체적으로 관리한다.
///
/// 담당 역할
/// - 시작 병사 생성
/// - 병사 추가 / 제거
/// - 병사 대형 배치
/// - 병사 수 변경 이벤트 전달
/// - 공격력 / 공격속도 / 투사체 강화 상태 관리
/// - 병사가 0명이 되면 GameOver 처리
///
/// 실제 공격 타이밍과 Column 단위 공격 처리는
/// SquadAttackResolver가 담당한다.
///
/// 각 병사의 실제 Raycast / Bullet FX 처리는
/// SoldierAttack이 담당한다.
/// </summary>
public class SquadManager : MonoBehaviour
{
    // =========================
    // 병사 설정
    // =========================

    [Header("병사 설정")]

    // 병사를 Instantiate / Destroy하지 않고
    // Object Pool에서 가져오고 반환한다.
    [SerializeField]
    private SoldierPool soldierPool;

    // 시작 병사 수 등의 Player 기본 스탯
    [SerializeField]
    private PlayerStats playerStats;


    // =========================
    // 대형 설정
    // =========================

    [Header("대형 설정")]

    // 병사 사이의 간격
    [SerializeField]
    private float spacing = 1.2f;

    // 한 줄에 배치할 수 있는 최대 병사 수
    [SerializeField]
    private int maxColumnCount = 7;


    // =========================
    // 공격 기준점
    // =========================

    [Header("공격 기준점")]

    // Squad 전체가 공유하는 앞쪽 사격 기준점
    [SerializeField]
    private Transform fireLine;


    // =========================
    // 공격 연출
    // =========================

    [Header("공격 연출")]

    // 기존 시각용 Bullet Pool
    //
    // 현재 HyperCasualBulletFX를 주로 사용하지만
    // 투사체 크기 강화 방향이 확정될 때까지 유지한다.
    [SerializeField]
    private BulletPool bulletPool;

    // 총알 Tracer / Muzzle / Impact FX
    [SerializeField]
    private HyperCasualBulletFx bulletFx;


    // =========================
    // 병사 목록
    // =========================

    // 현재 활성화되어 있는 병사 목록
    private readonly List<GameObject> soldiers =
        new();

    // SoldierAttack을 별도로 캐싱한 목록
    //
    // SquadAttackResolver와 강화 시스템에서 사용한다.
    private readonly List<SoldierAttack> soldierAttacks =
        new();


    /// <summary>
    /// SquadAttackResolver가 현재 SoldierAttack 목록을
    /// 읽기 전용으로 사용할 수 있도록 제공한다.
    /// </summary>
    public IReadOnlyList<SoldierAttack> SoldierAttacks =>
        soldierAttacks;


    /// <summary>
    /// 현재 병사 GameObject 목록.
    /// </summary>
    public IReadOnlyList<GameObject> Soldiers =>
        soldiers;


    /// <summary>
    /// 현재 보유 병사 수.
    /// </summary>
    public int CurrentCount =>
        soldiers.Count;


    /// <summary>
    /// 현재 Squad 대형의 Column 수.
    ///
    /// SquadAttackResolver와 UpdateFormation에서
    /// 동일한 기준을 사용한다.
    /// </summary>
    public int CurrentColumnCount
    {
        get
        {
            if (soldiers.Count <= 0)
            {
                return 0;
            }

            int columnCount =
                Mathf.CeilToInt(
                    Mathf.Sqrt(soldiers.Count)
                );

            return Mathf.Min(
                columnCount,
                maxColumnCount
            );
        }
    }


    // =========================
    // 공격력 강화 상태
    // =========================

    // 로비 영구 공격력 배율
    private float lobbyDamageMultiplier = 1f;

    // 인게임 공격력 배율
    private float inGameDamageMultiplier = 1f;


    // =========================
    // 공격속도 강화 상태
    // =========================

    // 로비 영구 공격속도 배율
    private float lobbyAttackSpeedMultiplier = 1f;

    // 인게임 공격속도 배율
    private float inGameAttackSpeedMultiplier = 1f;


    // =========================
    // 투사체 강화 상태
    // =========================

    // 현재 투사체 강화 단계
    private int projectileUpgradeLevel;


    /// <summary>
    /// 로비 × 인게임 공격력 최종 배율.
    /// </summary>
    private float FinalDamageMultiplier =>
        lobbyDamageMultiplier *
        inGameDamageMultiplier;


    /// <summary>
    /// 로비 × 인게임 공격속도 최종 배율.
    /// </summary>
    private float FinalAttackSpeedMultiplier =>
        lobbyAttackSpeedMultiplier *
        inGameAttackSpeedMultiplier;


    // =========================
    // Events
    // =========================

    /// <summary>
    /// 병사 수 변경 알림.
    /// </summary>
    public event Action<int> OnSoldierCountChanged;


    /// <summary>
    /// 모든 병사가 사라졌을 때 발생하는 GameOver 이벤트.
    /// </summary>
    public event Action OnGameOver;


    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogWarning(
                "[SquadManager] PlayerStats가 연결되지 않았습니다."
            );

            return;
        }

        // 시작 병사 생성
        AddUnit(
            playerStats.StartSoldierCount
        );
    }


    // =========================================================
    // Soldier 생성
    // =========================================================

    /// <summary>
    /// 지정한 수만큼 병사를 추가한다.
    ///
    /// Pool에서 병사를 가져온 뒤
    /// 현재 Squad의 강화 상태와 공격 관련 참조를 전달한다.
    /// </summary>
    public void AddUnit(
        int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (soldierPool == null)
        {
            Debug.LogWarning(
                "[SquadManager] SoldierPool이 연결되지 않았습니다."
            );

            return;
        }


        for (int i = 0;
             i < amount;
             i++)
        {
            GameObject soldier =
                soldierPool.GetSoldier(
                    transform
                );

            if (soldier == null)
            {
                continue;
            }


            // =========================
            // Squad 등록
            // =========================

            soldiers.Add(
                soldier
            );


            // =========================
            // SoldierUnit 초기화
            // =========================

            SoldierUnit unit =
                soldier.GetComponent<SoldierUnit>();

            if (unit != null)
            {
                unit.Init(
                    this
                );
            }


            // =========================
            // SoldierAttack 초기화
            // =========================

            SoldierAttack attack =
                soldier.GetComponent<SoldierAttack>();

            if (attack == null)
            {
                continue;
            }


            // 현재까지 적용된 공격력 강화
            attack.SetDamageMultiplier(
                FinalDamageMultiplier
            );

            // 현재까지 적용된 공격속도 강화
            attack.SetAttackSpeedMultiplier(
                FinalAttackSpeedMultiplier
            );

            // 현재 투사체 강화 단계
            attack.SetProjectileUpgradeLevel(
                projectileUpgradeLevel
            );

            // 기존 Bullet Pool
            attack.SetBulletPool(
                bulletPool
            );

            // Hyper Casual Bullet FX
            attack.SetBulletFx(
                bulletFx
            );

            // Squad 공통 발사 기준점
            attack.SetFireLine(
                fireLine
            );


            // 공격 컴포넌트 캐싱
            soldierAttacks.Add(
                attack
            );
        }


        // 병사 수 변경 후 대형 갱신
        UpdateFormation();

        // UI 등에게 병사 수 변경 전달
        OnSoldierCountChanged?.Invoke(
            CurrentCount
        );
    }


    // =========================================================
    // Soldier 제거
    // =========================================================

    /// <summary>
    /// SoldierUnit이 Death 처리를 완료한 뒤
    /// 실제 Squad에서 제거한다.
    /// </summary>
    public void RemoveUnit(
        SoldierUnit soldier)
    {
        if (soldier == null)
        {
            return;
        }

        GameObject soldierObject =
            soldier.gameObject;


        // 이미 제거된 병사라면 중복 처리하지 않는다.
        if (!soldiers.Remove(
            soldierObject))
        {
            return;
        }


        // =========================
        // 공격 목록에서도 제거
        // =========================

        SoldierAttack attack =
            soldierObject.GetComponent<SoldierAttack>();

        if (attack != null)
        {
            soldierAttacks.Remove(
                attack
            );
        }


        // =========================
        // Pool 반환
        // =========================

        if (soldierPool != null)
        {
            soldierPool.ReturnSoldier(
                soldierObject
            );
        }


        // =========================
        // Squad 갱신
        // =========================

        UpdateFormation();

        OnSoldierCountChanged?.Invoke(
            CurrentCount
        );

        CheckGameOver();
    }


    /// <summary>
    /// 지정한 수만큼 병사를 사망 처리한다.
    ///
    /// 즉시 리스트에서 제거하지 않고
    /// SoldierUnit.Die()를 호출한다.
    /// </summary>
    public void RemoveUnits(
        int amount)
    {
        if (amount <= 0 ||
            soldiers.Count <= 0)
        {
            return;
        }

        int removeCount =
            Mathf.Min(
                amount,
                soldiers.Count
            );


        // Die()는 즉시 soldiers 리스트를 제거하지 않으므로
        // 뒤에서부터 서로 다른 Index를 선택한다.
        for (int i = 0;
             i < removeCount;
             i++)
        {
            int index =
                soldiers.Count - 1 - i;

            GameObject soldierObject =
                soldiers[index];

            if (soldierObject == null)
            {
                continue;
            }

            SoldierUnit soldier =
                soldierObject.GetComponent<SoldierUnit>();

            if (soldier != null)
            {
                soldier.Die();
            }
        }
    }


    /// <summary>
    /// 마지막 병사 한 명을 사망 처리한다.
    /// DamageLine 등에서 사용한다.
    /// </summary>
    public void RemoveOneSoldier()
    {
        if (soldiers.Count <= 0)
        {
            return;
        }

        GameObject soldierObject =
            soldiers[
                soldiers.Count - 1
            ];

        if (soldierObject == null)
        {
            return;
        }

        SoldierUnit soldier =
            soldierObject.GetComponent<SoldierUnit>();

        if (soldier != null)
        {
            soldier.Die();
        }
    }


    // =========================================================
    // Formation
    // =========================================================

    /// <summary>
    /// 현재 병사 수에 맞춰
    /// Squad 대형을 다시 배치한다.
    /// </summary>
    private void UpdateFormation()
    {
        if (soldiers.Count == 0)
        {
            return;
        }


        int columnCount =
            CurrentColumnCount;

        if (columnCount <= 0)
        {
            return;
        }


        for (int i = 0;
             i < soldiers.Count;
             i++)
        {
            int row =
                i / columnCount;

            int column =
                i % columnCount;


            int rowStartIndex =
                row *
                columnCount;

            int remainingSoldiers =
                soldiers.Count -
                rowStartIndex;

            int soldiersInThisRow =
                Mathf.Min(
                    columnCount,
                    remainingSoldiers
                );


            // 해당 행을 중앙 정렬하기 위한 Offset
            float xOffset =
                (soldiersInThisRow - 1) *
                spacing *
                0.5f;


            float x =
                column *
                spacing -
                xOffset;

            float z =
                -row *
                spacing;


            soldiers[i]
                .transform
                .localPosition =
                new Vector3(
                    x,
                    0f,
                    z
                );
        }
    }


    // =========================================================
    // GameOver
    // =========================================================

    /// <summary>
    /// 병사가 모두 사라졌다면
    /// GameOver 이벤트와 기존 GameManager 처리를 실행한다.
    /// </summary>
    private void CheckGameOver()
    {
        if (soldiers.Count > 0)
        {
            return;
        }

        // Observer Pattern
        OnGameOver?.Invoke();


        // 기존 프로젝트 GameOver 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }


    // =========================================================
    // Damage Upgrade
    // =========================================================

    /// <summary>
    /// 인게임 공격력 증가량을 누적한다.
    ///
    /// percent = 0.1 → +10%
    /// </summary>
    public void UpgradeAllSoldierDamage(
        float percent)
    {
        inGameDamageMultiplier +=
            percent;

        ApplyDamageMultiplierToAllSoldiers();
    }


    /// <summary>
    /// 현재 최종 공격력 배율을
    /// 모든 병사에게 적용한다.
    /// </summary>
    private void ApplyDamageMultiplierToAllSoldiers()
    {
        float finalMultiplier =
            FinalDamageMultiplier;


        for (int i = 0;
             i < soldierAttacks.Count;
             i++)
        {
            SoldierAttack attack =
                soldierAttacks[i];

            if (attack == null)
            {
                continue;
            }

            attack.SetDamageMultiplier(
                finalMultiplier
            );
        }
    }


    /// <summary>
    /// 로비 영구 공격력 배율을 설정한다.
    /// </summary>
    public void SetLobbyDamageMultiplier(
        float multiplier)
    {
        lobbyDamageMultiplier =
            Mathf.Max(
                1f,
                multiplier
            );

        ApplyDamageMultiplierToAllSoldiers();
    }


    // =========================================================
    // Attack Speed Upgrade
    // =========================================================

    /// <summary>
    /// 인게임 공격속도 증가량을 누적한다.
    /// </summary>
    public void UpgradeAllSoldierAttackSpeed(
        float percent)
    {
        inGameAttackSpeedMultiplier +=
            percent;

        ApplyAttackSpeedMultiplierToAllSoldiers();
    }


    /// <summary>
    /// 현재 최종 공격속도 배율을
    /// 모든 병사에게 적용한다.
    /// </summary>
    private void ApplyAttackSpeedMultiplierToAllSoldiers()
    {
        float finalMultiplier =
            FinalAttackSpeedMultiplier;


        for (int i = 0;
             i < soldierAttacks.Count;
             i++)
        {
            SoldierAttack attack =
                soldierAttacks[i];

            if (attack == null)
            {
                continue;
            }

            attack.SetAttackSpeedMultiplier(
                finalMultiplier
            );
        }
    }


    /// <summary>
    /// 로비 영구 공격속도 배율을 설정한다.
    /// </summary>
    public void SetLobbyAttackSpeedMultiplier(
        float multiplier)
    {
        lobbyAttackSpeedMultiplier =
            Mathf.Max(
                1f,
                multiplier
            );

        ApplyAttackSpeedMultiplierToAllSoldiers();
    }


    // =========================================================
    // Projectile Upgrade
    // =========================================================

    /// <summary>
    /// 투사체 강화 단계를 증가시키고
    /// 모든 현재 병사에게 적용한다.
    ///
    /// 이후 추가되는 병사도 AddUnit에서
    /// 같은 강화 레벨을 적용받는다.
    /// </summary>
    public void UpgradeAllSoldierProjectile()
    {
        projectileUpgradeLevel++;


        for (int i = 0;
             i < soldierAttacks.Count;
             i++)
        {
            SoldierAttack attack =
                soldierAttacks[i];

            if (attack == null)
            {
                continue;
            }

            attack.SetProjectileUpgradeLevel(
                projectileUpgradeLevel
            );
        }
    }


    // =========================================================
    // Runtime Injection
    // =========================================================

    /// <summary>
    /// 동적으로 생성된 PlayerRoot에
    /// Scene의 SoldierPool을 전달한다.
    /// </summary>
    public void SetSoldierPool(
        SoldierPool newSoldierPool)
    {
        soldierPool =
            newSoldierPool;
    }


    /// <summary>
    /// Scene BulletFX를 전달한다.
    ///
    /// 이미 생성된 SoldierAttack이 있다면
    /// 해당 병사들에게도 즉시 적용한다.
    /// </summary>
    public void SetBulletFx(
        HyperCasualBulletFx fx)
    {
        bulletFx =
            fx;


        for (int i = 0;
             i < soldierAttacks.Count;
             i++)
        {
            SoldierAttack attack =
                soldierAttacks[i];

            if (attack == null)
            {
                continue;
            }

            attack.SetBulletFx(
                bulletFx
            );
        }
    }
}