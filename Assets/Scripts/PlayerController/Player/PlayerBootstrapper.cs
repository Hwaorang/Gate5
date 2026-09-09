using UnityEngine;

/// <summary>
/// PlayerRoot Prefab을 런타임에 생성하는 역할을 담당한다.
///
/// Scene에 PlayerRoot를 직접 배치하지 않고,
/// 게임 시작 시 필요한 Player를 생성하기 위한 진입점이다.
///
/// 현재 단계에서는 Player 생성만 담당하고,
/// UI / DamageLine 등의 외부 참조 연결은
/// 다음 단계에서 추가한다.
/// </summary>
public class PlayerBootstrapper : MonoBehaviour
{
    [Header("Player 생성")]

    // 런타임에 생성할 PlayerRoot Prefab
    [SerializeField]
    private GameObject playerPrefab;

    // Player가 생성될 위치
    [SerializeField]
    private Transform spawnPoint;


    // 런타임에 생성된 PlayerRoot
    private GameObject playerInstance;

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

    /// <summary>
    /// 생성된 PlayerRoot를 외부에서 확인할 수 있도록 제공한다.
    /// </summary>
    public GameObject PlayerInstance =>
        playerInstance;

    private PlayerContext playerContext;

    public PlayerContext PlayerContext =>
        playerContext;

    /// <summary>
    /// PlayerRoot Prefab을 생성한다.
    /// </summary>
    public GameObject SpawnPlayer()
    {
        // Prefab이 연결되지 않았다면 생성할 수 없다.
        if (playerPrefab == null)
        {
            Debug.LogWarning(
                "[PlayerBootstrapper] Player Prefab이 연결되지 않았습니다."
            );

            return null;
        }

        // 이미 생성된 Player가 있다면
        // 중복으로 생성하지 않는다.
        if (playerInstance != null)
        {
            return playerInstance;
        }


        // SpawnPoint가 있다면 해당 위치 사용
        Vector3 spawnPosition =
            spawnPoint != null
                ? spawnPoint.position
                : Vector3.zero;

        Quaternion spawnRotation =
            spawnPoint != null
                ? spawnPoint.rotation
                : Quaternion.identity;


        // PlayerRoot Prefab 생성
        playerInstance =
            Instantiate(
                playerPrefab,
                spawnPosition,
                spawnRotation
            );

        playerContext = playerInstance.GetComponent<PlayerContext>();

        if (playerContext == null)
        {
            Debug.LogError(
                "[PlayerBootstrapper] PlayerRoot에 PlayerContext가 없습니다."
            );
        }

        if (soldierCountUI != null)
        {
            soldierCountUI.Initialize(
                playerContext
            );
        }

        if (damageLine != null)
        {
            damageLine.Initialize(
                playerContext
            );
        }

        if (expProgressUI != null)
        {
            expProgressUI.Initialize(playerContext);
        }

        if (upgradeManager != null)
        {
            upgradeManager.Initialize(
                playerContext
            );
        }

        if (gameResultPresenter != null)
        {
            gameResultPresenter.Initialize(
                playerContext
            );
        }

        if (debugUIBootstrapper != null)
        {
            debugUIBootstrapper.Initialize(
                playerContext
            );
        }

        return playerInstance;
    }
}