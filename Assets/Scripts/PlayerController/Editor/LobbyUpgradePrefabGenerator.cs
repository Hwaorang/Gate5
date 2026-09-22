#if UNITY_EDITOR

using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class LobbyUpgradePrefabGenerator
{
    private const string PrefabFolder = "Assets/Prefabs/UI";
    private const string PrefabPath =
        PrefabFolder + "/UpgradePanel_Styled.prefab";

    [MenuItem("Tools/Gate5/UI/Create Lobby Upgrade Prefab")]
    private static void CreateUpgradePrefab()
    {
        // =========================================
        // 임시 Root 생성
        // =========================================
        GameObject root =
            new GameObject(
                "UpgradePanel",
                typeof(RectTransform),
                typeof(Image)
            );

        RectTransform rootRect =
            root.GetComponent<RectTransform>();

        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;

        rootRect.pivot =
            new Vector2(0.5f, 0.5f);

        rootRect.anchoredPosition =
            Vector2.zero;

        rootRect.sizeDelta =
            Vector2.zero;

        rootRect.offsetMin =
            Vector2.zero;

        rootRect.offsetMax =
            Vector2.zero;

        rootRect.localScale =
            Vector3.one;

        Image background =
            root.GetComponent<Image>();

        background.color =
            new Color32(7, 12, 22, 170);

        // =========================================
        // 중앙 패널
        // =========================================
        GameObject frame =
            CreateUIObject(
                "PanelFrame",
                root.transform
            );

        RectTransform frameRect =
            frame.GetComponent<RectTransform>();

        SetRect(
            frameRect,
            new Vector2(1540f, 610f),
            new Vector2(0f, 0f)
        );

        Image frameImage =
            frame.AddComponent<Image>();

        frameImage.sprite = GetUISprite();
        frameImage.type = Image.Type.Sliced;

        frameImage.color =
            new Color32(23, 31, 47, 250);

        Outline frameOutline =
            frame.AddComponent<Outline>();

        frameOutline.effectColor =
            new Color32(74, 137, 255, 170);

        frameOutline.effectDistance =
            new Vector2(3f, -3f);

        // =========================================
        // Title
        // =========================================
        CreateText(
            frame.transform,
            "TitleText",
            "LOBBY UPGRADE",
            46f,
            FontStyles.Bold,
            Color.white,
            new Vector2(700f, 70f),
            new Vector2(0f, 250f)
        );

        CreateText(
            frame.transform,
            "SubTitleText",
            "PERMANENT SQUAD UPGRADES",
            20f,
            FontStyles.Normal,
            new Color32(150, 170, 205, 255),
            new Vector2(600f, 40f),
            new Vector2(0f, 205f)
        );

        // =========================================
        // Cards
        // =========================================

        CreateUpgradeCard(
            frame.transform,
            "Attack",
            "ATTACK",
            "DMG",
            "AttackLevelText",
            "AttackCostText",
            "AttackButton",
            new Color32(255, 87, 87, 255),
            new Vector2(-570f, -30f)
        );

        CreateUpgradeCard(
            frame.transform,
            "Speed",
            "MOVE SPEED",
            "MOVE",
            "SpeedLevelText",
            "SpeedCostText",
            "SpeedButton",
            new Color32(68, 177, 255, 255),
            new Vector2(-190f, -30f)
        );

        CreateUpgradeCard(
            frame.transform,
            "AttackSpeed",
            "ATTACK SPEED",
            "RATE",
            "AttackSpeedLevelText",
            "AttackSpeedCostText",
            "AttackSpeedButton",
            new Color32(174, 105, 255, 255),
            new Vector2(190f, -30f)
        );

        CreateUpgradeCard(
            frame.transform,
            "Gold",
            "GOLD",
            "G",
            "GoldLevelText",
            "GoldCostText",
            "GoldButton",
            new Color32(255, 193, 61, 255),
            new Vector2(570f, -30f)
        );

        // =========================================
        // Close Button
        // =========================================
        CreateCloseButton(
            frame.transform
        );

        // =========================================
        // Prefab 저장
        // =========================================

        if (!Directory.Exists(PrefabFolder))
        {
            Directory.CreateDirectory(
                PrefabFolder
            );
        }

        PrefabUtility.SaveAsPrefabAsset(
            root,
            PrefabPath
        );

        Object.DestroyImmediate(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Object prefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(
                PrefabPath
            );

        Selection.activeObject = prefab;

        Debug.Log(
            $"[LobbyUpgradeUI] Prefab 생성 완료 : {PrefabPath}"
        );
    }

    // =============================================
    // Upgrade Card
    // =============================================

    private static void CreateUpgradeCard(
        Transform parent,
        string objectName,
        string title,
        string badgeText,
        string levelName,
        string costName,
        string buttonName,
        Color accentColor,
        Vector2 position)
    {
        GameObject card =
            CreateUIObject(
                objectName,
                parent
            );

        RectTransform cardRect =
            card.GetComponent<RectTransform>();

        SetRect(
            cardRect,
            new Vector2(330f, 390f),
            position
        );

        // Card Background
        Image cardImage =
            card.AddComponent<Image>();

        cardImage.sprite = GetUISprite();
        cardImage.type = Image.Type.Sliced;

        cardImage.color =
            new Color32(32, 44, 65, 255);

        Outline outline =
            card.AddComponent<Outline>();

        outline.effectColor =
            new Color32(75, 90, 115, 180);

        outline.effectDistance =
            new Vector2(2f, -2f);

        // =========================================
        // Accent Bar
        // =========================================
        GameObject accent =
            CreateUIObject(
                "AccentBar",
                card.transform
            );

        RectTransform accentRect =
            accent.GetComponent<RectTransform>();

        SetRect(
            accentRect,
            new Vector2(280f, 8f),
            new Vector2(0f, 182f)
        );

        Image accentImage =
            accent.AddComponent<Image>();

        accentImage.color = accentColor;

        // =========================================
        // Title
        // =========================================
        CreateText(
            card.transform,
            "CardTitle",
            title,
            26f,
            FontStyles.Bold,
            Color.white,
            new Vector2(300f, 50f),
            new Vector2(0f, 130f)
        );

        // =========================================
        // Badge
        // =========================================
        GameObject badge =
            CreateUIObject(
                "Badge",
                card.transform
            );

        RectTransform badgeRect =
            badge.GetComponent<RectTransform>();

        SetRect(
            badgeRect,
            new Vector2(90f, 40f),
            new Vector2(0f, 83f)
        );

        Image badgeImage =
            badge.AddComponent<Image>();

        badgeImage.sprite = GetUISprite();
        badgeImage.type = Image.Type.Sliced;

        Color badgeColor = accentColor;
        badgeColor.a = 0.20f;

        badgeImage.color = badgeColor;

        CreateText(
            badge.transform,
            "BadgeText",
            badgeText,
            16f,
            FontStyles.Bold,
            accentColor,
            new Vector2(85f, 35f),
            Vector2.zero
        );

        // =========================================
        // LEVEL
        // =========================================
        CreateText(
            card.transform,
            "LevelLabel",
            "LEVEL",
            14f,
            FontStyles.Bold,
            new Color32(145, 165, 200, 255),
            new Vector2(130f, 30f),
            new Vector2(-70f, 30f)
        );

        CreateText(
            card.transform,
            levelName,
            "Lv.0",
            31f,
            FontStyles.Bold,
            Color.white,
            new Vector2(150f, 55f),
            new Vector2(-70f, -10f)
        );

        // =========================================
        // COST
        // =========================================
        CreateText(
            card.transform,
            "CostLabel",
            "COST",
            14f,
            FontStyles.Bold,
            new Color32(145, 165, 200, 255),
            new Vector2(130f, 30f),
            new Vector2(70f, 30f)
        );

        CreateText(
            card.transform,
            costName,
            "100 G",
            27f,
            FontStyles.Bold,
            new Color32(255, 211, 91, 255),
            new Vector2(150f, 55f),
            new Vector2(70f, -10f)
        );

        // =========================================
        // Separator
        // =========================================
        GameObject separator =
            CreateUIObject(
                "Separator",
                card.transform
            );

        RectTransform separatorRect =
            separator.GetComponent<RectTransform>();

        SetRect(
            separatorRect,
            new Vector2(270f, 2f),
            new Vector2(0f, -65f)
        );

        Image separatorImage =
            separator.AddComponent<Image>();

        separatorImage.color =
            new Color32(75, 90, 115, 150);

        // =========================================
        // Button
        // =========================================
        CreateUpgradeButton(
            card.transform,
            buttonName,
            accentColor
        );
    }

    // =============================================
    // Upgrade Button
    // =============================================

    private static void CreateUpgradeButton(
        Transform parent,
        string objectName,
        Color accentColor)
    {
        GameObject buttonObject =
            CreateUIObject(
                objectName,
                parent
            );

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        SetRect(
            rect,
            new Vector2(260f, 65f),
            new Vector2(0f, -135f)
        );

        Image image =
            buttonObject.AddComponent<Image>();

        image.sprite = GetUISprite();
        image.type = Image.Type.Sliced;

        image.color = accentColor;

        Button button =
            buttonObject.AddComponent<Button>();

        button.targetGraphic = image;

        ColorBlock colors =
            button.colors;

        colors.normalColor =
            Color.white;

        colors.highlightedColor =
            new Color(1f, 1f, 1f, 0.82f);

        colors.pressedColor =
            new Color(
                0.70f,
                0.70f,
                0.70f,
                1f
            );

        colors.disabledColor =
            new Color(
                0.35f,
                0.35f,
                0.35f,
                0.5f
            );

        button.colors = colors;

        CreateText(
            buttonObject.transform,
            "Text (TMP)",
            "UPGRADE",
            20f,
            FontStyles.Bold,
            new Color32(18, 25, 36, 255),
            new Vector2(250f, 60f),
            Vector2.zero
        );
    }

    // =============================================
    // Close Button
    // =============================================

    private static void CreateCloseButton(
        Transform parent)
    {
        GameObject close =
            CreateUIObject(
                "CloseButton",
                parent
            );

        RectTransform rect =
            close.GetComponent<RectTransform>();

        SetRect(
            rect,
            new Vector2(120f, 52f),
            new Vector2(685f, 255f)
        );

        Image image =
            close.AddComponent<Image>();

        image.sprite = GetUISprite();
        image.type = Image.Type.Sliced;

        image.color =
            new Color32(53, 65, 84, 255);

        Button button =
            close.AddComponent<Button>();

        button.targetGraphic = image;

        ColorBlock colors =
            button.colors;

        colors.highlightedColor =
            new Color32(255, 98, 98, 255);

        colors.pressedColor =
            new Color32(195, 62, 62, 255);

        button.colors = colors;

        CreateText(
            close.transform,
            "Text (TMP)",
            "CLOSE",
            17f,
            FontStyles.Bold,
            Color.white,
            new Vector2(110f, 45f),
            Vector2.zero
        );
    }

    // =============================================
    // TMP Text
    // =============================================

    private static TextMeshProUGUI CreateText(
        Transform parent,
        string objectName,
        string value,
        float fontSize,
        FontStyles fontStyle,
        Color color,
        Vector2 size,
        Vector2 position)
    {
        GameObject obj =
            CreateUIObject(
                objectName,
                parent
            );

        RectTransform rect =
            obj.GetComponent<RectTransform>();

        SetRect(
            rect,
            size,
            position
        );

        TextMeshProUGUI text =
            obj.AddComponent<TextMeshProUGUI>();

        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;

        text.alignment =
            TextAlignmentOptions.Center;

        text.enableWordWrapping = false;

        text.raycastTarget = false;

        return text;
    }

    // =============================================
    // UI Object
    // =============================================

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

    // =============================================
    // RectTransform
    // =============================================

    private static void SetRect(
        RectTransform rect,
        Vector2 size,
        Vector2 position)
    {
        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.sizeDelta = size;

        rect.anchoredPosition =
            position;

        rect.localScale =
            Vector3.one;
    }

    // =============================================
    // Unity 기본 UI Sprite
    // =============================================

    private static Sprite GetUISprite()
    {
        return AssetDatabase
            .GetBuiltinExtraResource<Sprite>(
                "UI/Skin/UISprite.psd"
            );
    }
}

#endif