public class DamageUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public DamageUpgradeStrategy(
        SquadManager squadManager
    )
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
        squadManager.UpgradeAllSoldierDamage(value);
    }
}