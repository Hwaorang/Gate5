/// <summary>
/// UpgradeType에 맞는 강화 Strategy를 생성하는 Factory.
///
/// UpgradeManager는 강화 종류만 넘기고,
/// 실제 강화 로직은 각 Strategy가 담당한다.
///
/// 예:
/// Damage          -> DamageUpgradeStrategy
/// AttackSpeed     -> AttackSpeedUpgradeStrategy
/// ProjectileCount -> ProjectileUpgradeStrategy
/// MoveSpeed       -> MoveSpeedUpgradeStrategy
/// </summary>
public class UpgradeStrategyFactory
{
    // 병사 관련 강화에 필요한 SquadManager
    private readonly SquadManager squadManager;

    // Player 이동속도 강화에 필요한 PlayerStats
    private readonly PlayerStats playerStats;


    /// <summary>
    /// 각 Strategy가 사용할 시스템 참조를 전달받는다.
    /// </summary>
    public UpgradeStrategyFactory(
        SquadManager squadManager,
        PlayerStats playerStats)
    {
        this.squadManager =
            squadManager;

        this.playerStats =
            playerStats;
    }


    /// <summary>
    /// 전달받은 UpgradeType에 맞는 Strategy를 생성한다.
    ///
    /// 지원하지 않는 타입이 들어오면 null을 반환한다.
    /// </summary>
    public IUpgradeStrategy Create(
        UpgradeType type)
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

            case UpgradeType.ProjectileCount:
                return new ProjectileUpgradeStrategy(
                    squadManager
                );

            case UpgradeType.MoveSpeed:
                return new MoveSpeedUpgradeStrategy(
                    playerStats
                );

            default:
                return null;
        }
    }
}