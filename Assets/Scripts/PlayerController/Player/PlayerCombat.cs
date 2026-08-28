using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("적 탐색")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        Transform target = FindNearestEnemy();

        SetTargetToSoldiers(target);
    }

    /// <summary>
    /// 공격 범위 내 가장 가까운 Enemy 탐색
    /// </summary>
    private Transform FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer
        );

        Transform nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemies)
        {
            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    /// <summary>
    /// 현재 모든 병사에게 공격 대상 전달
    /// </summary>
    private void SetTargetToSoldiers(Transform target)
    {
        SoldierAttack[] soldiers =
            GetComponentsInChildren<SoldierAttack>();

        foreach (SoldierAttack soldier in soldiers)
        {
            soldier.SetTarget(target);
        }
    }
}