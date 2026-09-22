#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEngine;

public static class UpgradePanelResponsiveFixer
{
    private const string PrefabPath =
        "Assets/Prefabs/Character_PlayerController/UI/UpgradePanel_Styled.prefab";

    [MenuItem("Tools/Gate5/UI/Fix Styled Upgrade Panel Responsive")]
    private static void FixPrefab()
    {
        GameObject root =
            PrefabUtility.LoadPrefabContents(PrefabPath);

        if (root == null)
        {
            Debug.LogError(
                "[UpgradeUI] UpgradePanel_Styled.prefab을 찾지 못했습니다."
            );
            return;
        }

        // =========================================
        // Root = 부모 Canvas 전체
        // =========================================

        RectTransform rootRect =
            root.GetComponent<RectTransform>();

        Stretch(
            rootRect,
            0f, 0f,
            1f, 1f
        );

        // =========================================
        // 배경 PanelFrame
        // =========================================

        Set(
            root.transform,
            "PanelFrame",
            0.06f, 0.13f,
            0.94f, 0.87f
        );

        // =========================================
        // 제목
        // =========================================

        Set(
            root.transform,
            "TitleText",
            0.30f, 0.78f,
            0.70f, 0.87f
        );

        Set(
            root.transform,
            "SubTitleText",
            0.32f, 0.72f,
            0.68f, 0.77f
        );

        // =========================================
        // 카드 4개
        // =========================================

        LayoutCard(
            root.transform,
            "Attack",
            0.08f,
            0.285f
        );

        LayoutCard(
            root.transform,
            "Speed",
            0.295f,
            0.49f
        );

        LayoutCard(
            root.transform,
            "AttackSpeed",
            0.51f,
            0.705f
        );

        LayoutCard(
            root.transform,
            "Gold",
            0.715f,
            0.92f
        );

        // =========================================
        // Close Button
        // =========================================

        Set(
            root.transform,
            "CloseButton",
            0.84f, 0.78f,
            0.92f, 0.84f
        );

        // =========================================
        // Font 자동 크기
        // =========================================

        SetupFont(
            Find(root.transform, "TitleText"),
            25f,
            46f
        );

        SetupFont(
            Find(root.transform, "SubTitleText"),
            12f,
            20f
        );

        PrefabUtility.SaveAsPrefabAsset(
            root,
            PrefabPath
        );

        PrefabUtility.UnloadPrefabContents(root);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "[UpgradeUI] UpgradePanel_Styled 반응형 변환 완료!"
        );
    }

    // =============================================
    // 카드 내부
    // =============================================

    private static void LayoutCard(
        Transform root,
        string cardName,
        float left,
        float right)
    {
        Transform card =
            Find(root, cardName);

        if (card == null)
        {
            Debug.LogWarning(
                $"[UpgradeUI] {cardName}을 찾지 못했습니다."
            );
            return;
        }

        // 카드 자체
        Stretch(
            card.GetComponent<RectTransform>(),
            left,
            0.22f,
            right,
            0.69f
        );

        // 상단 컬러바
        Set(
            card,
            "AccentBar",
            0.06f, 0.94f,
            0.94f, 0.965f
        );

        // 카드 제목
        Set(
            card,
            "CardTitle",
            0.05f, 0.76f,
            0.95f, 0.89f
        );

        // DMG / MOVE / RATE / G
        Set(
            card,
            "Badge",
            0.34f, 0.64f,
            0.66f, 0.73f
        );

        // LEVEL
        Set(
            card,
            "LevelLabel",
            0.07f, 0.48f,
            0.43f, 0.55f
        );

        // COST
        Set(
            card,
            "CostLabel",
            0.57f, 0.48f,
            0.93f, 0.55f
        );

        // 실제 Level
        Set(
            card,
            cardName + "LevelText",
            0.06f, 0.35f,
            0.44f, 0.48f
        );

        // 실제 Cost
        Set(
            card,
            cardName + "CostText",
            0.52f, 0.35f,
            0.96f, 0.48f
        );

        // 구분선
        Set(
            card,
            "Separator",
            0.07f, 0.28f,
            0.93f, 0.287f
        );

        // 버튼
        Set(
            card,
            cardName + "Button",
            0.07f, 0.06f,
            0.93f, 0.20f
        );

        // 버튼 안 글자
        Transform button =
            Find(
                card,
                cardName + "Button"
            );

        if (button != null)
        {
            Transform buttonText =
                Find(button, "Text (TMP)");

            if (buttonText != null)
            {
                Stretch(
                    buttonText.GetComponent<RectTransform>(),
                    0f, 0f,
                    1f, 1f
                );

                SetupFont(
                    buttonText,
                    12f,
                    20f
                );
            }
        }

        // 글자 Auto Size
        SetupFont(
            Find(card, "CardTitle"),
            14f,
            26f
        );

        SetupFont(
            Find(card, "BadgeText"),
            10f,
            16f
        );

        SetupFont(
            Find(card, "LevelLabel"),
            9f,
            14f
        );

        SetupFont(
            Find(card, "CostLabel"),
            9f,
            14f
        );

        SetupFont(
            Find(
                card,
                cardName + "LevelText"
            ),
            16f,
            31f
        );

        SetupFont(
            Find(
                card,
                cardName + "CostText"
            ),
            14f,
            27f
        );
    }

    // =============================================
    // Anchor 설정
    // =============================================

    private static void Set(
        Transform parent,
        string objectName,
        float minX,
        float minY,
        float maxX,
        float maxY)
    {
        Transform target =
            Find(parent, objectName);

        if (target == null)
            return;

        Stretch(
            target.GetComponent<RectTransform>(),
            minX,
            minY,
            maxX,
            maxY
        );
    }

    private static void Stretch(
        RectTransform rect,
        float minX,
        float minY,
        float maxX,
        float maxY)
    {
        if (rect == null)
            return;

        rect.anchorMin =
            new Vector2(minX, minY);

        rect.anchorMax =
            new Vector2(maxX, maxY);

        rect.offsetMin =
            Vector2.zero;

        rect.offsetMax =
            Vector2.zero;

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.localScale =
            Vector3.one;

        rect.localRotation =
            Quaternion.identity;
    }

    // =============================================
    // TMP Auto Size
    // =============================================

    private static void SetupFont(
        Transform target,
        float min,
        float max)
    {
        if (target == null)
            return;

        TMP_Text text =
            target.GetComponent<TMP_Text>();

        if (text == null)
            return;

        text.enableAutoSizing = true;

        text.fontSizeMin = min;
        text.fontSizeMax = max;

        text.enableWordWrapping = false;
    }

    // =============================================
    // Recursive Find
    // =============================================

    private static Transform Find(
        Transform parent,
        string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result =
                Find(child, name);

            if (result != null)
                return result;
        }

        return null;
    }
}

#endif