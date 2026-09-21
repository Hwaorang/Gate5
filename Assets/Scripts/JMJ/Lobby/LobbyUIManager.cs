using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // =====================================================
    // 패널
    // =====================================================

    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject skinPanel;


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


    // =====================================================
    // 게임 시작 시
    // =====================================================

    private void Start()
    {
        // 처음에는 모든 패널 닫기
        CloseAllPanels();

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
        // ---------------------------------------------
        // 1. 스킨 패널 닫기
        // ---------------------------------------------

        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }


        // ---------------------------------------------
        // 2. 업그레이드 패널 열기
        // ---------------------------------------------

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }


        // ---------------------------------------------
        // 3. 로비 버튼 숨기기
        // ---------------------------------------------

        HideLobbyButtons();


        // ---------------------------------------------
        // 4. 실제 로비 캐릭터 보이기
        // ---------------------------------------------

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
        // ---------------------------------------------
        // 1. 업그레이드 패널 닫기
        // ---------------------------------------------

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }


        // ---------------------------------------------
        // 2. 로비 버튼 다시 보이기
        // ---------------------------------------------

        ShowLobbyButtons();


        // ---------------------------------------------
        // 3. 실제 로비 캐릭터 보이기
        // ---------------------------------------------

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
        // ---------------------------------------------
        // 1. 업그레이드 패널 닫기
        // ---------------------------------------------

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }


        // ---------------------------------------------
        // 2. 스킨 패널 열기
        // ---------------------------------------------

        if (skinPanel != null)
        {
            skinPanel.SetActive(true);
        }


        // ---------------------------------------------
        // 3. 로비 버튼 숨기기
        // ---------------------------------------------

        HideLobbyButtons();


        // ---------------------------------------------
        // 4. 실제 로비 캐릭터 숨기기
        // ---------------------------------------------

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
        // ---------------------------------------------
        // 1. 스킨 패널 닫기
        // ---------------------------------------------

        if (skinPanel != null)
        {
            skinPanel.SetActive(false);
        }


        // ---------------------------------------------
        // 2. 로비 버튼 다시 보이기
        // ---------------------------------------------

        ShowLobbyButtons();


        // ---------------------------------------------
        // 3. 실제 로비 캐릭터 다시 보이기
        // ---------------------------------------------

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
        // 업그레이드 버튼 숨기기
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(false);
        }


        // 스킨 버튼 숨기기
        if (skinButton != null)
        {
            skinButton.SetActive(false);
        }


        // 게임 시작 버튼 숨기기
        if (startButton != null)
        {
            startButton.SetActive(false);
        }
    }


    // =====================================================
    // 로비 버튼 다시 보이기
    // =====================================================

    private void ShowLobbyButtons()
    {
        // 업그레이드 버튼 보이기
        if (upgradeButton != null)
        {
            upgradeButton.SetActive(true);
        }


        // 스킨 버튼 보이기
        if (skinButton != null)
        {
            skinButton.SetActive(true);
        }


        // 게임 시작 버튼 보이기
        if (startButton != null)
        {
            startButton.SetActive(true);
        }
    }
}