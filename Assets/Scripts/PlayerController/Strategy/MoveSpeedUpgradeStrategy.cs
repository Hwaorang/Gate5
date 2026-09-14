/// <summary>
/// Player의 이동속도 강화 Strategy.
/// 실제 이동속도 값은 PlayerStats가 관리한다.
/// </summary>
public class MoveSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly PlayerStats playerStats;

    public MoveSpeedUpgradeStrategy(
        PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void Apply(float value)
    {
        if (playerStats == null)
        {
            return;
        }

        playerStats.UpgradeMoveSpeed(
            value
        );
    }
}