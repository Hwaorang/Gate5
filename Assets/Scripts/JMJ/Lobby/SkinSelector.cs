using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    [Header("스킨")]
    [SerializeField] private GameObject[] skins;

    [Header("스킨 이름")]
    [SerializeField] private string[] skinNames;
    [SerializeField] private TMP_Text skinNameText;

    [Header("버튼")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("버튼 색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    private int currentSkin;


    // =========================================================
    // 시작
    // =========================================================

    private void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "SaveManager가 없습니다."
            );

            return;
        }

        currentSkin =
            SaveManager.Instance.Data.selectedSkin;

        ClampCurrentSkin();

        UpdateSkin();
        UpdateButtons();
        UpdateSkinName();
    }


    // =========================================================
    // 이전 스킨
    // =========================================================

    public void PreviousSkin()
    {
        if (currentSkin <= 0)
        {
            return;
        }

        currentSkin--;

        SaveCurrentSkin();

        UpdateSkin();
        UpdateButtons();
        UpdateSkinName();
    }


    // =========================================================
    // 다음 스킨
    // =========================================================

    public void NextSkin()
    {
        if (skins == null ||
            skins.Length == 0)
        {
            return;
        }

        if (currentSkin >= skins.Length - 1)
        {
            return;
        }

        currentSkin++;

        SaveCurrentSkin();

        UpdateSkin();
        UpdateButtons();
        UpdateSkinName();
    }


    // =========================================================
    // 현재 스킨 저장
    // =========================================================

    private void SaveCurrentSkin()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.Data.selectedSkin =
            currentSkin;

        SaveManager.Instance.Save();
    }


    // =========================================================
    // 스킨 표시
    // =========================================================

    private void UpdateSkin()
    {
        if (skins == null)
        {
            return;
        }

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
            {
                continue;
            }

            skins[i].SetActive(
                i == currentSkin
            );
        }
    }


    // =========================================================
    // 이전 / 다음 버튼 상태
    // =========================================================

    private void UpdateButtons()
    {
        if (skins == null ||
            skins.Length == 0)
        {
            return;
        }

        if (previousButton != null)
        {
            bool canGoPrevious =
                currentSkin > 0;

            previousButton.interactable =
                canGoPrevious;

            SetButtonColor(
                previousButton,
                canGoPrevious
            );
        }

        if (nextButton != null)
        {
            bool canGoNext =
                currentSkin < skins.Length - 1;

            nextButton.interactable =
                canGoNext;

            SetButtonColor(
                nextButton,
                canGoNext
            );
        }
    }


    // =========================================================
    // 버튼 색상
    // =========================================================

    private void SetButtonColor(
        Button button,
        bool interactable
    )
    {
        if (button == null)
        {
            return;
        }

        ColorBlock colors =
            button.colors;

        colors.normalColor =
            interactable
                ? normalColor
                : disabledColor;

        colors.highlightedColor =
            interactable
                ? normalColor
                : disabledColor;

        colors.pressedColor =
            interactable
                ? normalColor
                : disabledColor;

        colors.selectedColor =
            interactable
                ? normalColor
                : disabledColor;

        button.colors = colors;
    }


    // =========================================================
    // 스킨 이름
    // =========================================================

    private void UpdateSkinName()
    {
        if (skinNameText == null)
        {
            return;
        }

        if (skinNames == null ||
            currentSkin < 0 ||
            currentSkin >= skinNames.Length)
        {
            skinNameText.text = "";

            return;
        }

        skinNameText.text =
            skinNames[currentSkin];
    }


    // =========================================================
    // 스킨 번호 보정
    // =========================================================

    private void ClampCurrentSkin()
    {
        if (skins == null ||
            skins.Length == 0)
        {
            currentSkin = 0;

            return;
        }

        if (currentSkin < 0)
        {
            currentSkin = 0;
        }

        if (currentSkin >= skins.Length)
        {
            currentSkin =
                skins.Length - 1;
        }
    }


    // =========================================================
    // ★ 리셋 직후 호출
    // =========================================================

    public void RefreshAfterReset()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        // 저장된 스킨 번호 다시 읽기
        currentSkin =
            SaveManager.Instance.Data.selectedSkin;

        ClampCurrentSkin();

        // 화면 즉시 갱신
        UpdateSkin();
        UpdateSkinName();
        UpdateButtons();

        Debug.Log(
            "SkinSelector 실시간 갱신 완료 : " +
            currentSkin
        );
    }
}

