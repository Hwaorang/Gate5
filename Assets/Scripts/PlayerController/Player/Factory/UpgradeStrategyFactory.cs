public class UpgradeStrategyFactory
{
    private readonly SquadManager squadManager;
    private readonly PlayerController playerController;

    public UpgradeStrategyFactory(
        SquadManager squadManager,
        PlayerController playerController)
    {
        this.squadManager = squadManager;
        this.playerController = playerController;
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
                return new MoveSpeedUpgradeStrategy(playerController);

            default:
                return null;
        }
    }
}