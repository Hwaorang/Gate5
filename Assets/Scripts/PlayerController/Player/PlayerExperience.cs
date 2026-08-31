using System;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Header("강화 시스템")]
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [Header("경험치 데이터")]
    [SerializeField]
    private ExperienceProgressionData progressionData;

    // 현재 누적 경험치
    private int currentExp;

    // 현재 레벨
    private int level;

    // 강화 선택을 기다리는 중인지 확인
    private bool isWaitingForUpgrade;

    public int CurrentExp => currentExp;

    public int RequiredExp => GetRequiredExp();

    public int Level => level;

    // 경험치 UI 갱신용 이벤트
    public event Action<int, int> OnExpChanged;

    /// <summary>
    /// 적 처치 등으로 경험치를 획득할 때 호출
    /// </summary>
    public void AddExp(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (isWaitingForUpgrade)
        {
            return;
        }

        currentExp += amount;

        int requiredExp = GetRequiredExp();

        // 경험치 UI 갱신
        OnExpChanged?.Invoke(
            currentExp,
            requiredExp
        );
#if UNITY_EDITOR
        Debug.Log(
            $"EXP : {currentExp} / {requiredExp}"
        );
#endif

        CheckLevelUp();
    }

    /// <summary>
    /// 현재 경험치가 레벨업 조건을 만족했는지 확인
    /// </summary>
    private void CheckLevelUp()
    {
        if (currentExp < GetRequiredExp())
        {
            return;
        }

        isWaitingForUpgrade = true;

        if (upgradeManager == null)
        {
            Debug.LogError(
                "UpgradeManager_PlayerController가 연결되지 않았습니다."
            );

            isWaitingForUpgrade = false;
            return;
        }

        upgradeManager.OpenUpgradePanel();
    }

    /// <summary>
    /// 강화 선택이 완료됐을 때 호출
    /// </summary>
    public void CompleteLevelUp()
    {
        if (!isWaitingForUpgrade)
        {
            return;
        }

        int requiredExp = GetRequiredExp();

        // 필요한 경험치만 차감
        currentExp -= requiredExp;

        level++;

        isWaitingForUpgrade = false;

        // 다음 레벨 필요 경험치로 UI 갱신
        OnExpChanged?.Invoke(
            currentExp,
            GetRequiredExp()
        );
#if UNITY_EDITOR
        Debug.Log(
            $"Level Up! 현재 레벨 : {level}"
        );
#endif
    }

    private int GetRequiredExp()
    {
        if (progressionData == null ||
            progressionData.requiredExp == null ||
            progressionData.requiredExp.Length == 0)
        {
            return int.MaxValue;
        }

        int index = Mathf.Min(
            level,
            progressionData.requiredExp.Length - 1
        );

        return progressionData.requiredExp[index];
    }
}