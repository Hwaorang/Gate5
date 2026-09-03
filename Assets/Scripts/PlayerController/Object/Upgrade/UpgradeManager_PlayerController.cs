using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeManager_PlayerController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Transform content;
    [SerializeField] private UpgradeButton upgradeButtonPrefab;

    [Header("Upgrade Data")]
    [SerializeField] private List<UpgradeData> upgradeDatas;

    [Header("Reference")]
    [SerializeField] private SquadManager squadManager;

    [Header("Experience")]
    [SerializeField]
    private PlayerExperience playerExperience;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private KillCounter killCounter;

    private Dictionary<UpgradeType, int> upgradeLevels
    = new Dictionary<UpgradeType, int>();

    private UpgradeStrategyFactory strategyFactory;

    private readonly List<UpgradeButton> createdButtons
        = new List<UpgradeButton>();

    private void Awake()
    {
        strategyFactory =
            new UpgradeStrategyFactory(
                squadManager,
                playerStats
            );
    }

    private void Start()
    {
        upgradePanel.SetActive(false);
    }

    private void Update()
    {
        // 테스트용
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            OpenUpgradePanel();
        }
    }

    public void OpenUpgradePanel()
    {
        // 모든 강화가 최대 레벨이면
        // 강화창을 열지 않는다.
        if (!HasAvailableUpgrade())
        {
            string message =
                        "모든 강화가 최대 레벨입니다.";
#if UNITY_EDITOR
            Debug.Log(message);
#endif
            GameMessageUI.Instance?.ShowMessage(message);
            return;
        }

        CreateUpgradeButtons();

        upgradePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void ClearButtons()
    {
        foreach (UpgradeButton button in createdButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        createdButtons.Clear();
    }

    private int GetUpgradeLevel(UpgradeType type)
    {
        if (!upgradeLevels.TryGetValue(type, out int level))
        {
            return 0;
        }

        return level;
    }

    public void SelectUpgrade(UpgradeData data)
    {
#if UNITY_EDITOR

        Debug.Log("[SelectUpgrade 호출됨]");
#endif

        if (data == null)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log(
            $"[선택 데이터] " +
            $"Name={data.upgradeName}, " +
            $"Type={data.upgradeType}, " +
            $"Value={data.value}"
        );
#endif

        int currentLevel = GetUpgradeLevel(data.upgradeType);

        // 최대 강화 레벨 도달
        if (currentLevel >= data.maxLevel)
        {
            Debug.Log(
                $"{data.upgradeName}은 최대 레벨입니다."
            );

            return;
        }

        IUpgradeStrategy strategy =
            strategyFactory.Create(data.upgradeType);

        if (strategy == null)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log(
            $"[강화 선택 확인] " +
            $"Name : {data.upgradeName} / " +
            $"Type : {data.upgradeType} / " +
            $"Value : {data.value}"
        );
#endif

        // 강화 적용
        strategy.Apply(data.value);

        // 현재 강화 단계 증가
        upgradeLevels[data.upgradeType] =
            currentLevel + 1;

        playerExperience.CompleteLevelUp();

        CloseUpgradePanel();
    }

    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void CreateUpgradeButtons()
    {
        ClearButtons();

        List<UpgradeData> selectedUpgrades = GetRandomUpgrades(3);

        foreach (UpgradeData data in selectedUpgrades)
        {
            UpgradeButton button =
                Instantiate(upgradeButtonPrefab, content);

            int currentLevel = GetUpgradeLevel(data.upgradeType);

            button.Setup(
                data,
                this,
                currentLevel
            );

            createdButtons.Add(button);
        }
    }

    private List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> candidates =
            new List<UpgradeData>();

        // 전체 UpgradeData 중
        // 아직 최대 레벨에 도달하지 않은 것만 후보에 추가
        foreach (UpgradeData data in upgradeDatas)
        {
            if (data == null)
            {
                continue;
            }

            int currentLevel =
                GetUpgradeLevel(data.upgradeType);

            // 최대 레벨이면 후보에서 제외
            if (currentLevel >= data.maxLevel)
            {
                continue;
            }

            candidates.Add(data);
        }

        List<UpgradeData> result =
            new List<UpgradeData>();

        // 후보 수보다 많이 뽑지 않도록 제한
        count = Mathf.Min(
            count,
            candidates.Count
        );

        // 중복 없이 랜덤 선택
        for (int i = 0; i < count; i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    candidates.Count
                );

            result.Add(
                candidates[randomIndex]
            );

            // 이미 뽑은 강화는 후보에서 제거
            candidates.RemoveAt(randomIndex);
        }

        return result;
    }

    /// <summary>
    /// 아직 최대 레벨에 도달하지 않은 강화가 있는지 확인한다.
    /// </summary>
    public bool HasAvailableUpgrade()
    {
        foreach (UpgradeData data in upgradeDatas)
        {
            if (data == null)
            {
                continue;
            }

            int currentLevel =
                GetUpgradeLevel(data.upgradeType);

            if (currentLevel < data.maxLevel)
            {
                return true;
            }
        }

        return false;
    }
}