using UnityEngine;

/// <summary>
/// 로비에서 저장된 영구 강화 데이터를
/// 게임 시작 시 Player와 Squad 시스템에 적용한다.
///
/// 적용 대상
/// - 공격력
/// - 이동속도
/// - 공격속도
///
/// 로비에서는 강화 레벨만 저장하고,
/// 실제 게임에서는 LobbyUpgradeData의
/// 레벨당 증가량을 이용해 최종 배율로 변환한다.
/// </summary>
public class LobbyUpgradeApplier : MonoBehaviour
{
    [Header("참조")]

    // Player 이동속도 강화 적용 대상
    [SerializeField] private PlayerStats playerStats;

    // 병사 공격력 / 공격속도 강화 적용 대상
    [SerializeField] private SquadManager squadManager;


    [Header("로비 강화 데이터")]

    // 로비 강화 1레벨당 증가량을 가지고 있는 ScriptableObject
    //
    // 예:
    // damageIncreasePerLevel = 0.05
    // → 공격력 강화 1레벨당 +5%
    [SerializeField] private LobbyUpgradeData upgradeData;


    [Header("테스트용 값")]

    // SaveManager 없이 PlayerScene만 단독 실행할 때
    // 임시 강화 레벨을 사용할지 여부
    //
    // 실제 Lobby → Game 흐름에서는 false로 사용한다.
    [SerializeField] private bool useTestData = false;

    [SerializeField] private int testAttackLevel = 2;
    [SerializeField] private int testMoveSpeedLevel = 1;
    [SerializeField] private int testAttackSpeedLevel = 3;


    private void Start()
    {
        ApplyLobbyUpgrades();
    }


    /// <summary>
    /// 현재 로비 강화 레벨을 읽어서
    /// 실제 게임에서 사용할 배율로 변환한 뒤 적용한다.
    ///
    /// 처리 순서
    /// 1. LobbyUpgradeData 확인
    /// 2. 테스트 데이터 또는 SaveManager 데이터 읽기
    /// 3. 강화 레벨 → 배율 변환
    /// 4. SquadManager / PlayerStats에 적용
    /// </summary>
    private void ApplyLobbyUpgrades()
    {
        // 강화 수치 데이터가 없으면
        // 실제 배율을 계산할 수 없으므로 종료
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


        // =========================
        // 강화 레벨 가져오기
        // =========================

        if (useTestData)
        {
            // PlayerScene 단독 테스트 시
            // Inspector에 입력한 임시 강화 레벨 사용
            attackLevel =
                testAttackLevel;

            moveSpeedLevel =
                testMoveSpeedLevel;

            attackSpeedLevel =
                testAttackSpeedLevel;
        }
        else
        {
            // 실제 게임에서는
            // Lobby에서 저장한 PlayerData를 사용한다.
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning(
                    "[LobbyUpgradeApplier] SaveManager가 없습니다."
                );

                return;
            }


            PlayerData data =
                SaveManager.Instance.Data;


            // 저장 데이터가 없는 경우 방어
            if (data == null)
            {
                Debug.LogWarning(
                    "[LobbyUpgradeApplier] PlayerData가 없습니다."
                );

                return;
            }


            attackLevel =
                data.attackLevel;

            moveSpeedLevel =
                data.speedLevel;

            attackSpeedLevel =
                data.attackSpeedLevel;
        }


        // =========================
        // 강화 레벨 → 실제 배율 변환
        // =========================

        // 예:
        // Attack Lv.5
        // 레벨당 +5%
        //
        // 1 + 5 × 0.05
        // = 1.25배
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


        // =========================
        // 실제 Player 시스템에 적용
        // =========================

        // 병사 공격력 강화
        if (squadManager != null)
        {
            squadManager.SetLobbyDamageMultiplier(
                damageMultiplier
            );

            // 병사 공격속도 강화
            squadManager.SetLobbyAttackSpeedMultiplier(
                attackSpeedMultiplier
            );
        }


        // Player 이동속도 강화
        if (playerStats != null)
        {
            playerStats.SetLobbyMoveSpeedMultiplier(
                moveSpeedMultiplier
            );
        }
    }
}