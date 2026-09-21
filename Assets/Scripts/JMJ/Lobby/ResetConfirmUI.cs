using UnityEngine;

public class ResetConfirmUI : MonoBehaviour
{
    // =====================================================
    // 초기화 확인 패널
    // =====================================================

    [Header("초기화 확인 패널")]
    [SerializeField] private GameObject resetConfirmPanel;


    // =====================================================
    // 시작할 때
    // =====================================================

    private void Start()
    {
        // 게임 시작 시 확인창은 숨김
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }


    // =====================================================
    // 리셋 버튼을 눌렀을 때
    // =====================================================

    public void OpenResetConfirm()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }
    }


    // =====================================================
    // 아니오 버튼
    // =====================================================

    public void CancelReset()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }


    // =====================================================
    // 예 버튼
    // =====================================================

    public void ConfirmReset()
    {
        // SaveManager가 존재하는지 확인
        if (SaveManager.Instance != null)
        {
            // 저장 데이터 초기화
            SaveManager.Instance.ResetData();
        }


        // 확인창 닫기
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }
}

