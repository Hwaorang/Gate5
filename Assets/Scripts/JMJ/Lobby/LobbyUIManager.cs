using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject skinPanel;
    [SerializeField] private GameObject difficultyPanel;

    [Header("리셋 확인")]
    [SerializeField] private GameObject resetConfirmPanel;

    [Header("실제 로비 캐릭터")]
    [SerializeField] private GameObject lobbyCharacter;

    [Header("로비 버튼")]
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject skinButton;
    [SerializeField] private GameObject difficultyButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject resetButton;


    // =========================================================
    // 시작
    // =========================================================

    private void Start()
    {
        CloseAllPanels();

        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        ShowLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 업그레이드 패널 열기
    // =========================================================

    public void OpenUpgradePanel()
    {
        CloseAllPanels();

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }

        HideLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 업그레이드 패널 닫기
    // =========================================================

    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        ShowLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 스킨 패널 열기
    // =========================================================

    public void OpenSkinPanel()
    {
        CloseAllPanels();

        if (skinPanel != null)
        {
            skinPanel.SetActive(true);
        }

        HideLobbyButtons();

        // 스킨 패널에서는
        // 원래 로비 캐릭터 숨김
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(false);
        }
    }


    // =========================================================
    // 스킨 패널 닫기
    // =========================================================

    public void CloseSkinPanel()
    {
        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }

        ShowLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 난이도 패널 열기
    // =========================================================

    public void OpenDifficultyPanel()
    {
        CloseAllPanels();

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }

        HideLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 난이도 패널 닫기
    // =========================================================

    public void CloseDifficultyPanel()
    {
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }

        ShowLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 리셋 확인창 열기
    // =========================================================

    public void OpenResetConfirm()
    {
        CloseAllPanels();

        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }

        HideLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 리셋 취소
    // =========================================================

    public void CancelReset()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        ShowLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // ★ 리셋 확인
    // =========================================================

    public void ConfirmReset()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "SaveManager가 없습니다."
            );

            return;
        }

        Debug.Log(
            "===== 게임 데이터 리셋 시작 ====="
        );


        // -----------------------------------------------------
        // 1. 저장 데이터 리셋
        // -----------------------------------------------------

        SaveManager.Instance.ResetData();


        // -----------------------------------------------------
        // 2. UI 즉시 갱신
        // -----------------------------------------------------

        RefreshLobbyAfterReset();


        // -----------------------------------------------------
        // 3. 리셋 확인창 닫기
        // -----------------------------------------------------

        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // 4. 로비 버튼 다시 표시
        // -----------------------------------------------------

        ShowLobbyButtons();


        // -----------------------------------------------------
        // 5. 로비 캐릭터 다시 표시
        // -----------------------------------------------------

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }


        Debug.Log(
            "===== 게임 데이터 리셋 완료 ====="
        );
    }


    // =========================================================
    // ★ 리셋 후 모든 UI 갱신
    // =========================================================

    private void RefreshLobbyAfterReset()
    {
        Debug.Log(
            "===== 리셋 후 UI 갱신 시작 ====="
        );


        // -----------------------------------------------------
        // SkinSelector
        // -----------------------------------------------------

        SkinSelector[] skinSelectors =
            FindObjectsByType<SkinSelector>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (
            SkinSelector selector
            in skinSelectors
        )
        {
            selector.RefreshAfterReset();
        }


        // -----------------------------------------------------
        // UpgradeManager
        // -----------------------------------------------------

        UpgradeManager[] upgradeManagers =
            FindObjectsByType<UpgradeManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (
            UpgradeManager upgradeManager
            in upgradeManagers
        )
        {
            upgradeManager.RefreshAfterReset();
        }


        Debug.Log(
            "===== 리셋 후 UI 갱신 완료 ====="
        );
    }


    // =========================================================
    // 모든 패널 닫기
    // =========================================================

    private void CloseAllPanels()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }
    }


    // =========================================================
    // 로비 버튼 숨기기
    // =========================================================

    private void HideLobbyButtons()
    {
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }

        if (skinButton != null)
        {
            skinButton.SetActive(false);
        }

        if (difficultyButton != null)
        {
            difficultyButton.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        if (resetButton != null)
        {
            resetButton.SetActive(false);
        }
    }


    // =========================================================
    // 로비 버튼 표시
    // =========================================================

    private void ShowLobbyButtons()
    {
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(true);
        }

        if (skinButton != null)
        {
            skinButton.SetActive(true);
        }

        if (difficultyButton != null)
        {
            difficultyButton.SetActive(true);
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        if (resetButton != null)
        {
            resetButton.SetActive(true);
        }
    }
}

