#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class UpgradePanelReferenceRepairer
{
    [MenuItem("Tools/Gate5/UI/Repair Upgrade UI References")]
    private static void RepairSelected()
    {
        GameObject root = Selection.activeGameObject;

        if (root == null)
        {
            Debug.LogWarning(
                "[UpgradeUI] Hierarchy에서 UpgradePanel을 선택해주세요."
            );
            return;
        }

        Repair(root);
    }

    public static void Repair(GameObject upgradePanel)
    {
        // 새 디자인 Prefab Instance
        Transform view =
            FindRecursive(
                upgradePanel.transform,
                "UpgradePanelView"
            );

        // 아직 View 구조가 아니면 자기 자신에서 검색
        if (view == null)
        {
            view = upgradePanel.transform;
        }

        // =========================================
        // 새 UI Component 찾기
        // =========================================

        Dictionary<string, UnityEngine.Object> references =
            new Dictionary<string, UnityEngine.Object>(
                StringComparer.OrdinalIgnoreCase
            );

        AddTextReference(
            references,
            view,
            "attackLevelText",
            "AttackLevelText"
        );

        AddTextReference(
            references,
            view,
            "attackCostText",
            "AttackCostText"
        );

        AddButtonReference(
            references,
            view,
            "attackButton",
            "AttackButton"
        );

        AddTextReference(
            references,
            view,
            "speedLevelText",
            "SpeedLevelText"
        );

        AddTextReference(
            references,
            view,
            "speedCostText",
            "SpeedCostText"
        );

        AddButtonReference(
            references,
            view,
            "speedButton",
            "SpeedButton"
        );

        AddTextReference(
            references,
            view,
            "attackSpeedLevelText",
            "AttackSpeedLevelText"
        );

        AddTextReference(
            references,
            view,
            "attackSpeedCostText",
            "AttackSpeedCostText"
        );

        AddButtonReference(
            references,
            view,
            "attackSpeedButton",
            "AttackSpeedButton"
        );

        AddTextReference(
            references,
            view,
            "goldLevelText",
            "GoldLevelText"
        );

        AddTextReference(
            references,
            view,
            "goldCostText",
            "GoldCostText"
        );

        AddButtonReference(
            references,
            view,
            "goldButton",
            "GoldButton"
        );

        AddButtonReference(
            references,
            view,
            "closeButton",
            "CloseButton"
        );

        // =========================================
        // Scene 내 MonoBehaviour 검색
        //
        // Inspector SerializedField 이름이
        // 일치하면 새 UI로 자동 연결
        // =========================================

        GameObject[] sceneRoots =
            upgradePanel.scene.GetRootGameObjects();

        int changedComponentCount = 0;
        int changedFieldCount = 0;

        foreach (GameObject sceneRoot in sceneRoots)
        {
            MonoBehaviour[] behaviours =
                sceneRoot.GetComponentsInChildren<MonoBehaviour>(
                    true
                );

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour == null)
                    continue;

                SerializedObject so =
                    new SerializedObject(behaviour);

                so.Update();

                SerializedProperty property =
                    so.GetIterator();

                bool componentChanged = false;

                while (property.NextVisible(true))
                {
                    if (property.propertyType !=
                        SerializedPropertyType.ObjectReference)
                    {
                        continue;
                    }

                    string fieldName =
                        NormalizeFieldName(
                            property.name
                        );

                    if (!references.TryGetValue(
                            fieldName,
                            out UnityEngine.Object newReference))
                    {
                        continue;
                    }

                    if (newReference == null)
                        continue;

                    // 이미 정상적으로 연결되어 있으면 건너뜀
                    if (property.objectReferenceValue ==
                        newReference)
                    {
                        continue;
                    }

                    property.objectReferenceValue =
                        newReference;

                    componentChanged = true;
                    changedFieldCount++;

                    Debug.Log(
                        $"[UpgradeUI] {behaviour.GetType().Name}" +
                        $".{property.name} → " +
                        $"{newReference.name}"
                    );
                }

                if (!componentChanged)
                    continue;

                Undo.RecordObject(
                    behaviour,
                    "Repair Upgrade UI References"
                );

                so.ApplyModifiedProperties();

                EditorUtility.SetDirty(
                    behaviour
                );

                changedComponentCount++;
            }
        }

        EditorSceneManager.MarkSceneDirty(
            upgradePanel.scene
        );

        Debug.Log(
            "[UpgradeUI] 참조 복구 완료\n" +
            $"Component : {changedComponentCount}\n" +
            $"Field : {changedFieldCount}"
        );
    }

    // =============================================
    // TMP 등록
    // =============================================

    private static void AddTextReference(
        Dictionary<string, UnityEngine.Object> map,
        Transform root,
        string fieldName,
        string objectName)
    {
        Transform target =
            FindRecursive(
                root,
                objectName
            );

        if (target == null)
        {
            Debug.LogWarning(
                $"[UpgradeUI] {objectName}을 찾지 못했습니다."
            );
            return;
        }

        TMP_Text text =
            target.GetComponent<TMP_Text>();

        if (text == null)
        {
            Debug.LogWarning(
                $"[UpgradeUI] {objectName}에 TMP_Text가 없습니다."
            );
            return;
        }

        map[fieldName] = text;
    }

    // =============================================
    // Button 등록
    // =============================================

    private static void AddButtonReference(
        Dictionary<string, UnityEngine.Object> map,
        Transform root,
        string fieldName,
        string objectName)
    {
        Transform target =
            FindRecursive(
                root,
                objectName
            );

        if (target == null)
        {
            Debug.LogWarning(
                $"[UpgradeUI] {objectName}을 찾지 못했습니다."
            );
            return;
        }

        Button button =
            target.GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning(
                $"[UpgradeUI] {objectName}에 Button이 없습니다."
            );
            return;
        }

        map[fieldName] = button;
    }

    // =============================================
    // _attackLevelText
    // m_attackLevelText
    // 같은 이름도 처리
    // =============================================

    private static string NormalizeFieldName(
        string fieldName)
    {
        if (fieldName.StartsWith("m_"))
        {
            fieldName =
                fieldName.Substring(2);
        }

        while (fieldName.StartsWith("_"))
        {
            fieldName =
                fieldName.Substring(1);
        }

        return fieldName;
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