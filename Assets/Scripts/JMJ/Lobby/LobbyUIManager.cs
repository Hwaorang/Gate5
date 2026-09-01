using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [Header("업그레이드 창")]
    [SerializeField] private GameObject upgradePanel;

    [Header("스킨 창")]
    [SerializeField] private GameObject skinPanel;

    [Header("난이도 창")]
    [SerializeField] private GameObject difficultyPanel;


    // =====================================================
    // 업그레이드 창 열기
    // =====================================================

    public void OpenUpgradePanel()
    {
        // 다른 창 닫기
        skinPanel.SetActive(false);
        difficultyPanel.SetActive(false);

        // 업그레이드 창 열기
        upgradePanel.SetActive(true);
    }


    // =====================================================
    // 업그레이드 창 닫기
    // =====================================================

    public void CloseUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }


    // =====================================================
    // 스킨 창 열기
    // =====================================================

    public void OpenSkinPanel()
    {
        // 다른 창 닫기
        upgradePanel.SetActive(false);
        difficultyPanel.SetActive(false);

        // 스킨 창 열기
        skinPanel.SetActive(true);
    }


    // =====================================================
    // 스킨 창 닫기
    // =====================================================

    public void CloseSkinPanel()
    {
        skinPanel.SetActive(false);
    }


    // =====================================================
    // 난이도 창 열기
    // =====================================================

    public void OpenDifficultyPanel()
    {
        // 다른 창 닫기
        upgradePanel.SetActive(false);
        skinPanel.SetActive(false);

        // 난이도 창 열기
        difficultyPanel.SetActive(true);
    }


    // =====================================================
    // 난이도 창 닫기
    // =====================================================

    public void CloseDifficultyPanel()
    {
        difficultyPanel.SetActive(false);
    }
}

