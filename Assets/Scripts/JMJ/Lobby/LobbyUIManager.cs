using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    // =====================================================
    // 패널
    // =====================================================

    [Header("패널")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject skinPanel;
    [SerializeField] private GameObject difficultyPanel;


    // =====================================================
    // 실제 로비 캐릭터
    // =====================================================

    [Header("실제 로비 캐릭터")]
    [SerializeField] private GameObject lobbyCharacter;


    // =====================================================
    // 강화 패널 열기
    // =====================================================

    public void OpenUpgradePanel()
    {
        // 다른 패널 닫기
        skinPanel.SetActive(false);
        difficultyPanel.SetActive(false);

        // 강화 패널 열기
        upgradePanel.SetActive(true);

        // 실제 로비 캐릭터 보이기
        lobbyCharacter.SetActive(true);
    }


    // =====================================================
    // 강화 패널 닫기
    // =====================================================

    public void CloseUpgradePanel()
    {
        // 강화 패널 닫기
        upgradePanel.SetActive(false);

        // 실제 로비 캐릭터 보이기
        lobbyCharacter.SetActive(true);
    }


    // =====================================================
    // 스킨 패널 열기
    // =====================================================

    public void OpenSkinPanel()
    {
        // 다른 패널 닫기
        upgradePanel.SetActive(false);
        difficultyPanel.SetActive(false);

        // 스킨 패널 열기
        skinPanel.SetActive(true);

        // 실제 로비 캐릭터 숨기기
        lobbyCharacter.SetActive(false);
    }


    // =====================================================
    // 스킨 패널 닫기
    // =====================================================

    public void CloseSkinPanel()
    {
        // 스킨 패널 닫기
        skinPanel.SetActive(false);

        // 실제 로비 캐릭터 다시 보이기
        lobbyCharacter.SetActive(true);
    }


    // =====================================================
    // 난이도 패널 열기
    // =====================================================

    public void OpenDifficultyPanel()
    {
        // 다른 패널 닫기
        upgradePanel.SetActive(false);
        skinPanel.SetActive(false);

        // 난이도 패널 열기
        difficultyPanel.SetActive(true);

        // 실제 로비 캐릭터 보이기
        lobbyCharacter.SetActive(true);
    }


    // =====================================================
    // 난이도 패널 닫기
    // =====================================================

    public void CloseDifficultyPanel()
    {
        // 난이도 패널 닫기
        difficultyPanel.SetActive(false);

        // 실제 로비 캐릭터 다시 보이기
        lobbyCharacter.SetActive(true);
    }
}