/// <summary>
/// 병사 전체의 공격력 강화 Strategy.
/// 실제 강화 처리는 SquadManager가 담당한다.
/// </summary>
public class DamageUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public DamageUpgradeStrategy(
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

        squadManager.UpgradeAllSoldierDamage(
            value
        );
    }
}