using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSkinTestController : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private TMP_Text attackLevelText;
    [SerializeField] private TMP_Text attackCostText;
    [SerializeField] private Button attackButton;

    [Header("Speed")]
    [SerializeField] private TMP_Text speedLevelText;
    [SerializeField] private TMP_Text speedCostText;
    [SerializeField] private Button speedButton;

    [Header("Attack Speed")]
    [SerializeField] private TMP_Text attackSpeedLevelText;
    [SerializeField] private TMP_Text attackSpeedCostText;
    [SerializeField] private Button attackSpeedButton;

    [Header("Gold")]
    [SerializeField] private TMP_Text goldLevelText;
    [SerializeField] private TMP_Text goldCostText;
    [SerializeField] private Button goldButton;

    private int attackLevel;
    private int speedLevel;
    private int attackSpeedLevel;
    private int goldLevel;

    private void Start()
    {
        attackButton.onClick.AddListener(UpgradeAttack);
        speedButton.onClick.AddListener(UpgradeSpeed);
        attackSpeedButton.onClick.AddListener(UpgradeAttackSpeed);
        goldButton.onClick.AddListener(UpgradeGold);

        RefreshUI();
    }

    private void UpgradeAttack()
    {
        attackLevel++;
        RefreshUI();

        Debug.Log($"Attack Upgrade : Lv.{attackLevel}");
    }

    private void UpgradeSpeed()
    {
        speedLevel++;
        RefreshUI();

        Debug.Log($"Speed Upgrade : Lv.{speedLevel}");
    }

    private void UpgradeAttackSpeed()
    {
        attackSpeedLevel++;
        RefreshUI();

        Debug.Log($"AttackSpeed Upgrade : Lv.{attackSpeedLevel}");
    }

    private void UpgradeGold()
    {
        goldLevel++;
        RefreshUI();

        Debug.Log($"Gold Upgrade : Lv.{goldLevel}");
    }

    private void RefreshUI()
    {
        attackLevelText.text = $"Lv.{attackLevel}";
        attackCostText.text = $"{100 + attackLevel * 50} G";

        speedLevelText.text = $"Lv.{speedLevel}";
        speedCostText.text = $"{100 + speedLevel * 50} G";

        attackSpeedLevelText.text = $"Lv.{attackSpeedLevel}";
        attackSpeedCostText.text = $"{100 + attackSpeedLevel * 50} G";

        goldLevelText.text = $"Lv.{goldLevel}";
        goldCostText.text = $"{100 + goldLevel * 50} G";
    }
}