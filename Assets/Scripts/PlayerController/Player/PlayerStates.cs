using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatData baseData;

    public float AttackDamage { get; private set; }
    public float AttackDelay { get; private set; }
    public float MoveSpeed { get; private set; }

    public int StartSoldierCount => baseData.startSoldierCount;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        AttackDamage = baseData.attackDamage;
        AttackDelay = baseData.attackDelay;
        MoveSpeed = baseData.moveSpeed;
    }

    public void IncreaseDamage(float percent)
    {
        AttackDamage += AttackDamage * percent;
    }

    public void IncreaseAttackSpeed(float percent)
    {
        AttackDelay -= AttackDelay * percent;

        AttackDelay = Mathf.Max(
            0.1f,
            AttackDelay
        );
    }

    public void IncreaseMoveSpeed(float percent)
    {
        MoveSpeed += MoveSpeed * percent;
    }

    /// <summary>
    /// 이동속도를 퍼센트 단위로 증가시킨다.
    /// 예: 0.1f = 10% 증가
    /// </summary>
    public void UpgradeMoveSpeed(float percent)
    {
        MoveSpeed *= 1f + percent;
    }
}