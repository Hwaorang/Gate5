#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Lobby 전용 CheatPanel 자동 생성 도구.
///
/// Unity 상단 메뉴:
/// Tools > Cheat > Create Lobby Cheat Panel
///
/// 생성 구조:
/// Canvas
/// ├─ CheatController
/// └─ CheatPanel
///     ├─ Background
///     ├─ TitleText
///     ├─ AddGoldButton
///     ├─ ResetGoldButton
///     └─ ResetUpgradeButton
///
/// 기존 게임 코드는 수정하지 않는다.
/// 버튼 OnClick은 생성 후 LobbyCheatUI 메서드에 연결한다.
/// </summary>
public static class CheatPanelCreator
{
    [MenuItem("Tools/Cheat/Create Lobby Cheat Panel")]
    public static void CreateCheatPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject(
                "Canvas",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler =
                canvasObject.GetComponent<CanvasScaler>();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(1920f, 1080f);

            Undo.RegisterCreatedObjectUndo(
                canvasObject,
                "Create Cheat Canvas"
            );
        }


        // =========================
        // CheatController
        // =========================

        GameObject cheatController =
            new GameObject("CheatController");

        cheatController.transform.SetParent(
            canvas.transform,
            false
        );

        Undo.RegisterCreatedObjectUndo(
            cheatController,
            "Create Cheat Controller"
        );


        // =========================
        // CheatPanel
        // =========================

        GameObject panel =
            new GameObject(
                "CheatPanel",
                typeof(RectTransform),
                typeof(Image)
            );

        panel.transform.SetParent(
            canvas.transform,
            false
        );

        Undo.RegisterCreatedObjectUndo(
            panel,
            "Create Cheat Panel"
        );

        RectTransform panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            new Vector2(0.5f, 0.5f);

        panelRect.anchorMax =
            new Vector2(0.5f, 0.5f);

        panelRect.pivot =
            new Vector2(0.5f, 0.5f);

        panelRect.sizeDelta =
            new Vector2(420f, 420f);

        panelRect.anchoredPosition =
            Vector2.zero;


        Image panelImage =
            panel.GetComponent<Image>();

        panelImage.color =
            new Color(
                0.08f,
                0.08f,
                0.08f,
                0.95f
            );


        // =========================
        // Title
        // =========================

        CreateText(
            panel.transform,
            "TitleText",
            "CHEAT",
            new Vector2(0f, 145f),
            new Vector2(320f, 60f),
            34
        );


        // =========================
        // Buttons
        // =========================

        CreateButton(
            panel.transform,
            "AddGoldButton",
            "Gold +10000",
            new Vector2(0f, 60f)
        );

        CreateButton(
            panel.transform,
            "ResetGoldButton",
            "Gold Reset",
            new Vector2(0f, -20f)
        );

        CreateButton(
            panel.transform,
            "ResetUpgradeButton",
            "Upgrade Reset",
            new Vector2(0f, -100f)
        );


        // =========================
        // Help Text
        // =========================

        CreateText(
            panel.transform,
            "HelpText",
            "F11 : Open / Close",
            new Vector2(0f, -165f),
            new Vector2(320f, 40f),
            18
        );


        // 생성 후 기본적으로 켜 둔다.
        // Inspector 연결이 끝난 뒤 Off 해도 된다.
        panel.SetActive(true);

        Selection.activeGameObject =
            panel;

        Debug.Log(
            "[CheatPanelCreator] CheatPanel 생성 완료.\n" +
            "CheatController에 LobbyCheatUI를 추가하고 " +
            "CheatPanel 및 버튼 OnClick을 연결해주세요."
        );
    }


    private static GameObject CreateButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 position)
    {
        GameObject buttonObject =
            new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button)
            );

        buttonObject.transform.SetParent(
            parent,
            false
        );

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.sizeDelta =
            new Vector2(300f, 60f);

        rect.anchoredPosition =
            position;


        Image image =
            buttonObject.GetComponent<Image>();

        image.color =
            new Color(
                0.22f,
                0.22f,
                0.22f,
                1f
            );


        GameObject textObject =
            new GameObject(
                "Text",
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

        textObject.transform.SetParent(
            buttonObject.transform,
            false
        );

        RectTransform textRect =
            textObject.GetComponent<RectTransform>();

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            Vector2.zero;

        textRect.offsetMax =
            Vector2.zero;


        TextMeshProUGUI text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text =
            label;

        text.fontSize =
            24;

        text.alignment =
            TextAlignmentOptions.Center;

        text.color =
            Color.white;


        return buttonObject;
    }


    private static GameObject CreateText(
        Transform parent,
        string objectName,
        string content,
        Vector2 position,
        Vector2 size,
        float fontSize)
    {
        GameObject textObject =
            new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

        textObject.transform.SetParent(
            parent,
            false
        );

        RectTransform rect =
            textObject.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.sizeDelta =
            size;

        rect.anchoredPosition =
            position;


        TextMeshProUGUI text =
            textObject.GetComponent<TextMeshProUGUI>();

        text.text =
            content;

        text.fontSize =
            fontSize;

        text.alignment =
            TextAlignmentOptions.Center;

        text.color =
            Color.white;


        return textObject;
    }
}
#endif
