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

    [SerializeField]
    private KillCounter killCounter;

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
        if (data == null)
        {
            return;
        }

        IUpgradeStrategy strategy =
            strategyFactory.Create(data.upgradeType);

        if (strategy == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning(
                $"Upgrade Strategy 없음 : {data.upgradeType}"
            );
#endif

            return;
        }

        // 실제 강화 적용
        strategy.Apply(data.value);

        // 강화 선택이 정상적으로 완료된 시점에서
        // KillCounter의 다음 강화 단계 진행
        if (killCounter != null)
        {
            killCounter.CompleteUpgrade();
        }

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