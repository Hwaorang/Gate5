using TMPro;
using UnityEngine;

public class SoldierCountUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SquadManager squadManager;

    [Header("UI")]
    [SerializeField] private TMP_Text countText;

    private void Start()
    {
        if (squadManager == null)
        {
            Debug.LogWarning(
                "[SoldierCountUI] SquadManager가 연결되지 않았습니다."
            );

            return;
        }

        // 병사 수가 변경될 때 UI 갱신
        squadManager.OnSoldierCountChanged += UpdateUI;

        // 게임 시작 시 현재 병사 수 표시
        UpdateUI(squadManager.CurrentCount);
    }

    /// <summary>
    /// 현재 병사 수를 화면에 표시한다.
    /// </summary>
    private void UpdateUI(int count)
    {
        if (countText == null)
        {
            return;
        }

        countText.text = $"Soldier : {count}";
    }

    private void OnDestroy()
    {
        // 오브젝트 삭제 시 이벤트 연결 해제
        if (squadManager != null)
        {
            squadManager.OnSoldierCountChanged -= UpdateUI;
        }
    }
}