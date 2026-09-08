using UnityEngine;
using GptAsset.HyperCasualBulletFX;

/// <summary>
/// 병사 한 명의 공격 기능을 담당한다.
///
/// 전체 공격 주기는 SquadManager가 관리하고,
/// SoldierAttack은 SquadManager에서 Fire()가 호출되었을 때
/// 실제 공격 판정과 시각 효과를 처리한다.
///
/// 공격 구조
/// - 실제 데미지 판정 : Raycast
/// - 총알 연출 : Hyper Casual Bullet FX
///
/// 강화 종류
/// - 공격력
/// - 공격속도
/// - 투사체 개수
/// - 최대 투사체 개수 이후 투사체 크기 증가
/// </summary>
public class SoldierAttack : MonoBehaviour
{
    [Header("공격 위치")]

    // 병사의 기본 발사 위치.
    // FireLine이 없을 경우 이 위치를 발사 시작점으로 사용한다.
    [SerializeField] private Transform firePoint;


    [Header("공격 판정")]

    // Raycast가 공격 대상으로 인식할 Layer.
    // Inspector에서 Enemy Layer를 지정한다.
    [SerializeField] private LayerMask enemyLayer;

    // Raycast가 도달할 수 있는 최대 공격 거리
    [SerializeField] private float attackRange = 30f;


    [Header("총알 연출")]

    // 기존 시각용 Bullet Object Pool.
    //
    // 현재 실제 공격 연출은 HyperCasualBulletFx를 사용하고 있으며,
    // FireVisualBullet()은 현재 Fire()에서 호출하지 않는다.
    //
    // 투사체 크기 강화 기능을 어떻게 처리할지 결정하기 전까지 유지한다.
    [SerializeField] private BulletPool bulletPool;

    // 기존 시각용 Bullet이 이동할 속도.
    // 현재 FireVisualBullet()에서만 사용한다.
    [SerializeField] private float visualBulletSpeed = 80f;


    [Header("기본 공격 스탯")]

    // 아무 강화도 적용되지 않았을 때 기본 공격력
    [SerializeField] private float baseDamage = 10f;

    // 아무 강화도 적용되지 않았을 때 기본 공격 간격
    //
    // 값이 작을수록 더 빠르게 공격한다.
    [SerializeField] private float baseAttackDelay = 1f;


    [Header("투사체 강화 데이터")]

    // 투사체 관련 설정값을 가지고 있는 ScriptableObject
    //
    // - 기본 투사체 개수
    // - 최대 투사체 개수
    // - 퍼짐 각도
    // - 기본 크기
    // - 크기 증가량
    [SerializeField]
    private ProjectileUpgradeData projectileUpgradeData;


    [Header("총알 FX")]

    // 실제 Bullet Collider 대신
    // 총알 궤적과 Impact 연출을 담당한다.
    [SerializeField]
    private HyperCasualBulletFx bulletFx;


    // =========================
    // 현재 적용된 공격 스탯
    // =========================

    // 로비 + 인게임 강화까지 반영된 최종 공격력
    private float currentDamage;

    // 공격속도 강화까지 반영된 최종 공격 간격
    private float currentAttackDelay;

    // 한 번 공격할 때 발사하는 투사체 개수
    private int projectileCount;

    // 현재 투사체 크기 배율
    //
    // 현재는 기존 FireVisualBullet에서 사용하며,
    // HyperCasualBulletFx에 실제 크기를 적용할지는 추후 결정한다.
    private float projectileScaleMultiplier;


    // Squad 전체가 공유하는 앞쪽 발사 기준점
    private Transform fireLine;

    [SerializeField]
    private SoldierAnimationController animationController;

    [SerializeField]
    private SoldierUnit soldierUnit;

    /// <summary>
    /// 현재 최종 공격력.
    /// </summary>
    public float CurrentDamage =>
        currentDamage;


    /// <summary>
    /// 현재 최종 공격 간격.
    /// </summary>
    public float CurrentAttackDelay =>
        currentAttackDelay;


    /// <summary>
    /// SquadManager가 공격 주기를 계산할 때 사용한다.
    /// </summary>
    public float AttackDelay =>
        currentAttackDelay;


    private void Awake()
    {
        if (soldierUnit == null)
        {
            soldierUnit =
                GetComponent<SoldierUnit>();
        }

        // SquadManager에서 강화 배율을 전달받기 전에도
        // 기본 공격값을 정상적으로 가지고 있도록 초기화한다.
        currentDamage =
            baseDamage;

        currentAttackDelay =
            baseAttackDelay;


        // 투사체 데이터가 존재한다면
        // 기본 강화 단계인 Lv.0 상태로 초기화한다.
        if (projectileUpgradeData != null)
        {
            ApplyProjectileUpgradeLevel(0);
        }
    }


    /// <summary>
    /// 한 번의 공격을 실행한다.
    ///
    /// 현재 projectileCount만큼 반복하며,
    /// 각 투사체의 퍼짐 각도를 계산한다.
    ///
    /// 실제 데미지는 FireRay(),
    /// 시각 효과는 PlayBulletFx()가 담당한다.
    /// </summary>
    public void Fire()
    {
        // 병사가 살아있는 상태가 아니라면
        // 공격하지 않는다.
        if (soldierUnit != null &&
            !soldierUnit.CanAttack)
        {
            return;
        }

        // 발사 위치 또는 투사체 데이터가 없으면
        // 정상적인 공격 계산이 불가능하므로 종료한다.
        if (firePoint == null ||
            projectileUpgradeData == null)
        {
            return;
        }

        //// 사격 애니메이션 재생
        //if (animationController != null)
        //{
        //    animationController.PlayShoot();
        //}


        // 실제 공격 시작 위치 계산
        Vector3 fireOrigin =
            GetFireOrigin();


        // 여러 투사체가 중앙을 기준으로
        // 좌우 대칭으로 퍼지도록 첫 번째 발사 각도를 계산한다.
        //
        // 예:
        // projectileCount = 3
        // spreadAngle = 10
        //
        // 결과:
        // -10도 / 0도 / +10도
        float startAngle =
            -projectileUpgradeData.spreadAngle *
            (projectileCount - 1) *
            0.5f;


        for (int i = 0;
             i < projectileCount;
             i++)
        {
            // 현재 투사체가 사용할 각도
            float angle =
                startAngle +
                projectileUpgradeData.spreadAngle *
                i;


            // FirePoint의 현재 방향을 기준으로
            // Y축 회전을 추가한다.
            Quaternion rotation =
                firePoint.rotation *
                Quaternion.Euler(
                    0f,
                    angle,
                    0f
                );


            // 실제 Raycast가 날아갈 방향
            Vector3 direction =
                rotation *
                Vector3.forward;


            // 실제 Raycast 피격 판정 수행
            //
            // Enemy에 맞으면 실제 hit.distance,
            // 아무것도 맞지 않으면 attackRange를 반환한다.
            float visualDistance =
                FireRay(
                    fireOrigin,
                    direction
                );


            // 실제 Raycast 거리와 동일한 거리만큼
            // 총알 FX를 재생한다.
            PlayBulletFx(
                fireOrigin,
                direction,
                visualDistance
            );
        }
    }


    /// <summary>
    /// 실제 공격 판정과 별개로
    /// 총알이 날아가는 시각 효과를 재생한다.
    ///
    /// Raycast에서 계산된 실제 피격 거리를 전달받기 때문에
    /// Enemy를 맞았을 경우 FX가 Enemy를 지나가지 않는다.
    /// </summary>
    private void PlayBulletFx(
        Vector3 origin,
        Vector3 direction,
        float distance)
    {
        if (bulletFx == null)
        {
            return;
        }

        bulletFx.Play(
            origin,
            direction,
            distance
        );
    }


    /// <summary>
    /// Raycast를 사용해 실제 Enemy 피격을 처리한다.
    ///
    /// Bullet Collider를 대량으로 생성하지 않고
    /// Raycast로 즉시 피격을 계산하기 때문에
    /// 병사 수가 많아졌을 때 Physics 비용을 줄일 수 있다.
    ///
    /// Enemy에 명중하면 실제 명중 거리(hit.distance)를 반환하고,
    /// 아무것도 맞지 않으면 최대 공격 거리(attackRange)를 반환한다.
    /// </summary>
    private float FireRay(
        Vector3 origin,
        Vector3 direction)
    {
        Ray ray =
            new Ray(
                origin,
                direction
            );


        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            attackRange,
            enemyLayer,
            QueryTriggerInteraction.Collide))
        {
            // 기존 테스트 Enemy 시스템
            EnemyHealth enemyHealth =
                hit.collider
                    .GetComponentInParent<EnemyHealth>();


            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    currentDamage
                );
            }
            else
            {
                // 팀 프로젝트에서 사용하는 Enemy 시스템
                Mon_Ctrl monCtrl =
                    hit.collider
                        .GetComponentInParent<Mon_Ctrl>();


                if (monCtrl != null)
                {
                    monCtrl.TakeDamage(
                        currentDamage
                    );
                }
            }


            // 실제 피격 위치에 Impact FX 출력
            if (bulletFx != null)
            {
                bulletFx.PlayImpact(
                    hit.point +
                    hit.normal * 0.1f,

                    hit.normal
                );
            }


            // 총알 FX가 Enemy를 지나가지 않도록
            // 실제 명중 거리 반환
            return hit.distance;
        }


        // 아무것도 맞지 않았다면
        // 최대 공격 거리까지 총알 FX 재생
        return attackRange;
    }


    /// <summary>
    /// 기존 시각용 Bullet GameObject를 출력한다.
    ///
    /// 현재 Fire()에서는 사용하지 않으며
    /// 실제 데미지 판정에도 관여하지 않는다.
    ///
    /// 투사체 크기 강화 기능과 연결되어 있으므로
    /// 해당 기능을 유지할지 결정하기 전까지 남겨둔다.
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


        if (bullet == null)
        {
            return;
        }


        // Object Pool에서 다시 사용되는 Bullet이므로
        // 이전 Scale이 남지 않도록 현재 Scale을 항상 다시 적용한다.
        bullet.transform.localScale =
            Vector3.one *
            projectileScaleMultiplier;


        Bullet bulletScript =
            bullet.GetComponent<Bullet>();


        if (bulletScript == null)
        {
            return;
        }


        // Bullet은 데미지 판정 없이
        // 시각적으로 이동하는 역할만 담당한다.
        bulletScript.InitVisual(
            direction,
            visualBulletSpeed,
            bulletPool
        );
    }


    /// <summary>
    /// SquadManager에서 계산한
    /// 최종 공격력 배율을 적용한다.
    ///
    /// 예:
    /// baseDamage = 10
    /// multiplier = 1.3
    ///
    /// currentDamage = 13
    /// </summary>
    public void SetDamageMultiplier(
        float multiplier)
    {
        currentDamage =
            baseDamage *
            multiplier;
    }


    /// <summary>
    /// SquadManager에서 전달받은
    /// 공격속도 배율을 적용한다.
    ///
    /// 공격속도가 높아질수록
    /// 실제 공격 간격은 짧아진다.
    ///
    /// 예:
    /// baseAttackDelay = 1
    /// multiplier = 2
    ///
    /// currentAttackDelay = 0.5초
    /// </summary>
    public void SetAttackSpeedMultiplier(
        float multiplier)
    {
        // 0 이하의 값으로 나누는 상황 방지
        if (multiplier <= 0f)
        {
            return;
        }


        currentAttackDelay =
            baseAttackDelay /
            multiplier;


        // 공격속도가 지나치게 빨라져
        // 과도한 Raycast / FX 호출이 발생하지 않도록
        // 최소 공격 간격을 0.1초로 제한한다.
        currentAttackDelay =
            Mathf.Max(
                0.1f,
                currentAttackDelay
            );
    }


    /// <summary>
    /// SquadManager가 가지고 있는
    /// 현재 투사체 강화 레벨을 Soldier에 적용한다.
    ///
    /// Gate 등으로 새 병사가 추가되더라도
    /// 현재 강화 상태를 동일하게 적용할 수 있다.
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
    /// 현재 투사체 개수와 크기 배율을 계산한다.
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
    /// 최대 투사체 개수 이후 남는 강화 레벨은
    /// projectileScaleMultiplier 증가에 사용한다.
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
            Mathf.Max(
                0,
                upgradeLevel
            );


        // 투사체 개수를 증가시킬 수 있는 최대 강화 횟수
        //
        // 예:
        // 기본 1발 / 최대 5발
        //
        // 5 - 1 = 4
        int countUpgradeLimit =
            projectileUpgradeData.maxProjectileCount -
            projectileUpgradeData.baseProjectileCount;


        // 현재 강화 단계 중
        // 투사체 개수 증가에 사용할 단계 수
        int countUpgradeLevel =
            Mathf.Min(
                upgradeLevel,
                countUpgradeLimit
            );


        // 최종 투사체 개수 계산
        projectileCount =
            projectileUpgradeData.baseProjectileCount +
            countUpgradeLevel;


        // 최대 투사체 개수에 도달한 이후
        // 남아있는 강화 레벨 계산
        int scaleUpgradeLevel =
            Mathf.Max(
                0,
                upgradeLevel -
                countUpgradeLimit
            );


        // 최종 투사체 크기 배율 계산
        //
        // 현재 HyperCasualBulletFx에는 직접 적용하지 않으며,
        // 최종 업그레이드 이후 FX 크기 증가 기능은
        // 팀 회의 후 적용 여부를 결정한다.
        projectileScaleMultiplier =
            projectileUpgradeData.baseScaleMultiplier +
            scaleUpgradeLevel *
            projectileUpgradeData.scaleIncreasePerLevel;
    }


    /// <summary>
    /// SquadManager에서 기존 BulletPool을 전달한다.
    ///
    /// 현재 FireVisualBullet()은 사용하지 않고 있으므로
    /// Bullet 방식 정리 후 제거 가능하다.
    /// </summary>
    public void SetBulletPool(
        BulletPool pool)
    {
        bulletPool =
            pool;
    }


    /// <summary>
    /// SquadManager에서 관리하는
    /// Hyper Casual Bullet FX를 전달받는다.
    /// </summary>
    public void SetBulletFx(
        HyperCasualBulletFx fx)
    {
        bulletFx =
            fx;
    }


    /// <summary>
    /// Squad 전체가 공유하는
    /// 앞쪽 발사 기준점을 설정한다.
    /// </summary>
    public void SetFireLine(
        Transform line)
    {
        fireLine =
            line;
    }


    /// <summary>
    /// 실제 Raycast와 Bullet FX가 시작할 위치를 계산한다.
    ///
    /// 각 병사의 X 위치는 유지하고,
    /// Y / Z는 Squad의 FireLine 위치를 사용한다.
    ///
    /// 이렇게 하면 뒤쪽 대형에 있는 병사도
    /// Squad 앞쪽 기준에서 공격 판정을 시작할 수 있다.
    /// </summary>
    private Vector3 GetFireOrigin()
    {
        if (firePoint == null)
        {
            return transform.position;
        }

        Vector3 origin = firePoint.position;

        // 공통 사격선의 Z 위치만 사용하고
        // X / Y는 실제 병사의 FirePoint 위치를 유지한다.
        if (fireLine != null)
        {
            origin.z = fireLine.position.z;
        }

        return origin;
    }
}