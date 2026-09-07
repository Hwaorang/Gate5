using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PlayerExperience의 EXP와 Level 정보를
/// 화면 UI에 표시한다.
///
/// 주요 역할
/// - 현재 EXP / 필요 EXP 표시
/// - 현재 Level 표시
/// - EXP Slider 갱신
/// - 모든 강화 완료 시 MAX 표시
///
/// PlayerExperience의 OnExpChanged 이벤트를 구독해서
/// EXP가 변경될 때마다 자동으로 UI를 갱신한다.
/// </summary>
public class ExpProgressUI : MonoBehaviour
{
    [Header("참조")]

    // 현재 EXP, 필요 EXP, Level 등의 정보를 제공하는 PlayerExperience
    [SerializeField] private PlayerExperience playerExperience;


    [Header("UI")]

    // 현재 EXP / 필요 EXP 표시
    //
    // 예:
    // 5 / 10
    [SerializeField] private TMP_Text progressText;

    // 현재 Level 표시
    //
    // PlayerExperience 내부 Level은 0부터 시작하므로
    // 화면에서는 +1 해서 표시한다.
    [SerializeField] private TMP_Text levelText;

    // 현재 EXP 진행도를 0~1 사이 값으로 표시하는 Slider
    [SerializeField] private Slider expSlider;


    private void OnEnable()
    {
        if (playerExperience == null)
        {
            Debug.LogWarning(
                "[ExpProgressUI] PlayerExperience가 연결되지 않았습니다."
            );

            return;
        }

        // EXP 변경 이벤트 구독
        playerExperience.OnExpChanged +=
            UpdateUI;

        // UI가 활성화될 때
        // 현재 EXP 상태를 즉시 한 번 표시한다.
        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }


    private void OnDisable()
    {
        // UI가 비활성화될 때 이벤트 구독 해제
        if (playerExperience != null)
        {
            playerExperience.OnExpChanged -=
                UpdateUI;
        }
    }


    /// <summary>
    /// 현재 EXP / Level / Slider 상태를 갱신한다.
    /// </summary>
    private void UpdateUI(
        int currentExp,
        int requiredExp)
    {
        if (playerExperience == null)
        {
            return;
        }


        // =========================
        // Level 표시
        // =========================

        if (levelText != null)
        {
            // 내부 Level은 0부터 시작하므로
            // 사용자에게는 Lv.1부터 보이도록 +1
            levelText.text =
                $"Lv. {playerExperience.Level + 1}";
        }


        // =========================
        // 모든 강화 완료
        // =========================

        if (playerExperience.IsAllUpgradesCompleted)
        {
            if (progressText != null)
            {
                progressText.text =
                    "MAX";
            }

            if (expSlider != null)
            {
                expSlider.value =
                    1f;
            }

            if (levelText != null)
            {
                levelText.text =
                    $"Lv. {playerExperience.Level + 1} MAX";
            }

            return;
        }


        // =========================
        // EXP Text
        // =========================

        if (progressText != null)
        {
            progressText.text =
                $"{currentExp} / {requiredExp}";
        }


        // =========================
        // EXP Slider
        // =========================

        if (expSlider != null)
        {
            // 잘못된 필요 EXP 값 방어
            if (requiredExp <= 0)
            {
                expSlider.value =
                    0f;
            }
            else
            {
                float progress =
                    (float)currentExp /
                    requiredExp;

                // EXP가 필요 EXP를 초과해도
                // Slider 값은 0~1 사이로 유지
                expSlider.value =
                    Mathf.Clamp01(progress);
            }
        }
    }
}