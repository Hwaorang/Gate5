using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LobbyUIManager : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject skinPanel;
    [SerializeField] private GameObject SettingPanel;

    [Header("리셋 확인")]
    [SerializeField] private GameObject resetConfirmPanel;

    [Header("게임 종료 확인")]
    [SerializeField] private GameObject exitConfirmPanel;

    [Header("실제 로비 캐릭터")]
    [SerializeField] private GameObject lobbyCharacter;

    [Header("로비 버튼")]
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject skinButton;
    [SerializeField] private GameObject SettingButton;
    [SerializeField] private GameObject startButton;

    [Header("사운드 설정")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("사운드 수치 표시")]
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text bgmVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    [SerializeField] private float defaultVolume = 0.5f;
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

        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(false);
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
    // 세팅 패널 열기
    // =========================================================

    public void OpenSettingPanel()
    {
        CloseAllPanels();

        if (SettingPanel != null)
        {
            SettingPanel.SetActive(true);
        }

        HideLobbyButtons();

        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =========================================================
    // 세팅 패널 닫기
    // =========================================================

    public void CloseSettingPanel()
    {
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(false);
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
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }

        Debug.Log(
            "리셋 확인창을 열었습니다."
        );
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

        // SettingPanel은 그대로 유지
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(true);
        }

        Debug.Log(
            "리셋을 취소했습니다."
        );
    }


    // =========================================================
    // 리셋 확인
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
        // 2. 오디오 설정 즉시 리셋
        // -----------------------------------------------------

        if (AudioManager_PlayerController.Instance != null)
        {
            // 실제 사운드 초기화
            AudioManager_PlayerController.Instance.SetMasterVolume(defaultVolume);
            AudioManager_PlayerController.Instance.SetBgmVolume(defaultVolume);
            AudioManager_PlayerController.Instance.SetSfxVolume(defaultVolume);
        }

        // Slider 초기화
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(defaultVolume);
        }

        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.SetValueWithoutNotify(defaultVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.SetValueWithoutNotify(defaultVolume);
        }

        // 숫자 표시 초기화
        if (masterVolumeText != null)
        {
            masterVolumeText.text = Mathf.RoundToInt(defaultVolume * 100f).ToString();
        }

        if (bgmVolumeText != null)
        {
            bgmVolumeText.text = Mathf.RoundToInt(defaultVolume * 100f).ToString();
        }

        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = Mathf.RoundToInt(defaultVolume * 100f).ToString();
        }
        // -----------------------------------------------------
        // 3. UI 즉시 갱신
        // -----------------------------------------------------

        RefreshLobbyAfterReset();


        // -----------------------------------------------------
        // 4. 리셋 확인창 닫기
        // -----------------------------------------------------

        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // 5. SettingPanel 유지
        // -----------------------------------------------------

        if (SettingPanel != null)
        {
            SettingPanel.SetActive(true);
        }


        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }


        Debug.Log(
            "===== 게임 데이터 리셋 완료 ====="
        );
    }


    // =========================================================
    // 게임 종료 확인창 열기
    // =========================================================

    public void OpenExitConfirm()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(true);
        }

        Debug.Log(
            "게임 종료 확인창을 열었습니다."
        );
    }


    // =========================================================
    // 게임 종료 취소
    // =========================================================

    public void CancelExit()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(false);
        }

        // SettingPanel 유지
        if (SettingPanel != null)
        {
            SettingPanel.SetActive(true);
        }

        Debug.Log(
            "게임 종료를 취소했습니다."
        );
    }


    // =========================================================
    // 게임 종료 확인
    // =========================================================

    public void ConfirmExit()
    {
        Debug.Log(
            "게임을 종료합니다."
        );

        Application.Quit();
    }


    // =========================================================
    // 리셋 후 모든 UI 갱신
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

        if (SettingPanel != null)
        {
            SettingPanel.SetActive(false);
        }

        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(false);
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

        if (SettingButton != null)
        {
            SettingButton.SetActive(false);
        }

        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // 중요
        // ResetButton은 SettingPanel의 자식이므로
        // 여기서 직접 끄지 않습니다.
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

        if (SettingButton != null)
        {
            SettingButton.SetActive(true);
        }

        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }
}