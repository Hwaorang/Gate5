using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valueText;

    private UpgradeData data;
    private UpgradeManager_PlayerController manager;

    public void Setup(
        UpgradeData upgradeData,
        UpgradeManager_PlayerController upgradeManager)
    {
        data = upgradeData;
        manager = upgradeManager;

        nameText.text = data.upgradeName;
        descriptionText.text = data.description;

        if (icon != null)
        {
            icon.sprite = data.icon;
        }

        if (valueText != null)
        {
            valueText.text = GetValueText(data);
        }
    }

    private string GetValueText(UpgradeData data)
    {
        switch (data.upgradeType)
        {
            case UpgradeType.Damage:
                return $"+{data.value * 100f:0}%";

            case UpgradeType.AttackSpeed:
                return $"+{data.value * 100f:0}%";

            case UpgradeType.SoldierCount:
                return $"+{data.value:0}";

            default:
                return "";
        }
    }

    public void OnClick()
    {
        if (data == null || manager == null)
        {
            return;
        }

        manager.SelectUpgrade(data);
    }
}