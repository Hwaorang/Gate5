public class MoveSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly PlayerStats playerStats;

    public MoveSpeedUpgradeStrategy(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void Apply(float value)
    {
        playerStats.UpgradeMoveSpeed(value);
    }
}