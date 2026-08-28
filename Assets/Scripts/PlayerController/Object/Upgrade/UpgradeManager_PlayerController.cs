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

    private void CreateUpgradeButtons()
    {
        ClearButtons();

        foreach (UpgradeData data in upgradeDatas)
        {
            UpgradeButton button =
                Instantiate(upgradeButtonPrefab, content);

            button.Setup(
                data,
                this
            );

            createdButtons.Add(button);
        }
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
}