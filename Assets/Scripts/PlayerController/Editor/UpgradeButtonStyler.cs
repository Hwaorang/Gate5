using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 기존 UpgradeButton Prefab의 기능은 유지하고
/// 카드 UI 외형만 자동으로 정리한다.
///
/// 사용:
/// Project 창에서 UpgradeButton Prefab 선택
/// → Tools
/// → UI
/// → Style Upgrade Button
/// </summary>
public static class UpgradeButtonStyler
{
    private static readonly Color CardColor =
        new Color32(255, 222, 80, 255);

    private static readonly Color InnerColor =
        new Color32(255, 245, 200, 255);

    private static readonly Color AccentColor =
        new Color32(245, 150, 20, 255);

    private static readonly Color TextColor =
        new Color32(95, 55, 15, 255);


    [MenuItem("Tools/UI/Style Upgrade Button")]
    public static void StyleUpgradeButton()
    {
        GameObject selected =
            Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Upgrade Button Styler",
                "UpgradeButton Prefab을 선택해주세요.",
                "확인"
            );

            return;
        }


        // =========================
        // Project 창의 Prefab Asset인지 확인
        // =========================

        if (PrefabUtility.IsPartOfPrefabAsset(selected))
        {
            string prefabPath =
                AssetDatabase.GetAssetPath(selected);

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogWarning(
                    "[UpgradeButtonStyler] Prefab 경로를 찾지 못했습니다."
                );

                return;
            }


            // Prefab 원본을 안전하게 편집용으로 로드
            GameObject prefabRoot =
                PrefabUtility.LoadPrefabContents(
                    prefabPath
                );

            try
            {
                ApplyStyle(
                    prefabRoot
                );


                // 수정된 Prefab 저장
                PrefabUtility.SaveAsPrefabAsset(
                    prefabRoot,
                    prefabPath
                );


                Debug.Log(
                    "[UpgradeButtonStyler] " +
                    "UpgradeButton Prefab 스타일 적용 완료!"
                );
            }
            finally
            {
                // 편집용 Prefab 해제
                PrefabUtility.UnloadPrefabContents(
                    prefabRoot
                );
            }

            return;
        }


        // =========================
        // Scene / Prefab Mode 오브젝트
        // =========================

        ApplyStyle(
            selected
        );

        EditorUtility.SetDirty(
            selected
        );

        Debug.Log(
            "[UpgradeButtonStyler] " +
            "UpgradeButton 스타일 적용 완료!"
        );
    }

    private static void ApplyStyle(
    GameObject selected)
    {
        UpgradeButton upgradeButton =
           selected.GetComponent<UpgradeButton>();

        if (upgradeButton == null)
        {
            EditorUtility.DisplayDialog(
                "Upgrade Button Styler",
                "선택한 오브젝트에 UpgradeButton 컴포넌트가 없습니다.",
                "확인"
            );

            return;
        }


        RectTransform root =
            selected.GetComponent<RectTransform>();

        if (root == null)
        {
            return;
        }


        Undo.RecordObject(
            selected,
            "Style Upgrade Button"
        );


        // ========================================
        // 카드 크기
        // ========================================

        root.sizeDelta =
            new Vector2(
                380f,
                520f
            );


        // LayoutGroup 안에서 크기 유지
        LayoutElement layout =
            selected.GetComponent<LayoutElement>();

        if (layout == null)
        {
            layout =
                selected.AddComponent<LayoutElement>();
        }

        layout.preferredWidth = 380f;
        layout.preferredHeight = 520f;
        layout.minWidth = 340f;
        layout.minHeight = 480f;


        // ========================================
        // 기존 Root Image
        // ========================================

        Image rootImage =
            selected.GetComponent<Image>();

        if (rootImage == null)
        {
            rootImage =
                selected.AddComponent<Image>();
        }

        rootImage.color =
            CardColor;

        Sprite defaultSprite =
            AssetDatabase.GetBuiltinExtraResource<Sprite>(
                "UI/Skin/UISprite.psd"
            );

        if (rootImage.sprite == null &&
            defaultSprite != null)
        {
            rootImage.sprite =
                defaultSprite;

            rootImage.type =
                Image.Type.Sliced;
        }


        // ========================================
        // 안쪽 패널
        // ========================================

        Transform oldInner =
            selected.transform.Find("AutoInnerPanel");

        if (oldInner == null)
        {
            GameObject inner =
                new GameObject(
                    "AutoInnerPanel",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );

            inner.transform.SetParent(
                selected.transform,
                false
            );

            // 뒤로 보내서 기존 Text / Icon을 가리지 않는다.
            inner.transform.SetAsFirstSibling();


            RectTransform innerRect =
                inner.GetComponent<RectTransform>();

            innerRect.anchorMin =
                Vector2.zero;

            innerRect.anchorMax =
                Vector2.one;

            innerRect.offsetMin =
                new Vector2(
                    18f,
                    18f
                );

            innerRect.offsetMax =
                new Vector2(
                    -18f,
                    -18f
                );


            Image innerImage =
                inner.GetComponent<Image>();

            innerImage.color =
                InnerColor;

            if (defaultSprite != null)
            {
                innerImage.sprite =
                    defaultSprite;

                innerImage.type =
                    Image.Type.Sliced;
            }

            // 클릭을 막지 않도록
            innerImage.raycastTarget =
                false;
        }


        // ========================================
        // 기존 TMP 글자 자동 정리
        // ========================================

        TMP_Text[] texts =
            selected.GetComponentsInChildren<TMP_Text>(
                true
            );

        foreach (TMP_Text text in texts)
        {
            text.color =
                TextColor;

            text.enableAutoSizing =
                true;

            text.fontSizeMin =
                18f;

            text.raycastTarget =
                false;


            string lowerName =
                text.gameObject.name.ToLower();


            // 강화 이름
            if (lowerName.Contains("name") ||
                lowerName.Contains("title"))
            {
                text.fontSizeMax = 34f;

                text.fontStyle =
                    FontStyles.Bold;
            }

            // 레벨
            else if (lowerName.Contains("level"))
            {
                text.fontSizeMax = 30f;

                text.fontStyle =
                    FontStyles.Bold;

                text.color =
                    AccentColor;
            }

            // 수치
            else if (lowerName.Contains("value"))
            {
                text.fontSizeMax = 32f;

                text.fontStyle =
                    FontStyles.Bold;

                text.color =
                    AccentColor;
            }

            // 설명
            else if (lowerName.Contains("description") ||
                     lowerName.Contains("desc"))
            {
                text.fontSizeMax = 24f;
            }

            else
            {
                text.fontSizeMax = 26f;
            }
        }


        // ========================================
        // Icon 비율 유지
        // ========================================

        Image[] images =
            selected.GetComponentsInChildren<Image>(
                true
            );

        foreach (Image image in images)
        {
            if (image == rootImage)
            {
                continue;
            }

            string lowerName =
                image.gameObject.name.ToLower();

            if (lowerName.Contains("icon"))
            {
                image.type =
                    Image.Type.Simple;

                image.preserveAspect =
                    true;

                image.raycastTarget =
                    false;
            }
        }


        // ========================================
        // Button 색 전환
        // ========================================

        Button button =
            selected.GetComponent<Button>();

        if (button != null)
        {
            ColorBlock colors =
                button.colors;

            colors.normalColor =
                Color.white;

            colors.highlightedColor =
                new Color(
                    1f,
                    0.95f,
                    0.75f,
                    1f
                );

            colors.pressedColor =
                new Color(
                    0.9f,
                    0.8f,
                    0.55f,
                    1f
                );

            button.colors =
                colors;
        }


        EditorUtility.SetDirty(
            selected
        );
    }
}