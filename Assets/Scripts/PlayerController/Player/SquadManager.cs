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
/// - 모든 병사의 공격 타이밍을 한 곳에서 관리
/// - 병사가 0명이 되면 GameOver 호출
///
/// 각 병사의 실제 공격 판정은 SoldierAttack이 담당하고,
/// SquadManager는 "언제 공격할지"와 "현재 강화 상태가 무엇인지"를 관리한다.
/// </summary>
public class SquadManager : MonoBehaviour
{
    [Header("병사 설정")]

    // 병사를 Instantiate / Destroy하지 않고
    // Object Pool에서 가져오고 반환하기 위해 사용하는 SoldierPool
    [SerializeField] private SoldierPool soldierPool;

    // 시작 병사 수 등 Player의 기본 정보를 가지고 있는 컴포넌트
    [SerializeField] private PlayerStats playerStats;


    [Header("대형 설정")]

    // 병사와 병사 사이의 간격
    [SerializeField] private float spacing = 1.2f;

    // 한 줄에 배치할 수 있는 최대 병사 수
    // 병사 수가 많아져도 가로로 끝없이 늘어나는 것을 방지한다.
    [SerializeField] private int maxColumnCount = 7;


    [Header("공격 기준점")]

    // 모든 병사의 사격 시작 Z 위치를 맞추기 위한 기준점
    // 각 병사의 X 위치는 유지하면서
    // FireLine의 Y / Z 위치를 사용한다.
    [SerializeField] private Transform fireLine;


    [Header("공격 연출")]

    // 기존 시각용 Bullet Object Pool
    // 실제 데미지 판정에는 사용하지 않고 연출용으로만 사용한다.
    [SerializeField] private BulletPool bulletPool;

    // Hyper Casual Bullet FX 패키지의 시각 효과 관리자
    // 실제 공격 판정은 Raycast,
    // 총알 궤적/Impact는 이 FX가 담당한다.
    [SerializeField] private HyperCasualBulletFx bulletFx;


    [Header("분산 사격")]

    // 한 프레임에 공격시킬 최대 병사 수
    //
    // 병사가 수백 명일 때 한 프레임에 전원이 동시에 Fire()를 호출하면
    // 순간적인 CPU 부하가 커질 수 있기 때문에
    // 여러 프레임에 나누어 사격한다.
    [SerializeField] private int shotsPerFrame = 20;


    // 현재 분대에 존재하는 실제 병사 GameObject 목록
    //
    // 병사 수 확인, 대형 배치, 제거 등에 사용한다.
    private readonly List<GameObject> soldiers = new();

    // soldiers에서 SoldierAttack만 따로 캐싱해 둔 목록
    //
    // 공격하거나 강화할 때마다 GetComponent를 반복 호출하지 않도록
    // 병사를 생성할 때 한 번만 찾아서 저장한다.
    private readonly List<SoldierAttack> soldierAttacks = new();


    // =========================
    // 공격력 강화 상태
    // =========================

    // 로비에서 구매한 영구 공격력 강화 배율
    // 기본값 1 = 강화 없음
    private float lobbyDamageMultiplier = 1f;

    // 게임 플레이 중 EXP 보상으로 획득한 공격력 강화 배율
    private float inGameDamageMultiplier = 1f;


    // =========================
    // 공격속도 강화 상태
    // =========================

    // 로비 영구 공격속도 강화 배율
    private float lobbyAttackSpeedMultiplier = 1f;

    // 인게임 공격속도 강화 배율
    private float inGameAttackSpeedMultiplier = 1f;


    // 현재 투사체 강화 레벨
    //
    // 예:
    // Lv.0 = 기본 1발
    // Lv.1 = 2발
    // Lv.2 = 3발
    //
    // 실제 계산은 SoldierAttack의 ProjectileUpgradeData에서 처리한다.
    private int projectileUpgradeLevel;


    // =========================
    // 분산 공격 상태
    // =========================

    // 다음 공격까지 기다린 시간
    private float attackTimer;

    // 현재 병사들이 분산 사격을 진행 중인지 여부
    private bool isFiring;

    // 이번 분산 사격에서 다음으로 공격할 SoldierAttack 인덱스
    private int fireIndex;


    /// <summary>
    /// 현재 보유 중인 병사 수.
    /// UI나 다른 시스템에서 병사 수를 확인할 때 사용한다.
    /// </summary>
    public int CurrentCount => soldiers.Count;


    /// <summary>
    /// 외부에서 현재 병사 목록을 읽기 전용으로 확인할 수 있도록 제공한다.
    /// 외부에서 직접 Add/Remove하지 못하게 IReadOnlyList로 반환한다.
    /// </summary>
    public IReadOnlyList<GameObject> Soldiers =>
        soldiers;


    /// <summary>
    /// 실제 병사에게 적용할 최종 공격력 배율.
    ///
    /// 기본 공격력 × 로비 배율 × 인게임 배율
    ///
    /// 예:
    /// Lobby = 1.25
    /// InGame = 1.10
    /// Final = 1.375
    /// </summary>
    private float FinalDamageMultiplier =>
        lobbyDamageMultiplier *
        inGameDamageMultiplier;


    /// <summary>
    /// 실제 병사에게 적용할 최종 공격속도 배율.
    ///
    /// 로비 강화와 인게임 강화를 서로 분리해서 관리한 뒤
    /// 최종적으로 곱해서 SoldierAttack에 전달한다.
    /// </summary>
    private float FinalAttackSpeedMultiplier =>
        lobbyAttackSpeedMultiplier *
        inGameAttackSpeedMultiplier;


    /// <summary>
    /// 병사 수가 바뀌었을 때 UI 등에 알리기 위한 이벤트.
    ///
    /// 예:
    /// 병사 추가 5 → 6
    /// 병사 사망 6 → 5
    /// </summary>
    public event Action<int> OnSoldierCountChanged;

    /// <summary>
    /// 모든 병사가 사라져 GameOver가 발생했음을
    /// 다른 시스템에 알리는 이벤트.
    ///
    /// Observer Pattern
    /// SquadManager는 결과 UI가 무엇인지 알 필요가 없고,
    /// GameOver 발생 사실만 외부에 전달한다.
    /// </summary>
    public event Action OnGameOver;


    private void Start()
    {
        // PlayerStats가 연결되지 않은 상태에서
        // StartSoldierCount를 읽으면 NullReference가 발생하므로 방어한다.
        if (playerStats == null)
        {
            Debug.LogWarning(
                "[SquadManager] PlayerStats가 연결되지 않았습니다."
            );

            return;
        }

        // PlayerStats에 설정된 시작 병사 수만큼 생성한다.
        AddUnit(playerStats.StartSoldierCount);
    }


    private void Update()
    {
        // 전체 병사의 공격 주기를 한 곳에서 계산한다.
        HandleAttackTimer();

        // 공격 시간이 되었을 경우
        // shotsPerFrame 수만큼 병사를 나누어 Fire()시킨다.
        HandleDistributedFire();
    }


    /// <summary>
    /// 지정한 수만큼 병사를 분대에 추가한다.
    ///
    /// 병사를 Pool에서 가져온 뒤:
    /// 1. soldiers 리스트에 등록
    /// 2. SoldierUnit에 SquadManager 전달
    /// 3. 현재 강화 상태 적용
    /// 4. 공격용 리스트에 등록
    /// 5. 대형 재배치
    /// 6. 병사 수 변경 이벤트 발생
    /// </summary>
    public void AddUnit(int amount)
    {
        // Pool이 없으면 병사를 생성할 수 없다.
        if (soldierPool == null)
        {
            Debug.LogWarning(
                "[SquadManager] SoldierPool이 연결되지 않았습니다."
            );

            return;
        }

        for (int i = 0; i < amount; i++)
        {
            // Pool에서 병사를 하나 가져온다.
            // 부모는 현재 SquadManager가 붙어 있는 PlayerRoot가 된다.
            GameObject soldier =
                soldierPool.GetSoldier(transform);

            // Pool에서 병사를 가져오지 못한 경우
            // 이번 반복만 건너뛴다.
            if (soldier == null)
            {
                continue;
            }

            // 현재 활성 병사 목록에 추가
            soldiers.Add(soldier);


            // SoldierUnit에게
            // 자신이 어느 Squad에 속해 있는지 알려준다.
            SoldierUnit unit =
                soldier.GetComponent<SoldierUnit>();

            if (unit != null)
            {
                unit.Init(this);
            }


            // 병사의 공격 컴포넌트를 한 번만 가져온다.
            SoldierAttack attack =
                soldier.GetComponent<SoldierAttack>();

            if (attack != null)
            {
                // 지금까지 누적된
                // 로비 + 인게임 공격력 강화를 새 병사에게 적용
                attack.SetDamageMultiplier(
                    FinalDamageMultiplier
                );

                // 현재 공격속도 강화 상태 적용
                attack.SetAttackSpeedMultiplier(
                    FinalAttackSpeedMultiplier
                );

                // 현재 투사체 강화 단계 적용
                //
                // 게임 도중 Gate로 새 병사가 추가되어도
                // 기존 병사와 같은 투사체 상태를 갖게 된다.
                attack.SetProjectileUpgradeLevel(
                    projectileUpgradeLevel
                );

                // 시각용 Bullet Pool 전달
                attack.SetBulletPool(
                    bulletPool
                );

                // HyperCasual Bullet FX 전달
                attack.SetBulletFx(
                    bulletFx
                );

                // Squad의 공통 발사 기준점 전달
                attack.SetFireLine(
                    fireLine
                );

                // 이후 공격/강화 시 GetComponent를 다시 하지 않도록 캐싱
                soldierAttacks.Add(attack);
            }
        }

        // 병사 수가 바뀌었으므로 현재 인원에 맞게 다시 정렬
        UpdateFormation();

        // SoldierCountUI 등에게 현재 병사 수 전달
        OnSoldierCountChanged?.Invoke(
            CurrentCount
        );
    }


    /// <summary>
    /// 특정 병사를 현재 분대에서 제거한다.
    ///
    /// soldiers와 soldierAttacks 양쪽에서 제거한 뒤
    /// SoldierPool로 반환한다.
    ///
    /// 병사 제거 후:
    /// - 대형 재배치
    /// - UI 갱신
    /// - GameOver 조건 확인
    /// 순서로 처리한다.
    /// </summary>
    public void RemoveUnit(SoldierUnit soldier)
    {
        if (soldier == null)
        {
            return;
        }

        GameObject soldierObject =
            soldier.gameObject;

        // 현재 분대에 등록되어 있지 않은 병사라면
        // 중복 제거하지 않고 종료한다.
        if (!soldiers.Remove(soldierObject))
        {
            return;
        }

        // 공격 리스트에서도 함께 제거
        SoldierAttack attack =
            soldierObject.GetComponent<SoldierAttack>();

        if (attack != null)
        {
            soldierAttacks.Remove(attack);
        }

        // 실제 Destroy 대신 Object Pool로 반환
        if (soldierPool != null)
        {
            soldierPool.ReturnSoldier(
                soldierObject
            );
        }

        // 남은 병사들 기준으로 다시 대형 배치
        UpdateFormation();

        // UI에 현재 병사 수 전달
        OnSoldierCountChanged?.Invoke(
            CurrentCount
        );

        // 마지막 병사가 제거된 경우 GameOver 처리
        CheckGameOver();
    }


    /// <summary>
    /// 현재 병사 수에 따라 자동으로 대형을 구성한다.
    ///
    /// 병사 수의 제곱근을 기준으로 열 개수를 정하고,
    /// maxColumnCount보다 넓어지지 않도록 제한한다.
    ///
    /// 각 행의 마지막 병사 수가 부족해도
    /// 해당 행만 가운데 정렬되도록 계산한다.
    /// </summary>
    private void UpdateFormation()
    {
        if (soldiers.Count == 0)
        {
            return;
        }

        // 병사 수가 늘어날수록
        // 대략 정사각형에 가까운 대형이 되도록 열 개수 계산
        int columnCount =
            Mathf.CeilToInt(
                Mathf.Sqrt(soldiers.Count)
            );

        // 너무 넓어지는 것 방지
        columnCount =
            Mathf.Min(
                columnCount,
                maxColumnCount
            );

        for (int i = 0; i < soldiers.Count; i++)
        {
            // 현재 병사가 몇 번째 행인지
            int row =
                i / columnCount;

            // 현재 행의 몇 번째 열인지
            int column =
                i % columnCount;

            // 현재 행의 첫 병사 인덱스
            int rowStartIndex =
                row * columnCount;

            // 현재 행부터 남아있는 전체 병사 수
            int remainingSoldiers =
                soldiers.Count -
                rowStartIndex;

            // 현재 행에 실제로 들어가는 병사 수
            //
            // 마지막 행은 columnCount보다 적을 수 있다.
            int soldiersInThisRow =
                Mathf.Min(
                    columnCount,
                    remainingSoldiers
                );

            // 현재 행 전체가 중앙에 오도록
            // 왼쪽으로 이동시킬 Offset 계산
            float xOffset =
                (soldiersInThisRow - 1) *
                spacing *
                0.5f;

            // 현재 병사의 X 위치
            float x =
                column * spacing -
                xOffset;

            // 행이 뒤로 갈수록 -Z 방향으로 배치
            float z =
                -row * spacing;

            soldiers[i].transform.localPosition =
                new Vector3(
                    x,
                    0f,
                    z
                );
        }
    }


    /// <summary>
    /// 현재 병사가 한 명도 남아있지 않으면
    /// GameOver 이벤트를 발생시키고
    /// 기존 GameManager의 GameOver를 호출한다.
    /// </summary>
    private void CheckGameOver()
    {
        // 아직 병사가 남아있으면 GameOver가 아니다.
        if (soldiers.Count > 0)
        {
            return;
        }

        // =========================
        // Observer Pattern
        // =========================
        //
        // 결과 UI, 통계 시스템 등은
        // 이 이벤트를 구독해서 각자 필요한 처리를 한다.
        //
        // SquadManager가 특정 UI를 직접 참조하지 않기 때문에
        // GameOver 화면 디자인이 바뀌어도
        // SquadManager를 수정할 필요가 없다.
        OnGameOver?.Invoke();

        // 기존 프로젝트의 GameOver 처리 유지
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }


    /// <summary>
    /// 인게임 EXP 보상으로 획득한
    /// 공격력 증가량을 누적한다.
    ///
    /// percent = 0.1이면 +10%
    /// </summary>
    public void UpgradeAllSoldierDamage(
        float percent)
    {
        // 인게임 전용 배율에만 추가한다.
        //
        // Lobby 배율과 분리되어 있으므로
        // 로비 강화 상태를 덮어쓰지 않는다.
        inGameDamageMultiplier += percent;

        // 현재 존재하는 병사들에게
        // 새 최종 배율을 다시 적용
        ApplyDamageMultiplierToAllSoldiers();
    }


    /// <summary>
    /// 현재 로비 공격력 배율과
    /// 인게임 공격력 배율을 곱한 결과를
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
    /// 로비에서 저장된 영구 공격력 배율을 설정한다.
    ///
    /// 예:
    /// 로비 공격력 +25%
    /// multiplier = 1.25
    ///
    /// 기존 값을 증가시키는 것이 아니라
    /// 저장된 로비 배율 값으로 설정한다.
    /// </summary>
    public void SetLobbyDamageMultiplier(
        float multiplier)
    {
        // 1보다 작은 값이 들어와
        // 기본 공격력보다 약해지는 상황 방지
        lobbyDamageMultiplier =
            Mathf.Max(
                1f,
                multiplier
            );

        // 이미 생성된 병사에게도 즉시 적용
        ApplyDamageMultiplierToAllSoldiers();
    }


    /// <summary>
    /// 인게임 공격속도 증가량을 누적한다.
    ///
    /// percent = 0.1이면
    /// 현재 인게임 공격속도 배율에 +10%
    /// </summary>
    public void UpgradeAllSoldierAttackSpeed(
        float percent)
    {
        inGameAttackSpeedMultiplier += percent;

        ApplyAttackSpeedMultiplierToAllSoldiers();
    }


    /// <summary>
    /// 로비 공격속도 × 인게임 공격속도를 계산해
    /// 현재 모든 병사에게 적용한다.
    ///
    /// SoldierAttack에서는 이 배율을 이용해
    /// 실제 AttackDelay를 감소시킨다.
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
    /// 로비에서 저장된 영구 공격속도 배율을 설정한다.
    ///
    /// 로비와 인게임 공격속도를 서로 독립적으로 관리하기 위해
    /// 이 함수에서는 lobbyAttackSpeedMultiplier만 변경한다.
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


    /// <summary>
    /// 투사체 강화 레벨을 1 증가시키고
    /// 현재 모든 병사에게 동일한 강화 레벨을 적용한다.
    ///
    /// 새 병사가 이후에 생성되더라도
    /// projectileUpgradeLevel 값을 AddUnit에서 다시 적용하므로
    /// 기존 병사와 같은 강화 상태를 갖는다.
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


    /// <summary>
    /// 지정한 수만큼 병사를 제거한다.
    ///
    /// 실제 병사 수보다 큰 값이 들어와도
    /// 현재 존재하는 병사까지만 제거한다.
    ///
    /// Gate의 감소 기능 등에 사용한다.
    /// </summary>
    public void RemoveUnits(int amount)
    {
        int removeCount =
            Mathf.Min(
                amount,
                soldiers.Count
            );

        for (int i = 0;
             i < removeCount;
             i++)
        {
            // 대형의 가장 마지막 병사를 선택
            GameObject soldierObject =
                soldiers[
                    soldiers.Count - 1
                ];

            SoldierUnit soldier =
                soldierObject
                    .GetComponent<SoldierUnit>();

            if (soldier != null)
            {
                RemoveUnit(soldier);
            }
        }
    }


    /// <summary>
    /// 현재 분대의 마지막 병사 한 명을 제거한다.
    ///
    /// DamageLine을 Enemy가 통과했을 때
    /// 병사 1명 감소 처리에 사용한다.
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

        SoldierUnit soldier =
            soldierObject
                .GetComponent<SoldierUnit>();

        if (soldier != null)
        {
            RemoveUnit(soldier);
        }
    }


    /// <summary>
    /// Squad 전체의 공격 주기를 계산한다.
    ///
    /// 각 SoldierAttack이 Update에서 공격 시간을 계산하지 않고
    /// SquadManager 한 곳에서만 타이머를 계산해서
    /// 병사 수가 많을 때 Update 호출 비용을 줄인다.
    /// </summary>
    private void HandleAttackTimer()
    {
        // 공격 가능한 병사가 없거나
        // 이미 한 번의 분산 사격을 진행 중이면
        // 다음 공격 타이머를 진행하지 않는다.
        if (soldierAttacks.Count == 0 ||
            isFiring)
        {
            return;
        }

        attackTimer +=
            Time.deltaTime;

        // 모든 병사가 같은 강화 상태를 공유하므로
        // 첫 번째 병사의 AttackDelay를 Squad의 공격 주기로 사용한다.
        SoldierAttack firstAttack =
            soldierAttacks[0];

        if (firstAttack == null)
        {
            return;
        }

        float attackDelay =
            firstAttack.AttackDelay;

        // 아직 다음 공격 시간이 되지 않았다면 대기
        if (attackTimer < attackDelay)
        {
            return;
        }

        // 다음 공격 주기를 위해 타이머 초기화
        attackTimer = 0f;

        // 분산 사격 시작
        isFiring = true;

        // 첫 번째 병사부터 공격하도록 인덱스 초기화
        fireIndex = 0;
    }


    /// <summary>
    /// 병사들의 Fire() 호출을 여러 프레임에 분산한다.
    ///
    /// 예:
    /// 병사 100명
    /// shotsPerFrame = 20
    ///
    /// → 약 5프레임에 걸쳐 100명이 사격한다.
    ///
    /// 이를 통해 한 프레임에 Raycast와 FX 호출이
    /// 몰리는 현상을 완화한다.
    /// </summary>
    private void HandleDistributedFire()
    {
        // 현재 공격 시간이 아니라면 처리하지 않는다.
        if (!isFiring)
        {
            return;
        }

        int firedThisFrame = 0;

        // 아직 공격하지 않은 병사가 남아 있고
        // 이번 프레임의 최대 사격 수를 넘지 않은 동안 반복
        while (
            fireIndex <
            soldierAttacks.Count &&
            firedThisFrame <
            shotsPerFrame)
        {
            SoldierAttack attack =
                soldierAttacks[fireIndex];

            if (attack != null)
            {
                // SoldierAttack은
                // 실제 Raycast 판정과 시각 FX 실행만 담당한다.
                attack.Fire();
            }

            fireIndex++;
            firedThisFrame++;
        }

        // 모든 병사가 이번 공격을 완료했다면
        // 분산 사격 상태 종료
        if (fireIndex >=
            soldierAttacks.Count)
        {
            isFiring = false;
        }
    }
}