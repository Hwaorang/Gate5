using UnityEngine;

public class DamageUpgradeStrategy : IUpgradeStrategy
{
    private readonly SquadManager squadManager;

    public DamageUpgradeStrategy(
        SquadManager squadManager
    )
    {
        this.squadManager = squadManager;
    }

    public void Apply(float value)
    {
#if UNITY_EDITOR
        Debug.Log(
            $"[DamageStrategy 진입] value = {value}"
        );
#endif

        squadManager.UpgradeAllSoldierDamage(value);
    }
}