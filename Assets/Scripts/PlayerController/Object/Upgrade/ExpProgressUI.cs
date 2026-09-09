using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// PlayerExperience의 EXP와 Level 정보를
/// 화면 UI에 표시한다.
///
/// 현재는 Inspector에서 PlayerExperience를 연결해서 사용할 수 있고,
/// 이후 PlayerRoot가 동적으로 생성되면
/// PlayerContext를 통해 새로운 PlayerExperience를 전달받을 수 있다.
/// </summary>
public class ExpProgressUI : MonoBehaviour
{
    [Header("참조")]

    // 현재 EXP 정보를 제공하는 PlayerExperience
    // 동적 Player 전환 전까지는 기존 Inspector 연결을 유지한다.
    [SerializeField]
    private PlayerExperience playerExperience;


    [Header("UI")]

    // 현재 EXP / 필요 EXP 표시
    [SerializeField]
    private TMP_Text progressText;

    // 현재 Level 표시
    [SerializeField]
    private TMP_Text levelText;

    // EXP 진행도 표시
    [SerializeField]
    private Slider expSlider;


    private void Awake()
    {
        SetupSlider();
    }


    private void OnEnable()
    {
        // 동적 Player 방식에서는
        // OnEnable 시점에 아직 PlayerExperience가 없을 수도 있다.
        if (playerExperience == null)
        {
            return;
        }

        SubscribePlayerExperience();

        // 현재 상태 즉시 표시
        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }


    private void OnDisable()
    {
        UnsubscribePlayerExperience();
    }


    /// <summary>
    /// 동적으로 생성된 Player의 정보를 전달받는다.
    /// </summary>
    public void Initialize(PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[ExpProgressUI] PlayerContext가 없습니다."
            );

            return;
        }

        SetPlayerExperience(
            context.PlayerExperience
        );
    }


    /// <summary>
    /// 현재 사용하는 PlayerExperience를
    /// 새로운 PlayerExperience로 교체한다.
    /// </summary>
    private void SetPlayerExperience(
        PlayerExperience newPlayerExperience)
    {
        // =========================
        // 기존 이벤트 연결 해제
        // =========================

        UnsubscribePlayerExperience();


        // =========================
        // 새로운 PlayerExperience 저장
        // =========================

        playerExperience =
            newPlayerExperience;


        if (playerExperience == null)
        {
            Debug.LogWarning(
                "[ExpProgressUI] 전달받은 PlayerExperience가 없습니다."
            );

            return;
        }


        // =========================
        // 새로운 이벤트 연결
        // =========================

        if (isActiveAndEnabled)
        {
            SubscribePlayerExperience();
        }


        // =========================
        // 현재 EXP 즉시 표시
        // =========================

        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }


    /// <summary>
    /// PlayerExperience의 EXP 변경 이벤트를 구독한다.
    /// </summary>
    private void SubscribePlayerExperience()
    {
        if (playerExperience == null)
        {
            return;
        }

        // 중복 구독 방지
        playerExperience.OnExpChanged -= UpdateUI;
        playerExperience.OnExpChanged += UpdateUI;
    }


    /// <summary>
    /// PlayerExperience의 EXP 변경 이벤트 구독을 해제한다.
    /// </summary>
    private void UnsubscribePlayerExperience()
    {
        if (playerExperience == null)
        {
            return;
        }

        playerExperience.OnExpChanged -= UpdateUI;
    }


    /// <summary>
    /// EXP Slider는 표시 전용으로 사용한다.
    ///
    /// 키보드 이동 입력으로 Slider 값이
    /// 변경되는 것을 방지한다.
    /// </summary>
    private void SetupSlider()
    {
        if (expSlider == null)
        {
            return;
        }

        // 사용자가 직접 조작하지 못하도록 설정
        expSlider.interactable = false;

        // A/D, 방향키 등의 UI Navigation 차단
        Navigation navigation =
            expSlider.navigation;

        navigation.mode =
            Navigation.Mode.None;

        expSlider.navigation =
            navigation;
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
            if (requiredExp <= 0)
            {
                expSlider.value =
                    0f;

                return;
            }

            float progress =
                (float)currentExp /
                requiredExp;

            expSlider.value =
                Mathf.Clamp01(progress);
        }
    }
}