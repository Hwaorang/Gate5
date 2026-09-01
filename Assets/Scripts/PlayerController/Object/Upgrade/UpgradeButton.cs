using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valueText;

    // 현재 강화 레벨 표시
    [SerializeField] private TMP_Text levelText;

    private UpgradeData upgradeData;
    private UpgradeManager_PlayerController upgradeManager;

    /// <summary>
    /// 강화 버튼에 표시할 정보를 설정한다.
    /// </summary>
    public void Setup(
        UpgradeData data,
        UpgradeManager_PlayerController manager,
        int currentLevel)
    {
        upgradeData = data;
        upgradeManager = manager;

        if (icon != null)
        {
            icon.sprite = data.icon;
        }

        if (nameText != null)
        {
            nameText.text = data.upgradeName;
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.description;
        }

        if (valueText != null)
        {
            valueText.text = GetValueText(data);
        }

        // 현재 레벨 / 최대 레벨 표시
        if (levelText != null)
        {
            levelText.text =
                $"Lv. {currentLevel} / {data.maxLevel}";
        }
    }

    public void OnClick()
    {
        if (upgradeData == null ||
            upgradeManager == null)
        {
            return;
        }

        upgradeManager.SelectUpgrade(upgradeData);
    }

    private string GetValueText(UpgradeData data)
    {
        switch (data.upgradeType)
        {
            case UpgradeType.Damage:
                return $"+{data.value * 100f:0}%";

            case UpgradeType.AttackSpeed:
                return $"+{data.value * 100f:0}%";

            case UpgradeType.MoveSpeed:
                return $"+{data.value * 100f:0}%";

            case UpgradeType.ProjectileCount:
                return $"+{data.value:0}";

            default:
                return "";
        }
    }
}