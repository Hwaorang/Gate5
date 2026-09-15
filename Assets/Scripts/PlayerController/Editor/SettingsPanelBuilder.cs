using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class SettingsPanelBuilder
{
    private static readonly Color FrameColor =
        new Color32(245, 165, 35, 255);

    private static readonly Color InnerColor =
        new Color32(255, 239, 190, 255);

    private static readonly Color TitleColor =
        new Color32(255, 210, 30, 255);

    private static readonly Color TextColor =
        new Color32(90, 50, 15, 255);

    private static readonly Color DimColor =
        new Color(0f, 0f, 0f, 0.6f);


    [MenuItem("Tools/UI/Create Settings Panel")]
    public static void CreateSettingsPanel()
    {
        GameObject selected =
            Selection.activeGameObject;


        if (selected == null)
        {
            ShowError(
                "Canvas를 먼저 선택해주세요."
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
        // 기존 SettingsPanel 백업
        // =========================

        Transform oldPanel =
            canvas.transform.Find(
                "SettingsPanel"
            );


        if (oldPanel != null)
        {
            oldPanel.name =
                "SettingsPanel_Backup";

            oldPanel.gameObject
                .SetActive(false);
        }


        // =========================
        // Root
        // =========================

        RectTransform root =
            CreateRect(
                "SettingsPanel",
                canvas.transform
            );

        StretchFull(
            root
        );


        SettingsPanelUI panelUI =
            root.gameObject
                .AddComponent<SettingsPanelUI>();


        AudioSettingsUI audioUI =
            root.gameObject
                .AddComponent<AudioSettingsUI>();


        // =========================
        // Background
        // =========================

        Image block =
            CreateImage(
                "BlockBackground",
                root,
                DimColor
            );

        StretchFull(
            block.rectTransform
        );


        // =========================
        // Frame
        // =========================

        Image frame =
            CreateImage(
                "SettingsFrame",
                root,
                FrameColor
            );

        SetCenter(
            frame.rectTransform,
            900f,
            720f,
            0f,
            0f
        );


        Image inner =
            CreateImage(
                "InnerPanel",
                frame.transform,
                InnerColor
            );

        StretchPadding(
            inner.rectTransform,
            45f,
            45f,
            140f,
            100f
        );


        // =========================
        // Title
        // =========================

        Image titleBar =
            CreateImage(
                "TitleBar",
                frame.transform,
                TitleColor
            );

        SetTopCenter(
            titleBar.rectTransform,
            520f,
            110f,
            0f,
            25f
        );


        TMP_Text titleText =
            CreateText(
                "TitleText",
                titleBar.transform,
                "SETTINGS",
                44f
            );

        StretchFull(
            titleText.rectTransform
        );


        // =========================
        // Volume Rows
        // =========================

        VolumeRow master =
            CreateVolumeRow(
                "MasterRow",
                inner.transform,
                "MASTER",
                150f
            );


        VolumeRow bgm =
            CreateVolumeRow(
                "BgmRow",
                inner.transform,
                "BGM",
                0f
            );


        VolumeRow sfx =
            CreateVolumeRow(
                "SfxRow",
                inner.transform,
                "SFX",
                -150f
            );


        // =========================
        // Close Button
        // =========================

        Button closeButton =
            CreateButton(
                "CloseButton",
                frame.transform,
                "CLOSE"
            );


        RectTransform closeRect =
            closeButton.GetComponent<RectTransform>();


        closeRect.anchorMin =
            new Vector2(
                0.5f,
                0f
            );

        closeRect.anchorMax =
            new Vector2(
                0.5f,
                0f
            );

        closeRect.pivot =
            new Vector2(
                0.5f,
                0f
            );

        closeRect.sizeDelta =
            new Vector2(
                300f,
                85f
            );

        closeRect.anchoredPosition =
            new Vector2(
                0f,
                25f
            );


        // 클릭음도 자동 추가
        if (closeButton.GetComponent<UIButtonSound>() == null)
        {
            closeButton.gameObject
                .AddComponent<UIButtonSound>();
        }


        // =========================
        // SettingsPanelUI 연결
        // =========================

        SerializedObject panelSerialized =
            new SerializedObject(
                panelUI
            );


        SetReference(
            panelSerialized,
            "closeButton",
            closeButton
        );


        panelSerialized
            .ApplyModifiedProperties();


        // =========================
        // AudioSettingsUI 연결
        // =========================

        SerializedObject audioSerialized =
            new SerializedObject(
                audioUI
            );


        SetReference(
            audioSerialized,
            "masterSlider",
            master.slider
        );

        SetReference(
            audioSerialized,
            "masterValueText",
            master.valueText
        );


        SetReference(
            audioSerialized,
            "bgmSlider",
            bgm.slider
        );

        SetReference(
            audioSerialized,
            "bgmValueText",
            bgm.valueText
        );


        SetReference(
            audioSerialized,
            "sfxSlider",
            sfx.slider
        );

        SetReference(
            audioSerialized,
            "sfxValueText",
            sfx.valueText
        );


        audioSerialized
            .ApplyModifiedProperties();


        // 처음에는 닫힌 상태
        root.gameObject
            .SetActive(false);


        Selection.activeGameObject =
            root.gameObject;


        Debug.Log(
            "[SettingsPanelBuilder] " +
            "SettingsPanel 생성 완료!"
        );
    }


    // ========================================================
    // Volume Row
    // ========================================================

    private class VolumeRow
    {
        public Slider slider;
        public TMP_Text valueText;
    }


    private static VolumeRow CreateVolumeRow(
        string name,
        Transform parent,
        string label,
        float y)
    {
        RectTransform row =
            CreateRect(
                name,
                parent
            );


        row.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        row.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        row.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        row.sizeDelta =
            new Vector2(
                720f,
                100f
            );

        row.anchoredPosition =
            new Vector2(
                0f,
                y
            );


        TMP_Text labelText =
            CreateText(
                "Label",
                row,
                label,
                30f
            );


        SetRect(
            labelText.rectTransform,
            170f,
            70f,
            -260f,
            0f
        );


        Slider slider =
            CreateSlider(
                "Slider",
                row
            );


        SetRect(
            slider.GetComponent<RectTransform>(),
            360f,
            50f,
            20f,
            0f
        );


        TMP_Text valueText =
            CreateText(
                "ValueText",
                row,
                "100%",
                28f
            );


        SetRect(
            valueText.rectTransform,
            100f,
            60f,
            260f,
            0f
        );


        VolumeRow result =
            new VolumeRow();

        result.slider =
            slider;

        result.valueText =
            valueText;


        return result;
    }


    // ========================================================
    // Slider
    // ========================================================

    private static Slider CreateSlider(
        string name,
        Transform parent)
    {
        GameObject sliderObject =
            DefaultControls.CreateSlider(
                new DefaultControls.Resources()
            );


        sliderObject.name =
            name;


        Undo.RegisterCreatedObjectUndo(
            sliderObject,
            "Create Slider"
        );


        sliderObject.transform.SetParent(
            parent,
            false
        );


        Slider slider =
            sliderObject.GetComponent<Slider>();


        slider.minValue =
            0f;

        slider.maxValue =
            1f;

        slider.value =
            1f;

        slider.wholeNumbers =
            false;


        return slider;
    }


    // ========================================================
    // Button
    // ========================================================

    private static Button CreateButton(
        string name,
        Transform parent,
        string text)
    {
        GameObject obj =
            DefaultControls.CreateButton(
                new DefaultControls.Resources()
            );


        obj.name =
            name;


        Undo.RegisterCreatedObjectUndo(
            obj,
            $"Create {name}"
        );


        obj.transform.SetParent(
            parent,
            false
        );


        TMP_Text oldText =
            obj.GetComponentInChildren<TMP_Text>();


        if (oldText != null)
        {
            oldText.text =
                text;
        }
        else
        {
            Text legacyText =
                obj.GetComponentInChildren<Text>();


            if (legacyText != null)
            {
                legacyText.text =
                    text;
            }
        }


        return obj.GetComponent<Button>();
    }


    // ========================================================
    // Creation Helpers
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
        string text,
        float fontSize)
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


        TextMeshProUGUI tmp =
            obj.GetComponent<TextMeshProUGUI>();


        tmp.text =
            text;

        tmp.fontSize =
            fontSize;

        tmp.color =
            TextColor;

        tmp.alignment =
            TextAlignmentOptions.Center;

        tmp.enableAutoSizing =
            true;

        tmp.fontSizeMin =
            18f;

        tmp.fontSizeMax =
            fontSize;


        return tmp;
    }


    // ========================================================
    // Rect Helpers
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


    private static void StretchPadding(
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


    private static void SetRect(
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


    // ========================================================
    // Serialized
    // ========================================================

    private static void SetReference(
        SerializedObject serialized,
        string propertyName,
        Object value)
    {
        SerializedProperty property =
            serialized.FindProperty(
                propertyName
            );


        if (property == null)
        {
            Debug.LogWarning(
                $"[SettingsPanelBuilder] " +
                $"{propertyName}을 찾지 못했습니다."
            );

            return;
        }


        property.objectReferenceValue =
            value;
    }


    private static void ShowError(
        string message)
    {
        EditorUtility.DisplayDialog(
            "Settings Panel Builder",
            message,
            "확인"
        );
    }
}