using TMPro;
using UnityEngine;

public class KillProgressUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private KillCounter killCounter;

    [SerializeField] private TMP_Text progressText;


    private void Start()
    {
        if (killCounter == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                "KillCounter가 연결되지 않았습니다."
            );
#endif

            return;
        }

        // 킬 진행도 변경 이벤트 구독
        killCounter.OnKillProgressChanged += UpdateUI;

        // 게임 시작 시 초기 UI 표시
        UpdateUI(
            killCounter.TotalKills,
            killCounter.RequiredKills
        );
    }


    /// <summary>
    /// 킬 진행도 UI 갱신
    /// </summary>
    private void UpdateUI(
        int currentKills,
        int requiredKills
    )
    {
        if (progressText == null)
        {
            return;
        }

        progressText.text =
            $"{currentKills} / {requiredKills}";
    }


    private void OnDestroy()
    {
        // 오브젝트가 사라질 때 이벤트 구독 해제
        if (killCounter != null)
        {
            killCounter.OnKillProgressChanged -= UpdateUI;
        }
    }
}