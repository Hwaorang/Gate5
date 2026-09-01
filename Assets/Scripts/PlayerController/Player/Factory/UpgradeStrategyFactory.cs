public class UpgradeStrategyFactory
{
    private readonly SquadManager squadManager;
    private readonly PlayerStats playerStats;

    public UpgradeStrategyFactory(
    SquadManager squadManager,
    PlayerStats playerStats)
    {
        this.squadManager = squadManager;
        this.playerStats = playerStats;
    }

    public IUpgradeStrategy Create(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage:
                return new DamageUpgradeStrategy(squadManager);

            case UpgradeType.AttackSpeed:
                return new AttackSpeedUpgradeStrategy(squadManager);

            case UpgradeType.ProjectileCount:
                return new ProjectileUpgradeStrategy(squadManager);

            case UpgradeType.MoveSpeed:
                return new MoveSpeedUpgradeStrategy(playerStats);

            default:
                return null;
        }
    }
}