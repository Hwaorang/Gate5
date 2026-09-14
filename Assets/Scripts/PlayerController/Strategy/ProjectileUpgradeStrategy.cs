/// <summary>
/// 병사의 투사체 강화 Strategy.
///
/// 현재는 value 값이 아니라
/// SquadManager 내부의 projectileUpgradeLevel을
/// 1단계 증가시키는 방식으로 동작한다.
/// </summary>
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
        if (squadManager == null)
        {
            return;
        }

        squadManager.UpgradeAllSoldierProjectile();
    }
}