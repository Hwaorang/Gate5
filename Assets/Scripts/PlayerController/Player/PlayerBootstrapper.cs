using UnityEngine;
using GptAsset.HyperCasualBulletFX;

/// <summary>
/// PlayerRoot Prefab을 런타임에 생성하고,
/// 생성된 Player의 주요 시스템을 Scene 시스템과 연결한다.
///
/// 주요 역할
/// - PlayerRoot Prefab 생성
/// - 필수 컴포넌트 / 참조 검증
/// - SoldierPool / BulletFX 등 Scene 시스템 주입
/// - UI / Upgrade / Camera / DamageLine 등에 PlayerContext 전달
/// </summary>
public class PlayerBootstrapper : MonoBehaviour
{
    // =========================
    // Player 생성
    // =========================

    [Header("Player 생성")]

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform spawnPoint;


    // =========================
    // Player 의존 시스템
    // =========================

    [Header("Player 의존 시스템")]

    [SerializeField]
    private DamageLine damageLine;

    [SerializeField]
    private SoldierCountUI soldierCountUI;

    [SerializeField]
    private ExpProgressUI expProgressUI;

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [SerializeField]
    private GameResultPresenter gameResultPresenter;

    [SerializeField]
    private DebugUIBootstrapper debugUIBootstrapper;

    [SerializeField]
    private CameraViewController cameraViewController;

    [SerializeField]
    private GameUIRoot gameUIRoot;


    // =========================
    // Scene References
    // =========================

    [Header("Scene References")]

    [SerializeField]
    private SoldierPool soldierPool;

    [SerializeField]
    private HyperCasualBulletFx bulletFx;


    // =========================
    // Runtime
    // =========================

    private GameObject playerInstance;

    private PlayerContext playerContext;


    public GameObject PlayerInstance =>
        playerInstance;

    public PlayerContext PlayerContext =>
        playerContext;


    private void Start()
    {
        // Scene 외부 시스템 탐색
        ResolveSceneReferences();

        // 게임 시작 전에 필수 참조 검사
        if (!ValidateReferences())
        {
            Debug.LogError(
                "[PlayerBootstrapper] 필수 설정이 누락되어 Player를 생성하지 않습니다."
            );

            return;
        }

        SpawnPlayer();
    }


    /// <summary>
    /// PlayerRoot Prefab을 생성한다.
    /// </summary>
    public GameObject SpawnPlayer()
    {
        // 중복 생성 방지
        if (playerInstance != null)
        {
            return playerInstance;
        }

        if (playerPrefab == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] Player Prefab이 없습니다."
            );

            return null;
        }


        // =========================
        // 생성 위치 계산
        // =========================

        Vector3 spawnPosition =
            spawnPoint != null
                ? spawnPoint.position
                : Vector3.zero;

        Quaternion spawnRotation =
            spawnPoint != null
                ? spawnPoint.rotation
                : Quaternion.identity;


        // =========================
        // Player 생성
        // =========================

        playerInstance =
            Instantiate(
                playerPrefab,
                spawnPosition,
                spawnRotation
            );


        // =========================
        // PlayerContext 확인
        // =========================

        playerContext =
            playerInstance.GetComponent<PlayerContext>();

        if (playerContext == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] 생성된 PlayerRoot에 PlayerContext가 없습니다."
            );

            Destroy(playerInstance);

            playerInstance = null;

            return null;
        }


        // =========================
        // Player 내부 시스템 검증
        // =========================

        if (!ValidatePlayerContext())
        {
            Debug.LogError(
                "[PlayerBootstrapper] 생성된 Player 내부 구성이 올바르지 않습니다."
            );

            Destroy(playerInstance);

            playerInstance = null;
            playerContext = null;

            return null;
        }


        // =========================
        // Scene 시스템 먼저 주입
        // =========================

        InjectSceneReferences();


        // =========================
        // 외부 시스템 연결
        // =========================

        InitializePlayerSystems();


        Debug.Log(
            "[PlayerBootstrapper] Player 초기화 완료"
        );

        return playerInstance;
    }


    /// <summary>
    /// Inspector 및 PlayerRoot Prefab의
    /// 필수 구성을 게임 시작 전에 검사한다.
    /// </summary>
    [ContextMenu("Validate References")]
    private bool ValidateReferences()
    {
        bool isValid = true;


        // =========================
        // Player Prefab
        // =========================

        if (playerPrefab == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] Player Prefab이 연결되지 않았습니다."
            );

            return false;
        }


        // =========================
        // Prefab 필수 Component 검사
        // =========================

        if (playerPrefab.GetComponent<PlayerContext>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 PlayerContext가 없습니다."
            );

            isValid = false;
        }

        if (playerPrefab.GetComponent<PlayerController>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 PlayerController가 없습니다."
            );

            isValid = false;
        }

        if (playerPrefab.GetComponent<PlayerStats>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 PlayerStats가 없습니다."
            );

            isValid = false;
        }

        if (playerPrefab.GetComponent<PlayerExperience>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 PlayerExperience가 없습니다."
            );

            isValid = false;
        }

        if (playerPrefab.GetComponent<SquadManager>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 SquadManager가 없습니다."
            );

            isValid = false;
        }

        // 오늘 실제로 빠져서 공격이 작동하지 않았던 Component
        if (playerPrefab.GetComponent<SquadAttackResolver>() == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot Prefab에 SquadAttackResolver가 없습니다."
            );

            isValid = false;
        }


        // =========================
        // FireLine 검사
        // =========================

        Transform fireLine =
            playerPrefab.transform.Find(
                "FireLine"
            );

        if (fireLine == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] PlayerRoot Prefab에서 FireLine을 찾을 수 없습니다."
            );
        }


        // =========================
        // Scene 필수 Reference
        // =========================

        if (soldierPool == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] SoldierPool이 연결되지 않았습니다."
            );

            isValid = false;
        }

        if (upgradeManager == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] UpgradeManager가 연결되지 않았습니다."
            );

            isValid = false;
        }


        // =========================
        // 선택적 / 시각적 Reference
        // =========================

        if (bulletFx == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] BulletFX가 연결되지 않았습니다. 총알 FX가 표시되지 않을 수 있습니다."
            );
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] SpawnPoint가 없습니다. Vector3.zero에서 생성합니다."
            );
        }

        if (damageLine == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] DamageLine이 연결되지 않았습니다."
            );
        }

        if (soldierCountUI == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] SoldierCountUI가 연결되지 않았습니다."
            );
        }

        if (expProgressUI == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] ExpProgressUI가 연결되지 않았습니다."
            );
        }

        if (gameResultPresenter == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] GameResultPresenter가 연결되지 않았습니다."
            );
        }

        if (cameraViewController == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] CameraViewController가 연결되지 않았습니다."
            );
        }

        if (debugUIBootstrapper == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] DebugUIBootstrapper가 연결되지 않았습니다."
            );
        }

        return isValid;
    }


    /// <summary>
    /// 실제 생성된 PlayerContext 내부의
    /// 주요 Component가 정상인지 확인한다.
    /// </summary>
    private bool ValidatePlayerContext()
    {
        if (playerContext.PlayerController == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerController를 찾을 수 없습니다."
            );

            return false;
        }

        if (playerContext.PlayerStats == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerStats를 찾을 수 없습니다."
            );

            return false;
        }

        if (playerContext.PlayerExperience == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerExperience를 찾을 수 없습니다."
            );

            return false;
        }

        if (playerContext.SquadManager == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] SquadManager를 찾을 수 없습니다."
            );

            return false;
        }

        return true;
    }


    /// <summary>
    /// Scene에 존재하는 시스템을
    /// 동적으로 생성된 Player에 전달한다.
    ///
    /// SquadManager.Start()가 실행되기 전에
    /// SoldierPool / BulletFX를 전달하는 것이 중요하다.
    /// </summary>
    private void InjectSceneReferences()
    {
        SquadManager squadManager =
            playerContext.SquadManager;


        // =========================
        // Soldier Pool
        // =========================

        if (soldierPool != null)
        {
            squadManager.SetSoldierPool(
                soldierPool
            );
        }


        // =========================
        // Bullet FX
        // =========================

        if (bulletFx != null)
        {
            squadManager.SetBulletFx(
                bulletFx
            );
        }
    }


    /// <summary>
    /// 생성된 PlayerContext를
    /// Player에 의존하는 외부 시스템에 전달한다.
    /// </summary>
    private void InitializePlayerSystems()
    {
        // =========================
        // Upgrade
        // =========================

        if (upgradeManager != null)
        {
            // UpgradeManager가
            // 새 SquadManager / Stats / Experience를 받는다.
            upgradeManager.Initialize(
                playerContext
            );

            // PlayerExperience도
            // Scene UpgradeManager를 받는다.
            playerContext.PlayerExperience
                .SetUpgradeManager(
                    upgradeManager
                );
        }


        // =========================
        // Damage Line
        // =========================

        if (damageLine != null)
        {
            damageLine.Initialize(
                playerContext
            );
        }


        // =========================
        // Soldier Count UI
        // =========================

        if (soldierCountUI != null)
        {
            soldierCountUI.Initialize(
                playerContext
            );
        }


        // =========================
        // EXP UI
        // =========================

        if (expProgressUI != null)
        {
            expProgressUI.Initialize(
                playerContext
            );
        }


        // =========================
        // Game Result
        // =========================

        if (gameResultPresenter != null)
        {
            gameResultPresenter.Initialize(
                playerContext
            );
        }


        // =========================
        // Debug UI
        // =========================

        if (debugUIBootstrapper != null)
        {
            debugUIBootstrapper.Initialize(
                playerContext
            );
        }


        // =========================
        // Camera
        // =========================

        if (cameraViewController != null)
        {
            cameraViewController.Initialize(
                playerContext
            );
        }
    }

    /// <summary>
    /// PlayerSystem 밖에 존재하는 Scene 전용 시스템을 찾는다.
    /// Inspector에 직접 연결되어 있다면 기존 참조를 우선 사용한다.
    /// </summary>
    private void ResolveSceneReferences()
    {
        // =========================
        // Game UI
        // =========================

        if (gameUIRoot == null)
        {
            gameUIRoot =
                FindFirstObjectByType<GameUIRoot>();
        }


        if (gameUIRoot != null)
        {
            if (upgradeManager == null)
            {
                upgradeManager =
                    gameUIRoot.UpgradeManager;
            }

            if (gameResultPresenter == null)
            {
                gameResultPresenter =
                    gameUIRoot.GameResultPresenter;
            }

            if (soldierCountUI == null)
            {
                soldierCountUI =
                    gameUIRoot.SoldierCountUI;
            }

            if (expProgressUI == null)
            {
                expProgressUI =
                    gameUIRoot.ExpProgressUI;
            }
        }


        // =========================
        // Camera
        // =========================

        if (cameraViewController == null)
        {
            cameraViewController =
                FindFirstObjectByType<CameraViewController>();
        }
    }
}