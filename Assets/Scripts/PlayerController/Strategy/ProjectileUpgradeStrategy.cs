public class ProjectileUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public ProjectileUpgradeStrategy(
        SquadManager squadManager)
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
        squadManager.UpgradeAllSoldierProjectile();
    }
}