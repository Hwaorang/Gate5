using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 인게임 HUD / Menu의 상태를 관리한다.
///
/// Closed
/// - HUD OFF
/// - Menu OFF
/// - 게임 진행
///
/// HUD
/// - HUD ON
/// - Menu OFF
/// - 게임 Pause
///
/// Menu
/// - HUD ON
/// - Menu ON
/// - 게임 Pause
/// </summary>
public class InGameMenuPresenter : MonoBehaviour
{
    private enum UIState
    {
        Closed,
        HUD,
        Menu
    }


    [Header("HUD")]

    // 게임 시작 시 항상 보이는 HUD 열기 버튼
    [SerializeField]
    private Button hudToggleButton;

    // 실제 진행 정보가 표시되는 HUD
    [SerializeField]
    private GameObject hudRoot;

    // HUD 내부의 닫기 버튼
    // 필요 없으면 None이어도 된다.
    [SerializeField]
    private Button hudCloseButton;

    // HUD 내부의 Menu 버튼
    [SerializeField]
    private Button menuButton;


    [Header("Menu")]

    [SerializeField]
    private InGameMenuUI menuUI;


    [Header("Settings")]

    [SerializeField]
    private GameObject settingsPanel;


    private UIState currentState =
        UIState.Closed;


    private void Start()
    {
        // =========================
        // 게임 시작 상태
        // =========================

        SetState(
            UIState.Closed
        );
    }


    private void Update()
    {
        // =========================
        // ESC 입력
        // =========================

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


    private void OnEnable()
    {
        // =========================
        // HUD 버튼
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
        // Menu 내부 버튼
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
    }


    private void OnDisable()
    {
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
    }


    /// <summary>
    /// HUD 열기 버튼.
    ///
    /// 닫혀있으면 HUD를 열고,
    /// HUD/Menu가 열려있다면 모두 닫는다.
    /// </summary>
    private void ToggleHUD()
    {
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
    /// HUD에서 Menu 버튼을 눌렀을 때.
    /// </summary>
    private void OpenMenu()
    {
        SetState(
            UIState.Menu
        );
    }


    /// <summary>
    /// Continue 또는 HUD 닫기.
    /// </summary>
    private void CloseAll()
    {
        SetState(
            UIState.Closed
        );
    }


    /// <summary>
    /// ESC는 현재 UI 상태에 따라 동작한다.
    /// </summary>
    private void HandleEscape()
    {
        switch (currentState)
        {
            // 아무 UI도 없으면 HUD 열기
            case UIState.Closed:

                SetState(
                    UIState.HUD
                );

                break;


            // HUD가 열려있으면 닫기
            case UIState.HUD:

                SetState(
                    UIState.Closed
                );

                break;


            // Menu가 열려있으면
            // Menu와 HUD 모두 닫기
            case UIState.Menu:

                SetState(
                    UIState.Closed
                );

                break;
        }
    }


    /// <summary>
    /// UI 상태를 한 곳에서 변경한다.
    /// </summary>
    private void SetState(
        UIState newState)
    {
        currentState =
            newState;


        // =========================
        // Closed
        // =========================

        if(currentState == UIState.Closed)
{
            if (menuUI != null)
            {
                menuUI.Hide();
            }

            if (hudRoot != null)
            {
                hudRoot.SetActive(false);
            }

            // 다음에 HUD를 열었을 때 다시 보이도록
            if (menuButton != null)
            {
                menuButton.gameObject.SetActive(true);
            }

            if (hudCloseButton != null)
            {
                hudCloseButton.gameObject.SetActive(true);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.Resume();
            }

            return;
        }


        // =========================
        // HUD
        // =========================

        if (currentState == UIState.HUD)
        {
            if (hudRoot != null)
            {
                hudRoot.SetActive(true);
            }

            if (menuButton != null)
            {
                menuButton.gameObject.SetActive(true);
            }

            if (hudCloseButton != null)
            {
                hudCloseButton.gameObject.SetActive(true);
            }

            if (menuUI != null)
            {
                menuUI.Hide();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.Pause();
            }

            return;
        }


        // =========================
        // Menu
        // =========================

        if (currentState == UIState.Menu)
        {
            if (hudRoot != null)
            {
                hudRoot.SetActive(true);
            }

            // 메뉴가 열려 있을 동안
            // HUD의 조작 버튼은 숨긴다.
            if (menuButton != null)
            {
                menuButton.gameObject.SetActive(false);
            }

            if (hudCloseButton != null)
            {
                hudCloseButton.gameObject.SetActive(false);
            }

            if (menuUI != null)
            {
                menuUI.Show();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.Pause();
            }

            return;
        }
    }


    private void OpenSettings()
    {
        if (settingsPanel == null)
        {
            return;
        }

        // 이후 설정창 단계에서 조금 더 다듬을 예정
        settingsPanel.SetActive(true);
    }


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