using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image icon;

    private UpgradeData upgradeData;
    private UpgradeManager upgradeManager;

    public void Setup(
        UpgradeData data,
        UpgradeManager manager
    )
    {
        upgradeData = data;
        upgradeManager = manager;

        nameText.text = data.upgradeName;
        descriptionText.text = data.description;

        if (icon != null)
        {
            icon.sprite = data.icon;
        }
    }

    public void OnClick()
    {
        if (upgradeManager == null ||
            upgradeData == null)
        {
            return;
        }

        upgradeManager.SelectUpgrade(
            upgradeData
        );
    }
}