using UnityEngine;

/// <summary>
/// 로비에서 저장한 영구 강화 데이터를
/// 게임 시작 시 Player/Squad 시스템에 적용한다.
/// </summary>
public class LobbyUpgradeApplier : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SquadManager squadManager;

    [Header("레벨당 강화량")]
    [SerializeField] private float damageIncreasePerLevel = 0.05f;
    [SerializeField] private float moveSpeedIncreasePerLevel = 0.05f;
    [SerializeField] private float attackSpeedIncreasePerLevel = 0.05f;

    [Header("테스트용 값")]
    [SerializeField] private bool useTestData = true;
    [SerializeField] private int testAttackLevel = 2;
    [SerializeField] private int testMoveSpeedLevel = 1;
    [SerializeField] private int testAttackSpeedLevel = 3;

    private void Start()
    {
        ApplyLobbyUpgrades();
    }

    private void ApplyLobbyUpgrades()
    {
        int attackLevel;
        int moveSpeedLevel;
        int attackSpeedLevel;

        PlayerData data = null;

        if (useTestData)
        {
            attackLevel = testAttackLevel;
            moveSpeedLevel = testMoveSpeedLevel;
            attackSpeedLevel = testAttackSpeedLevel;
        }
        else
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning(
                    "[LobbyUpgradeApplier] SaveManager가 없습니다."
                );

                return;
            }

            data = SaveManager.Instance.Data;

            attackLevel = data.attackLevel;
            moveSpeedLevel = data.speedLevel;
            attackSpeedLevel = data.attackSpeedLevel;
        }

        float damageBonus =
            attackLevel * damageIncreasePerLevel;

        float moveSpeedBonus =
            moveSpeedLevel * moveSpeedIncreasePerLevel;

        float attackSpeedBonus =
            attackSpeedLevel * attackSpeedIncreasePerLevel;

        squadManager?.UpgradeAllSoldierDamage(
            damageBonus
        );

        squadManager?.UpgradeAllSoldierAttackSpeed(
            attackSpeedBonus
        );

        playerStats?.UpgradeMoveSpeed(
            moveSpeedBonus
        );

#if UNITY_EDITOR
        Debug.Log(
            $"Lobby Upgrade Test / " +
            $"Attack Lv.{attackLevel} / " +
            $"MoveSpeed Lv.{moveSpeedLevel} / " +
            $"AttackSpeed Lv.{attackSpeedLevel}"
        );

        // 실제 SaveManager 데이터를 사용하는 경우에만 출력
        if (!useTestData && data != null)
        {
            Debug.Log(
                $"[Lobby 적용] " +
                $"공격력 Lv.{data.attackLevel} / " +
                $"이동속도 Lv.{data.speedLevel} / " +
                $"공격속도 Lv.{data.attackSpeedLevel} / " +
                $"난이도 {data.selectedDifficulty}"
            );
        }

        if (playerStats != null)
        {
            Debug.Log(
                $"실제 MoveSpeed : {playerStats.MoveSpeed}"
            );
        }
#endif
    }
}