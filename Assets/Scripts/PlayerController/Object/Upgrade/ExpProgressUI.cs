using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 현재 경험치와 레벨을
/// 화면에 표시하는 UI
/// </summary>
public class ExpProgressUI : MonoBehaviour
{
    [Header("참조")]

    // PlayerRoot에 붙어 있는 경험치 관리 스크립트
    [SerializeField]
    private PlayerExperience playerExperience;

    // 현재 경험치 / 필요 경험치 텍스트
    [SerializeField]
    private TMP_Text progressText;

    // 현재 레벨 표시
    [SerializeField]
    private TMP_Text levelText;

    // 경험치 게이지
    [SerializeField]
    private Slider expSlider;


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

        // 경험치가 변경될 때마다 UI 업데이트
        playerExperience.OnExpChanged += UpdateUI;

        // 게임 시작 시 초기 UI 표시
        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }


    /// <summary>
    /// 경험치와 레벨 UI를 갱신한다.
    /// </summary>
    private void UpdateUI(
        int currentExp,
        int requiredExp)
    {
        // 경험치 텍스트
        if (progressText != null)
        {
            progressText.text =
                $"{currentExp} / {requiredExp}";
        }

        // 경험치 Slider
        if (expSlider != null)
        {
            expSlider.minValue = 0f;
            expSlider.maxValue = requiredExp;
            expSlider.value = currentExp;
        }

        // 현재 레벨
        if (levelText != null)
        {
            levelText.text =
                $"Lv. {playerExperience.Level + 1}";
        }
    }


    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (playerExperience != null)
        {
            playerExperience.OnExpChanged -= UpdateUI;
        }
    }
}