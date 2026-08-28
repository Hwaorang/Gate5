using UnityEngine;

public class SoldierAttack : MonoBehaviour
{
    [Header("총알 설정")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("공격 스탯")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float baseAttackDelay = 1f;

    private float currentDamage;
    private float currentAttackDelay;

    private float attackTimer;
    private Transform target;

    public float CurrentDamage => currentDamage;
    public float CurrentAttackDelay => currentAttackDelay;

    private void Awake()
    {
        currentDamage = baseDamage;
        currentAttackDelay = baseAttackDelay;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= currentAttackDelay)
        {
            Fire();

            attackTimer = 0f;
        }
    }

    /// <summary>
    /// PlayerCombat에서 현재 공격 대상을 전달받는다.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Fire()
    {
        if (target == null)
        {
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bulletScript =
            bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Init(
                target,
                currentDamage
            );
        }
    }

    /// <summary>
    /// 현재 공격력 배율 적용
    /// </summary>
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage =
            baseDamage * multiplier;
    }

    /// <summary>
    /// 현재 공격속도 배율 적용
    /// </summary>
    public void SetAttackSpeedMultiplier(float multiplier)
    {
        currentAttackDelay =
            baseAttackDelay / multiplier;

        currentAttackDelay =
            Mathf.Max(0.1f, currentAttackDelay);
    }
}