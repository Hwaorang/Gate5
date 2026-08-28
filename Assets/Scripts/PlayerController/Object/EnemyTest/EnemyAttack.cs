using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float attackDelay = 1f;

    // 다음 공격이 가능한 시간
    private float nextAttackTime;

    private void OnTriggerStay(Collider other)
    {
        // 공격 쿨타임 중이면 무시
        if (Time.time < nextAttackTime)
        {
            return;
        }

        // 충돌한 병사 찾기
        SoldierUnit soldier =
            other.GetComponentInParent<SoldierUnit>();

        if (soldier == null)
        {
            return;
        }

        // 먼저 쿨타임 설정
        // 같은 프레임에 다른 Soldier가 들어와도 공격하지 못하게 함
        nextAttackTime = Time.time + attackDelay;

        Attack(soldier);
    }

    private void Attack(SoldierUnit soldier)
    {
#if UNITY_EDITOR
        Debug.Log($"좀비가 {soldier.name} 공격");
#endif
        soldier.Die();
    }
}