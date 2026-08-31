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
    private PlayerController playerController;

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
                playerController
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
        if (data == null)
        {
            return;
        }

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

            button.Setup(data, this);

            createdButtons.Add(button);
        }
    }

    private List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> candidates =
            new List<UpgradeData>(upgradeDatas);

        List<UpgradeData> result =
            new List<UpgradeData>();

        count = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex =
                Random.Range(0, candidates.Count);

            result.Add(candidates[randomIndex]);

            // 뽑은 데이터는 후보에서 제거
            // 같은 강화가 한 번에 중복 등장하는 것을 방지
            candidates.RemoveAt(randomIndex);
        }

        return result;
    }
}