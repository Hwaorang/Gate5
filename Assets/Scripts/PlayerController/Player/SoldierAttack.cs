using UnityEngine;

/// <summary>
/// 병사의 공격 기능을 담당한다.
///
/// 현재 구조
/// - 공격 타이밍은 SquadManager에서 관리
/// - SoldierAttack은 Fire() 호출을 받으면 공격 수행
/// - 실제 데미지 판정은 Raycast 사용
/// - Bullet은 데미지 판정 없이 시각 연출만 담당
///
/// 강화
/// - 공격력 증가
/// - 공격속도 증가
/// - 투사체 개수 증가
/// - 최대 투사체 개수 이후 투사체 크기 증가
/// </summary>
public class SoldierAttack : MonoBehaviour
{
    [Header("공격 위치")]
    // 총알과 Raycast가 시작되는 위치
    [SerializeField] private Transform firePoint;


    [Header("공격 판정")]
    // Raycast가 Enemy만 감지하도록 설정
    [SerializeField] private LayerMask enemyLayer;

    // Raycast 최대 거리
    [SerializeField] private float attackRange = 30f;


    [Header("총알 연출")]
    // 시각용 Bullet을 가져올 Object Pool
    [SerializeField] private BulletPool bulletPool;

    // 실제 판정은 즉시 이루어지기 때문에
    // 시각적으로 어색하지 않도록 총알을 빠르게 이동시킨다.
    [SerializeField] private float visualBulletSpeed = 80f;


    [Header("기본 공격 스탯")]
    // 강화가 적용되기 전 기본 공격력
    [SerializeField] private float baseDamage = 10f;

    // 강화가 적용되기 전 기본 공격 간격
    [SerializeField] private float baseAttackDelay = 1f;


    [Header("투사체 강화 데이터")]
    // 투사체 개수, 최대 개수, 퍼짐 각도,
    // 크기 증가량 등을 가지고 있는 ScriptableObject
    [SerializeField] private ProjectileUpgradeData projectileUpgradeData;


    // 현재 적용 중인 실제 공격력
    private float currentDamage;

    // 현재 적용 중인 실제 공격 간격
    private float currentAttackDelay;

    // 현재 한 번의 공격에서 발사할 투사체 개수
    private int projectileCount;

    // 현재 시각용 투사체 크기 배율
    private float projectileScaleMultiplier;


    // 외부에서 현재 공격 정보를 확인하기 위한 프로퍼티
    public float CurrentDamage => currentDamage;
    public float CurrentAttackDelay => currentAttackDelay;

    // SquadManager가 공격 주기를 확인할 때 사용
    public float AttackDelay => currentAttackDelay;


    private void Awake()
    {
        // 시작 시 기본 스탯 적용
        currentDamage = baseDamage;
        currentAttackDelay = baseAttackDelay;

        // 투사체 강화가 하나도 없는 기본 상태 적용
        ApplyProjectileUpgradeLevel(0);
    }


    /// <summary>
    /// SquadManager에서 호출하는 실제 공격 함수.
    ///
    /// 현재 투사체 개수만큼 공격하며,
    /// 여러 발일 경우 좌우로 일정 각도만큼 퍼져서 발사한다.
    /// </summary>
    public void Fire()
    {
        if (firePoint == null ||
            projectileUpgradeData == null)
        {
            return;
        }

        // 여러 발을 중앙 기준으로 균등하게 퍼뜨리기 위한 시작 각도
        //
        // 예: 3발, spreadAngle = 5
        // -5 / 0 / +5
        float startAngle =
            -projectileUpgradeData.spreadAngle *
            (projectileCount - 1) *
            0.5f;

        for (int i = 0; i < projectileCount; i++)
        {
            // 현재 투사체의 퍼짐 각도 계산
            float angle =
                startAngle +
                projectileUpgradeData.spreadAngle * i;

            // FirePoint 방향에 퍼짐 각도를 적용
            Quaternion rotation =
                firePoint.rotation *
                Quaternion.Euler(0f, angle, 0f);

            Vector3 direction =
                rotation * Vector3.forward;

            // 실제 데미지 판정
            FireRay(direction);

            // 시각용 총알 출력
            FireVisualBullet(rotation, direction);
        }
    }


    /// <summary>
    /// Raycast를 이용해 실제 공격 판정을 처리한다.
    ///
    /// Bullet Collider를 사용하지 않기 때문에
    /// 대량의 총알에서 발생하는 Physics 비용을 줄일 수 있다.
    /// </summary>
    private void FireRay(Vector3 direction)
    {
        Ray ray = new Ray(
            firePoint.position,
            direction
        );

        bool isHit = Physics.Raycast(
            ray,
            out RaycastHit hit,
            attackRange,
            enemyLayer
        );

        if (!isHit)
        {
            return;
        }

        Debug.Log(
            $"{name} : Ray Hit = {hit.collider.name} / Layer = {LayerMask.LayerToName(hit.collider.gameObject.layer)}"
        );

        EnemyHealth enemy =
            hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(currentDamage);
    }


    /// <summary>
    /// 실제 데미지와는 관계없는 시각용 Bullet을 출력한다.
    ///
    /// Bullet에는 Collider / Rigidbody가 필요하지 않으며
    /// 빠르게 앞으로 이동하는 연출만 담당한다.
    /// </summary>
    private void FireVisualBullet(
        Quaternion rotation,
        Vector3 direction)
    {
        if (bulletPool == null)
        {
            return;
        }

        GameObject bullet =
            bulletPool.GetBullet(
                firePoint.position,
                rotation
            );

        // Object Pool에서 재사용되는 Bullet이므로
        // 이전 Scale이 남지 않게 항상 직접 설정한다.
        bullet.transform.localScale =
            Vector3.one * projectileScaleMultiplier;

        Bullet bulletScript =
            bullet.GetComponent<Bullet>();

        if (bulletScript == null)
        {
            return;
        }

        bulletScript.InitVisual(
            direction,
            visualBulletSpeed,
            bulletPool
        );
    }


    /// <summary>
    /// 공격력 강화 배율을 적용한다.
    ///
    /// 예:
    /// baseDamage = 10
    /// multiplier = 1.2
    /// → currentDamage = 12
    /// </summary>
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage =
            baseDamage * multiplier;
    }


    /// <summary>
    /// 공격속도 강화 배율을 적용한다.
    ///
    /// 공격속도가 증가하면 공격 간격은 감소한다.
    ///
    /// 예:
    /// baseAttackDelay = 1
    /// multiplier = 2
    /// → currentAttackDelay = 0.5초
    /// </summary>
    public void SetAttackSpeedMultiplier(float multiplier)
    {
        // 0 이하로 나누는 상황 방지
        if (multiplier <= 0f)
        {
            return;
        }

        currentAttackDelay =
            baseAttackDelay / multiplier;

        // 공격속도가 지나치게 빨라지는 것을 방지
        currentAttackDelay =
            Mathf.Max(
                0.1f,
                currentAttackDelay
            );
    }


    /// <summary>
    /// SquadManager가 가지고 있는
    /// 현재 투사체 강화 레벨을 적용한다.
    ///
    /// Object Pool에서 다시 사용되는 Soldier도
    /// 현재 강화 상태를 그대로 적용받을 수 있다.
    /// </summary>
    public void SetProjectileUpgradeLevel(
        int upgradeLevel)
    {
        ApplyProjectileUpgradeLevel(
            upgradeLevel
        );
    }


    /// <summary>
    /// 투사체 강화 레벨에 따라
    /// 현재 발사 개수와 크기를 계산한다.
    ///
    /// 예:
    /// 기본 1발 / 최대 5발
    ///
    /// Lv.0 → 1발
    /// Lv.1 → 2발
    /// Lv.2 → 3발
    /// Lv.3 → 4발
    /// Lv.4 → 5발
    ///
    /// 최대 개수 도달 이후부터는
    /// 투사체 크기가 증가한다.
    /// </summary>
    private void ApplyProjectileUpgradeLevel(
        int upgradeLevel)
    {
        if (projectileUpgradeData == null)
        {
            return;
        }

        // 잘못된 음수 강화 레벨 방지
        upgradeLevel =
            Mathf.Max(0, upgradeLevel);


        // 투사체 개수를 증가시킬 수 있는 최대 횟수
        //
        // 기본 1 / 최대 5
        // → 개수 강화 가능 횟수 = 4
        int countUpgradeLimit =
            projectileUpgradeData.maxProjectileCount -
            projectileUpgradeData.baseProjectileCount;


        // 현재 강화 중 개수 증가에 사용할 레벨
        int countUpgradeLevel =
            Mathf.Min(
                upgradeLevel,
                countUpgradeLimit
            );


        // 실제 현재 투사체 개수
        projectileCount =
            projectileUpgradeData.baseProjectileCount +
            countUpgradeLevel;


        // 최대 투사체 개수 이후 남은 강화 레벨
        int scaleUpgradeLevel =
            Mathf.Max(
                0,
                upgradeLevel - countUpgradeLimit
            );


        // 실제 현재 투사체 크기
        projectileScaleMultiplier =
            projectileUpgradeData.baseScaleMultiplier +
            scaleUpgradeLevel *
            projectileUpgradeData.scaleIncreasePerLevel;


#if UNITY_EDITOR
        Debug.Log(
            $"{name}" +
            $" / Projectile Lv.{upgradeLevel}" +
            $" / Count : {projectileCount}" +
            $" / Scale : {projectileScaleMultiplier:F1}"
        );
#endif
    }


    /// <summary>
    /// SquadManager에서 BulletPool을 전달할 때 사용한다.
    /// Pool에서 가져온 Soldier에도 다시 연결할 수 있다.
    /// </summary>
    public void SetBulletPool(BulletPool pool)
    {
        bulletPool = pool;
    }
}