public class AttackSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public AttackSpeedUpgradeStrategy(
        SquadManager squadManager
    )
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
        squadManager.UpgradeAllSoldierAttackSpeed(value);
    }
}