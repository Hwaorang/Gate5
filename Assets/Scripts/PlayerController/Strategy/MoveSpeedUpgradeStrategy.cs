using UnityEngine;

public class MoveSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly PlayerStats playerStats;

    public MoveSpeedUpgradeStrategy(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void Apply(float value)
    {
        // PlayerStats가 전달되지 않은 경우
        // NullReferenceException 대신 원인을 알려준다.
        if (playerStats == null)
        {
#if UNITY_EDITOR
            Debug.LogError(
                "[MoveSpeedUpgradeStrategy] PlayerStats가 연결되지 않았습니다."
            );
#endif
            return;
        }

        playerStats.UpgradeMoveSpeed(value);
    }
}