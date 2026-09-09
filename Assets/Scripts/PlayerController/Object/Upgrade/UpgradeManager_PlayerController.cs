using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이 중 레벨업 시 표시되는 강화 선택 UI와
/// 실제 강화 적용 흐름을 관리한다.
///
/// 주요 역할
/// - 강화 패널 열기 / 닫기
/// - 현재 선택 가능한 강화 목록 필터링
/// - 중복 없이 랜덤 강화 3개 선택
/// - UpgradeButton 생성
/// - 강화 선택 시 Strategy를 통해 실제 능력치 적용
/// - 강화 레벨 기록
/// - PlayerExperience의 레벨업 완료 처리
///
/// 강화 적용 자체는 각 Strategy가 담당하고,
/// 이 클래스는 "어떤 강화를 보여주고 선택하게 할지"를 관리한다.
/// </summary>
public class UpgradeManager_PlayerController : MonoBehaviour
{
    [Header("UI")]

    // 강화 선택 UI 전체 패널
    [SerializeField] private GameObject upgradePanel;

    // 생성된 UpgradeButton이 들어갈 부모 Transform
    [SerializeField] private Transform content;

    // 강화 버튼 Prefab
    [SerializeField] private UpgradeButton upgradeButtonPrefab;


    [Header("Upgrade Data")]

    // 게임에서 사용할 전체 강화 데이터 목록
    //
    // 각 UpgradeData에는
    // - 이름
    // - UpgradeType
    // - 강화 값
    // - 최대 레벨
    // 등이 들어 있다.
    [SerializeField]
    private List<UpgradeData> upgradeDatas;


    [Header("Reference")]

    // 병사 공격력 / 공격속도 / 투사체 강화에 사용
    [SerializeField] private SquadManager squadManager;

    // 이동속도 강화에 사용
    [SerializeField] private PlayerStats playerStats;

    // EXP 레벨업 상태를 완료 처리하기 위한 참조
    [SerializeField] private PlayerExperience playerExperience;


    // =========================
    // 강화 진행 상태
    // =========================

    // UpgradeType별 현재 강화 레벨 저장
    //
    // 예:
    // Damage -> 3
    // AttackSpeed -> 2
    // ProjectileCount -> 4
    private readonly Dictionary<UpgradeType, int> upgradeLevels =
        new Dictionary<UpgradeType, int>();


    // UpgradeType에 맞는 Strategy를 생성하는 Factory
    private UpgradeStrategyFactory strategyFactory;


    // 현재 강화 패널에 생성되어 있는 버튼 목록
    //
    // 다음 레벨업 때 기존 버튼을 정리하기 위해 저장한다.
    private readonly List<UpgradeButton> createdButtons =
        new List<UpgradeButton>();


    private void Awake()
    {
        CreateStrategyFactory();
    }

    /// <summary>
    /// 동적으로 생성된 PlayerRoot의 시스템들을 전달받는다.
    ///
    /// 기존 Inspector 참조 대신
    /// PlayerContext 내부의 실제 Player 컴포넌트를 사용한다.
    /// </summary>
    public void Initialize(
        PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[UpgradeManager] PlayerContext가 없습니다."
            );

            return;
        }

        // =========================
        // 새로운 Player 참조 적용
        // =========================

        squadManager =
            context.SquadManager;

        playerStats =
            context.PlayerStats;

        playerExperience =
            context.PlayerExperience;


        // =========================
        // StrategyFactory 재생성
        // =========================

        // 기존 Factory는 이전 Player 참조를 가지고 있을 수 있으므로
        // 새로운 Player 기준으로 다시 만든다.
        CreateStrategyFactory();
    }

    /// <summary>
    /// 현재 연결되어 있는 Player 시스템을 사용해서
    /// UpgradeStrategyFactory를 생성한다.
    ///
    /// Player가 동적으로 교체되었을 경우에도
    /// 다시 호출해서 새로운 Player를 기준으로
    /// Strategy를 생성할 수 있다.
    /// </summary>
    private void CreateStrategyFactory()
    {
        if (squadManager == null ||
            playerStats == null)
        {
            // 동적 Player 생성 방식에서는
            // Awake 시점에 아직 참조가 없을 수도 있으므로
            // 바로 에러 처리하지 않고 Factory 생성을 미룬다.
            strategyFactory = null;

            return;
        }

        strategyFactory =
            new UpgradeStrategyFactory(
                squadManager,
                playerStats
            );
    }


    private void Start()
    {
        // 게임 시작 시 강화 패널이 열려 있지 않도록 한다.
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }


    /// <summary>
    /// 강화 패널을 연다.
    ///
    /// 아직 선택 가능한 강화가 존재하면
    /// 랜덤 강화 버튼을 생성하고 게임을 일시정지한다.
    /// </summary>
    public void OpenUpgradePanel()
    {
        if (upgradePanel == null)
        {
            Debug.LogWarning(
                "[UpgradeManager] UpgradePanel이 연결되지 않았습니다."
            );

            return;
        }

        // 모든 강화가 최대 레벨이면
        // 더 이상 강화창을 열지 않는다.
        if (!HasAvailableUpgrade())
        {
            string message =
                "모든 강화가 최대 레벨입니다.";

            GameMessageUI.Instance?.ShowMessage(
                message
            );

            return;
        }

        // 현재 선택 가능한 강화 버튼 생성
        CreateUpgradeButtons();

        // 강화 패널 표시
        upgradePanel.SetActive(true);

        // 강화 선택 중에는 게임 진행을 멈춘다.
        Time.timeScale = 0f;
    }


    /// <summary>
    /// 이전 강화 선택 때 생성된 버튼들을 제거한다.
    ///
    /// 다음 레벨업마다 새로운 랜덤 강화 목록을 만들기 때문에
    /// 기존 버튼은 전부 제거한다.
    /// </summary>
    private void ClearButtons()
    {
        foreach (UpgradeButton button in createdButtons)
        {
            if (button != null)
            {
                Destroy(
                    button.gameObject
                );
            }
        }

        createdButtons.Clear();
    }


    /// <summary>
    /// 특정 UpgradeType의 현재 강화 레벨을 반환한다.
    ///
    /// 아직 한 번도 강화하지 않은 Type이면 0을 반환한다.
    /// </summary>
    private int GetUpgradeLevel(
        UpgradeType type)
    {
        if (!upgradeLevels.TryGetValue(
                type,
                out int level))
        {
            return 0;
        }

        return level;
    }


    /// <summary>
    /// 플레이어가 강화 버튼 하나를 선택했을 때 호출된다.
    ///
    /// 처리 순서
    /// 1. 데이터 유효성 확인
    /// 2. 최대 레벨 확인
    /// 3. UpgradeType에 맞는 Strategy 생성
    /// 4. 실제 강화 적용
    /// 5. 강화 레벨 증가
    /// 6. EXP 레벨업 완료 처리
    /// 7. 강화 패널 닫기
    /// </summary>
    public void SelectUpgrade(
        UpgradeData data)
    {
        if (data == null)
        {
            return;
        }

        int currentLevel =
            GetUpgradeLevel(
                data.upgradeType
            );


        // 이미 최대 레벨이라면
        // 강화 적용을 진행하지 않는다.
        if (currentLevel >= data.maxLevel)
        {
            GameMessageUI.Instance?.ShowMessage(
                $"{data.upgradeName}은 최대 레벨입니다."
            );

            return;
        }


        if (strategyFactory == null)
        {
            Debug.LogWarning(
                "[UpgradeManager] StrategyFactory가 생성되지 않았습니다."
            );

            return;
        }


        // 현재 UpgradeType에 맞는 Strategy 생성
        IUpgradeStrategy strategy =
            strategyFactory.Create(
                data.upgradeType
            );

        if (strategy == null)
        {
            Debug.LogWarning(
                $"[UpgradeManager] " +
                $"{data.upgradeType} Strategy를 찾을 수 없습니다."
            );

            return;
        }


        // 실제 강화 적용
        strategy.Apply(
            data.value
        );


        // 해당 UpgradeType의 현재 강화 단계 증가
        upgradeLevels[data.upgradeType] =
            currentLevel + 1;


        // EXP 시스템에
        // 이번 레벨업 처리가 끝났음을 알려준다.
        if (playerExperience != null)
        {
            playerExperience.CompleteLevelUp();
        }


        // 강화 선택 완료 후 게임 재개
        CloseUpgradePanel();

        // 남은 EXP가 다음 레벨 조건까지 충족했다면
        // 다음 강화창을 다시 연다.
        if (playerExperience != null)
        {
            playerExperience.CheckPendingLevelUp();
        }
    }


    /// <summary>
    /// 강화 패널을 닫고 게임 진행을 다시 시작한다.
    /// </summary>
    private void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }


    /// <summary>
    /// 현재 선택 가능한 강화 중
    /// 랜덤으로 최대 3개를 선택해 버튼을 생성한다.
    /// </summary>
    private void CreateUpgradeButtons()
    {
        // 이전에 만들어둔 버튼 제거
        ClearButtons();


        if (content == null ||
            upgradeButtonPrefab == null)
        {
            Debug.LogWarning(
                "[UpgradeManager] " +
                "Content 또는 UpgradeButtonPrefab이 연결되지 않았습니다."
            );

            return;
        }


        // 최대 3개의 랜덤 강화 선택
        List<UpgradeData> selectedUpgrades =
            GetRandomUpgrades(3);


        foreach (UpgradeData data in selectedUpgrades)
        {
            UpgradeButton button =
                Instantiate(
                    upgradeButtonPrefab,
                    content
                );


            int currentLevel =
                GetUpgradeLevel(
                    data.upgradeType
                );


            // 버튼에 표시할 강화 정보 전달
            button.Setup(
                data,
                this,
                currentLevel
            );


            // 다음 패널 갱신 때 제거하기 위해 저장
            createdButtons.Add(
                button
            );
        }
    }


    /// <summary>
    /// 아직 최대 레벨에 도달하지 않은 강화 중에서
    /// 지정한 개수만큼 랜덤 선택한다.
    ///
    /// 같은 강화가 한 번의 선택창에
    /// 중복해서 나오지 않도록
    /// 선택한 데이터는 후보 목록에서 제거한다.
    /// </summary>
    private List<UpgradeData> GetRandomUpgrades(
        int count)
    {
        List<UpgradeData> candidates =
            new List<UpgradeData>();


        if (upgradeDatas == null)
        {
            return candidates;
        }


        // 전체 UpgradeData 중
        // 아직 최대 레벨이 아닌 강화만 후보에 추가
        foreach (UpgradeData data in upgradeDatas)
        {
            if (data == null)
            {
                continue;
            }

            int currentLevel =
                GetUpgradeLevel(
                    data.upgradeType
                );


            // 이미 최대 레벨이면 후보에서 제외
            if (currentLevel >= data.maxLevel)
            {
                continue;
            }

            candidates.Add(
                data
            );
        }


        List<UpgradeData> result =
            new List<UpgradeData>();


        // 남아있는 후보보다 많이 뽑지 않도록 제한
        count =
            Mathf.Min(
                count,
                candidates.Count
            );


        // 후보 목록에서 랜덤으로 하나씩 선택
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


            // 이미 선택한 강화는 제거해서
            // 같은 선택창에서 중복되지 않도록 한다.
            candidates.RemoveAt(
                randomIndex
            );
        }


        return result;
    }


    /// <summary>
    /// 아직 최대 레벨에 도달하지 않은 강화가
    /// 하나라도 존재하는지 확인한다.
    ///
    /// 모든 강화가 최대 레벨이라면 false를 반환한다.
    /// </summary>
    public bool HasAvailableUpgrade()
    {
        if (upgradeDatas == null)
        {
            return false;
        }


        foreach (UpgradeData data in upgradeDatas)
        {
            if (data == null)
            {
                continue;
            }


            int currentLevel =
                GetUpgradeLevel(
                    data.upgradeType
                );


            if (currentLevel < data.maxLevel)
            {
                return true;
            }
        }


        return false;
    }
}