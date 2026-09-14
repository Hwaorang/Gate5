using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인게임 HUD 화면 표시만 담당한다.
/// 게임 로직은 직접 참조하지 않는다.
/// </summary>
public class GameHUDUI : MonoBehaviour
{
    [Header("Game Info")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;


    [Header("Skill HUD")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text projectileText;
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Decorative Images")]
    [SerializeField] private Image levelBoxImage;
    [SerializeField] private Image timerBoxImage;

    private void Awake()
    {
        SetupHudLayout();
        SetupDecorativeImages();
    }

    private void SetupHudLayout()
    {
        // =========================
        // StatusPanel 크게 만들기
        // =========================

        Transform statusPanelTransform =
            transform.Find("StatusPanel");

        if (statusPanelTransform != null)
        {
            RectTransform statusPanel =
                statusPanelTransform.GetComponent<RectTransform>();

            if (statusPanel != null)
            {
                statusPanel.sizeDelta =
                    new Vector2(
                        1350f,
                        820f
                    );
            }
        }


        // =========================
        // Header 설정
        // =========================

        Transform headerTransform =
            transform.Find("StatusPanel/Header");

        if (headerTransform != null)
        {
            RectTransform header =
                headerTransform.GetComponent<RectTransform>();

            if (header != null)
            {
                // Header 높이를 키운다.
                header.offsetMin =
                    new Vector2(
                        70f,
                        -250f
                    );

                header.offsetMax =
                    new Vector2(
                        -70f,
                        -90f
                    );
            }


            HorizontalLayoutGroup layout =
                headerTransform.GetComponent<HorizontalLayoutGroup>();

            if (layout != null)
            {
                layout.spacing = 60f;

                layout.childAlignment =
                    TextAnchor.MiddleCenter;

                // 자식 크기를 강제로 화면 전체까지
                // 늘리지 않도록 변경
                layout.childForceExpandWidth = false;
                layout.childForceExpandHeight = false;

                layout.childControlWidth = true;
                layout.childControlHeight = true;
            }
        }


        // =========================
        // LevelBox 크게 만들기
        // =========================

        SetupHeaderBox(
            "StatusPanel/Header/LevelBox",
            480f,
            160f
        );


        // =========================
        // TimerBox 크게 만들기
        // =========================

        SetupHeaderBox(
            "StatusPanel/Header/TimerBox",
            480f,
            160f
        );
    }


    private void SetupHeaderBox(
        string path,
        float width,
        float height)
    {
        Transform target =
            transform.Find(path);

        if (target == null)
        {
            return;
        }


        LayoutElement layout =
            target.GetComponent<LayoutElement>();

        if (layout == null)
        {
            layout =
                target.gameObject.AddComponent<LayoutElement>();
        }


        layout.preferredWidth =
            width;

        layout.preferredHeight =
            height;

        layout.minWidth =
            width;

        layout.minHeight =
            height;

        layout.flexibleWidth =
            0f;

        layout.flexibleHeight =
            0f;
    }

    private void SetupDecorativeImages()
    {
        // =========================
        // LevelBox
        // =========================

        Transform levelBox =
            transform.Find(
                "StatusPanel/Header/LevelBox"
            );

        if (levelBox != null)
        {
            Image image =
                levelBox.GetComponent<Image>();

            if (image != null)
            {
                image.type =
                    Image.Type.Simple;

                image.preserveAspect =
                    true;
            }
        }


        // =========================
        // TimerBox
        // =========================

        Transform timerBox =
            transform.Find(
                "StatusPanel/Header/TimerBox"
            );

        if (timerBox != null)
        {
            Image image =
                timerBox.GetComponent<Image>();

            if (image != null)
            {
                image.type =
                    Image.Type.Simple;

                image.preserveAspect =
                    true;
            }
        }
    }


    public void SetLevel(int level)
    {
        if (levelText == null)
        {
            return;
        }

        levelText.text = $"Lv. {level}";
    }


    public void SetTime(float time)
    {
        if (timerText == null)
        {
            return;
        }

        int minutes =
            Mathf.FloorToInt(time / 60f);

        int seconds =
            Mathf.FloorToInt(time % 60f);

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }


    public void SetDamageLevel(int level)
    {
        SetSkillLevel(
            damageText,
            "damage",
            level
        );
    }


    public void SetAttackSpeedLevel(int level)
    {
        SetSkillLevel(
            attackSpeedText,
            "attackSpeed",
            level
        );
    }


    public void SetProjectileLevel(int level)
    {
        SetSkillLevel(
            projectileText,
            "projectile",
            level
        );
    }


    public void SetMoveSpeedLevel(int level)
    {
        SetSkillLevel(
            moveSpeedText,
            "moveSpeed",
            level
        );
    }


    private void SetSkillLevel(
        TMP_Text text,
        string skillName,
        int level)
    {
        if (text == null)
        {
            return;
        }

        // 아직 획득하지 않은 강화
        if (level <= 0)
        {
            text.gameObject.SetActive(false);
            return;
        }

        text.gameObject.SetActive(true);

        text.text =
            $"{skillName} Lv.{level}";
    }
}