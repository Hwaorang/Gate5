using UnityEngine;

/// <summary>
/// 병사의 자동 사격을 담당하는 스크립트
///
/// 역할:
/// 1. 일정 공격 주기마다 전방으로 총알 발사
/// 2. 공격력 / 공격속도 강화 적용
/// 3. 투사체 강화 단계에 따라
///    - 투사체 개수 증가
///    - 최대 개수 이후 투사체 크기 증가
///
/// 실제 강화 수치는 ProjectileUpgradeData에서 가져온다.
/// </summary>
public class SoldierAttack : MonoBehaviour
{
    [Header("총알 설정")]

    // 발사할 Bullet Prefab
    [SerializeField]
    private GameObject bulletPrefab;

    [Header("총알 풀")]
    [SerializeField]
    private BulletPool bulletPool;

    // 총알이 생성될 위치와 기본 발사 방향
    [SerializeField]
    private Transform firePoint;


    [Header("기본 공격 스탯")]

    // 병사의 기본 공격력
    [SerializeField]
    private float baseDamage = 10f;

    // 병사의 기본 공격 간격
    // 값이 작을수록 더 빠르게 공격한다.
    [SerializeField]
    private float baseAttackDelay = 1f;


    [Header("투사체 강화 데이터")]

    // 투사체 기본 개수, 최대 개수,
    // 크기 증가량, 퍼짐 각도 등의 설정 데이터
    [SerializeField]
    private ProjectileUpgradeData projectileUpgradeData;


    // 현재 실제로 적용되고 있는 공격력
    private float currentDamage;

    // 현재 실제 공격 간격
    private float currentAttackDelay;

    // 마지막 공격 이후 흐른 시간
    private float attackTimer;


    // 현재 한 번에 발사하는 투사체 개수
    private int projectileCount;

    // 현재 투사체 크기 배율
    // 1 = 기본 크기
    // 1.2 = 기본보다 20% 큰 크기
    private float projectileScaleMultiplier;


    // 외부에서 현재 공격력 / 공격속도를 확인할 수 있도록 제공
    public float CurrentDamage => currentDamage;
    public float CurrentAttackDelay => currentAttackDelay;


    private void Awake()
    {
        // 게임 시작 시 기본 공격 스탯 적용
        currentDamage = baseDamage;
        currentAttackDelay = baseAttackDelay;

        // 투사체 강화 0레벨 상태 적용
        ApplyProjectileUpgradeLevel(0);
    }


    private void Update()
    {
        Attack();
    }


    /// <summary>
    /// 공격 쿨타임을 계산하고,
    /// 공격 가능한 시간이 되면 Fire()를 호출한다.
    ///
    /// 현재 게임 방식은 적을 직접 탐색하지 않고
    /// 적이 없어도 전방으로 계속 자동 사격한다.
    /// </summary>
    private void Attack()
    {
        attackTimer += Time.deltaTime;

        // 아직 공격 쿨타임이 지나지 않았다면 종료
        if (attackTimer < currentAttackDelay)
        {
            return;
        }

        Fire();

        // 공격 후 타이머 초기화
        attackTimer = 0f;
    }


    /// <summary>
    /// 현재 투사체 개수만큼 총알을 생성한다.
    ///
    /// 투사체가 여러 개라면
    /// FirePoint의 forward 방향을 기준으로
    /// 좌우에 일정한 각도로 퍼지도록 발사한다.
    /// </summary>
    private void Fire()
    {
        if (bulletPool == null ||
            firePoint == null ||
            projectileUpgradeData == null)
        {
            return;
        }

        float startAngle =
            -projectileUpgradeData.spreadAngle *
            (projectileCount - 1) *
            0.5f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle =
                startAngle +
                projectileUpgradeData.spreadAngle * i;

            Quaternion rotation =
                firePoint.rotation *
                Quaternion.Euler(
                    0f,
                    angle,
                    0f
                );

            // Instantiate 대신 Pool에서 가져오기
            GameObject bullet =
                bulletPool.GetBullet(
                    firePoint.position,
                    rotation
                );

            // Pool에서 재사용되므로
            // 크기도 현재 강화값으로 다시 정확히 설정
            bullet.transform.localScale =
                Vector3.one *
                projectileScaleMultiplier;

            Bullet bulletScript =
                bullet.GetComponent<Bullet>();

            if (bulletScript != null)
            {
                bulletScript.Init(
                    rotation * Vector3.forward,
                    currentDamage,
                    bulletPool
                );
            }
        }
    }


    /// <summary>
    /// 공격력 강화 배율을 적용한다.
    ///
    /// 예:
    /// baseDamage = 10
    /// multiplier = 1.2
    /// → 실제 공격력 = 12
    /// </summary>
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage =
            baseDamage * multiplier;
    }


    /// <summary>
    /// 공격속도 강화 배율을 적용한다.
    ///
    /// 공격속도가 증가할수록
    /// 공격 간격은 짧아져야 하므로
    /// 기본 공격 간격을 multiplier로 나눈다.
    ///
    /// 예:
    /// baseAttackDelay = 1초
    /// multiplier = 2
    /// → 공격 간격 = 0.5초
    /// </summary>
    public void SetAttackSpeedMultiplier(float multiplier)
    {
        // 잘못된 값으로 나누는 상황 방지
        if (multiplier <= 0f)
        {
            return;
        }

        currentAttackDelay =
            baseAttackDelay / multiplier;

        // 공격속도가 너무 빨라져
        // 지나치게 많은 총알이 생성되는 것을 방지
        currentAttackDelay =
            Mathf.Max(
                0.1f,
                currentAttackDelay
            );
    }


    /// <summary>
    /// SquadManager가 관리하고 있는
    /// 현재 투사체 강화 레벨을 Soldier에게 적용한다.
    ///
    /// 기존 값에 계속 +1 하는 방식이 아니라
    /// 전달받은 강화 레벨을 기준으로
    /// 현재 상태를 다시 계산한다.
    ///
    /// 따라서 Object Pool에서 병사가 다시 생성되어도
    /// 현재 강화 상태를 정확하게 재적용할 수 있다.
    /// </summary>
    public void SetProjectileUpgradeLevel(int upgradeLevel)
    {
        ApplyProjectileUpgradeLevel(
            upgradeLevel
        );
    }


    /// <summary>
    /// 투사체 강화 레벨을 기준으로
    /// 투사체 개수와 크기를 계산한다.
    ///
    /// 예:
    ///
    /// BaseCount = 1
    /// MaxCount = 5
    ///
    /// Lv.0 → 1발
    /// Lv.1 → 2발
    /// Lv.2 → 3발
    /// Lv.3 → 4발
    /// Lv.4 → 5발
    ///
    /// 이후:
    /// Lv.5 → 5발 + 크기 증가
    /// Lv.6 → 5발 + 크기 추가 증가
    /// </summary>
    private void ApplyProjectileUpgradeLevel(
        int upgradeLevel)
    {
        if (projectileUpgradeData == null)
        {
            Debug.LogWarning(
                $"{name} : ProjectileUpgradeData가 연결되지 않았습니다."
            );

            return;
        }


        // 음수 레벨 방지
        upgradeLevel =
            Mathf.Max(
                0,
                upgradeLevel
            );


        // 개수 증가에 사용할 수 있는
        // 최대 강화 횟수 계산
        //
        // 예:
        // 기본 1발 / 최대 5발
        // → 개수 증가 가능 횟수 = 4
        int countUpgradeLimit =
            projectileUpgradeData.maxProjectileCount -
            projectileUpgradeData.baseProjectileCount;


        // 현재 강화 레벨 중
        // 투사체 개수 증가에 사용할 레벨 계산
        int countUpgradeLevel =
            Mathf.Min(
                upgradeLevel,
                countUpgradeLimit
            );


        // 현재 실제 투사체 개수 계산
        projectileCount =
            projectileUpgradeData.baseProjectileCount +
            countUpgradeLevel;


        // 최대 투사체 개수를 넘어선 이후의
        // 남는 강화 레벨 계산
        //
        // 예:
        // 최대 개수 도달이 Lv.4인데
        // 현재 Lv.6이면
        // 크기 강화 레벨 = 2
        int scaleUpgradeLevel =
            Mathf.Max(
                0,
                upgradeLevel - countUpgradeLimit
            );


        // 현재 투사체 크기 배율 계산
        projectileScaleMultiplier =
            projectileUpgradeData.baseScaleMultiplier +
            scaleUpgradeLevel *
            projectileUpgradeData.scaleIncreasePerLevel;


#if UNITY_EDITOR
        Debug.Log(
            $"{name} / Projectile Lv.{upgradeLevel}" +
            $" / Count : {projectileCount}" +
            $" / Scale : {projectileScaleMultiplier:F1}"
        );
#endif
    }

    public void SetBulletPool(BulletPool pool)
    {
        bulletPool = pool;
    }
}