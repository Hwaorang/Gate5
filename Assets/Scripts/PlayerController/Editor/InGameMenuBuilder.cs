using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인게임 메뉴 UI 자동 생성 도구.
///
/// 사용:
/// Canvas_Game 선택
/// → Tools
/// → UI
/// → Create InGame Menu
/// </summary>
public static class InGameMenuBuilder
{
    private static readonly Color Orange =
        new Color32(244, 158, 35, 255);

    private static readonly Color DarkOrange =
        new Color32(215, 112, 12, 255);

    private static readonly Color Cream =
        new Color32(255, 244, 214, 255);

    private static readonly Color Brown =
        new Color32(100, 55, 10, 255);


    [MenuItem("Tools/UI/Create InGame Menu")]
    public static void CreateInGameMenu()
    {
        GameObject selected =
            Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "InGame Menu Builder",
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
                "InGame Menu Builder",
                "Canvas를 찾을 수 없습니다.",
                "확인"
            );

            return;
        }


        // =========================
        // 기존 Menu 백업
        // =========================

        Transform oldMenu =
            canvas.transform.Find(
                "InGameMenuPanel"
            );

        if (oldMenu != null)
        {
            Undo.RecordObject(
                oldMenu.gameObject,
                "Backup Old Menu"
            );

            oldMenu.name =
                "InGameMenuPanel_Backup";

            oldMenu.gameObject.SetActive(false);
        }


        // =========================
        // Menu Root
        // =========================

        RectTransform menuRoot =
            CreateRect(
                "InGameMenuPanel",
                canvas.transform
            );

        StretchFull(menuRoot);


        // 투명 배경
        Image blockBackground =
            CreateImage(
                "BlockBackground",
                menuRoot,
                new Color(
                    0f,
                    0f,
                    0f,
                    0.25f
                )
            );

        StretchFull(
            blockBackground.rectTransform
        );


        // =========================
        // Menu Panel
        // =========================

        Image panel =
            CreateImage(
                "MenuFrame",
                menuRoot,
                Cream
            );

        SetCenter(
            panel.rectTransform,
            560f,
            720f,
            0f,
            0f
        );


        // =========================
        // Title Bar
        // =========================

        Image titleBar =
            CreateImage(
                "TitleBar",
                panel.transform,
                Orange
            );

        SetTopCenter(
            titleBar.rectTransform,
            420f,
            100f,
            0f,
            -25f
        );


        TMP_Text titleText =
            CreateText(
                "TitleText",
                titleBar.transform,
                "MENU",
                46f,
                Color.white,
                TextAlignmentOptions.Center
            );

        StretchFullWithPadding(
            titleText.rectTransform,
            10f
        );


        // =========================
        // Button List
        // =========================

        RectTransform buttonList =
            CreateRect(
                "ButtonList",
                panel.transform
            );

        SetStretch(
            buttonList,
            80f,
            80f,
            155f,
            70f
        );


        VerticalLayoutGroup layout =
            buttonList.gameObject
                .AddComponent<VerticalLayoutGroup>();

        layout.spacing = 22f;

        layout.padding =
            new RectOffset(
                15,
                15,
                15,
                15
            );

        layout.childAlignment =
            TextAnchor.MiddleCenter;

        layout.childControlWidth = true;
        layout.childControlHeight = true;

        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;


        // =========================
        // Buttons
        // =========================

        Button continueButton =
            CreateMenuButton(
                "ContinueButton",
                buttonList,
                "CONTINUE"
            );

        Button settingsButton =
            CreateMenuButton(
                "SettingsButton",
                buttonList,
                "SETTINGS"
            );

        Button retryButton =
            CreateMenuButton(
                "RetryButton",
                buttonList,
                "RETRY"
            );

        Button lobbyButton =
            CreateMenuButton(
                "LobbyButton",
                buttonList,
                "LOBBY"
            );


        // =========================
        // InGameMenuUI
        // =========================

        InGameMenuUI menuUI =
            menuRoot.gameObject
                .AddComponent<InGameMenuUI>();


        SerializedObject menuUIObject =
            new SerializedObject(
                menuUI
            );

        SetObjectReference(
            menuUIObject,
            "continueButton",
            continueButton
        );

        SetObjectReference(
            menuUIObject,
            "settingsButton",
            settingsButton
        );

        SetObjectReference(
            menuUIObject,
            "retryButton",
            retryButton
        );

        SetObjectReference(
            menuUIObject,
            "lobbyButton",
            lobbyButton
        );

        menuUIObject.ApplyModifiedProperties();


        // =========================
        // Presenter 자동 연결
        // =========================

        Transform root =
            canvas.transform.root;


        InGameMenuPresenter presenter =
            root.GetComponentInChildren
                <InGameMenuPresenter>(
                    true
                );


        if (presenter != null)
        {
            SerializedObject presenterObject =
                new SerializedObject(
                    presenter
                );

            SetObjectReference(
                presenterObject,
                "menuUI",
                menuUI
            );

            presenterObject
                .ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning(
                "[InGameMenuBuilder] " +
                "InGameMenuPresenter를 찾지 못했습니다."
            );
        }


        // 처음에는 숨긴다.
        menuRoot.gameObject.SetActive(false);


        Selection.activeGameObject =
            menuRoot.gameObject;


        EditorUtility.SetDirty(
            canvas.gameObject
        );


        Debug.Log(
            "[InGameMenuBuilder] " +
            "인게임 메뉴 생성 및 연결 완료!"
        );
    }


    // ========================================================
    // Button
    // ========================================================

    private static Button CreateMenuButton(
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
                typeof(Button),
                typeof(LayoutElement)
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


        LayoutElement layout =
            obj.GetComponent<LayoutElement>();

        layout.preferredHeight =
            90f;


        TMP_Text text =
            CreateText(
                "Text",
                obj.transform,
                label,
                34f,
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
    // Create Helpers
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

        image.color =
            color;


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
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;

        text.enableAutoSizing = true;
        text.fontSizeMin = 20f;
        text.fontSizeMax = fontSize;


        return text;
    }


    // ========================================================
    // RectTransform
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
        float y)
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
                y
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


    // ========================================================
    // Serialized
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
                $"[InGameMenuBuilder] " +
                $"{propertyName} 필드를 찾지 못했습니다."
            );

            return;
        }

        property.objectReferenceValue =
            target;
    }
}