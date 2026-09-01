using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpProgressUI : MonoBehaviour
{
    // 플레이어의 경험치 정보를 가져오기 위한 참조
    [SerializeField] private PlayerExperience playerExperience;

    [Header("UI")]
    // 현재 경험치 / 필요 경험치를 표시하는 텍스트
    [SerializeField] private TMP_Text progressText;
    //현재 레벨을 표시하는 텍스트
    [SerializeField] private TMP_Text levelText;

    // 경험치 진행도를 표시하는 슬라이더
    [SerializeField] private Slider expSlider;

    private void Start()
    {
        // PlayerExperience가 연결되지 않았다면
        // 경험치 정보를 가져올 수 없으므로 종료
        if (playerExperience == null)
        {
            Debug.LogWarning(
                "[ExpProgressUI] PlayerExperience가 연결되지 않았습니다."
            );

            return;
        }

        // 경험치가 변경될 때마다 UpdateUI가 호출되도록 이벤트 등록
        playerExperience.OnExpChanged += UpdateUI;

        // 게임 시작 시 현재 경험치 상태를 한 번 표시
        UpdateUI(
            playerExperience.CurrentExp,
            playerExperience.RequiredExp
        );
    }

    /// <summary>
    /// 경험치 텍스트와 슬라이더를 갱신한다.
    /// </summary>
    private void UpdateUI(
    int currentExp,
    int requiredExp)
    {
        if (playerExperience == null)
        {
            return;
        }

        // 현재 레벨 표시
        if (levelText != null)
        {
            levelText.text =
                $"Lv. {playerExperience.Level + 1}";
        }

        // 모든 강화가 완료된 상태
        if (playerExperience.IsAllUpgradesCompleted)
        {
            if (progressText != null)
            {
                progressText.text = "MAX";
            }

            if (expSlider != null)
            {
                expSlider.value = 1f;
            }

            if (levelText != null)
            {
                levelText.text =
                    $"Lv. {playerExperience.Level + 1} MAX";
            }

            return;
        }

        // 경험치 텍스트
        if (progressText != null)
        {
            progressText.text =
                $"{currentExp} / {requiredExp}";
        }

        // 경험치 슬라이더
        if (expSlider != null)
        {
            if (requiredExp <= 0)
            {
                expSlider.value = 0f;
            }
            else
            {
                expSlider.value =
                    (float)currentExp / requiredExp;
            }
        }
    }

    private void OnDestroy()
    {
        // 이 오브젝트가 제거될 때 이벤트 등록 해제
        // 해제하지 않으면 삭제된 UI를 계속 호출하는 문제가 생길 수 있음
        if (playerExperience != null)
        {
            playerExperience.OnExpChanged -= UpdateUI;
        }
    }
}