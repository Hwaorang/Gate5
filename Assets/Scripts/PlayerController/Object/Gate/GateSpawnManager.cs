using System.Collections;
using UnityEngine;

/// <summary>
/// 일정 시간마다 GateChoiceGroup을 생성한다.
///
/// PlayerRoot는 런타임에 생성되므로
/// PlayerController가 나타날 때까지 기다린 뒤 참조를 캐싱한다.
///
/// Gate의 폭과 중심은 PlayerController가 제공하는
/// 실제 RoadLeftEdge / RoadRightEdge 기준 영역을 사용한다.
/// </summary>
public class GateSpawnManager : MonoBehaviour
{
    // ========================================================
    // Config
    // ========================================================

    [Header("Spawn Config")]

    [SerializeField]
    private GateSpawnConfig spawnConfig;


    // ========================================================
    // Prefab
    // ========================================================

    [Header("Gate Prefab")]

    [Tooltip(
        "Left / Right Gate가 들어있는 " +
        "GateChoiceGroup Prefab"
    )]
    [SerializeField]
    private GameObject gateChoiceGroupPrefab;


    // ========================================================
    // Spawn Position
    // ========================================================

    [Header("Spawn Point")]

    [SerializeField]
    private Transform spawnPoint;


    // ========================================================
    // Parent
    // ========================================================

    [Header("Hierarchy")]

    [Tooltip(
        "생성된 Gate를 정리할 부모. " +
        "없으면 이 오브젝트 아래에 생성한다."
    )]
    [SerializeField]
    private Transform gateContainer;


    // ========================================================
    // Player Movement Range
    // ========================================================

    [Header("Player Movement Range")]

    [Tooltip(
        "도로 영역을 아직 찾지 못했을 때만 사용할 기본 Gate 폭"
    )]
    [SerializeField]
    [Min(1f)]
    private float fallbackMovementWidth = 8f;


    // PlayerRoot가 Runtime에 생성되므로
    // Inspector 연결이 아닌 Runtime Cache로 사용한다.
    private PlayerController playerController;


    // ========================================================
    // Stage Movement
    // ========================================================

    [Header("Stage Movement")]

    [Tooltip(
        "Gate를 Stage와 같은 속도/방향으로 움직이기 위한 StageMapManager. " +
        "비어 있으면 Scene에서 자동 탐색한다."
    )]
    [SerializeField]
    private StageMapManager stageMapManager;


    // ========================================================
    // Runtime
    // ========================================================

    private Coroutine spawnRoutine;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        if (stageMapManager == null)
        {
            stageMapManager =
                Object.FindFirstObjectByType<StageMapManager>();
        }
    }


    private void Start()
    {
        StartSpawning();
    }


    private void OnDisable()
    {
        StopSpawning();
    }


    // ========================================================
    // Player Resolve
    // ========================================================

    /// <summary>
    /// 런타임에 생성된 PlayerRoot에서
    /// PlayerController를 찾아 한 번만 캐싱한다.
    /// </summary>
    private bool TryResolvePlayerController()
    {
        if (playerController != null)
        {
            return true;
        }


        playerController =
            Object.FindFirstObjectByType<PlayerController>();


        if (playerController == null)
        {
            return false;
        }


        Debug.Log(
            "[GateSpawnManager] PlayerController 발견"
        );


        return true;
    }


    /// <summary>
    /// PlayerController를 통해 실제 도로 영역이
    /// 준비되었는지 확인한다.
    /// </summary>
    private bool TryGetRoadArea(
        out float centerX,
        out float width)
    {
        centerX = 0f;
        width = fallbackMovementWidth;


        if (!TryResolvePlayerController())
        {
            return false;
        }


        return playerController.TryGetRoadArea(
            out centerX,
            out width
        );
    }


    // ========================================================
    // Spawn Control
    // ========================================================

    public void StartSpawning()
    {
        if (spawnRoutine != null)
        {
            return;
        }


        if (!ValidateSettings())
        {
            return;
        }


        spawnRoutine =
            StartCoroutine(
                SpawnRoutine()
            );
    }


    public void StopSpawning()
    {
        if (spawnRoutine == null)
        {
            return;
        }


        StopCoroutine(
            spawnRoutine
        );


        spawnRoutine =
            null;
    }


    // ========================================================
    // Coroutine
    // ========================================================

    private IEnumerator SpawnRoutine()
    {
        // =============================================
        // PlayerRoot 생성 대기
        // =============================================

        while (!TryResolvePlayerController())
        {
            yield return null;
        }


        // =============================================
        // StageChunk / Road Edge 준비 대기
        // =============================================

        float roadCenterX = 0f;
        float roadWidth = 0f;


        while (!TryGetRoadArea(
                   out roadCenterX,
                   out roadWidth))
        {
            yield return null;
        }


        Debug.Log(
            $"[GateSpawnManager] 도로 영역 준비 완료 | " +
            $"CenterX : {roadCenterX:0.00} | " +
            $"Width : {roadWidth:0.00} | " +
            $"Player XLimit : {playerController.XLimit:0.00}"
        );


        // =============================================
        // 첫 Gate 등장 전 대기
        // =============================================

        if (spawnConfig.InitialDelay > 0f)
        {
            yield return new WaitForSeconds(
                spawnConfig.InitialDelay
            );
        }


        // =============================================
        // 반복 Spawn
        // =============================================

        while (true)
        {
            SpawnGateGroup();


            float nextInterval =
                spawnConfig.GetRandomSpawnInterval();


            yield return new WaitForSeconds(
                nextInterval
            );
        }
    }


    // ========================================================
    // Spawn
    // ========================================================

    private void SpawnGateGroup()
    {
        Transform parent =
            gateContainer != null
                ? gateContainer
                : transform;


        // =============================================
        // 실제 도로 중심 / 폭 가져오기
        // =============================================

        float roadCenterX;
        float roadWidth;


        if (!TryGetRoadArea(
                out roadCenterX,
                out roadWidth))
        {
            Debug.LogWarning(
                "[GateSpawnManager] 도로 영역을 찾지 못해 " +
                "이번 Gate 생성을 건너뜁니다."
            );

            return;
        }


        // =============================================
        // Spawn Position
        // =============================================

        Vector3 spawnPosition =
            spawnPoint.position;


        // X = 0이 아니라
        // 실제 RoadLeftEdge / RoadRightEdge의 중앙을 사용한다.
        spawnPosition.x =
            roadCenterX;


        GameObject spawnedGate =
            Instantiate(
                gateChoiceGroupPrefab,
                spawnPosition,
                spawnPoint.rotation,
                parent
            );


        spawnedGate.name =
            $"{gateChoiceGroupPrefab.name}_Runtime";


        // ====================================================
        // Gate Layout
        // ====================================================

        GateGroupLayout layout =
            spawnedGate.GetComponent<GateGroupLayout>();


        if (layout == null)
        {
            Debug.LogError(
                $"[GateSpawnManager] " +
                $"{spawnedGate.name} 루트에 " +
                $"GateGroupLayout이 없습니다."
            );
        }
        else
        {
            // 실제 도로 전체 폭을
            // Left / Right Gate가 나누어 사용한다.
            layout.Configure(
                roadWidth
            );
        }


        // ====================================================
        // Gate Movement
        // ====================================================

        GateMover mover =
            spawnedGate.GetComponent<GateMover>();


        if (mover == null)
        {
            Debug.LogWarning(
                $"[GateSpawnManager] " +
                $"{spawnedGate.name} 루트에 GateMover가 없습니다."
            );
        }
        else
        {
            float moveSpeed =
                stageMapManager != null
                    ? stageMapManager.MoveSpeed
                    : spawnConfig.MoveSpeed;


            Vector3 moveDirection =
                stageMapManager != null
                    ? stageMapManager.MoveDirection
                    : Vector3.back;


            mover.Initialize(
                moveDirection,
                moveSpeed,
                spawnConfig.MaxTravelDistance
            );
        }


        Debug.Log(
            $"[GateSpawnManager] Gate 생성 | " +
            $"Road CenterX : {roadCenterX:0.00} | " +
            $"Road Width : {roadWidth:0.00} | " +
            $"Player XLimit : {playerController.XLimit:0.00}"
        );
    }


    // ========================================================
    // Validation
    // ========================================================

    private bool ValidateSettings()
    {
        if (spawnConfig == null)
        {
            Debug.LogWarning(
                "[GateSpawnManager] " +
                "GateSpawnConfig가 없습니다."
            );

            return false;
        }


        if (gateChoiceGroupPrefab == null)
        {
            Debug.LogWarning(
                "[GateSpawnManager] " +
                "GateChoiceGroup Prefab이 없습니다."
            );

            return false;
        }


        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "[GateSpawnManager] " +
                "SpawnPoint가 없습니다."
            );

            return false;
        }


        return true;
    }
}
