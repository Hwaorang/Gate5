using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 인게임 HUD / Menu / Settings의 상태를 관리한다.
///
/// Closed
/// - HUD OFF
/// - Menu OFF
/// - Settings OFF
/// - 게임 진행
///
/// HUD
/// - HUD ON
/// - Menu OFF
/// - Settings OFF
/// - 게임 Pause
///
/// Menu
/// - HUD ON
/// - Menu ON
/// - Settings OFF
/// - 게임 Pause
///
/// Settings
/// - HUD ON
/// - Menu OFF
/// - Settings ON
/// - 게임 Pause
/// </summary>
public class InGameMenuPresenter : MonoBehaviour
{
    private enum UIState
    {
        Closed,
        HUD,
        Menu,
        Settings
    }

    [Header("Upgrade")]

    [SerializeField]
    private UpgradeManager_PlayerController upgradeManager;

    /// <summary>
    /// HUD / Menu / Settings 중 하나가 열려 있는지.
    /// </summary>
    public bool IsMenuPauseActive =>
        currentState != UIState.Closed;

    // ========================================================
    // HUD
    // ========================================================

    [Header("HUD")]

    // 게임 중 HUD를 여는 버튼
    [SerializeField]
    private Button hudToggleButton;

    // 실제 HUD 전체
    [SerializeField]
    private GameObject hudRoot;

    // HUD 내부 닫기 버튼
    [SerializeField]
    private Button hudCloseButton;

    // HUD 내부 Menu 버튼
    [SerializeField]
    private Button menuButton;

    [SerializeField]
    private UIPanelTransition hudTransition;


    // ========================================================
    // Menu
    // ========================================================

    [Header("Menu")]

    [SerializeField]
    private InGameMenuUI menuUI;


    // ========================================================
    // Settings
    // ========================================================

    [Header("Settings")]

    [SerializeField]
    private SettingsPanelUI settingsPanelUI;


    // ========================================================
    // State
    // ========================================================

    private UIState currentState =
        UIState.Closed;

    // GameOver 이후 일반 인게임 UI 입력을 막기 위한 값
    private bool gameOverLocked;

    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        if (hudTransition == null &&
            hudRoot != null)
        {
            hudTransition =
                hudRoot.GetComponent<UIPanelTransition>();
        }

        if (upgradeManager == null)
        {
            GameUIRoot uiRoot =
                GetComponentInParent<GameUIRoot>();

            if (uiRoot != null)
            {
                upgradeManager =
                    uiRoot.UpgradeManager;
            }
        }
    }

    private void Start()
    {
        InitializeClosedState();
    }


    private void Update()
    {
        // =========================
        // GameOver 검사
        // =========================

        if (IsGameOver())
        {
            // GameOver UI 정리는 한 번만 실행
            if (!gameOverLocked)
            {
                LockForGameOver();
            }

            // GameOver 중에는
            // ESC를 포함한 메뉴 입력을 처리하지 않는다.
            return;
        }


        if (Keyboard.current == null)
        {
            return;
        }


        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }


        HandleEscape();
    }

    /// <summary>
    /// Scene 시작 시 UI의 초기 모습만 설정한다.
    ///
    /// 시작할 때는 닫기 애니메이션을 재생하지 않고
    /// 모든 패널을 즉시 숨긴 상태로 만든다.
    ///
    /// 여기서는 GameManager.Resume()을 호출하지 않는다.
    /// 실제 게임 시작 여부는 다른 Game Flow 시스템이 담당한다.
    /// </summary>
    private void InitializeClosedState()
    {
        currentState =
            UIState.Closed;


        // =========================
        // Menu
        // =========================

        if (menuUI != null)
        {
            menuUI.HideImmediate();
        }


        // =========================
        // Settings
        // =========================

        if (settingsPanelUI != null)
        {
            settingsPanelUI.HideImmediate();
        }


        // =========================
        // HUD
        // =========================

        if (hudTransition != null)
        {
            hudTransition.HideImmediate();
        }
        else if (hudRoot != null)
        {
            hudRoot.SetActive(
                false
            );
        }


        // =========================
        // HUD Toggle
        // =========================

        if (hudToggleButton != null)
        {
            hudToggleButton.gameObject.SetActive(
                true
            );
        }


        // =========================
        // HUD Buttons
        // =========================

        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(
                true
            );
        }


        if (hudCloseButton != null)
        {
            hudCloseButton.gameObject.SetActive(
                true
            );
        }
    }

    // ========================================================
    // GameOver Lock
    // ========================================================

    /// <summary>
    /// 현재 GameOver 상태인지 확인한다.
    /// </summary>
    private bool IsGameOver()
    {
        return
            GameManager.Instance != null &&
            GameManager.Instance.IsGameOver;
    }


    /// <summary>
    /// GameOver가 발생하면
    /// 일반 HUD / Menu / Settings를 모두 닫고
    /// 더 이상 열리지 않게 한다.
    ///
    /// 주의:
    /// SetState(UIState.Closed)는 사용하지 않는다.
    /// Closed는 GameManager.Resume()을 호출하기 때문이다.
    /// </summary>
    private void LockForGameOver()
    {
        gameOverLocked = true;

        currentState =
            UIState.Closed;


        // HUD 닫기
        if (hudRoot != null)
        {
            hudRoot.SetActive(false);
        }


        // HUD를 여는 버튼도 숨기기
        if (hudToggleButton != null)
        {
            hudToggleButton.gameObject.SetActive(
                false
            );
        }


        // Menu 닫기
        if (menuUI != null)
        {
            menuUI.Hide();
        }


        // Settings 닫기
        if (settingsPanelUI != null)
        {
            settingsPanelUI.Hide();
        }


        // HUD 안쪽 조작 버튼들도 숨김
        if (menuButton != null)
        {
            menuButton.gameObject.SetActive(
                false
            );
        }


        if (hudCloseButton != null)
        {
            hudCloseButton.gameObject.SetActive(
                false
            );
        }


        Debug.Log(
            "[InGameMenuPresenter] " +
            "GameOver - HUD / Menu / Settings 입력 차단"
        );
    }

    private void OnEnable()
    {
        // =========================
        // HUD Button Events
        // =========================

        if (hudToggleButton != null)
        {
            hudToggleButton.onClick.AddListener(
                ToggleHUD
            );
        }


        if (hudCloseButton != null)
        {
            hudCloseButton.onClick.AddListener(
                CloseAll
            );
        }


        if (menuButton != null)
        {
            menuButton.onClick.AddListener(
                OpenMenu
            );
        }


        // =========================
        // Menu Events
        // =========================

        if (menuUI != null)
        {
            menuUI.OnContinueClicked +=
                CloseAll;

            menuUI.OnSettingsClicked +=
                OpenSettings;

            menuUI.OnRetryClicked +=
                Retry;

            menuUI.OnLobbyClicked +=
                GoLobby;
        }


        // =========================
        // Settings Events
        // =========================

        if (settingsPanelUI != null)
        {
            settingsPanelUI.OnCloseClicked +=
                CloseSettings;
        }
    }


    private void OnDisable()
    {
        // =========================
        // HUD Button Events
        // =========================

        if (hudToggleButton != null)
        {
            hudToggleButton.onClick.RemoveListener(
                ToggleHUD
            );
        }


        if (hudCloseButton != null)
        {
            hudCloseButton.onClick.RemoveListener(
                CloseAll
            );
        }


        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(
                OpenMenu
            );
        }


        // =========================
        // Menu Events
        // =========================

        if (menuUI != null)
        {
            menuUI.OnContinueClicked -=
                CloseAll;

            menuUI.OnSettingsClicked -=
                OpenSettings;

            menuUI.OnRetryClicked -=
                Retry;

            menuUI.OnLobbyClicked -=
                GoLobby;
        }


        // =========================
        // Settings Events
        // =========================

        if (settingsPanelUI != null)
        {
            settingsPanelUI.OnCloseClicked -=
                CloseSettings;
        }
    }


    // ========================================================
    // HUD
    // ========================================================

    /// <summary>
    /// HUD 열기 버튼.
    ///
    /// Closed -> HUD
    /// HUD/Menu/Settings -> Closed
    /// </summary>
    private void ToggleHUD()
    {
        if (IsGameOver())
        {
            return;
        }

        if (currentState ==
            UIState.Closed)
        {
            SetState(
                UIState.HUD
            );
        }
        else
        {
            SetState(
                UIState.Closed
            );
        }
    }


    /// <summary>
    /// HUD 내부 Menu 버튼.
    /// </summary>
    private void OpenMenu()
    {
        if (IsGameOver())
        {
            return;
        }

        SetState(
            UIState.Menu
        );
    }


    /// <summary>
    /// Continue 또는 HUD Close.
    /// 모든 UI를 닫고 게임을 재개한다.
    /// </summary>
    private void CloseAll()
    {
        if (IsGameOver())
        {
            return;
        }

        SetState(
            UIState.Closed
        );
    }


    // ========================================================
    // Settings
    // ========================================================

    /// <summary>
    /// Menu의 Settings 버튼.
    /// </summary>
    private void OpenSettings()
    {
        if (IsGameOver())
        {
            return;
        }

        if (settingsPanelUI == null)
        {
            Debug.LogWarning(
                "[InGameMenuPresenter] " +
                "SettingsPanelUI가 연결되지 않았습니다."
            );

            return;
        }


        SetState(
            UIState.Settings
        );
    }


    /// <summary>
    /// Settings의 Close 버튼.
    ///
    /// 게임으로 바로 돌아가는 것이 아니라
    /// 이전 Menu 화면으로 돌아간다.
    /// </summary>
    private void CloseSettings()
    {
        if (IsGameOver())
        {
            return;
        }

        SetState(
            UIState.Menu
        );
    }


    // ========================================================
    // ESC
    // ========================================================

    /// <summary>
    /// 현재 UI 상태에 따라 ESC 동작을 결정한다.
    /// </summary>
    private void HandleEscape()
    {
        if (IsGameOver())
        {
            return;
        }

        switch (currentState)
        {
            // 게임 중 ESC
            // -> HUD 열기
            case UIState.Closed:

                SetState(
                    UIState.HUD
                );

                break;


            // HUD에서 ESC
            // -> 게임으로 복귀
            case UIState.HUD:

                SetState(
                    UIState.Closed
                );

                break;


            // Menu에서 ESC
            // -> 게임으로 복귀
            case UIState.Menu:

                SetState(
                    UIState.Closed
                );

                break;


            // Settings에서 ESC
            // -> Menu로 복귀
            // 게임은 계속 Pause
            case UIState.Settings:

                SetState(
                    UIState.Menu
                );

                break;
        }
    }


    // ========================================================
    // State
    // ========================================================

    /// <summary>
    /// 모든 UI 상태 변경은 이 메서드를 통해 처리한다.
    /// </summary>
    private void SetState(
        UIState newState)
    {
        if (IsGameOver())
        {
            return;
        }

        currentState =
            newState;


        switch (currentState)
        {
            // =================================================
            // Closed
            // =================================================

            case UIState.Closed:

                if (menuUI != null)
                {
                    menuUI.Hide();
                }


                if (settingsPanelUI != null)
                {
                    settingsPanelUI.Hide();
                }


                if (hudTransition != null)
                {
                    hudTransition.Hide();
                }
                else if (hudRoot != null)
                {
                    hudRoot.SetActive(
                        false
                    );
                }


                // 다음 HUD 오픈을 위해 복구
                if (menuButton != null)
                {
                    menuButton.gameObject.SetActive(
                        true
                    );
                }


                if (hudCloseButton != null)
                {
                    hudCloseButton.gameObject.SetActive(
                        true
                    );
                }


                if (GameManager.Instance != null)
                {
                    // 강화 선택창이 아직 열려 있다면
                    // HUD/Menu만 닫고 게임은 계속 Pause 상태로 유지한다.
                    if (upgradeManager != null &&
                        upgradeManager.IsUpgradePanelOpen)
                    {
                        break;
                    }


                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.Resume();
                    }
                }

                break;


            // =================================================
            // HUD
            // =================================================

            case UIState.HUD:

                if (hudTransition != null)
                {
                    hudTransition.Show();
                }
                else if (hudRoot != null)
                {
                    hudRoot.SetActive(
                        true
                    );
                }


                if (menuUI != null)
                {
                    menuUI.Hide();
                }


                if (settingsPanelUI != null)
                {
                    settingsPanelUI.Hide();
                }


                if (menuButton != null)
                {
                    menuButton.gameObject.SetActive(
                        true
                    );
                }


                if (hudCloseButton != null)
                {
                    hudCloseButton.gameObject.SetActive(
                        true
                    );
                }


                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Pause();
                }

                break;


            // =================================================
            // Menu
            // =================================================

            case UIState.Menu:

                if (hudRoot != null)
                {
                    hudRoot.SetActive(true);
                }


                // HUD의 조작 버튼은
                // Menu와 겹치지 않도록 숨긴다.
                if (menuButton != null)
                {
                    menuButton.gameObject.SetActive(
                        false
                    );
                }


                if (hudCloseButton != null)
                {
                    hudCloseButton.gameObject.SetActive(
                        false
                    );
                }


                if (settingsPanelUI != null)
                {
                    settingsPanelUI.Hide();
                }


                if (menuUI != null)
                {
                    menuUI.Show();
                }


                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Pause();
                }

                break;


            // =================================================
            // Settings
            // =================================================

            case UIState.Settings:

                // HUD 자체는 유지
                if (hudRoot != null)
                {
                    hudRoot.SetActive(true);
                }


                // HUD 조작 버튼 숨기기
                if (menuButton != null)
                {
                    menuButton.gameObject.SetActive(
                        false
                    );
                }


                if (hudCloseButton != null)
                {
                    hudCloseButton.gameObject.SetActive(
                        false
                    );
                }


                // Menu는 숨기고
                if (menuUI != null)
                {
                    menuUI.Hide();
                }


                // Settings 표시
                if (settingsPanelUI != null)
                {
                    settingsPanelUI.Show();
                }


                // Settings에서도 계속 Pause
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Pause();
                }

                break;
        }
    }


    // ========================================================
    // Game Flow
    // ========================================================

    private void Retry()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Retry();
        }
    }


    private void GoLobby()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoLobby();
        }
    }
}