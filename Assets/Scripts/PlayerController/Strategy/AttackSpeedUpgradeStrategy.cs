/// <summary>
/// 병사 전체의 공격속도 강화 Strategy.
/// </summary>
public class AttackSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public AttackSpeedUpgradeStrategy(
        SquadManager squadManager)
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.UpgradeAllSoldierAttackSpeed(
            value
        );
    }
}