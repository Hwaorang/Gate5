using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// DebugCanvas를 런타임에 생성하고
/// F12로 DebugPanel을 열고 닫는다.
///
/// PlayerRoot가 동적으로 생성될 경우
/// PlayerContext를 통해 실제 Player 시스템의 참조를 전달받는다.
/// </summary>
public class DebugUIBootstrapper : MonoBehaviour
{
    [Header("Debug UI")]

    // 런타임에 생성할 DebugCanvas Prefab
    [SerializeField]
    private GameObject debugCanvasPrefab;


    [Header("게임 시스템 참조")]

    // 현재 단계에서는 기존 Inspector 참조도 유지한다.
    [SerializeField]
    private SquadManager squadManager;

    [SerializeField]
    private PlayerExperience playerExperience;

    // UpgradeManager는 Scene 시스템이므로
    // PlayerContext가 아니라 Inspector에서 계속 받는다.
    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;


    // 생성된 DebugCanvas
    private GameObject debugCanvasInstance;

    // F12로 표시/숨김할 DebugPanel
    private GameObject debugPanel;

    // 생성된 DebugCheatPanel
    private DebugCheatPanel debugCheatPanel;

    private DebugPerformanceUI debugPerformanceUI;


    private void Start()
    {
#if UNITY_EDITOR
        CreateDebugUI();
#endif
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            ToggleDebugPanel();
        }
#endif
    }


    /// <summary>
    /// 동적으로 생성된 Player의 참조를 전달받는다.
    /// </summary>
    public void Initialize(
        PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[DebugUIBootstrapper] PlayerContext가 없습니다."
            );

            return;
        }

        // 새로운 Player 시스템으로 참조 교체
        squadManager =
            context.SquadManager;

        playerExperience =
            context.PlayerExperience;


        // 아직 DebugCanvas가 생성되지 않았다면 생성
        if (debugCanvasInstance == null)
        {
            CreateDebugUI();
        }


        // 생성되어 있는 Debug UI에
        // 새로운 Player 참조 전달
        ApplyPlayerReferences();
    }


    /// <summary>
    /// DebugCanvas Prefab을 런타임에 생성한다.
    /// </summary>
    private void CreateDebugUI()
    {
        // 중복 생성 방지
        if (debugCanvasInstance != null)
        {
            return;
        }

        if (debugCanvasPrefab == null)
        {
            Debug.LogWarning(
                "[DebugUIBootstrapper] DebugCanvas Prefab이 연결되지 않았습니다."
            );

            return;
        }


        // =========================
        // DebugCanvas 생성
        // =========================

        debugCanvasInstance =
            Instantiate(debugCanvasPrefab);


        // =========================
        // DebugPanel 찾기
        // =========================

        Transform panelTransform =
            debugCanvasInstance.transform.Find(
                "DebugPanel"
            );

        if (panelTransform != null)
        {
            debugPanel =
                panelTransform.gameObject;

            // 게임 시작 시 DebugPanel은 숨김
            debugPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "[DebugUIBootstrapper] DebugPanel을 찾을 수 없습니다."
            );
        }


        // =========================
        // DebugCheatPanel 찾기
        // =========================

        debugCheatPanel =
            debugCanvasInstance
                .GetComponent<DebugCheatPanel>();

        debugPerformanceUI =
            debugCanvasInstance
                .GetComponent<DebugPerformanceUI>();


        // 현재 가지고 있는 Player 참조 전달
        ApplyPlayerReferences();
    }


    /// <summary>
    /// DebugCheatPanel에 현재 Player 시스템 참조를 전달한다.
    /// </summary>
    private void ApplyPlayerReferences()
    {
        if (debugCheatPanel == null)
        {
            return;
        }

        debugCheatPanel.Initialize(
            squadManager,
            playerExperience,
            upgradeManager
        );

        // =========================
        // Performance UI
        // =========================

        if (debugPerformanceUI != null)
        {
            debugPerformanceUI.Initialize(
                squadManager,
                playerExperience
            );
        }
    }


    /// <summary>
    /// DebugPanel을 표시하거나 숨긴다.
    /// </summary>
    public void ToggleDebugPanel()
    {
        if (debugPanel == null)
        {
            return;
        }

        debugPanel.SetActive(
            !debugPanel.activeSelf
        );
    }
}