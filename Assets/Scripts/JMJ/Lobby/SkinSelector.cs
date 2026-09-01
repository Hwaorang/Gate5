using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    [Header("스킨 오브젝트")]
    [SerializeField] private GameObject[] skins;

    [Header("스킨 이름")]
    [SerializeField] private string[] skinNames;

    [Header("스킨 이름 UI")]
    [SerializeField] private TMP_Text skinNameText;

    [Header("스킨 변경 버튼")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("버튼 색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;


    // 현재 선택된 스킨 번호
    private int currentSkin;


    // =====================================================
    // 시작
    // =====================================================

    private void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 존재하지 않습니다.");
            return;
        }

        // 저장된 스킨 번호 가져오기
        currentSkin =
            SaveManager.Instance.Data.selectedSkin;


        // 스킨이 없는 경우
        if (skins == null || skins.Length == 0)
        {
            Debug.LogWarning("등록된 스킨이 없습니다.");
            return;
        }


        // 잘못된 번호가 저장되어 있다면 0번으로
        if (currentSkin < 0 ||
            currentSkin >= skins.Length)
        {
            currentSkin = 0;
        }


        // 스킨 적용
        ApplySkin();
    }


    // =====================================================
    // 다음 스킨
    // =====================================================

    public void NextSkin()
    {
        // 스킨이 없는 경우
        if (skins == null || skins.Length == 0)
        {
            return;
        }


        // 마지막 스킨이면 이동하지 않음
        if (currentSkin >= skins.Length - 1)
        {
            return;
        }


        // 다음 스킨으로 이동
        currentSkin++;


        // 스킨 적용
        ApplySkin();


        // 저장
        SaveSkin();
    }


    // =====================================================
    // 이전 스킨
    // =====================================================

    public void PreviousSkin()
    {
        // 스킨이 없는 경우
        if (skins == null || skins.Length == 0)
        {
            return;
        }


        // 첫 번째 스킨이면 이동하지 않음
        if (currentSkin <= 0)
        {
            return;
        }


        // 이전 스킨으로 이동
        currentSkin--;


        // 스킨 적용
        ApplySkin();


        // 저장
        SaveSkin();
    }


    // =====================================================
    // 스킨 적용
    // =====================================================

    private void ApplySkin()
    {
        // 모든 스킨 확인
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] != null)
            {
                // 현재 스킨만 활성화
                skins[i].SetActive(i == currentSkin);
            }
        }


        // 스킨 이름 변경
        UpdateSkinName();


        // 버튼 상태 변경
        UpdateButtons();
    }


    // =====================================================
    // 스킨 이름 변경
    // =====================================================

    private void UpdateSkinName()
    {
        if (skinNameText == null)
        {
            return;
        }


        if (skinNames != null &&
            currentSkin >= 0 &&
            currentSkin < skinNames.Length)
        {
            skinNameText.text =
                skinNames[currentSkin];
        }
        else
        {
            skinNameText.text =
                "Skin " + (currentSkin + 1);
        }
    }


    // =====================================================
    // 버튼 상태 변경
    // =====================================================

    private void UpdateButtons()
    {
        // 첫 번째 스킨이면 이전 버튼 비활성화
        bool canGoPrevious =
            currentSkin > 0;


        // 마지막 스킨이면 다음 버튼 비활성화
        bool canGoNext =
            currentSkin < skins.Length - 1;


        // 버튼 색상 변경
        SetButtonColor(
            previousButton,
            canGoPrevious
        );


        SetButtonColor(
            nextButton,
            canGoNext
        );
    }


    // =====================================================
    // 버튼 색상 변경
    // =====================================================

    private void SetButtonColor(
    Button button,
    bool active)
    {
        if (button == null)
        {
            return;
        }


        // 버튼 클릭 가능 여부
        button.interactable = active;


        // 버튼 이미지
        Image image =
            button.GetComponent<Image>();


        if (image == null)
        {
            return;
        }


        // 활성 / 비활성 색상
        if (active)
        {
            image.color = normalColor;
        }
        else
        {
            image.color = disabledColor;
        }
    }


    // =====================================================
    // 스킨 저장
    // =====================================================

    private void SaveSkin()
    {
        SaveManager.Instance.Data.selectedSkin =
            currentSkin;


        SaveManager.Instance.Save();


        Debug.Log(
            "현재 스킨 저장 : " +
            currentSkin
        );
    }


    // =====================================================
    // 현재 스킨 번호 가져오기
    // =====================================================

    public int GetCurrentSkin()
    {
        return currentSkin;
    }
}