using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인게임 HUD를 자동 생성하는 Editor Tool.
///
/// 사용 방법:
/// 1. Canvas_Game 선택
/// 2. Tools -> UI -> Create Game HUD
/// 3. HUD 자동 생성
///
/// 기존 HUD가 있다면 HUD_Backup으로 변경하고 비활성화한다.
/// </summary>
public static class GameHUDBuilder
{
    // =========================
    // UI Color
    // =========================

    private static readonly Color Orange =
        new Color32(244, 158, 35, 255);

    private static readonly Color DarkOrange =
        new Color32(215, 112, 12, 255);

    private static readonly Color Cream =
        new Color32(255, 244, 214, 255);

    private static readonly Color Brown =
        new Color32(100, 55, 10, 255);

    private static readonly Color Dim =
        new Color(0f, 0f, 0f, 0.45f);


    [MenuItem("Tools/UI/Create Game HUD")]
    public static void CreateGameHUD()
    {
        GameObject selected =
            Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Game HUD Builder",
                "Canvas_Game을 먼저 선택해주세요.",
                "확인"
            );

            return;
        }


        Canvas canvas =
            selected.GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas =
                selected.GetComponentInParent<Canvas>();
        }


        if (canvas == null)
        {
            EditorUtility.DisplayDialog(
                "Game HUD Builder",
                "선택한 오브젝트에서 Canvas를 찾지 못했습니다.\nCanvas_Game을 선택해주세요.",
                "확인"
            );

            return;
        }


        // =========================
        // 기존 HUD 백업
        // =========================

        Transform oldHUD =
            canvas.transform.Find("HUD");

        if (oldHUD != null)
        {
            Undo.RecordObject(
                oldHUD.gameObject,
                "Backup Old HUD"
            );

            oldHUD.name = "HUD_Backup";

            oldHUD.gameObject.SetActive(false);
        }


        // =========================
        // HUD Root
        // =========================

        RectTransform hud =
            CreateRect(
                "HUD",
                canvas.transform
            );

        StretchFull(hud);


        // HUD 전체 뒤 배경
        Image dimBackground =
            CreateImage(
                "DimBackground",
                hud,
                Dim
            );

        StretchFull(
            dimBackground.rectTransform
        );


        // =========================
        // Status Panel
        // =========================

        Image statusPanel =
            CreateImage(
                "StatusPanel",
                hud,
                Orange
            );

        SetCenter(
            statusPanel.rectTransform,
            1100f,
            720f,
            0f,
            0f
        );


        // =========================
        // Title
        // =========================

        TMP_Text titleText =
            CreateText(
                "TitleText",
                statusPanel.transform,
                "STATUS",
                44f,
                Color.white,
                TextAlignmentOptions.Center
            );

        SetTopCenter(
            titleText.rectTransform,
            400f,
            60f,
            0f,
            20f
        );


        // =========================
        // Header
        // =========================

        RectTransform header =
            CreateRect(
                "Header",
                statusPanel.transform
            );

        SetTopStretch(
            header,
            60f,
            60f,
            95f,
            110f
        );


        HorizontalLayoutGroup headerLayout =
            header.gameObject.AddComponent
                <HorizontalLayoutGroup>();

        headerLayout.spacing = 30f;

        headerLayout.childAlignment =
            TextAnchor.MiddleCenter;

        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;

        headerLayout.childForceExpandWidth = true;
        headerLayout.childForceExpandHeight = true;


        // =========================
        // Level Box
        // =========================

        Image levelBox =
            CreateImage(
                "LevelBox",
                header,
                Cream
            );

        levelBox.type = Image.Type.Simple;

        levelBox.preserveAspect =
            true;

        AddLayoutElement(
            levelBox.gameObject,
            100f
        );


        TMP_Text levelText =
            CreateText(
                "LevelText",
                levelBox.transform,
                "Lv. 1",
                38f,
                Brown,
                TextAlignmentOptions.Center
            );

        StretchFullWithPadding(
            levelText.rectTransform,
            15f
        );


        // =========================
        // Timer Box
        // =========================

        Image timerBox =
            CreateImage(
                "TimerBox",
                header,
                Cream
            );

        timerBox.type = Image.Type.Simple;

        timerBox.preserveAspect =
            true;

        AddLayoutElement(
            timerBox.gameObject,
            100f
        );


        TMP_Text timerText =
            CreateText(
                "TimerText",
                timerBox.transform,
                "00:00",
                38f,
                Brown,
                TextAlignmentOptions.Center
            );

        StretchFullWithPadding(
            timerText.rectTransform,
            15f
        );


        // =========================
        // Skill Panel
        // =========================

        Image skillPanel =
            CreateImage(
                "SkillPanel",
                statusPanel.transform,
                Cream
            );

        SetStretch(
            skillPanel.rectTransform,
            60f,
            60f,
            225f,
            150f
        );


        TMP_Text skillTitle =
            CreateText(
                "SkillTitleText",
                skillPanel.transform,
                "SKILLS",
                36f,
                Brown,
                TextAlignmentOptions.Center
            );

        SetTopCenter(
            skillTitle.rectTransform,
            500f,
            55f,
            0f,
            20f
        );


        // =========================
        // Skill HUD
        // =========================

        RectTransform skillHUD =
            CreateRect(
                "SkillHUD",
                skillPanel.transform
            );

        SetStretch(
            skillHUD,
            50f,
            50f,
            90f,
            35f
        );


        VerticalLayoutGroup skillLayout =
            skillHUD.gameObject.AddComponent
                <VerticalLayoutGroup>();

        skillLayout.padding =
            new RectOffset(
                20,
                20,
                15,
                15
            );

        skillLayout.spacing = 10f;

        skillLayout.childAlignment =
            TextAnchor.MiddleLeft;

        skillLayout.childControlWidth = true;
        skillLayout.childControlHeight = true;

        skillLayout.childForceExpandWidth = true;
        skillLayout.childForceExpandHeight = false;


        TMP_Text damageText =
            CreateSkillText(
                "DamageText",
                skillHUD,
                "Damage Lv.1"
            );

        TMP_Text attackSpeedText =
            CreateSkillText(
                "AttackSpeedText",
                skillHUD,
                "Attack Speed Lv.1"
            );

        TMP_Text projectileText =
            CreateSkillText(
                "ProjectileText",
                skillHUD,
                "Projectile Lv.1"
            );

        TMP_Text moveSpeedText =
            CreateSkillText(
                "MoveSpeedText",
                skillHUD,
                "Move Speed Lv.1"
            );


        // =========================
        // Bottom Buttons
        // =========================

        RectTransform bottomButtons =
            CreateRect(
                "BottomButtons",
                statusPanel.transform
            );

        SetBottomCenter(
            bottomButtons,
            600f,
            85f,
            0f,
            35f
        );


        HorizontalLayoutGroup bottomLayout =
            bottomButtons.gameObject
                .AddComponent<HorizontalLayoutGroup>();

        bottomLayout.spacing = 30f;

        bottomLayout.childAlignment =
            TextAnchor.MiddleCenter;

        bottomLayout.childControlWidth = true;
        bottomLayout.childControlHeight = true;

        bottomLayout.childForceExpandWidth = true;
        bottomLayout.childForceExpandHeight = true;


        Button menuButton =
            CreateButton(
                "MenuButton",
                bottomButtons,
                "MENU"
            );


        Button closeButton =
            CreateButton(
                "HUDCloseButton",
                bottomButtons,
                "CLOSE"
            );


        // =========================
        // HUD Toggle Button
        // HUD 밖에 있어야 한다.
        // =========================

        Button hudToggleButton =
            FindOrCreateHUDToggleButton(
                canvas.transform
            );


        // =========================
        // GameHUDUI 생성
        // =========================

        GameHUDUI gameHUDUI =
            hud.gameObject.AddComponent<GameHUDUI>();


        SerializedObject hudUIObject =
            new SerializedObject(
                gameHUDUI
            );

        SetObjectReference(
            hudUIObject,
            "levelText",
            levelText
        );

        SetObjectReference(
            hudUIObject,
            "timerText",
            timerText
        );

        SetObjectReference(
            hudUIObject,
            "damageText",
            damageText
        );

        SetObjectReference(
            hudUIObject,
            "attackSpeedText",
            attackSpeedText
        );

        SetObjectReference(
            hudUIObject,
            "projectileText",
            projectileText
        );

        SetObjectReference(
            hudUIObject,
            "moveSpeedText",
            moveSpeedText
        );

        hudUIObject.ApplyModifiedProperties();


        // =========================
        // Presenter 자동 연결
        // =========================

        Transform uiRoot =
            canvas.transform.root;


        GameHUDPresenter hudPresenter =
            uiRoot.GetComponentInChildren
                <GameHUDPresenter>(
                    true
                );

        if (hudPresenter != null)
        {
            SerializedObject presenterObject =
                new SerializedObject(
                    hudPresenter
                );

            SetObjectReference(
                presenterObject,
                "hudUI",
                gameHUDUI
            );

            presenterObject
                .ApplyModifiedProperties();
        }


        InGameMenuPresenter menuPresenter =
            uiRoot.GetComponentInChildren
                <InGameMenuPresenter>(
                    true
                );

        if (menuPresenter != null)
        {
            SerializedObject menuObject =
                new SerializedObject(
                    menuPresenter
                );

            SetObjectReference(
                menuObject,
                "hudToggleButton",
                hudToggleButton
            );

            SetObjectReference(
                menuObject,
                "hudRoot",
                hud.gameObject
            );

            SetObjectReference(
                menuObject,
                "hudCloseButton",
                closeButton
            );

            SetObjectReference(
                menuObject,
                "menuButton",
                menuButton
            );

            menuObject.ApplyModifiedProperties();
        }


        // 새 HUD 선택
        Selection.activeGameObject =
            hud.gameObject;


        EditorUtility.SetDirty(
            canvas.gameObject
        );


        Debug.Log(
            "[GameHUDBuilder] HUD 자동 생성 완료!"
        );
    }


    // ========================================================
    // HUD Toggle
    // ========================================================

    private static Button FindOrCreateHUDToggleButton(
        Transform canvas)
    {
        Transform existing =
            canvas.Find(
                "HUDToggleButton"
            );

        if (existing != null)
        {
            Button existingButton =
                existing.GetComponent<Button>();

            if (existingButton != null)
            {
                SetBottomRight(
                    existingButton
                        .GetComponent<RectTransform>(),
                    200f,
                    70f,
                    40f,
                    40f
                );

                return existingButton;
            }
        }


        Button button =
            CreateButton(
                "HUDToggleButton",
                canvas,
                "HUD"
            );


        SetBottomRight(
            button.GetComponent<RectTransform>(),
            200f,
            70f,
            40f,
            40f
        );


        return button;
    }


    // ========================================================
    // UI Creation
    // ========================================================

    private static RectTransform CreateRect(
        string name,
        Transform parent)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        Undo.RegisterCreatedObjectUndo(
            obj,
            $"Create {name}"
        );

        obj.transform.SetParent(
            parent,
            false
        );

        return obj.GetComponent<RectTransform>();
    }


    private static Image CreateImage(
        string name,
        Transform parent,
        Color color)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

        Undo.RegisterCreatedObjectUndo(
            obj,
            $"Create {name}"
        );

        obj.transform.SetParent(
            parent,
            false
        );


        Image image =
            obj.GetComponent<Image>();

        image.color = color;


        Sprite sprite =
            AssetDatabase
                .GetBuiltinExtraResource<Sprite>(
                    "UI/Skin/UISprite.psd"
                );

        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }


        return image;
    }


    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

        Undo.RegisterCreatedObjectUndo(
            obj,
            $"Create {name}"
        );

        obj.transform.SetParent(
            parent,
            false
        );


        TextMeshProUGUI text =
            obj.GetComponent<TextMeshProUGUI>();

        text.text = value;

        text.fontSize =
            fontSize;

        text.color =
            color;

        text.alignment =
            alignment;

        text.enableAutoSizing =
            true;

        text.fontSizeMin =
            20f;

        text.fontSizeMax =
            fontSize;


        return text;
    }


    private static TMP_Text CreateSkillText(
        string name,
        Transform parent,
        string value)
    {
        TMP_Text text =
            CreateText(
                name,
                parent,
                value,
                32f,
                Brown,
                TextAlignmentOptions.MidlineLeft
            );


        LayoutElement layout =
            text.gameObject
                .AddComponent<LayoutElement>();

        layout.preferredHeight =
            55f;


        return text;
    }


    private static Button CreateButton(
        string name,
        Transform parent,
        string label)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button)
            );

        Undo.RegisterCreatedObjectUndo(
            obj,
            $"Create {name}"
        );

        obj.transform.SetParent(
            parent,
            false
        );


        Image image =
            obj.GetComponent<Image>();

        image.color =
            DarkOrange;


        Sprite sprite =
            AssetDatabase
                .GetBuiltinExtraResource<Sprite>(
                    "UI/Skin/UISprite.psd"
                );

        if (sprite != null)
        {
            image.sprite = sprite;
            image.type = Image.Type.Sliced;
        }


        Button button =
            obj.GetComponent<Button>();

        button.targetGraphic =
            image;


        TMP_Text text =
            CreateText(
                "Text",
                obj.transform,
                label,
                32f,
                Color.white,
                TextAlignmentOptions.Center
            );

        StretchFullWithPadding(
            text.rectTransform,
            10f
        );


        return button;
    }


    // ========================================================
    // Layout
    // ========================================================

    private static void StretchFull(
        RectTransform rect)
    {
        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            Vector2.zero;

        rect.offsetMax =
            Vector2.zero;
    }


    private static void StretchFullWithPadding(
        RectTransform rect,
        float padding)
    {
        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            new Vector2(
                padding,
                padding
            );

        rect.offsetMax =
            new Vector2(
                -padding,
                -padding
            );
    }


    private static void SetCenter(
        RectTransform rect,
        float width,
        float height,
        float x,
        float y)
    {
        rect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.sizeDelta =
            new Vector2(
                width,
                height
            );

        rect.anchoredPosition =
            new Vector2(
                x,
                y
            );
    }


    private static void SetTopCenter(
        RectTransform rect,
        float width,
        float height,
        float x,
        float top)
    {
        rect.anchorMin =
            new Vector2(
                0.5f,
                1f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                1f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                1f
            );

        rect.sizeDelta =
            new Vector2(
                width,
                height
            );

        rect.anchoredPosition =
            new Vector2(
                x,
                -top
            );
    }


    private static void SetTopStretch(
        RectTransform rect,
        float left,
        float right,
        float top,
        float height)
    {
        rect.anchorMin =
            new Vector2(
                0f,
                1f
            );

        rect.anchorMax =
            new Vector2(
                1f,
                1f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                1f
            );

        rect.offsetMin =
            new Vector2(
                left,
                -top - height
            );

        rect.offsetMax =
            new Vector2(
                -right,
                -top
            );
    }


    private static void SetStretch(
        RectTransform rect,
        float left,
        float right,
        float top,
        float bottom)
    {
        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            new Vector2(
                left,
                bottom
            );

        rect.offsetMax =
            new Vector2(
                -right,
                -top
            );
    }


    private static void SetBottomCenter(
        RectTransform rect,
        float width,
        float height,
        float x,
        float bottom)
    {
        rect.anchorMin =
            new Vector2(
                0.5f,
                0f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0f
            );

        rect.sizeDelta =
            new Vector2(
                width,
                height
            );

        rect.anchoredPosition =
            new Vector2(
                x,
                bottom
            );
    }


    private static void SetBottomRight(
        RectTransform rect,
        float width,
        float height,
        float right,
        float bottom)
    {
        rect.anchorMin =
            new Vector2(
                1f,
                0f
            );

        rect.anchorMax =
            new Vector2(
                1f,
                0f
            );

        rect.pivot =
            new Vector2(
                1f,
                0f
            );

        rect.sizeDelta =
            new Vector2(
                width,
                height
            );

        rect.anchoredPosition =
            new Vector2(
                -right,
                bottom
            );
    }


    private static void AddLayoutElement(
        GameObject obj,
        float preferredHeight)
    {
        LayoutElement layout =
            obj.AddComponent<LayoutElement>();

        layout.preferredHeight =
            preferredHeight;
    }


    // ========================================================
    // Serialized Reference
    // ========================================================

    private static void SetObjectReference(
        SerializedObject serializedObject,
        string propertyName,
        Object target)
    {
        SerializedProperty property =
            serializedObject.FindProperty(
                propertyName
            );

        if (property == null)
        {
            Debug.LogWarning(
                $"[GameHUDBuilder] " +
                $"{serializedObject.targetObject.name}에서 " +
                $"{propertyName} 필드를 찾지 못했습니다."
            );

            return;
        }

        property.objectReferenceValue =
            target;
    }
}