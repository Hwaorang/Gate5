using System;
using UnityEngine;

/// <summary>
/// Player의 경험치와 레벨업 흐름을 관리한다.
///
/// 주요 역할
/// - Enemy 처치 등으로 EXP 획득
/// - 현재 EXP와 필요 EXP 비교
/// - 레벨업 조건 달성 시 강화창 오픈
/// - 강화 선택 완료 후 EXP 차감 및 Level 증가
/// - 모든 강화가 최대 레벨이면 EXP 시스템 종료
/// - EXP UI 갱신 이벤트 전달
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    [Header("강화 시스템")]

    // 레벨업 조건을 만족했을 때
    // 강화 선택 UI를 열기 위한 UpgradeManager 참조
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;


    [Header("경험치 데이터")]

    // 레벨별 필요 경험치를 가지고 있는 ScriptableObject
    //
    // 예:
    // Lv.0 -> 10 EXP
    // Lv.1 -> 20 EXP
    // Lv.2 -> 30 EXP
    [SerializeField]
    private ExperienceProgressionData progressionData;


    // =========================
    // 현재 EXP 상태
    // =========================

    // 현재 보유 중인 EXP
    private int currentExp;

    // 현재 Player 레벨
    //
    // 0부터 시작하며,
    // CompleteLevelUp()이 호출될 때 1씩 증가한다.
    private int level;

    // true인 동안에도 EXP는 계속 누적하지만,
    // 새로운 강화창이 중복으로 열리는 것은 방지한다.
    private bool isWaitingForUpgrade;

    // 모든 강화가 최대 레벨에 도달했는지 여부
    //
    // true가 되면 더 이상 EXP를 획득하지 않는다.
    private bool isAllUpgradesCompleted;


    /// <summary>
    /// 모든 강화가 최대 레벨인지 확인할 때 사용한다.
    /// EXP UI에서 MAX 상태를 표시할 때도 사용할 수 있다.
    /// </summary>
    public bool IsAllUpgradesCompleted =>
        isAllUpgradesCompleted;


    /// <summary>
    /// 현재 보유 EXP.
    /// </summary>
    public int CurrentExp =>
        currentExp;


    /// <summary>
    /// 현재 레벨에서 다음 레벨업에 필요한 EXP.
    /// </summary>
    public int RequiredExp =>
        GetRequiredExp();


    /// <summary>
    /// 현재 Player 레벨.
    /// </summary>
    public int Level =>
        level;

    /// <summary>
    /// UI에서 사용할 현재 플레이어 레벨.
    ///
    /// 내부 level이 0부터 시작하므로
    /// 실제 표시 레벨은 +1 한다.
    /// </summary>
    public int CurrentLevel => level + 1;


    /// <summary>
    /// EXP가 변경됐을 때 UI에 전달하는 이벤트.
    ///
    /// 첫 번째 값 = 현재 EXP
    /// 두 번째 값 = 필요 EXP
    /// </summary>
    public event Action<int, int> OnExpChanged;


    /// <summary>
    /// Enemy 처치 등으로 EXP를 획득할 때 호출한다.
    ///
    /// EXP를 추가한 뒤 UI를 갱신하고
    /// 레벨업 조건을 만족했는지 확인한다.
    /// </summary>
    public void AddExp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (isAllUpgradesCompleted)
        {
            return;
        }

        // 강화창이 열려 있어도 EXP 자체는 누적한다.
        currentExp += amount;

        OnExpChanged?.Invoke(
            currentExp,
            GetRequiredExp()
        );

        // 이미 강화 선택 중이라면
        // 새로운 강화창은 열지 않는다.
        if (isWaitingForUpgrade)
        {
            return;
        }

        CheckLevelUp();
    }


    /// <summary>
    /// 현재 EXP가 레벨업 조건을 만족했는지 확인한다.
    ///
    /// 조건을 만족하면:
    /// 1. 사용 가능한 강화가 있는지 확인
    /// 2. 모든 강화가 MAX라면 EXP 시스템 종료
    /// 3. 아니라면 강화 선택 대기 상태로 변경
    /// 4. UpgradePanel 오픈
    /// </summary>
    private void CheckLevelUp()
    {
        int requiredExp =
            GetRequiredExp();


        // 아직 필요한 EXP에 도달하지 않았다면 종료
        if (currentExp < requiredExp)
        {
            return;
        }


        // UpgradeManager가 없으면
        // 강화창을 열 수 없으므로 종료
        if (upgradeManager == null)
        {
            Debug.LogWarning(
                "[PlayerExperience] UpgradeManager가 연결되지 않았습니다."
            );

            return;
        }


        // 모든 강화가 최대 레벨이면
        // 더 이상 레벨업/EXP 진행을 하지 않는다.
        if (!upgradeManager.HasAvailableUpgrade())
        {
            isAllUpgradesCompleted =
                true;


            // UI에서 EXP Bar가 꽉 찬 상태로 보이도록
            // 현재 EXP를 필요 EXP와 동일하게 맞춘다.
            currentExp =
                requiredExp;


            // EXP UI 갱신
            OnExpChanged?.Invoke(
                currentExp,
                requiredExp
            );


            // Player에게 모든 강화가 완료됐음을 알린다.
            GameMessageUI.Instance?.ShowMessage(
                "모든 강화가 최대 레벨입니다."
            );

            return;
        }


        // 강화 선택이 완료될 때까지
        // 추가 EXP 획득을 막는다.
        isWaitingForUpgrade =
            true;


        // 강화 선택 UI 오픈
        upgradeManager.OpenUpgradePanel();
    }


    /// <summary>
    /// 강화 선택이 완료됐을 때
    /// UpgradeManager에서 호출한다.
    ///
    /// 처리 순서
    /// 1. 현재 레벨의 필요 EXP 차감
    /// 2. Level 증가
    /// 3. 강화 대기 상태 해제
    /// 4. 다음 레벨 기준으로 EXP UI 갱신
    /// </summary>
    public void CompleteLevelUp()
    {
        // 강화 선택 대기 상태가 아니라면
        // 중복 레벨업 처리를 하지 않는다.
        if (!isWaitingForUpgrade)
        {
            return;
        }


        int requiredExp =
            GetRequiredExp();


        // 현재 레벨업에 필요한 EXP만 차감한다.
        //
        // 초과 EXP가 있었다면 남은 값은 유지된다.
        currentExp -=
            requiredExp;


        // Player 레벨 증가
        level++;


        // 강화 선택 완료
        isWaitingForUpgrade =
            false;


        // 다음 레벨의 필요 EXP 기준으로
        // UI를 다시 갱신한다.
        OnExpChanged?.Invoke(
            currentExp,
            GetRequiredExp()
        );
    }


    /// <summary>
    /// 현재 레벨에서 필요한 EXP를 반환한다.
    ///
    /// progressionData가 없거나 데이터가 비어있으면
    /// 레벨업이 발생하지 않도록 int.MaxValue를 반환한다.
    ///
    /// 현재 level이 배열의 마지막 인덱스를 넘어가더라도
    /// 마지막 필요 EXP 값을 계속 사용한다.
    /// </summary>
    private int GetRequiredExp()
    {
        if (progressionData == null ||
            progressionData.requiredExp == null ||
            progressionData.requiredExp.Length == 0)
        {
            return int.MaxValue;
        }


        // 레벨이 배열 범위를 벗어나지 않도록 제한
        int index =
            Mathf.Min(
                level,
                progressionData.requiredExp.Length - 1
            );


        return
            progressionData.requiredExp[index];
    }

    public void CheckPendingLevelUp()
    {
        if (isWaitingForUpgrade ||
            isAllUpgradesCompleted)
        {
            return;
        }

        CheckLevelUp();
    }

    /// <summary>
    /// 동적으로 생성된 PlayerExperience에
    /// Scene의 UpgradeManager를 전달한다.
    /// </summary>
    public void SetUpgradeManager(
        UpgradeManager_PlayerController manager)
    {
        upgradeManager = manager;
    }
}