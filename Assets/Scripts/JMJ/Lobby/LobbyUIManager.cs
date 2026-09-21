using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // =====================================================
    // 패널
    // =====================================================

    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject skinPanel;

    // 초기화 확인창
    [SerializeField] private GameObject resetConfirmPanel;


    // =====================================================
    // 실제 로비 캐릭터
    // =====================================================

    [Header("실제 로비 캐릭터")]
    [SerializeField] private GameObject lobbyCharacter;


    // =====================================================
    // 로비 버튼
    // =====================================================

    [Header("로비 버튼")]
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject skinButton;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject resetButton;


    // =====================================================
    // 게임 시작 시
    // =====================================================

    private void Start()
    {
        // 모든 패널 닫기
        CloseAllPanels();

        // 초기화 확인창 닫기
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        // 로비 버튼 보이기
        ShowLobbyButtons();

        // 실제 로비 캐릭터 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 업그레이드 패널 열기
    // =====================================================

    public void OpenUpgradePanel()
    {
        // 스킨 패널 닫기
        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }

        // 업그레이드 패널 열기
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }

        // 로비 버튼 숨기기
        HideLobbyButtons();

        // 로비 캐릭터 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 업그레이드 패널 닫기
    // =====================================================

    public void CloseUpgradePanel()
    {
        // 업그레이드 패널 닫기
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        // 로비 버튼 다시 보이기
        ShowLobbyButtons();

        // 로비 캐릭터 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 스킨 패널 열기
    // =====================================================

    public void OpenSkinPanel()
    {
        // 업그레이드 패널 닫기
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        // 스킨 패널 열기
        if (skinPanel != null)
        {
            skinPanel.SetActive(true);
        }

        // 로비 버튼 숨기기
        HideLobbyButtons();

        // 실제 로비 캐릭터 숨기기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(false);
        }
    }


    // =====================================================
    // 스킨 패널 닫기
    // =====================================================

    public void CloseSkinPanel()
    {
        // 스킨 패널 닫기
        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }

        // 로비 버튼 다시 보이기
        ShowLobbyButtons();

        // 로비 캐릭터 다시 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 리셋 확인창 열기
    // =====================================================

    public void OpenResetConfirm()
    {
        // 다른 패널 닫기
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }

        // 리셋 확인창 열기
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }

        // 로비 버튼 숨기기
        HideLobbyButtons();

        // 로비 캐릭터는 보이게 유지
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 리셋 취소
    // =====================================================

    public void CancelReset()
    {
        // 리셋 확인창 닫기
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        // 로비 버튼 다시 보이기
        ShowLobbyButtons();

        // 로비 캐릭터 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 리셋 확인
    // =====================================================

    public void ConfirmReset()
    {
        // SaveManager의 저장 데이터 초기화
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetData();
        }

        // 리셋 확인창 닫기
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }

        // 로비 버튼 다시 보이기
        ShowLobbyButtons();

        // 로비 캐릭터 보이기
        if (lobbyCharacter != null)
        {
            lobbyCharacter.SetActive(true);
        }
    }


    // =====================================================
    // 모든 패널 닫기
    // =====================================================

    private void CloseAllPanels()
    {
        // 업그레이드 패널 닫기
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }

        // 스킨 패널 닫기
        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }
    }


    // =====================================================
    // 로비 버튼 숨기기
    // =====================================================

    private void HideLobbyButtons()
    {
        // 업그레이드 버튼
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }

        // 스킨 버튼
        if (skinButton != null)
        {
            skinButton.SetActive(false);
        }

        // 게임 시작 버튼
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // 리셋 버튼
        if (resetButton != null)
        {
            resetButton.SetActive(false);
        }
    }


    // =====================================================
    // 로비 버튼 다시 보이기
    // =====================================================

    private void ShowLobbyButtons()
    {
        // 업그레이드 버튼
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(true);
        }

        // 스킨 버튼
        if (skinButton != null)
        {
            skinButton.SetActive(true);
        }

        // 게임 시작 버튼
        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        // 리셋 버튼
        if (resetButton != null)
        {
            resetButton.SetActive(true);
        }
    }
}
