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

    [Header("로비 강화 데이터")]
    [SerializeField] private LobbyUpgradeData upgradeData;

    [Header("테스트용 값")]
    [SerializeField] private bool useTestData = false;
    [SerializeField] private int testAttackLevel = 2;
    [SerializeField] private int testMoveSpeedLevel = 1;
    [SerializeField] private int testAttackSpeedLevel = 3;

    private void Start()
    {
        ApplyLobbyUpgrades();
    }

    private void ApplyLobbyUpgrades()
    {
        if (upgradeData == null)
        {
            Debug.LogWarning(
                "[LobbyUpgradeApplier] LobbyUpgradeData가 연결되지 않았습니다."
            );
            return;
        }

        int attackLevel;
        int moveSpeedLevel;
        int attackSpeedLevel;

        PlayerData data = null;

        // 테스트 데이터를 사용할 경우
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

        // 로비 강화 레벨을 실제 배율로 변환
        float damageMultiplier =
            1f +
            attackLevel *
            upgradeData.damageIncreasePerLevel;

        float moveSpeedMultiplier =
            1f +
            moveSpeedLevel *
            upgradeData.moveSpeedIncreasePerLevel;

        float attackSpeedMultiplier =
            1f +
            attackSpeedLevel *
            upgradeData.attackSpeedIncreasePerLevel;

        // 로비 전용 배율 적용
        squadManager?.SetLobbyDamageMultiplier(
            damageMultiplier
        );

        squadManager?.SetLobbyAttackSpeedMultiplier(
            attackSpeedMultiplier
        );

        playerStats?.SetLobbyMoveSpeedMultiplier(
            moveSpeedMultiplier
        );

#if UNITY_EDITOR
        Debug.Log(
            $"[Lobby 강화 적용] " +
            $"Damage x{damageMultiplier:F2} / " +
            $"MoveSpeed x{moveSpeedMultiplier:F2} / " +
            $"AttackSpeed x{attackSpeedMultiplier:F2}"
        );

        if (!useTestData && data != null)
        {
            Debug.Log(
                $"저장 데이터 / " +
                $"Attack Lv.{data.attackLevel} / " +
                $"MoveSpeed Lv.{data.speedLevel} / " +
                $"AttackSpeed Lv.{data.attackSpeedLevel} / " +
                $"난이도 {data.selectedDifficulty}"
            );
        }
#endif
    }
}