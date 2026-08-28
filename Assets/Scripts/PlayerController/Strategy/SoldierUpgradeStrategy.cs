public class SoldierUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public SoldierUpgradeStrategy(
        SquadManager squadManager
    )
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
        squadManager.AddUnit((int)value);
    }
}