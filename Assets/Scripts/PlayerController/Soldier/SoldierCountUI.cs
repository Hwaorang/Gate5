using TMPro;
using UnityEngine;

/// <summary>
/// 현재 Squad의 병사 수를 UI에 표시한다.
///
/// 기존에는 Inspector에서 SquadManager를 직접 연결하고,
/// 이후 PlayerRoot가 동적으로 생성되면
/// PlayerContext를 통해 새로운 SquadManager를 전달받을 수 있다.
/// </summary>
public class SoldierCountUI : MonoBehaviour
{
    [Header("참조")]

    // 현재 병사 수와 병사 수 변경 이벤트를 제공하는 SquadManager
    // 현재 단계에서는 Inspector 연결도 유지한다.
    [SerializeField]
    private SquadManager squadManager;


    [Header("UI")]

    // 현재 병사 수 표시
    [SerializeField]
    private TMP_Text countText;


    private void OnEnable()
    {
        // 동적 Player 생성 방식에서는
        // OnEnable 시점에 아직 SquadManager가 전달되지 않았을 수도 있다.
        if (squadManager == null)
        {
            return;
        }

        // 혹시 중복 구독되어 있을 가능성을 방지
        squadManager.OnSoldierCountChanged -= UpdateUI;
        squadManager.OnSoldierCountChanged += UpdateUI;

        // 현재 병사 수 즉시 표시
        UpdateUI(
            squadManager.CurrentCount
        );
    }


    private void OnDisable()
    {
        if (squadManager == null)
        {
            return;
        }

        // 이벤트 연결 해제
        squadManager.OnSoldierCountChanged -= UpdateUI;
    }


    /// <summary>
    /// 동적으로 생성된 Player의 정보를 전달받는다.
    /// </summary>
    public void Initialize(PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[SoldierCountUI] PlayerContext가 없습니다."
            );

            return;
        }

        SetSquadManager(
            context.SquadManager
        );
    }


    /// <summary>
    /// 현재 사용 중인 SquadManager를 새로운 SquadManager로 교체한다.
    /// </summary>
    private void SetSquadManager(
        SquadManager newSquadManager)
    {
        // =========================
        // 기존 Manager 연결 해제
        // =========================

        if (squadManager != null)
        {
            squadManager.OnSoldierCountChanged -= UpdateUI;
        }


        // =========================
        // 새로운 Manager 저장
        // =========================

        squadManager = newSquadManager;

        if (squadManager == null)
        {
            Debug.LogWarning(
                "[SoldierCountUI] 전달받은 SquadManager가 없습니다."
            );

            return;
        }


        // =========================
        // 새로운 Manager 이벤트 연결
        // =========================

        // UI가 현재 활성화되어 있을 때만 이벤트를 구독한다.
        // 비활성 상태라면 나중에 OnEnable에서 구독한다.
        if (isActiveAndEnabled)
        {
            squadManager.OnSoldierCountChanged += UpdateUI;
        }


        // 새로운 SquadManager의 현재 값으로 즉시 갱신
        UpdateUI(
            squadManager.CurrentCount
        );
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