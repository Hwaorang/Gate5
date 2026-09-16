using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
    [Header("실제 로비 캐릭터 스킨")]
    [SerializeField] private GameObject[] skins;

    [Header("스킨 패널 미리보기 스킨")]
    [SerializeField] private GameObject[] previewSkins;

    [Header("스킨 이름")]
    [SerializeField] private string[] skinNames;

    [Header("UI")]
    [SerializeField] private TMP_Text skinNameText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    [Header("버튼 색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    private int currentSkin;


    private void Start()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager가 존재하지 않습니다.");
            return;
        }

        currentSkin = SaveManager.Instance.Data.selectedSkin;

        if (skins == null || skins.Length == 0)
        {
            Debug.LogWarning("실제 로비 스킨이 등록되어 있지 않습니다.");
            return;
        }

        if (currentSkin < 0 || currentSkin >= skins.Length)
        {
            currentSkin = 0;
        }

        ApplySkin();
    }


    // =====================================================
    // 다음 스킨
    // =====================================================

    public void NextSkin()
    {
        if (skins == null || skins.Length == 0)
        {
            return;
        }

        if (currentSkin >= skins.Length - 1)
        {
            return;
        }

        currentSkin++;

        ApplySkin();

        SaveSkin();
    }


    // =====================================================
    // 이전 스킨
    // =====================================================

    public void PreviousSkin()
    {
        if (skins == null || skins.Length == 0)
        {
            return;
        }

        if (currentSkin <= 0)
        {
            return;
        }

        currentSkin--;

        ApplySkin();

        SaveSkin();
    }


    // =====================================================
    // 스킨 적용
    // =====================================================

    private void ApplySkin()
    {
        Debug.Log("현재 스킨 번호 : " + currentSkin);


        // =================================================
        // 실제 로비 캐릭터
        // =================================================

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
            {
                Debug.LogWarning(
                    "Skins[" + i + "]가 비어 있습니다."
                );

                continue;
            }

            bool active = (i == currentSkin);

            skins[i].SetActive(active);

            Debug.Log(
                "실제 스킨 [" + i + "] : " +
                skins[i].name +
                " / Active = " +
                active
            );
        }


        // =================================================
        // 스킨 패널 미리보기 캐릭터
        // =================================================

        if (previewSkins == null || previewSkins.Length == 0)
        {
            Debug.LogError("Preview Skins가 비어 있습니다!");
        }
        else
        {
            for (int i = 0; i < previewSkins.Length; i++)
            {
                if (previewSkins[i] == null)
                {
                    Debug.LogError(
                        "Preview Skins[" + i + "]가 비어 있습니다!"
                    );

                    continue;
                }

                bool active = (i == currentSkin);

                previewSkins[i].SetActive(active);

                Debug.Log(
                    "미리보기 변경 : " +
                    previewSkins[i].name +
                    " / 부모 : " +
                    previewSkins[i].transform.parent.name +
                    " / Active : " +
                    previewSkins[i].activeSelf +
                    " / 현재 스킨 : " +
                    currentSkin
                );
            }
        }


        // =================================================
        // UI
        // =================================================

        UpdateSkinName();
        UpdateButtons();
    }


    // =====================================================
    // 스킨 이름
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
            skinNameText.text = skinNames[currentSkin];
        }
        else
        {
            skinNameText.text = "Skin " + (currentSkin + 1);
        }
    }


    // =====================================================
    // 버튼 상태
    // =====================================================

    private void UpdateButtons()
    {
        bool canGoPrevious = currentSkin > 0;

        bool canGoNext =
            currentSkin < skins.Length - 1;

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
    // 버튼 색상
    // =====================================================

    private void SetButtonColor(
        Button button,
        bool active)
    {
        if (button == null)
        {
            return;
        }

        button.interactable = active;

        Image image =
            button.GetComponent<Image>();

        if (image == null)
        {
            return;
        }

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
    // 저장
    // =====================================================

    private void SaveSkin()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.Data.selectedSkin =
            currentSkin;

        SaveManager.Instance.Save();

        Debug.Log(
            "현재 스킨 저장 : " +
            currentSkin
        );
    }


    // =====================================================
    // 현재 스킨 번호
    // =====================================================

    public int GetCurrentSkin()
    {
        return currentSkin;
    }
}