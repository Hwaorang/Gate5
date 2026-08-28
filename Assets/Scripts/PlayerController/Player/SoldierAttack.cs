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

    public float CurrentDamage => currentDamage;
    public float CurrentAttackDelay => currentAttackDelay;

    private void Awake()
    {
        currentDamage = baseDamage;
        currentAttackDelay = baseAttackDelay;
    }

    public void Fire(Transform target)
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

    public void SetDamageMultiplier(float multiplier)
    {
        currentDamage =
            baseDamage * multiplier;
    }

    public void SetAttackSpeedMultiplier(float multiplier)
    {
        currentAttackDelay =
            baseAttackDelay / multiplier;

        currentAttackDelay =
            Mathf.Max(0.1f, currentAttackDelay);
    }
}