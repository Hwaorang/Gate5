using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 개발용 Debug UI를 런타임에 생성하고
/// F12 키로 DebugPanel을 표시/숨김 처리한다.
///
/// Scene에 DebugCanvas를 직접 배치하지 않고
/// Prefab을 실행 시 생성한다.
/// </summary>
public class DebugUIBootstrapper : MonoBehaviour
{
    [Header("Debug UI")]

    // 런타임에 생성할 DebugCanvas Prefab
    [SerializeField]
    private GameObject debugCanvasPrefab;


    [Header("게임 시스템 참조")]

    [SerializeField]
    private SquadManager squadManager;

    [SerializeField]
    private PlayerExperience playerExperience;

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;


    // 생성된 DebugCanvas
    private GameObject debugCanvasInstance;

    // 실제로 켰다 끌 DebugPanel
    private GameObject debugPanel;


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

        // F12로 DebugPanel 열기 / 닫기
        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            ToggleDebugPanel();
        }
#endif
    }


    /// <summary>
    /// DebugCanvas Prefab을 생성하고
    /// 필요한 시스템 참조를 전달한다.
    /// </summary>
    private void CreateDebugUI()
    {
        if (debugCanvasPrefab == null)
        {
            Debug.LogWarning(
                "[DebugUIBootstrapper] DebugCanvas Prefab이 연결되지 않았습니다."
            );

            return;
        }

        // 중복 생성 방지
        if (debugCanvasInstance != null)
        {
            return;
        }


        // =========================
        // DebugCanvas 생성
        // =========================

        debugCanvasInstance =
            Instantiate(
                debugCanvasPrefab
            );


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

            // 처음에는 숨긴다.
            debugPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "[DebugUIBootstrapper] DebugPanel을 찾을 수 없습니다."
            );
        }


        // =========================
        // DebugCheatPanel 초기화
        // =========================

        DebugCheatPanel cheatPanel =
            debugCanvasInstance
                .GetComponent<DebugCheatPanel>();

        if (cheatPanel != null)
        {
            cheatPanel.Initialize(
                squadManager,
                playerExperience,
                upgradeManager
            );
        }
    }


    /// <summary>
    /// DebugPanel만 표시/숨김 처리한다.
    ///
    /// DebugCanvas 자체는 계속 활성화 상태이므로
    /// Debug 시스템 초기화 상태가 유지된다.
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