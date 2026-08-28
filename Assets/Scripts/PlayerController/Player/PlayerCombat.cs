using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float attackDelay = 1f;

    [Header("적 설정")]
    [SerializeField] private LayerMask enemyLayer;

    private float attackTimer;

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // 공격 가능한 시간이 되면 적 탐색
        if (attackTimer >= attackDelay)
        {
            Transform target = FindNearestEnemy();

            if (target != null)
            {
                Fire(target);
                attackTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 공격 범위 안에서 가장 가까운 적을 찾는다.
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
    /// 공격 실행.
    /// 현재는 테스트용 로그만 출력하고,
    /// 나중에 총알 생성 기능을 추가할 예정.
    /// </summary>
    private void Fire(Transform target)
    {
        SoldierAttack[] soldiers =
        GetComponentsInChildren<SoldierAttack>();

        foreach (SoldierAttack soldier in soldiers)
        {
            soldier.Fire(target);
        }

        // 이후 추가 예정
        // 총알 생성
        // 총구 위치 설정
        // 총알 방향 설정
        // 공격 애니메이션 실행
    }
}