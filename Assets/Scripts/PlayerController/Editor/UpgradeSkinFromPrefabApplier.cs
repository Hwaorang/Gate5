#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UpgradeSkinFromPrefabApplier
{
    private const string SkinPrefabPath =
        "Assets/Prefabs/Character_PlayerController/UI/UpgradePanel_Styled.prefab";

    [MenuItem("Tools/Gate5/UI/Apply Styled Prefab Skin")]
    private static void ApplySkin()
    {
        GameObject targetRoot = Selection.activeGameObject;

        if (targetRoot == null)
        {
            Debug.LogWarning(
                "[UpgradeSkin] 기존 UpgradePanel을 선택해주세요."
            );
            return;
        }

        GameObject skinPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(
                SkinPrefabPath
            );

        if (skinPrefab == null)
        {
            Debug.LogError(
                $"[UpgradeSkin] 프리팹을 찾을 수 없습니다.\n{SkinPrefabPath}"
            );
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(
            targetRoot,
            "Apply Upgrade Skin"
        );

        // =========================================
        // Root 디자인
        // =========================================
        CopyVisual(
            skinPrefab,
            targetRoot,
            false
        );

        // =========================================
        // 기존 로직 오브젝트
        // 절대 교체하지 않고 디자인만 복사
        // =========================================

        string[] existingObjects =
        {
            "Attack",
            "AttackLevelText",
            "AttackCostText",
            "AttackButton",

            "Speed",
            "SpeedLevelText",
            "SpeedCostText",
            "SpeedButton",

            "AttackSpeed",
            "AttackSpeedLevelText",
            "AttackSpeedCostText",
            "AttackSpeedButton",

            "Gold",
            "GoldLevelText",
            "GoldCostText",
            "GoldButton",

            "CloseButton"
        };

        foreach (string objectName in existingObjects)
        {
            Transform source =
                FindRecursive(
                    skinPrefab.transform,
                    objectName
                );

            Transform target =
                FindRecursive(
                    targetRoot.transform,
                    objectName
                );

            if (source == null)
            {
                Debug.LogWarning(
                    $"[UpgradeSkin] Skin에서 {objectName}을 찾지 못했습니다."
                );
                continue;
            }

            if (target == null)
            {
                Debug.LogWarning(
                    $"[UpgradeSkin] 기존 UpgradePanel에서 {objectName}을 찾지 못했습니다."
                );
                continue;
            }

            CopyVisual(
                source.gameObject,
                target.gameObject,
                true
            );
        }

        // =========================================
        // 중앙 배경
        // =========================================

        Transform sourceFrame =
            FindRecursive(
                skinPrefab.transform,
                "PanelFrame"
            );

        if (sourceFrame != null)
        {
            GameObject frame =
                GetOrCreateEmpty(
                    targetRoot.transform,
                    "PanelFrame"
                );

            // 항상 뒤쪽
            frame.transform.SetSiblingIndex(0);

            CopyVisual(
                sourceFrame.gameObject,
                frame,
                true
            );
        }

        // =========================================
        // Root 장식
        // =========================================

        CopyOrCreateDecoration(
            skinPrefab.transform,
            targetRoot.transform,
            "TitleText"
        );

        CopyOrCreateDecoration(
            skinPrefab.transform,
            targetRoot.transform,
            "SubTitleText"
        );

        // =========================================
        // 카드별 장식
        // =========================================

        string[] cards =
        {
            "Attack",
            "Speed",
            "AttackSpeed",
            "Gold"
        };

        string[] decorations =
        {
            "AccentBar",
            "CardTitle",
            "Badge",
            "LevelLabel",
            "CostLabel",
            "Separator"
        };

        foreach (string cardName in cards)
        {
            Transform sourceCard =
                FindRecursive(
                    skinPrefab.transform,
                    cardName
                );

            Transform targetCard =
                FindRecursive(
                    targetRoot.transform,
                    cardName
                );

            if (sourceCard == null ||
                targetCard == null)
            {
                continue;
            }

            foreach (string decoration in decorations)
            {
                CopyOrCreateDecoration(
                    sourceCard,
                    targetCard,
                    decoration
                );
            }
        }

        ApplyResponsiveLayout(targetRoot.transform);

        EditorUtility.SetDirty(targetRoot);

        Debug.Log(
            "[UpgradeSkin] Styled Prefab 디자인 적용 완료!\n" +
            "기존 Button / OnClick / TMP 참조 / Script는 유지됩니다."
        );
    }

    // ===================================================
    // 반응형 UI 배치
    // ===================================================

    private static void ApplyResponsiveLayout(
        Transform root)
    {
        RectTransform rootRect =
            root.GetComponent<RectTransform>();

        if (rootRect == null)
            return;

        // ---------------------------------------------
        // UpgradePanel = Canvas 전체 영역
        // ---------------------------------------------

        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;

        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        rootRect.pivot =
            new Vector2(0.5f, 0.5f);

        rootRect.localScale = Vector3.one;

        // ---------------------------------------------
        // 중앙 Panel
        //
        // 픽셀 크기 대신
        // 화면의 약 84% x 72% 사용
        // ---------------------------------------------

        Transform frame =
            FindRecursive(
                root,
                "PanelFrame"
            );

        if (frame != null)
        {
            SetStretchRect(
                frame.GetComponent<RectTransform>(),
                new Vector2(0.08f, 0.14f),
                new Vector2(0.92f, 0.86f),
                Vector2.zero,
                Vector2.zero
            );
        }

        // ---------------------------------------------
        // Title
        // ---------------------------------------------

        Transform title =
            FindRecursive(
                root,
                "TitleText"
            );

        if (title != null)
        {
            SetAnchorRect(
                title.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.79f),
                new Vector2(0.44f, 0.10f)
            );
        }

        Transform subtitle =
            FindRecursive(
                root,
                "SubTitleText"
            );

        if (subtitle != null)
        {
            SetAnchorRect(
                subtitle.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.735f),
                new Vector2(0.40f, 0.055f)
            );
        }

        // ---------------------------------------------
        // 카드 4개
        //
        // 화면 비율 기준으로 배치
        // ---------------------------------------------

        LayoutCard(
            root,
            "Attack",
            0.215f
        );

        LayoutCard(
            root,
            "Speed",
            0.405f
        );

        LayoutCard(
            root,
            "AttackSpeed",
            0.595f
        );

        LayoutCard(
            root,
            "Gold",
            0.785f
        );

        // ---------------------------------------------
        // Close
        // ---------------------------------------------

        Transform close =
            FindRecursive(
                root,
                "CloseButton"
            );

        if (close != null)
        {
            SetAnchorRect(
                close.GetComponent<RectTransform>(),
                new Vector2(0.855f, 0.79f),
                new Vector2(0.075f, 0.06f)
            );
        }
    }


    // ===================================================
    // 카드 배치
    // ===================================================

    private static void LayoutCard(
        Transform root,
        string cardName,
        float x)
    {
        Transform card =
            FindRecursive(
                root,
                cardName
            );

        if (card == null)
            return;

        // 카드 자체
        SetAnchorRect(
            card.GetComponent<RectTransform>(),
            new Vector2(x, 0.48f),
            new Vector2(0.17f, 0.48f)
        );

        // Accent
        SetChildRect(
            card,
            "AccentBar",
            new Vector2(0.5f, 0.965f),
            new Vector2(0.84f, 0.025f)
        );

        // 제목
        SetChildRect(
            card,
            "CardTitle",
            new Vector2(0.5f, 0.82f),
            new Vector2(0.90f, 0.12f)
        );

        // Badge
        SetChildRect(
            card,
            "Badge",
            new Vector2(0.5f, 0.69f),
            new Vector2(0.30f, 0.09f)
        );

        // LEVEL
        SetChildRect(
            card,
            "LevelLabel",
            new Vector2(0.30f, 0.52f),
            new Vector2(0.33f, 0.08f)
        );

        // COST
        SetChildRect(
            card,
            "CostLabel",
            new Vector2(0.70f, 0.52f),
            new Vector2(0.33f, 0.08f)
        );

        // 실제 Level 값
        Transform levelText =
            FindDirectDynamicText(
                card,
                cardName,
                true
            );

        if (levelText != null)
        {
            SetAnchorRect(
                levelText.GetComponent<RectTransform>(),
                new Vector2(0.30f, 0.42f),
                new Vector2(0.34f, 0.13f)
            );
        }

        // 실제 Cost 값
        Transform costText =
            FindDirectDynamicText(
                card,
                cardName,
                false
            );

        if (costText != null)
        {
            SetAnchorRect(
                costText.GetComponent<RectTransform>(),
                new Vector2(0.70f, 0.42f),
                new Vector2(0.38f, 0.13f)
            );
        }

        // Separator
        SetChildRect(
            card,
            "Separator",
            new Vector2(0.5f, 0.29f),
            new Vector2(0.84f, 0.008f)
        );

        // Upgrade Button
        string buttonName =
            cardName + "Button";

        Transform button =
            FindRecursive(
                card,
                buttonName
            );

        if (button != null)
        {
            SetAnchorRect(
                button.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.13f),
                new Vector2(0.78f, 0.15f)
            );
        }
    }


    // ===================================================
    // Level / Cost 이름 찾기
    // ===================================================

    private static Transform FindDirectDynamicText(
        Transform card,
        string cardName,
        bool level)
    {
        string objectName;

        if (level)
        {
            objectName =
                cardName + "LevelText";
        }
        else
        {
            objectName =
                cardName + "CostText";
        }

        return FindRecursive(
            card,
            objectName
        );
    }


    // ===================================================
    // 자식 위치 설정
    // ===================================================

    private static void SetChildRect(
        Transform parent,
        string childName,
        Vector2 anchor,
        Vector2 normalizedSize)
    {
        Transform child =
            FindRecursive(
                parent,
                childName
            );

        if (child == null)
            return;

        SetAnchorRect(
            child.GetComponent<RectTransform>(),
            anchor,
            normalizedSize
        );
    }


    // ===================================================
    // Anchor 기반 Rect 설정
    //
    // normalizedSize = 부모 크기의 비율
    // ===================================================

    private static void SetAnchorRect(
        RectTransform rect,
        Vector2 anchor,
        Vector2 normalizedSize)
    {
        if (rect == null)
            return;

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        RectTransform parent =
            rect.parent as RectTransform;

        if (parent == null)
            return;

        float width =
            parent.rect.width *
            normalizedSize.x;

        float height =
            parent.rect.height *
            normalizedSize.y;

        rect.sizeDelta =
            new Vector2(
                width,
                height
            );

        rect.anchoredPosition =
            Vector2.zero;

        rect.localScale =
            Vector3.one;
    }


    // ===================================================
    // Stretch Rect
    // ===================================================

    private static void SetStretchRect(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        if (rect == null)
            return;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        rect.localScale =
            Vector3.one;
    }

    // =============================================
    // 장식 오브젝트 복사
    // =============================================

    private static void CopyOrCreateDecoration(
        Transform sourceParent,
        Transform targetParent,
        string objectName)
    {
        Transform source =
            FindRecursive(
                sourceParent,
                objectName
            );

        if (source == null)
            return;

        Transform existing =
            FindRecursiveDirect(
                targetParent,
                objectName
            );

        // 이미 있으면 디자인만 갱신
        if (existing != null)
        {
            CopyVisual(
                source.gameObject,
                existing.gameObject,
                true
            );

            // Badge처럼 안쪽에
            // BadgeText가 있는 구조까지 갱신
            CopyChildrenVisual(
                source,
                existing
            );

            return;
        }

        // 새 장식은 통째로 복제해도
        // 팀 로직과 관계없음
        GameObject clone =
            Object.Instantiate(
                source.gameObject,
                targetParent
            );

        clone.name = source.name;

        Undo.RegisterCreatedObjectUndo(
            clone,
            "Create Upgrade Decoration"
        );
    }

    // =============================================
    // 자식 디자인 갱신
    // =============================================

    private static void CopyChildrenVisual(
        Transform source,
        Transform target)
    {
        foreach (Transform sourceChild in source)
        {
            Transform targetChild =
                FindRecursiveDirect(
                    target,
                    sourceChild.name
                );

            if (targetChild == null)
                continue;

            CopyVisual(
                sourceChild.gameObject,
                targetChild.gameObject,
                true
            );

            CopyChildrenVisual(
                sourceChild,
                targetChild
            );
        }
    }

    // =============================================
    // 핵심: 디자인만 복사
    // =============================================

    private static void CopyVisual(
        GameObject source,
        GameObject target,
        bool copyRect)
    {
        // -----------------------------------------
        // RectTransform
        // -----------------------------------------

        RectTransform sourceRect =
            source.GetComponent<RectTransform>();

        RectTransform targetRect =
            target.GetComponent<RectTransform>();

        if (copyRect &&
            sourceRect != null &&
            targetRect != null)
        {
            targetRect.anchorMin =
                sourceRect.anchorMin;

            targetRect.anchorMax =
                sourceRect.anchorMax;

            targetRect.pivot =
                sourceRect.pivot;

            targetRect.anchoredPosition =
                sourceRect.anchoredPosition;

            targetRect.sizeDelta =
                sourceRect.sizeDelta;

            targetRect.localScale =
                sourceRect.localScale;

            targetRect.localRotation =
                sourceRect.localRotation;
        }

        // -----------------------------------------
        // Image
        // ★ 교체한 Sprite가 여기서 복사됨
        // -----------------------------------------

        Image sourceImage =
            source.GetComponent<Image>();

        if (sourceImage != null)
        {
            Image targetImage =
                target.GetComponent<Image>();

            if (targetImage == null)
            {
                targetImage =
                    Undo.AddComponent<Image>(
                        target
                    );
            }

            targetImage.sprite =
                sourceImage.sprite;

            targetImage.overrideSprite =
                sourceImage.overrideSprite;

            targetImage.color =
                sourceImage.color;

            targetImage.material =
                sourceImage.material;

            targetImage.type =
                sourceImage.type;

            targetImage.preserveAspect =
                sourceImage.preserveAspect;

            targetImage.fillCenter =
                sourceImage.fillCenter;

            targetImage.pixelsPerUnitMultiplier =
                sourceImage.pixelsPerUnitMultiplier;

            targetImage.raycastTarget =
                sourceImage.raycastTarget;
        }

        // -----------------------------------------
        // Outline
        // -----------------------------------------

        Outline sourceOutline =
            source.GetComponent<Outline>();

        if (sourceOutline != null)
        {
            Outline targetOutline =
                target.GetComponent<Outline>();

            if (targetOutline == null)
            {
                targetOutline =
                    Undo.AddComponent<Outline>(
                        target
                    );
            }

            targetOutline.effectColor =
                sourceOutline.effectColor;

            targetOutline.effectDistance =
                sourceOutline.effectDistance;

            targetOutline.useGraphicAlpha =
                sourceOutline.useGraphicAlpha;
        }

        // -----------------------------------------
        // TMP
        // ★ text 내용은 복사하지 않음
        // -----------------------------------------

        TMP_Text sourceText =
            source.GetComponent<TMP_Text>();

        TMP_Text targetText =
            target.GetComponent<TMP_Text>();

        if (sourceText != null &&
            targetText != null)
        {
            // text.text는 일부러 안 건드림
            // Level / Cost 런타임 값 보호

            targetText.font =
                sourceText.font;

            targetText.fontSharedMaterial =
                sourceText.fontSharedMaterial;

            targetText.fontSize =
                sourceText.fontSize;

            targetText.fontStyle =
                sourceText.fontStyle;

            targetText.color =
                sourceText.color;

            targetText.alignment =
                sourceText.alignment;

            targetText.enableWordWrapping =
                sourceText.enableWordWrapping;

            targetText.characterSpacing =
                sourceText.characterSpacing;

            targetText.lineSpacing =
                sourceText.lineSpacing;

            targetText.raycastTarget =
                sourceText.raycastTarget;
        }

        // -----------------------------------------
        // Button
        // ★ OnClick은 절대 변경하지 않음
        // -----------------------------------------

        Button sourceButton =
            source.GetComponent<Button>();

        Button targetButton =
            target.GetComponent<Button>();

        if (sourceButton != null &&
            targetButton != null)
        {
            targetButton.transition =
                sourceButton.transition;

            targetButton.colors =
                sourceButton.colors;

            targetButton.spriteState =
                sourceButton.spriteState;

            targetButton.animationTriggers =
                sourceButton.animationTriggers;

            // onClick은 복사하지 않음!
            // 기존 팀원 연결 그대로 유지

            Image targetImage =
                target.GetComponent<Image>();

            if (targetImage != null)
            {
                targetButton.targetGraphic =
                    targetImage;
            }
        }
    }

    // =============================================
    // Recursive Find
    // =============================================

    private static Transform FindRecursive(
        Transform parent,
        string objectName)
    {
        if (parent.name == objectName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result =
                FindRecursive(
                    child,
                    objectName
                );

            if (result != null)
                return result;
        }

        return null;
    }

    private static Transform FindRecursiveDirect(
        Transform parent,
        string objectName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == objectName)
                return child;
        }

        return null;
    }

    // =============================================
    // Empty Decorative Object
    // =============================================

    private static GameObject GetOrCreateEmpty(
        Transform parent,
        string objectName)
    {
        Transform existing =
            parent.Find(objectName);

        if (existing != null)
            return existing.gameObject;

        GameObject obj =
            new GameObject(
                objectName,
                typeof(RectTransform)
            );

        obj.transform.SetParent(
            parent,
            false
        );

        Undo.RegisterCreatedObjectUndo(
            obj,
            "Create Upgrade UI Decoration"
        );

        return obj;
    }
}

#endif