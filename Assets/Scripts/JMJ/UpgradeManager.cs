using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Header("공격력 강화")]
    [SerializeField] private int attackBaseCost = 100;
    [SerializeField] private int attackCostIncrease = 50;
    [SerializeField] private int attackMaxLevel = 10;
    [SerializeField] private int attackIncrease = 5;

    [Header("이동속도 강화")]
    [SerializeField] private int speedBaseCost = 100;
    [SerializeField] private int speedCostIncrease = 50;
    [SerializeField] private int speedMaxLevel = 10;
    [SerializeField] private float speedIncrease = 0.5f;

    [Header("체력 강화")]
    [SerializeField] private int hpBaseCost = 100;
    [SerializeField] private int hpCostIncrease = 50;
    [SerializeField] private int hpMaxLevel = 10;
    [SerializeField] private int hpIncrease = 20;

    [Header("UI")]
    [SerializeField] private TMP_Text goldText;

    [SerializeField] private TMP_Text attackLevelText;
    [SerializeField] private TMP_Text attackCostText;

    [SerializeField] private TMP_Text speedLevelText;
    [SerializeField] private TMP_Text speedCostText;

    [SerializeField] private TMP_Text hpLevelText;
    [SerializeField] private TMP_Text hpCostText;

    private void Start()
    {
        RefreshUI();
    }

    // =========================
    // 공격력 강화
    // =========================

    public void UpgradeAttack()
    {
        int level = SaveManager.Instance.Data.attackLevel;

        if (level >= attackMaxLevel)
        {
            Debug.Log("공격력이 최대 레벨입니다.");
            return;
        }

        int cost = attackBaseCost +
                   level * attackCostIncrease;

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        SaveManager.Instance.Data.attackLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }

    // =========================
    // 이동속도 강화
    // =========================

    public void UpgradeSpeed()
    {
        int level = SaveManager.Instance.Data.speedLevel;

        if (level >= speedMaxLevel)
        {
            Debug.Log("이동속도가 최대 레벨입니다.");
            return;
        }

        int cost = speedBaseCost +
                   level * speedCostIncrease;

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        SaveManager.Instance.Data.speedLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }

    // =========================
    // 체력 강화
    // =========================

    public void UpgradeHP()
    {
        int level = SaveManager.Instance.Data.hpLevel;

        if (level >= hpMaxLevel)
        {
            Debug.Log("체력이 최대 레벨입니다.");
            return;
        }

        int cost = hpBaseCost +
                   level * hpCostIncrease;

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        SaveManager.Instance.Data.hpLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }

    // =========================
    // UI 갱신
    // =========================

    public void RefreshUI()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        PlayerData data = SaveManager.Instance.Data;

        // 골드
        if (goldText != null)
        {
            goldText.text = data.gold.ToString();
        }

        // 공격력
        if (attackLevelText != null)
        {
            attackLevelText.text =
                "Lv. " + data.attackLevel;
        }

        if (attackCostText != null)
        {
            if (data.attackLevel >= attackMaxLevel)
            {
                attackCostText.text = "MAX";
            }
            else
            {
                attackCostText.text =
                    GetAttackCost() + " G";
            }
        }

        // 이동속도
        if (speedLevelText != null)
        {
            speedLevelText.text =
                "Lv. " + data.speedLevel;
        }

        if (speedCostText != null)
        {
            if (data.speedLevel >= speedMaxLevel)
            {
                speedCostText.text = "MAX";
            }
            else
            {
                speedCostText.text =
                    GetSpeedCost() + " G";
            }
        }

        // 체력
        if (hpLevelText != null)
        {
            hpLevelText.text =
                "Lv. " + data.hpLevel;
        }

        if (hpCostText != null)
        {
            if (data.hpLevel >= hpMaxLevel)
            {
                hpCostText.text = "MAX";
            }
            else
            {
                hpCostText.text =
                    GetHPCost() + " G";
            }
        }
    }

    public int GetAttackCost()
    {
        int level = SaveManager.Instance.Data.attackLevel;

        return attackBaseCost +
               level * attackCostIncrease;
    }

    public int GetSpeedCost()
    {
        int level = SaveManager.Instance.Data.speedLevel;

        return speedBaseCost +
               level * speedCostIncrease;
    }

    public int GetHPCost()
    {
        int level = SaveManager.Instance.Data.hpLevel;

        return hpBaseCost +
               level * hpCostIncrease;
    }

    // =========================
    // 실제 능력치 가져오기
    // =========================

    public int GetAttack()
    {
        return 10 +
               SaveManager.Instance.Data.attackLevel *
               attackIncrease;
    }

    public float GetSpeed()
    {
        return 5f +
               SaveManager.Instance.Data.speedLevel *
               speedIncrease;
    }

    public int GetMaxHP()
    {
        return 100 +
               SaveManager.Instance.Data.hpLevel *
               hpIncrease;
    }
}