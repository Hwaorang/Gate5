using TMPro;
using UnityEngine;

/// <summary>
/// 현재 경험치 / 다음 레벨업 필요 경험치를
/// 화면에 표시하는 UI
/// </summary>
public class ExpProgressUI : MonoBehaviour
{
    [Header("참조")]

    // PlayerRoot에 붙어 있는 경험치 관리 스크립트
    [SerializeField]
    private PlayerExperience playerExperience;

    // 경험치 진행도를 표시할 TMP Text
    [SerializeField]
    private TMP_Text progressText;


    private void Start()
    {
        if (playerExperience == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                "PlayerExperience가 연결되지 않았습니다."
            );
#endif
            return;
        }

        // 경험치가 변경될 때마다 UI 갱신
        playerExperience.OnExpChanged += UpdateUI;

        // 게임 시작 시 초기 경험치 표시
        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }


    /// <summary>
    /// 경험치 진행도 UI 갱신
    /// </summary>
    private void UpdateUI(
        int currentExp,
        int requiredExp
    )
    {
        if (progressText == null)
        {
            return;
        }

        progressText.text =
            $"{currentExp} / {requiredExp}";
    }


    private void OnDestroy()
    {
        // 이벤트 중복 구독 방지를 위해 해제
        if (playerExperience != null)
        {
            playerExperience.OnExpChanged -= UpdateUI;
        }
    }
}