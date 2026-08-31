using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [Header("강화 시스템 참조")]

    // 처치 조건을 만족했을 때 강화창을 열기 위한 매니저
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;


    [Header("강화 진행 데이터")]

    // 강화 단계별 필요한 처치 수를 가지고 있는 ScriptableObject
    [SerializeField]
    private UpgradeProgressionData progressionData;


    // 현재 강화 단계에서 처치한 적의 수
    private int currentKills;

    // 현재 몇 번째 강화를 진행 중인지 나타내는 인덱스
    // 0 = 첫 번째 강화
    // 1 = 두 번째 강화
    // 2 = 세 번째 강화
    private int upgradeIndex;


    // 외부 UI 등에서 현재 처치 수를 확인할 수 있도록 Property 제공
    public int CurrentKills => currentKills;


    /// <summary>
    /// 적이 죽었을 때 호출
    /// 현재 처치 수를 증가시키고 강화 조건을 확인한다.
    /// </summary>
    public void AddKill()
    {
        // 적 1마리 처치
        currentKills++;


        // 현재 강화 단계에서 필요한 처치 수 가져오기
        int requiredKills = GetRequiredKills();

#if UNITY_EDITOR
        // 테스트용 로그
        Debug.Log(
            $"Kill Count : {currentKills} / {requiredKills}"
        );
#endif


        // 현재 처치 수가 필요한 처치 수 이상이라면
        if (currentKills >= requiredKills)
        {
            // 다음 강화 조건을 위해 현재 킬 수 초기화
            currentKills = 0;


            // 다음 강화 단계로 이동
            upgradeIndex++;


            // 강화 선택창 열기
            upgradeManager.OpenUpgradePanel();
        }
    }


    /// <summary>
    /// 현재 강화 단계에서 필요한 처치 수를 반환한다.
    /// </summary>
    private int GetRequiredKills()
    {
        // ProgressionData가 연결되지 않았거나
        // requiredKills 배열이 비어있는 경우 예외 처리
        if (progressionData == null ||
            progressionData.requiredKills == null ||
            progressionData.requiredKills.Length == 0)
        {
            Debug.LogWarning(
                "UpgradeProgressionData가 설정되지 않았습니다."
            );


            // 사실상 강화가 발생하지 않도록 매우 큰 값 반환
            return int.MaxValue;
        }


        // 현재 upgradeIndex가 배열 범위를 넘어가지 않도록 제한
        //
        // 예:
        // 배열 길이 = 5
        // upgradeIndex가 7이어도 마지막 값인 index 4를 사용
        int index = Mathf.Min(
            upgradeIndex,
            progressionData.requiredKills.Length - 1
        );


        // 현재 단계에 필요한 처치 수 반환
        return progressionData.requiredKills[index];
    }
}