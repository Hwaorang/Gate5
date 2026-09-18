#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 선택한 GameOverPanel을 코드로 재구성하는 Editor Tool.
///
/// 사용:
/// 1. Hierarchy에서 GameOverPanel 선택
/// 2. Tools > GameOver > Build Selected GameOver UI
/// </summary>
public static class GameOverPanelBuilder
{
    private static readonly Color OverlayColor =
        new Color(0.01f, 0.015f, 0.025f, 0.82f);

    private static readonly Color CardColor =
        new Color(0.055f, 0.065f, 0.085f, 0.98f);

    private static readonly Color StatColor =
        new Color(1f, 1f, 1f, 0.055f);

    private static readonly Color AccentColor =
        new Color(1f, 0.23f, 0.28f, 1f);

    private static readonly Color RetryColor =
        new Color(1f, 0.69f, 0.18f, 1f);

    private static readonly Color LobbyColor =
        new Color(0.18f, 0.21f, 0.27f, 1f);

    private static readonly Color WhiteColor =
        new Color(0.96f, 0.97f, 1f, 1f);

    private static readonly Color MutedColor =
        new Color(0.62f, 0.66f, 0.74f, 1f);


    [MenuItem(
        "Tools/GameOver/Build Selected GameOver UI"
    )]
    public static void BuildSelected()
    {
        GameObject root =
            Selection.activeGameObject;

        if (root == null)
        {
            Debug.LogError(
                "[GameOverPanelBuilder] " +
                "Hierarchy에서 GameOverPanel을 선택해주세요."
            );

            return;
        }

        GameResultUI resultUI =
            root.GetComponent<GameResultUI>();

        if (resultUI == null)
        {
            Debug.LogError(
                "[GameOverPanelBuilder] " +
                "선택한 오브젝트에 GameResultUI가 없습니다."
            );

            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(
            root,
            "Build GameOver UI"
        );

        ClearChildren(
            root.transform
        );

        RectTransform rootRect =
            GetOrAddRectTransform(
                root
            );

        StretchFull(
            rootRect
        );

        Image overlay =
            GetOrAdd<Image>(
                root
            );

        overlay.sprite = null;
        overlay.color = OverlayColor;
        overlay.raycastTarget = true;

        CanvasGroup canvasGroup =
            GetOrAdd<CanvasGroup>(
                root
            );

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;


        // =====================================================
        // 중앙 카드
        // =====================================================

        GameObject card =
            CreateUIObject(
                "Content",
                root.transform
            );

        RectTransform cardRect =
            card.GetComponent<RectTransform>();

        SetCenteredRect(
            cardRect,
            new Vector2(
                590f,
                520f
            ),
            Vector2.zero
        );

        Image cardImage =
            card.AddComponent<Image>();

        cardImage.color =
            CardColor;

        Shadow cardShadow =
            card.AddComponent<Shadow>();

        cardShadow.effectColor =
            new Color(
                0f,
                0f,
                0f,
                0.55f
            );

        cardShadow.effectDistance =
            new Vector2(
                0f,
                -12f
            );


        // =====================================================
        // 상단 Accent
        // =====================================================

        CreateSolidRect(
            "Accent",
            card.transform,
            AccentColor,
            new Vector2(
                590f,
                6f
            ),
            new Vector2(
                0f,
                257f
            )
        );


        // =====================================================
        // Title
        // =====================================================

        TMP_Text title =
            CreateText(
                "TitleText",
                card.transform,
                "GAME OVER",
                56f,
                FontStyles.Bold,
                AccentColor
            );

        SetCenteredRect(
            title.rectTransform,
            new Vector2(
                500f,
                78f
            ),
            new Vector2(
                0f,
                185f
            )
        );

        title.alignment =
            TextAlignmentOptions.Center;

        Outline titleOutline =
            title.gameObject.AddComponent<Outline>();

        titleOutline.effectColor =
            new Color(
                0f,
                0f,
                0f,
                0.7f
            );

        titleOutline.effectDistance =
            new Vector2(
                2f,
                -2f
            );


        TMP_Text subtitle =
            CreateText(
                "SubtitleText",
                card.transform,
                "RUN ENDED",
                18f,
                FontStyles.Normal,
                MutedColor
            );

        SetCenteredRect(
            subtitle.rectTransform,
            new Vector2(
                400f,
                32f
            ),
            new Vector2(
                0f,
                137f
            )
        );

        subtitle.alignment =
            TextAlignmentOptions.Center;

        subtitle.characterSpacing =
            8f;


        // =====================================================
        // Stats
        // =====================================================

        TMP_Text survivalValue =
            CreateStatRow(
                card.transform,
                "SurvivalStat",
                "SURVIVAL TIME",
                "00:00",
                55f
            );

        TMP_Text levelValue =
            CreateStatRow(
                card.transform,
                "LevelStat",
                "LEVEL",
                "Lv. 1",
                -40f
            );


        // =====================================================
        // Buttons
        // =====================================================

        Button retryButton =
            CreateButton(
                card.transform,
                "RetryButton",
                "RETRY",
                RetryColor,
                new Vector2(
                    -126f,
                    -185f
                )
            );

        Button lobbyButton =
            CreateButton(
                card.transform,
                "LobbyButton",
                "LOBBY",
                LobbyColor,
                new Vector2(
                    126f,
                    -185f
                )
            );


        TMP_Text hint =
            CreateText(
                "HintText",
                card.transform,
                "Retry to jump back in, or return to the lobby.",
                14f,
                FontStyles.Normal,
                MutedColor
            );

        SetCenteredRect(
            hint.rectTransform,
            new Vector2(
                500f,
                32f
            ),
            new Vector2(
                0f,
                -235f
            )
        );

        hint.alignment =
            TextAlignmentOptions.Center;


        // =====================================================
        // GameResultUI 자동 연결
        // =====================================================

        SerializedObject serialized =
            new SerializedObject(
                resultUI
            );

        serialized.FindProperty(
            "survivalTimeText"
        ).objectReferenceValue =
            survivalValue;

        serialized.FindProperty(
            "levelText"
        ).objectReferenceValue =
            levelValue;

        serialized.FindProperty(
            "retryButton"
        ).objectReferenceValue =
            retryButton;

        serialized.FindProperty(
            "lobbyButton"
        ).objectReferenceValue =
            lobbyButton;

        serialized.FindProperty(
            "canvasGroup"
        ).objectReferenceValue =
            canvasGroup;

        serialized.FindProperty(
            "contentRoot"
        ).objectReferenceValue =
            cardRect;

        serialized.ApplyModifiedProperties();

        EditorUtility.SetDirty(
            resultUI
        );

        EditorUtility.SetDirty(
            root
        );

        Debug.Log(
            "[GameOverPanelBuilder] " +
            "GameOver UI 생성 및 GameResultUI 연결 완료"
        );
    }


    private static TMP_Text CreateStatRow(
        Transform parent,
        string name,
        string labelText,
        string valueText,
        float y)
    {
        GameObject row =
            CreateUIObject(
                name,
                parent
            );

        RectTransform rowRect =
            row.GetComponent<RectTransform>();

        SetCenteredRect(
            rowRect,
            new Vector2(
                480f,
                76f
            ),
            new Vector2(
                0f,
                y
            )
        );

        Image background =
            row.AddComponent<Image>();

        background.color =
            StatColor;


        TMP_Text label =
            CreateText(
                "Label",
                row.transform,
                labelText,
                16f,
                FontStyles.Normal,
                MutedColor
            );

        SetAnchoredRect(
            label.rectTransform,
            new Vector2(
                0f,
                0.5f
            ),
            new Vector2(
                0f,
                0.5f
            ),
            new Vector2(
                24f,
                0f
            ),
            new Vector2(
                210f,
                50f
            )
        );

        label.alignment =
            TextAlignmentOptions.Left;


        TMP_Text value =
            CreateText(
                "Value",
                row.transform,
                valueText,
                28f,
                FontStyles.Bold,
                WhiteColor
            );

        SetAnchoredRect(
            value.rectTransform,
            new Vector2(
                1f,
                0.5f
            ),
            new Vector2(
                1f,
                0.5f
            ),
            new Vector2(
                -24f,
                0f
            ),
            new Vector2(
                220f,
                50f
            )
        );

        value.alignment =
            TextAlignmentOptions.Right;

        return value;
    }


    private static Button CreateButton(
        Transform parent,
        string name,
        string text,
        Color color,
        Vector2 position)
    {
        GameObject buttonObject =
            CreateUIObject(
                name,
                parent
            );

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        SetCenteredRect(
            rect,
            new Vector2(
                220f,
                60f
            ),
            position
        );

        Image image =
            buttonObject.AddComponent<Image>();

        image.color =
            color;

        Button button =
            buttonObject.AddComponent<Button>();

        ColorBlock colors =
            button.colors;

        colors.normalColor =
            Color.white;

        colors.highlightedColor =
            new Color(
                1f,
                1f,
                1f,
                0.90f
            );

        colors.pressedColor =
            new Color(
                0.82f,
                0.82f,
                0.82f,
                1f
            );

        colors.selectedColor =
            Color.white;

        button.colors =
            colors;

        Shadow shadow =
            buttonObject.AddComponent<Shadow>();

        shadow.effectColor =
            new Color(
                0f,
                0f,
                0f,
                0.35f
            );

        shadow.effectDistance =
            new Vector2(
                0f,
                -4f
            );


        TMP_Text buttonText =
            CreateText(
                "Text",
                buttonObject.transform,
                text,
                20f,
                FontStyles.Bold,
                WhiteColor
            );

        StretchFull(
            buttonText.rectTransform
        );

        buttonText.alignment =
            TextAlignmentOptions.Center;

        buttonText.raycastTarget =
            false;

        return button;
    }


    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        FontStyles fontStyle,
        Color color)
    {
        GameObject obj =
            CreateUIObject(
                name,
                parent
            );

        TextMeshProUGUI text =
            obj.AddComponent<TextMeshProUGUI>();

        text.text =
            value;

        text.fontSize =
            fontSize;

        text.fontStyle =
            fontStyle;

        text.color =
            color;

        text.enableWordWrapping =
            false;

        text.raycastTarget =
            false;

        if (TMP_Settings.defaultFontAsset != null)
        {
            text.font =
                TMP_Settings.defaultFontAsset;
        }

        return text;
    }


    private static void CreateSolidRect(
        string name,
        Transform parent,
        Color color,
        Vector2 size,
        Vector2 position)
    {
        GameObject obj =
            CreateUIObject(
                name,
                parent
            );

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        SetCenteredRect(
            rect,
            size,
            position
        );

        Image image =
            obj.AddComponent<Image>();

        image.color =
            color;

        image.raycastTarget =
            false;
    }


    private static GameObject CreateUIObject(
        string name,
        Transform parent)
    {
        GameObject obj =
            new GameObject(
                name,
                typeof(RectTransform)
            );

        obj.transform.SetParent(
            parent,
            false
        );

        return obj;
    }


    private static void ClearChildren(
        Transform root)
    {
        for (int i =
                 root.childCount - 1;
             i >= 0;
             i--)
        {
            Object.DestroyImmediate(
                root.GetChild(i).gameObject
            );
        }
    }


    private static RectTransform GetOrAddRectTransform(
        GameObject obj)
    {
        RectTransform rect =
            obj.GetComponent<RectTransform>();

        if (rect != null)
        {
            return rect;
        }

        return
            obj.AddComponent<RectTransform>();
    }


    private static T GetOrAdd<T>(
        GameObject obj)
        where T : Component
    {
        T component =
            obj.GetComponent<T>();

        if (component == null)
        {
            component =
                obj.AddComponent<T>();
        }

        return component;
    }


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


    private static void SetCenteredRect(
        RectTransform rect,
        Vector2 size,
        Vector2 position)
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
            size;

        rect.anchoredPosition =
            position;

        rect.localScale =
            Vector3.one;
    }


    private static void SetAnchoredRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 position,
        Vector2 size)
    {
        rect.anchorMin =
            anchorMin;

        rect.anchorMax =
            anchorMax;

        rect.pivot =
            anchorMin;

        rect.sizeDelta =
            size;

        rect.anchoredPosition =
            position;

        rect.localScale =
            Vector3.one;
    }
}

#endif
