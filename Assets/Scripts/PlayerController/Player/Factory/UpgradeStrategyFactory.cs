public class UpgradeStrategyFactory
{
    private readonly SquadManager squadManager;

    public UpgradeStrategyFactory(
        SquadManager squadManager
    )
    {
        this.squadManager = squadManager;
    }

    public IUpgradeStrategy Create(
        UpgradeType type
    )
    {
        switch (type)
        {
            case UpgradeType.Damage:
                return new DamageUpgradeStrategy(
                    squadManager
                );

            case UpgradeType.AttackSpeed:
                return new AttackSpeedUpgradeStrategy(
                    squadManager
                );

            case UpgradeType.SoldierCount:
                return new SoldierUpgradeStrategy(
                    squadManager
                );

            default:
                return null;
        }
    }
}