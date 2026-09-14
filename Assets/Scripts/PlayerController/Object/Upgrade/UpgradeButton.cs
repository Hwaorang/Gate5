using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 강화 선택 UI의 버튼 하나를 담당한다.
///
/// 역할
/// - UpgradeData의 이름 / 설명 / 아이콘 / 강화 수치 표시
/// - 현재 강화 레벨과 최대 레벨 표시
/// - 버튼 클릭 시 UpgradeManager에 선택한 강화 데이터 전달
/// </summary>
public class UpgradeButton : MonoBehaviour
{
    [Header("UI")]

    // 강화 아이콘
    [SerializeField] private Image icon;

    // 강화 이름
    [SerializeField] private TMP_Text nameText;

    // 강화 설명
    [SerializeField] private TMP_Text descriptionText;

    // 실제 강화 수치 표시
    //
    // 예:
    // Damage +10%
    // Projectile +1
    [SerializeField] private TMP_Text valueText;

    // 현재 강화 레벨 / 최대 레벨 표시
    //
    // 예:
    // Lv. 2 / 5
    [SerializeField] private TMP_Text levelText;


    // 현재 버튼이 가지고 있는 강화 데이터
    private UpgradeData upgradeData;

    // 버튼 클릭 시 강화 적용을 요청할 Manager
    private UpgradeManager_PlayerController upgradeManager;

    // 같은 버튼이 짧은 시간에 여러 번 눌리는 것을 방지
    private bool isClicked;


    /// <summary>
    /// 강화 버튼에 표시할 정보를 설정한다.
    ///
    /// UpgradeManager에서 버튼을 생성한 뒤 호출하며,
    /// UpgradeData의 내용을 UI에 반영한다.
    /// </summary>
    public void Setup(
        UpgradeData data,
        UpgradeManager_PlayerController manager,
        int currentLevel)
    {
        // 데이터가 없으면 버튼 내용을 설정할 수 없다.
        if (data == null)
        {
            Debug.LogWarning(
                "[UpgradeButton] UpgradeData가 없습니다."
            );

            return;
        }

        upgradeData =
            data;

        upgradeManager =
            manager;

        // 재사용되거나 다시 Setup될 수 있으므로
        // 클릭 상태 초기화
        isClicked =
            false;


        // 아이콘 설정
        if (icon != null)
        {
            icon.sprite =
                data.icon;
        }


        // 강화 이름 설정
        if (nameText != null)
        {
            nameText.text =
                data.upgradeName;
        }


        // 강화 설명 설정
        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }


        // 강화 수치 표시
        if (valueText != null)
        {
            valueText.text =
                GetValueText(data);
        }


        // 현재 강화 레벨 / 최대 레벨 표시
        if (levelText != null)
        {
            levelText.text =
                $"Lv. {currentLevel} / {data.maxLevel}";
        }
    }


    /// <summary>
    /// 강화 버튼을 클릭했을 때 호출한다.
    ///
    /// 현재 버튼이 가지고 있는 UpgradeData를
    /// UpgradeManager에 전달해서 실제 강화 적용을 요청한다.
    /// </summary>
    public void OnClick()
    {
        // 이미 한 번 클릭한 버튼이라면
        // 중복 적용하지 않는다.
        if (isClicked)
        {
            return;
        }


        // 필요한 참조가 없다면 처리하지 않는다.
        if (upgradeData == null ||
            upgradeManager == null)
        {
            return;
        }


        isClicked =
            true;


        // 실제 강화 적용은 UpgradeManager가 담당한다.
        upgradeManager.SelectUpgrade(
            upgradeData
        );
    }


    /// <summary>
    /// UpgradeType에 따라
    /// 버튼에 표시할 강화 수치를 문자열로 변환한다.
    ///
    /// Damage / AttackSpeed / MoveSpeed는
    /// 0.1 -> +10% 형태로 표시하고,
    ///
    /// ProjectileCount는
    /// +1 형태로 표시한다.
    /// </summary>
    private string GetValueText(
        UpgradeData data)
    {
        if (data == null)
        {
            return "";
        }

        switch (data.upgradeType)
        {
            case UpgradeType.Damage:
                return
                    $"+{data.value * 100f:0}%";

            case UpgradeType.AttackSpeed:
                return
                    $"+{data.value * 100f:0}%";

            case UpgradeType.MoveSpeed:
                return
                    $"+{data.value * 100f:0}%";

            case UpgradeType.ProjectileCount:
                return
                    $"+{data.value:0}";

            default:
                return "";
        }
    }
}