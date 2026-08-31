using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [Header("강화 시스템 참조")]
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [Header("강화 진행 데이터")]
    [SerializeField]
    private UpgradeProgressionData progressionData;

    // 현재 강화 단계에서 처치한 적 수
    private int totalKills;

    // 현재 강화 단계
    // 0 = 첫 번째 강화
    // 1 = 두 번째 강화
    private int upgradeIndex;

    // 강화창이 열려 있고
    // 플레이어가 아직 강화를 선택하지 않은 상태인지 확인
    private bool isWaitingForUpgrade;

    public int UpgradeIndex => upgradeIndex;

    public bool IsWaitingForUpgrade => isWaitingForUpgrade;

    public int TotalKills => totalKills;
    public int RequiredKills => GetRequiredKills();

    public event System.Action<int, int> OnKillProgressChanged;

    /// <summary>
    /// 적이 사망했을 때 호출
    /// </summary>
    public void AddKill()
    {
        // 강화 선택 중이라면 추가 처리 방지
        if (isWaitingForUpgrade)
        {
            return;
        }

        // 총 처치 수 증가
        totalKills++;

        int requiredKills = GetRequiredKills();

        // UI 갱신
        OnKillProgressChanged?.Invoke(
            totalKills,
            requiredKills
        );

        Debug.Log(
            $"Kill Count : {totalKills} / {requiredKills}"
        );

        // 누적 처치 수가 현재 강화 조건에 도달했는지 확인
        if (totalKills >= requiredKills)
        {
            OpenUpgrade();
        }
    }

    /// <summary>
    /// 강화 조건을 달성했을 때 강화창을 연다.
    /// </summary>
    private void OpenUpgrade()
    {
        // 중복으로 강화창이 열리는 것 방지
        if (isWaitingForUpgrade)
        {
            return;
        }

        isWaitingForUpgrade = true;

        Debug.Log(
            $"[KillCounter] {upgradeIndex + 1}번째 강화 조건 달성"
        );

        if (upgradeManager == null)
        {
            Debug.LogError(
                "UpgradeManager_PlayerController가 연결되지 않았습니다."
            );

            isWaitingForUpgrade = false;
            return;
        }

        // 여기에서는 아직
        // currentKills 초기화와 upgradeIndex 증가를 하지 않는다.
        upgradeManager.OpenUpgradePanel();
    }

    /// <summary>
    /// 플레이어가 실제로 강화를 선택했을 때 호출
    /// </summary>
    public void CompleteUpgrade()
    {
        // 강화 대기 상태가 아니라면
        // 중복 호출 방지
        if (!isWaitingForUpgrade)
        {
            return;
        }

        // 실제 강화 선택이 끝났으므로
        // 다음 강화 단계로 이동
        upgradeIndex++;

        // 다시 킬 카운트 가능
        isWaitingForUpgrade = false;

#if UNITY_EDITOR
        Debug.Log(
            $"[KillCounter] 강화 선택 완료 / 다음 단계 : {upgradeIndex}"
        );
#endif
        OnKillProgressChanged?.Invoke(
            totalKills,
            GetRequiredKills()
        );
    }

    /// <summary>
    /// 현재 강화 단계에 필요한 처치 수 반환
    /// </summary>
    private int GetRequiredKills()
    {
        if (progressionData == null ||
            progressionData.requiredKills == null ||
            progressionData.requiredKills.Length == 0)
        {
            Debug.LogWarning(
                "UpgradeProgressionData가 설정되지 않았습니다."
            );

            return int.MaxValue;
        }

        // upgradeIndex가 배열 범위를 넘어가면
        // 마지막 필요 킬 수를 계속 사용한다.
        int index = Mathf.Min(
            upgradeIndex,
            progressionData.requiredKills.Length - 1
        );

        return progressionData.requiredKills[index];
    }
}