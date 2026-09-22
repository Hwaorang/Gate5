#if UNITY_EDITOR

using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class UpgradePanelVisualReplacer
{
    private const string StyledPrefabPath =
        "Assets/Prefabs/Character_PlayerController/UI/UpgradePanel_Styled.prefab";

    private static readonly string[] LogicObjectNames =
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

    private static readonly string[] DynamicTextNames =
    {
        "AttackLevelText",
        "AttackCostText",

        "SpeedLevelText",
        "SpeedCostText",

        "AttackSpeedLevelText",
        "AttackSpeedCostText",

        "GoldLevelText",
        "GoldCostText"
    };

    private static readonly string[] ButtonNames =
    {
        "AttackButton",
        "SpeedButton",
        "AttackSpeedButton",
        "GoldButton",
        "CloseButton"
    };

    [MenuItem(
        "Tools/Gate5/UI/Replace Upgrade Visuals (Keep Logic)"
    )]
    private static void ReplaceVisuals()
    {
        GameObject targetRoot =
            Selection.activeGameObject;

        if (targetRoot == null)
        {
            Debug.LogWarning(
                "[UpgradeUI] 기존 UpgradePanel을 선택해주세요."
            );

            return;
        }

        GameObject styledPrefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(
                StyledPrefabPath
            );

        if (styledPrefab == null)
        {
            Debug.LogError(
                "[UpgradeUI] Styled Prefab을 찾지 못했습니다.\n" +
                StyledPrefabPath
            );

            return;
        }

        Undo.SetCurrentGroupName(
            "Replace Upgrade UI Visuals"
        );

        int undoGroup =
            Undo.GetCurrentGroup();

        // =========================================
        // 1. 기존 UI 오브젝트 찾기
        // =========================================

        Dictionary<string, Transform> oldObjects =
            new Dictionary<string, Transform>();

        foreach (string objectName in LogicObjectNames)
        {
            Transform target =
                FindRecursive(
                    targetRoot.transform,
                    objectName
                );

            if (target != null)
            {
                oldObjects[objectName] = target;
            }
        }

        // =========================================
        // 2. Styled Prefab 생성
        // =========================================

        GameObject view =
            PrefabUtility.InstantiatePrefab(
                styledPrefab,
                targetRoot.transform
            ) as GameObject;

        if (view == null)
        {
            Debug.LogError(
                "[UpgradeUI] Styled Prefab 생성 실패"
            );

            return;
        }

        Undo.RegisterCreatedObjectUndo(
            view,
            "Create UpgradePanel View"
        );

        view.name = "UpgradePanelView";

        RectTransform viewRect =
            view.GetComponent<RectTransform>();

        if (viewRect != null)
        {
            viewRect.anchorMin =
                Vector2.zero;

            viewRect.anchorMax =
                Vector2.one;

            viewRect.offsetMin =
                Vector2.zero;

            viewRect.offsetMax =
                Vector2.zero;

            viewRect.localScale =
                Vector3.one;
        }

        // =========================================
        // 3. 새 UI 오브젝트 찾기
        // =========================================

        Dictionary<string, Transform> newObjects =
            new Dictionary<string, Transform>();

        foreach (string objectName in LogicObjectNames)
        {
            Transform target =
                FindRecursive(
                    view.transform,
                    objectName
                );

            if (target != null)
            {
                newObjects[objectName] = target;
            }
            else
            {
                Debug.LogWarning(
                    $"[UpgradeUI] Styled Prefab에서 " +
                    $"{objectName}을 찾지 못했습니다."
                );
            }
        }

        // =========================================
        // 4. Old -> New Object Mapping 생성
        // =========================================

        Dictionary<Object, Object> objectMap =
            new Dictionary<Object, Object>();

        foreach (string objectName in LogicObjectNames)
        {
            if (!oldObjects.TryGetValue(
                    objectName,
                    out Transform oldTransform))
            {
                continue;
            }

            if (!newObjects.TryGetValue(
                    objectName,
                    out Transform newTransform))
            {
                continue;
            }

            AddObjectMappings(
                oldTransform.gameObject,
                newTransform.gameObject,
                objectMap
            );
        }

        // =========================================
        // 5. 현재 Level / Cost 문자열 보존
        // =========================================

        foreach (string textName in DynamicTextNames)
        {
            if (!oldObjects.TryGetValue(
                    textName,
                    out Transform oldTransform))
            {
                continue;
            }

            if (!newObjects.TryGetValue(
                    textName,
                    out Transform newTransform))
            {
                continue;
            }

            TMP_Text oldText =
                oldTransform.GetComponent<TMP_Text>();

            TMP_Text newText =
                newTransform.GetComponent<TMP_Text>();

            if (oldText == null ||
                newText == null)
            {
                continue;
            }

            Undo.RecordObject(
                newText,
                "Copy Upgrade Text Value"
            );

            // 디자인은 Styled Prefab 사용
            // 실제 값만 기존 값 유지
            newText.text = oldText.text;
        }

        // =========================================
        // 6. 기존 Button OnClick 보존
        // =========================================

        foreach (string buttonName in ButtonNames)
        {
            if (!oldObjects.TryGetValue(
                    buttonName,
                    out Transform oldTransform))
            {
                continue;
            }

            if (!newObjects.TryGetValue(
                    buttonName,
                    out Transform newTransform))
            {
                continue;
            }

            Button oldButton =
                oldTransform.GetComponent<Button>();

            Button newButton =
                newTransform.GetComponent<Button>();

            if (oldButton == null ||
                newButton == null)
            {
                continue;
            }

            CopyButtonOnClick(
                oldButton,
                newButton
            );
        }

        // =========================================
        // 7. Scene의 SerializedField 자동 재연결
        // =========================================

        RemapSerializedReferences(
            targetRoot,
            objectMap
        );

        // =========================================
        // 8. 기존 Visual 자식 삭제
        //
        // Root UpgradePanel 자체와
        // Root에 붙은 Script는 절대 삭제 X
        // =========================================

        List<GameObject> childrenToDelete =
            new List<GameObject>();

        for (int i = 0;
             i < targetRoot.transform.childCount;
             i++)
        {
            Transform child =
                targetRoot.transform.GetChild(i);

            if (child.gameObject == view)
                continue;

            childrenToDelete.Add(
                child.gameObject
            );
        }

        foreach (GameObject child in childrenToDelete)
        {
            Undo.DestroyObjectImmediate(child);
        }

        // =========================================
        // 9. 기존 Root Image가 화면을 가리지 않게
        // =========================================

        Image rootImage =
            targetRoot.GetComponent<Image>();

        if (rootImage != null)
        {
            Undo.RecordObject(
                rootImage,
                "Disable Old Upgrade Background"
            );

            Color color =
                rootImage.color;

            color.a = 0f;

            rootImage.color = color;

            rootImage.raycastTarget = false;
        }

        // =========================================
        // 기존 Visual 삭제 완료
        // =========================================

        // 새 UI 기준으로 Missing / SerializedField 자동 복구
        UpgradePanelReferenceRepairer.Repair(
            targetRoot
        );


        // =========================================
        // 10. 저장
        // =========================================

        EditorUtility.SetDirty(
            targetRoot
        );

        EditorSceneManager.MarkSceneDirty(
            targetRoot.scene
        );

        Undo.CollapseUndoOperations(
            undoGroup
        );

        Debug.Log(
            "[UpgradeUI] UI 교체 완료!\n" +
            "기존 UpgradePanel Root와 Script는 유지했습니다.\n" +
            "TMP_Text / Button SerializedField도 새 UI로 자동 재연결했습니다."
        );
    }

    // =============================================
    // Old -> New Component Mapping
    // =============================================

    private static void AddObjectMappings(
        GameObject oldObject,
        GameObject newObject,
        Dictionary<Object, Object> map)
    {
        map[oldObject] =
            newObject;

        map[oldObject.transform] =
            newObject.transform;

        Component[] oldComponents =
            oldObject.GetComponents<Component>();

        foreach (Component oldComponent
                 in oldComponents)
        {
            if (oldComponent == null)
                continue;

            Component newComponent =
                newObject.GetComponent(
                    oldComponent.GetType()
                );

            if (newComponent == null)
                continue;

            map[oldComponent] =
                newComponent;
        }
    }

    // =============================================
    // Inspector SerializedField 재연결
    // =============================================

    private static void RemapSerializedReferences(
        GameObject targetRoot,
        Dictionary<Object, Object> objectMap)
    {
        GameObject[] sceneRoots =
            targetRoot.scene.GetRootGameObjects();

        int changedCount = 0;

        foreach (GameObject sceneRoot in sceneRoots)
        {
            MonoBehaviour[] behaviours =
                sceneRoot.GetComponentsInChildren<MonoBehaviour>(
                    true
                );

            foreach (MonoBehaviour behaviour
                     in behaviours)
            {
                if (behaviour == null)
                    continue;

                SerializedObject serializedObject =
                    new SerializedObject(
                        behaviour
                    );

                serializedObject.Update();

                SerializedProperty property =
                    serializedObject.GetIterator();

                bool changed = false;

                while (property.NextVisible(true))
                {
                    if (property.propertyType !=
                        SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    Object oldReference =
                        property.objectReferenceValue;

                    if (oldReference == null)
                        continue;

                    if (!objectMap.TryGetValue(
                            oldReference,
                            out Object newReference))
                    {
                        continue;
                    }

                    property.objectReferenceValue =
                        newReference;

                    changed = true;
                }

                if (!changed)
                    continue;

                Undo.RecordObject(
                    behaviour,
                    "Remap Upgrade UI References"
                );

                serializedObject
                    .ApplyModifiedProperties();

                EditorUtility.SetDirty(
                    behaviour
                );

                changedCount++;
            }
        }

        Debug.Log(
            $"[UpgradeUI] Inspector 참조 자동 변경 : " +
            $"{changedCount}개 Component"
        );
    }

    // =============================================
    // Button.onClick만 복사
    //
    // Styled Button의 Sprite / 색상 / 디자인은
    // 그대로 유지
    // =============================================

    private static void CopyButtonOnClick(
        Button oldButton,
        Button newButton)
    {
        SerializedObject oldSerialized =
            new SerializedObject(
                oldButton
            );

        SerializedObject newSerialized =
            new SerializedObject(
                newButton
            );

        oldSerialized.Update();
        newSerialized.Update();

        SerializedProperty oldOnClick =
            oldSerialized.FindProperty(
                "m_OnClick"
            );

        if (oldOnClick == null)
            return;

        Undo.RecordObject(
            newButton,
            "Copy Button OnClick"
        );

        newSerialized
            .CopyFromSerializedProperty(
                oldOnClick
            );

        newSerialized
            .ApplyModifiedProperties();
    }

    // =============================================
    // Recursive Find
    // =============================================

    private static Transform FindRecursive(
        Transform parent,
        string objectName)
    {
        if (parent.name == objectName)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform result =
                FindRecursive(
                    child,
                    objectName
                );

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}

#endif