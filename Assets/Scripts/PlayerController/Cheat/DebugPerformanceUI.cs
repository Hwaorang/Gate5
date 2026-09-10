using TMPro;
using UnityEngine;

/// <summary>
/// 개발 중 성능과 Player 상태를 빠르게 확인하기 위한 Debug UI.
///
/// 표시 정보
/// - FPS
/// - 현재 병사 수
/// - 현재 Column 수
/// - 공격 간격
/// - 현재 EXP
///
/// 매 프레임 TMP_Text를 갱신하지 않고
/// 일정 주기마다 갱신해서 불필요한 UI 비용을 줄인다.
/// </summary>
public class DebugPerformanceUI : MonoBehaviour
{
    [Header("UI")]

    [SerializeField]
    private TMP_Text infoText;


    [Header("갱신 설정")]

    // 몇 초마다 UI를 갱신할지
    [SerializeField]
    private float refreshInterval = 0.25f;


    // 동적으로 생성된 Player에서 전달받는다.
    private SquadManager squadManager;
    private PlayerExperience playerExperience;


    // FPS 계산용
    private float refreshTimer;
    private float fps;


    /// <summary>
    /// 동적으로 생성된 Player의 시스템을 전달받는다.
    /// </summary>
    public void Initialize(
        SquadManager squad,
        PlayerExperience experience)
    {
        squadManager = squad;
        playerExperience = experience;

        RefreshUI();
    }


    private void Update()
    {
        // Time.timeScale = 0 상태에서도
        // Debug 정보가 갱신되도록 unscaledDeltaTime 사용
        refreshTimer +=
            Time.unscaledDeltaTime;

        if (refreshTimer <
            refreshInterval)
        {
            return;
        }


        // =========================
        // FPS 계산
        // =========================

        if (Time.unscaledDeltaTime > 0f)
        {
            fps =
                1f /
                Time.unscaledDeltaTime;
        }


        refreshTimer = 0f;

        RefreshUI();
    }


    /// <summary>
    /// 현재 Debug 정보를 화면에 표시한다.
    /// </summary>
    private void RefreshUI()
    {
        if (infoText == null)
        {
            return;
        }


        // =========================
        // Squad 정보
        // =========================

        int soldierCount = 0;
        int columnCount = 0;
        float attackDelay = 0f;


        if (squadManager != null)
        {
            soldierCount =
                squadManager.CurrentCount;

            columnCount =
                squadManager.CurrentColumnCount;

            attackDelay =
                GetCurrentAttackDelay();
        }


        // =========================
        // EXP 정보
        // =========================

        int currentExp = 0;
        int requiredExp = 0;
        int level = 1;


        if (playerExperience != null)
        {
            currentExp =
                playerExperience.CurrentExp;

            requiredExp =
                playerExperience.RequiredExp;

            level =
                playerExperience.CurrentLevel;
        }


        // =========================
        // UI 출력
        // =========================

        infoText.text =
            $"FPS : {fps:F0}\n" +
            $"Soldiers : {soldierCount}\n" +
            $"Columns : {columnCount}\n" +
            $"Attack Delay : {attackDelay:F2}s\n" +
            $"Level : {level}\n" +
            $"EXP : {currentExp} / {requiredExp}";
    }


    /// <summary>
    /// 현재 살아있는 병사 중 첫 SoldierAttack의
    /// 공격 간격을 가져온다.
    ///
    /// 모든 병사가 같은 강화 상태를 공유하므로
    /// 대표 하나의 AttackDelay만 확인한다.
    /// </summary>
    private float GetCurrentAttackDelay()
    {
        if (squadManager == null)
        {
            return 0f;
        }


        var attacks =
            squadManager.SoldierAttacks;


        if (attacks == null)
        {
            return 0f;
        }


        for (int i = 0;
             i < attacks.Count;
             i++)
        {
            SoldierAttack attack =
                attacks[i];

            if (attack == null ||
                !attack.CanAttack)
            {
                continue;
            }

            return attack.AttackDelay;
        }


        return 0f;
    }
}