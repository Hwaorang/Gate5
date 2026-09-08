using TMPro;
using UnityEngine;

/// <summary>
/// 현재 Squad의 병사 수를 UI에 표시한다.
///
/// SquadManager의 OnSoldierCountChanged 이벤트를 구독해서
/// 병사 수가 바뀔 때마다 자동으로 UI를 갱신한다.
/// </summary>
public class SoldierCountUI : MonoBehaviour
{
    [Header("참조")]

    // 현재 병사 수와 병사 수 변경 이벤트를 제공하는 SquadManager
    [SerializeField] private SquadManager squadManager;


    [Header("UI")]

    // 현재 병사 수를 표시할 TextMeshPro 텍스트
    [SerializeField] private TMP_Text countText;


    private void OnEnable()
    {
        if (squadManager == null)
        {
            Debug.LogWarning(
                "[SoldierCountUI] SquadManager가 연결되지 않았습니다."
            );

            return;
        }

        // 병사 수 변경 이벤트 구독
        squadManager.OnSoldierCountChanged +=
            UpdateUI;

        // UI가 처음 활성화될 때
        // 현재 병사 수도 즉시 한 번 표시한다.
        UpdateUI(
            squadManager.CurrentCount
        );
    }


    private void OnDisable()
    {
        // UI가 비활성화될 때 이벤트 연결 해제
        //
        // 이벤트를 계속 구독한 상태로 남겨두면
        // 비활성 UI에도 UpdateUI가 호출될 수 있기 때문에
        // OnDisable에서 해제한다.
        if (squadManager != null)
        {
            squadManager.OnSoldierCountChanged -=
                UpdateUI;
        }
    }


    /// <summary>
    /// 전달받은 현재 병사 수를 화면에 표시한다.
    /// </summary>
    private void UpdateUI(
        int count)
    {
        if (countText == null)
        {
            return;
        }

        countText.text =
            $"Soldier : {count}";
    }
}