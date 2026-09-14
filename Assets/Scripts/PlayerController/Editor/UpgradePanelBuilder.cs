using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 강화 선택 UI 자동 생성 도구.
///
/// 사용 방법:
/// Canvas_Game 선택
/// → Tools
/// → UI
/// → Create Upgrade Panel
///
/// 기존 UpgradePanel은 Backup으로 변경하고,
/// UpgradeManager의 upgradePanel / content를
/// 새 UI로 자동 연결한다.
/// </summary>
public static class UpgradePanelBuilder
{
    // =========================
    // Color Theme
    // =========================

    private static readonly Color Orange =
        new Color32(255, 154, 0, 255);

    private static readonly Color Yellow =
        new Color32(255, 211, 25, 255);

    private static readonly Color Cream =
        new Color32(255, 244, 205, 255);

    private static readonly Color Brown =
        new Color32(105, 55, 10, 255);

    private static readonly Color Dim =
        new Color(0f, 0f, 0f, 0.55f);


    [MenuItem("Tools/UI/Create Upgrade Panel")]
    public static void CreateUpgradePanel()
    {
        GameObject selected =
            Selection.activeGameObject;

        if (selected == null)
        {
            ShowError(
                "Canvas_Game을 먼저 선택해주세요."
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
            ShowError(
                "Canvas를 찾을 수 없습니다."
            );

            return;
        }


        // =========================
        // 기존 UpgradePanel 백업
        // =========================

        Transform oldPanel =
            canvas.transform.Find(
                "UpgradePanel"
            );

        if (oldPanel != null)
        {
            Undo.RecordObject(
                oldPanel.gameObject,
                "Backup Upgrade Panel"
            );

            oldPanel.name =
                "UpgradePanel_Backup";

            oldPanel.gameObject
                .SetActive(false);
        }


        // =========================
        // UpgradePanel Root
        // =========================

        RectTransform panelRoot =
            CreateRect(
                "UpgradePanel",
                canvas.transform
            );

        StretchFull(
            panelRoot
        );


        // =========================
        // 어두운 배경
        // =========================

        Image blockBackground =
            CreateImage(
                "BlockBackground",
                panelRoot,
                Dim
            );

        StretchFull(
            blockBackground.rectTransform
        );


        // =========================
        // Upgrade Frame
        // =========================

        Image upgradeFrame =
            CreateImage(
                "UpgradeFrame",
                panelRoot,
                Orange
            );

        SetCenter(
            upgradeFrame.rectTransform,
            1500f,
            800f,
            0f,
            0f
        );


        // =========================
        // 내부 패널
        // =========================

        Image innerPanel =
            CreateImage(
                "InnerPanel",
                upgradeFrame.transform,
                Cream
            );

        StretchWithPadding(
            innerPanel.rectTransform,
            55f,
            55f,
            145f,
            55f
        );


        // =========================
        // Title
        // =========================

        Image titleBar =
            CreateImage(
                "TitleBar",
                upgradeFrame.transform,
                Yellow
            );

        SetTopCenter(
            titleBar.rectTransform,
            700f,
            120f,
            0f,
            25f
        );


        TMP_Text titleText =
            CreateText(
                "TitleText",
                titleBar.transform,
                "SELECT UPGRADE",
                46f,
                Brown,
                TextAlignmentOptions.Center
            );

        StretchFullWithPadding(
            titleText.rectTransform,
            15f
        );


        // =========================
        // Content
        // =========================

        RectTransform content =
            CreateRect(
                "Content",
                innerPanel.transform
            );

        StretchWithPadding(
            content,
            45f,
            45f,
            50f,
            45f
        );


        // 강화 카드 3개 자동 가로 정렬
        HorizontalLayoutGroup layout =
            content.gameObject
                .AddComponent<HorizontalLayoutGroup>();

        layout.padding =
            new RectOffset(
                20,
                20,
                20,
                20
            );

        layout.spacing =
            35f;

        layout.childAlignment =
            TextAnchor.MiddleCenter;

        layout.childControlWidth =
            true;

        layout.childControlHeight =
            true;

        layout.childForceExpandWidth =
            true;

        layout.childForceExpandHeight =
            true;


        // =========================
        // UpgradeManager 자동 연결
        // =========================

        Transform root =
            canvas.transform.root;

        UpgradeManager_PlayerController upgradeManager =
            root.GetComponentInChildren
                <UpgradeManager_PlayerController>(
                    true
                );

        if (upgradeManager != null)
        {
            SerializedObject upgradeObject =
                new SerializedObject(
                    upgradeManager
                );


            SetObjectReference(
                upgradeObject,
                "upgradePanel",
                panelRoot.gameObject
            );

            SetObjectReference(
                upgradeObject,
                "content",
                content
            );


            // 기존 UpgradeButton Prefab은 유지
            SerializedProperty buttonPrefabProperty =
                upgradeObject.FindProperty(
                    "upgradeButtonPrefab"
                );

            if (buttonPrefabProperty != null &&
                buttonPrefabProperty.objectReferenceValue == null)
            {
                Debug.LogWarning(
                    "[UpgradePanelBuilder] " +
                    "UpgradeManager의 UpgradeButtonPrefab이 비어 있습니다. " +
                    "기존 UpgradeButton Prefab을 연결해주세요."
                );
            }


            upgradeObject
                .ApplyModifiedProperties();

            EditorUtility.SetDirty(
                upgradeManager
            );
        }
        else
        {
            Debug.LogWarning(
                "[UpgradePanelBuilder] " +
                "UpgradeManager_PlayerController를 찾지 못했습니다."
            );
        }


        // 게임 시작 시 닫힌 상태
        panelRoot.gameObject
            .SetActive(false);


        Selection.activeGameObject =
            panelRoot.gameObject;


        EditorUtility.SetDirty(
            canvas.gameObject
        );


        Debug.Log(
            "[UpgradePanelBuilder] " +
            "UpgradePanel 생성 및 UpgradeManager 연결 완료!"
        );
    }


    // ========================================================
    // Creation
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
            image.sprite =
                sprite;

            image.type =
                Image.Type.Sliced;
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

        text.text =
            value;

        text.fontSize =
            fontSize;

        text.color =
            color;

        text.alignment =
            alignment;

        text.enableAutoSizing =
            true;

        text.fontSizeMin =
            22f;

        text.fontSizeMax =
            fontSize;


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


    private static void StretchWithPadding(
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
                $"[UpgradePanelBuilder] " +
                $"{propertyName} 필드를 찾지 못했습니다."
            );

            return;
        }

        property.objectReferenceValue =
            target;
    }


    private static void ShowError(
        string message)
    {
        EditorUtility.DisplayDialog(
            "Upgrade Panel Builder",
            message,
            "확인"
        );
    }
}