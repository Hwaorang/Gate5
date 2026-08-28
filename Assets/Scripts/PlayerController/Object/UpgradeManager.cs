using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private SquadManager squadManager;

    private UpgradeStrategyFactory strategyFactory;

    private void Awake()
    {
        strategyFactory =
            new UpgradeStrategyFactory(squadManager);
    }

    public void OpenUpgradePanel()
    {
        upgradePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void SelectUpgrade(
        UpgradeData upgradeData
    )
    {
        if (upgradeData == null)
        {
            return;
        }

        IUpgradeStrategy strategy =
            strategyFactory.Create(
                upgradeData.upgradeType
            );

        if (strategy == null)
        {
            Debug.LogWarning(
                $"Strategy 없음 : {upgradeData.upgradeType}"
            );

            return;
        }

        strategy.Apply(upgradeData.value);

        CloseUpgradePanel();
    }

    private void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);

        Time.timeScale = 1f;
    }
}