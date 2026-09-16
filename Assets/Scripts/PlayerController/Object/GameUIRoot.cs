using UnityEngine;

/// <summary>
/// 인게임 UI 묶음의 진입점.
///
/// GameUIRoot Prefab 내부의 UI 컴포넌트는
/// Inspector에 직접 연결하지 않아도 자식에서 자동 탐색한다.
///
/// PlayerBootstrapper는 이 컴포넌트만 찾으면
/// Player 관련 UI들을 초기화할 수 있다.
/// </summary>
public class GameUIRoot : MonoBehaviour
{
    // ========================================================
    // Player 연동 UI
    // ========================================================

    [Header("Player 연동 UI")]

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    [SerializeField]
    private GameResultPresenter gameResultPresenter;

    [SerializeField]
    private SoldierCountUI soldierCountUI;

    [SerializeField]
    private ExpProgressUI expProgressUI;

    [SerializeField]
    private GameHUDPresenter gameHUDPresenter;


    // ========================================================
    // 독립 UI
    // ========================================================

    [Header("독립 UI")]

    [SerializeField]
    private InGameMenuPresenter inGameMenuPresenter;

    [SerializeField]
    private GameTimerUI gameTimerUI;

    [SerializeField]
    private GameResultUI gameResultUI;


    // ========================================================
    // Canvas
    // ========================================================

    [Header("Canvas")]

    [SerializeField]
    private Canvas rootCanvas;

    [Tooltip(
        "다른 Scene으로 옮겨도 Camera 참조가 끊기지 않도록 " +
        "Screen Space - Overlay를 사용한다."
    )]
    [SerializeField]
    private bool forceScreenSpaceOverlay = true;


    // ========================================================
    // Public Properties
    // PlayerBootstrapper에서 사용
    // ========================================================

    public UpgradeManager_PlayerController UpgradeManager =>
        upgradeManager;

    public GameResultPresenter GameResultPresenter =>
        gameResultPresenter;

    public SoldierCountUI SoldierCountUI =>
        soldierCountUI;

    public ExpProgressUI ExpProgressUI =>
        expProgressUI;

    public GameHUDPresenter GameHUDPresenter =>
        gameHUDPresenter;


    public InGameMenuPresenter InGameMenuPresenter =>
        inGameMenuPresenter;

    public GameTimerUI GameTimerUI =>
        gameTimerUI;

    public GameResultUI GameResultUI =>
        gameResultUI;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        ResolveReferences();
        ConfigureCanvas();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        ResolveReferences();
        ConfigureCanvas();
    }
#endif


    // ========================================================
    // Reference Resolve
    // ========================================================

    /// <summary>
    /// GameUIRoot Prefab 내부의 컴포넌트를 자동으로 찾는다.
    ///
    /// includeInactive = true이므로
    /// 처음에 비활성화된 GameOverPanel / UpgradePanel도 찾을 수 있다.
    /// </summary>
    [ContextMenu("Resolve UI References")]
    public void ResolveReferences()
    {
        if (upgradeManager == null)
        {
            upgradeManager =
                GetComponentInChildren
                    <UpgradeManager_PlayerController>(
                        true
                    );
        }


        if (gameResultPresenter == null)
        {
            gameResultPresenter =
                GetComponentInChildren
                    <GameResultPresenter>(
                        true
                    );
        }


        if (soldierCountUI == null)
        {
            soldierCountUI =
                GetComponentInChildren
                    <SoldierCountUI>(
                        true
                    );
        }


        if (expProgressUI == null)
        {
            expProgressUI =
                GetComponentInChildren
                    <ExpProgressUI>(
                        true
                    );
        }


        if (gameHUDPresenter == null)
        {
            gameHUDPresenter =
                GetComponentInChildren
                    <GameHUDPresenter>(
                        true
                    );
        }


        if (inGameMenuPresenter == null)
        {
            inGameMenuPresenter =
                GetComponentInChildren
                    <InGameMenuPresenter>(
                        true
                    );
        }


        if (gameTimerUI == null)
        {
            gameTimerUI =
                GetComponentInChildren
                    <GameTimerUI>(
                        true
                    );
        }


        if (gameResultUI == null)
        {
            gameResultUI =
                GetComponentInChildren
                    <GameResultUI>(
                        true
                    );
        }


        if (rootCanvas == null)
        {
            rootCanvas =
                GetComponentInChildren<Canvas>(
                    true
                );
        }
    }


    // ========================================================
    // Canvas
    // ========================================================

    private void ConfigureCanvas()
    {
        if (rootCanvas == null ||
            !forceScreenSpaceOverlay)
        {
            return;
        }


        rootCanvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        // Overlay 모드에서는 Scene Camera 참조가 필요 없다.
        rootCanvas.worldCamera =
            null;
    }
}
