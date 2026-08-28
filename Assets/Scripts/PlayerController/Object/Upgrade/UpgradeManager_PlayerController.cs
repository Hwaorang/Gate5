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

    private UpgradeStrategyFactory strategyFactory;

    private readonly List<UpgradeButton> createdButtons
        = new List<UpgradeButton>();

    private void Awake()
    {
        strategyFactory =
            new UpgradeStrategyFactory(squadManager);
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

    public void SelectUpgrade(UpgradeData data)
    {
        IUpgradeStrategy strategy =
            strategyFactory.Create(data.upgradeType);

        if (strategy == null)
        {
            Debug.LogWarning(
                $"Upgrade Strategy 없음 : {data.upgradeType}"
            );

            return;
        }

        strategy.Apply(data.value);

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