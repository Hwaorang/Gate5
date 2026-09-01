using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // =====================================================
    // 공격력 강화 설정
    // =====================================================

    [Header("공격력 강화")]
    [SerializeField] private int attackBaseCost = 100;
    [SerializeField] private int attackCostIncrease = 50;
    [SerializeField] private int attackMaxLevel = 10;

    // 레벨 1당 증가하는 공격력
    [SerializeField] private int attackIncrease = 5;


    // =====================================================
    // 이동속도 강화 설정
    // =====================================================

    [Header("이동속도 강화")]
    [SerializeField] private int speedBaseCost = 100;
    [SerializeField] private int speedCostIncrease = 50;
    [SerializeField] private int speedMaxLevel = 10;

    // 레벨 1당 증가하는 이동속도
    [SerializeField] private float speedIncrease = 0.5f;


    // =====================================================
    // 공격속도 강화 설정
    // =====================================================

    [Header("공격속도 강화")]
    [SerializeField] private int attackSpeedBaseCost = 100;
    [SerializeField] private int attackSpeedCostIncrease = 50;
    [SerializeField] private int attackSpeedMaxLevel = 10;

    // 레벨 1당 증가하는 공격속도
    [SerializeField] private float attackSpeedIncrease = 0.1f;


    // =====================================================
    // 스테이지 클리어 골드 강화 설정
    // =====================================================

    [Header("스테이지 골드 획득량 강화")]
    [SerializeField] private int goldRewardBaseCost = 200;
    [SerializeField] private int goldRewardCostIncrease = 100;
    [SerializeField] private int goldRewardMaxLevel = 10;

    // 레벨 1당 추가되는 골드
    [SerializeField] private int goldRewardIncrease = 10;


    // =====================================================
    // UI
    // =====================================================

    [Header("골드 UI")]
    [SerializeField] private TMP_Text goldText;


    [Header("공격력 UI")]
    [SerializeField] private TMP_Text attackLevelText;
    [SerializeField] private TMP_Text attackCostText;


    [Header("이동속도 UI")]
    [SerializeField] private TMP_Text speedLevelText;
    [SerializeField] private TMP_Text speedCostText;


    [Header("공격속도 UI")]
    [SerializeField] private TMP_Text attackSpeedLevelText;
    [SerializeField] private TMP_Text attackSpeedCostText;


    [Header("스테이지 골드 UI")]
    [SerializeField] private TMP_Text goldRewardLevelText;
    [SerializeField] private TMP_Text goldRewardCostText;


    private void Start()
    {
        RefreshUI();
    }


    // =====================================================
    // 공격력 강화
    // =====================================================

    public void UpgradeAttack()
    {
        PlayerData data = SaveManager.Instance.Data;

        // 최대 레벨 검사
        if (data.attackLevel >= attackMaxLevel)
        {
            Debug.Log("공격력이 최대 레벨입니다.");
            return;
        }

        int cost = GetAttackCost();

        // 골드 사용
        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        // 레벨 증가
        data.attackLevel++;

        // 저장
        SaveManager.Instance.Save();

        // UI 갱신
        RefreshUI();
    }


    // =====================================================
    // 이동속도 강화
    // =====================================================

    public void UpgradeSpeed()
    {
        PlayerData data = SaveManager.Instance.Data;

        if (data.speedLevel >= speedMaxLevel)
        {
            Debug.Log("이동속도가 최대 레벨입니다.");
            return;
        }

        int cost = GetSpeedCost();

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        data.speedLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }


    // =====================================================
    // 공격속도 강화
    // =====================================================

    public void UpgradeAttackSpeed()
    {
        PlayerData data = SaveManager.Instance.Data;

        if (data.attackSpeedLevel >= attackSpeedMaxLevel)
        {
            Debug.Log("공격속도가 최대 레벨입니다.");
            return;
        }

        int cost = GetAttackSpeedCost();

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        data.attackSpeedLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }


    // =====================================================
    // 스테이지 골드 획득량 강화
    // =====================================================

    public void UpgradeGoldReward()
    {
        PlayerData data = SaveManager.Instance.Data;

        if (data.goldRewardLevel >= goldRewardMaxLevel)
        {
            Debug.Log("스테이지 골드 획득량이 최대 레벨입니다.");
            return;
        }

        int cost = GetGoldRewardUpgradeCost();

        if (!SaveManager.Instance.SpendGold(cost))
        {
            Debug.Log("골드가 부족합니다.");
            return;
        }

        data.goldRewardLevel++;

        SaveManager.Instance.Save();

        RefreshUI();
    }


    // =====================================================
    // 공격력 강화 비용
    // =====================================================

    public int GetAttackCost()
    {
        int level = SaveManager.Instance.Data.attackLevel;

        return attackBaseCost +
               level * attackCostIncrease;
    }


    // =====================================================
    // 이동속도 강화 비용
    // =====================================================

    public int GetSpeedCost()
    {
        int level = SaveManager.Instance.Data.speedLevel;

        return speedBaseCost +
               level * speedCostIncrease;
    }


    // =====================================================
    // 공격속도 강화 비용
    // =====================================================

    public int GetAttackSpeedCost()
    {
        int level =
            SaveManager.Instance.Data.attackSpeedLevel;

        return attackSpeedBaseCost +
               level * attackSpeedCostIncrease;
    }


    // =====================================================
    // 스테이지 골드 강화 비용
    // =====================================================

    public int GetGoldRewardUpgradeCost()
    {
        int level =
            SaveManager.Instance.Data.goldRewardLevel;

        return goldRewardBaseCost +
               level * goldRewardCostIncrease;
    }


    // =====================================================
    // UI 갱신
    // =====================================================

    public void RefreshUI()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        PlayerData data = SaveManager.Instance.Data;


        // -------------------------
        // 골드
        // -------------------------

        if (goldText != null)
        {
            goldText.text = data.gold.ToString();
        }


        // -------------------------
        // 공격력
        // -------------------------

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


        // -------------------------
        // 이동속도
        // -------------------------

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


        // -------------------------
        // 공격속도
        // -------------------------

        if (attackSpeedLevelText != null)
        {
            attackSpeedLevelText.text =
                "Lv. " + data.attackSpeedLevel;
        }

        if (attackSpeedCostText != null)
        {
            if (data.attackSpeedLevel >= attackSpeedMaxLevel)
            {
                attackSpeedCostText.text = "MAX";
            }
            else
            {
                attackSpeedCostText.text =
                    GetAttackSpeedCost() + " G";
            }
        }


        // -------------------------
        // 스테이지 골드
        // -------------------------

        if (goldRewardLevelText != null)
        {
            goldRewardLevelText.text =
                "Lv. " + data.goldRewardLevel;
        }

        if (goldRewardCostText != null)
        {
            if (data.goldRewardLevel >= goldRewardMaxLevel)
            {
                goldRewardCostText.text = "MAX";
            }
            else
            {
                goldRewardCostText.text =
                    GetGoldRewardUpgradeCost() + " G";
            }
        }
    }


    // =====================================================
    // 실제 능력치 계산
    // =====================================================

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


    public float GetAttackSpeed()
    {
        return 1f +
               SaveManager.Instance.Data.attackSpeedLevel *
               attackSpeedIncrease;
    }


    // =====================================================
    // 스테이지 클리어 골드 계산
    // =====================================================

    public int GetStageGold()
    {
        return 100 +
               SaveManager.Instance.Data.goldRewardLevel *
               goldRewardIncrease;
    }
}