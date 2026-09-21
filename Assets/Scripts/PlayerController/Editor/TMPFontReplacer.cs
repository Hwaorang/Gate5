using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TMPFontReplacer : EditorWindow
{
    [Header("Font")]
    private TMP_FontAsset sourceFont;
    private TMP_FontAsset targetFont;

    [Header("Color")]
    private bool changeColor = true;
    private Color targetColor = Color.white;


    [MenuItem("Tools/UI/TMP Font Replacer")]
    public static void Open()
    {
        GetWindow<TMPFontReplacer>(
            "TMP Font Replacer"
        );
    }


    private void OnGUI()
    {
        GUILayout.Label(
            "TMP Font / Color 일괄 변경",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();


        // =========================
        // Font
        // =========================

        GUILayout.Label(
            "Font",
            EditorStyles.boldLabel
        );


        sourceFont =
            (TMP_FontAsset)EditorGUILayout.ObjectField(
                "기존 Font",
                sourceFont,
                typeof(TMP_FontAsset),
                false
            );


        targetFont =
            (TMP_FontAsset)EditorGUILayout.ObjectField(
                "변경 Font",
                targetFont,
                typeof(TMP_FontAsset),
                false
            );


        EditorGUILayout.Space();


        // =========================
        // Color
        // =========================

        GUILayout.Label(
            "Color",
            EditorStyles.boldLabel
        );


        changeColor =
            EditorGUILayout.Toggle(
                "색상도 변경",
                changeColor
            );


        if (changeColor)
        {
            targetColor =
                EditorGUILayout.ColorField(
                    "변경 Color",
                    targetColor
                );
        }


        EditorGUILayout.Space();


        EditorGUILayout.HelpBox(
            "Hierarchy에서 변경할 UI의 부모 오브젝트를 선택하세요.\n" +
            "기존 Font와 같은 TMP Text만 변경됩니다.",
            MessageType.Info
        );


        EditorGUILayout.Space();


        if (GUILayout.Button(
                "선택한 UI 아래 Font / Color 변경",
                GUILayout.Height(35f)
            ))
        {
            ReplaceFontAndColor();
        }
    }


    private void ReplaceFontAndColor()
    {
        GameObject root =
            Selection.activeGameObject;


        if (root == null)
        {
            Debug.LogWarning(
                "[TMPFontReplacer] " +
                "Hierarchy에서 UI Root를 선택해주세요."
            );

            return;
        }


        if (sourceFont == null)
        {
            Debug.LogWarning(
                "[TMPFontReplacer] " +
                "기존 Font를 연결해주세요."
            );

            return;
        }


        if (targetFont == null)
        {
            Debug.LogWarning(
                "[TMPFontReplacer] " +
                "변경 Font를 연결해주세요."
            );

            return;
        }


        TMP_Text[] texts =
            root.GetComponentsInChildren<TMP_Text>(
                true
            );


        int changedCount = 0;


        foreach (TMP_Text text in texts)
        {
            if (text == null)
            {
                continue;
            }


            // 지정한 기존 Font를 사용하는
            // TMP Text만 변경한다.
            if (text.font != sourceFont)
            {
                continue;
            }


            Undo.RecordObject(
                text,
                "Replace TMP Font And Color"
            );


            // Font 변경
            text.font =
                targetFont;


            // Color 변경
            if (changeColor)
            {
                text.color =
                    targetColor;
            }


            EditorUtility.SetDirty(
                text
            );


            PrefabUtility
                .RecordPrefabInstancePropertyModifications(
                    text
                );


            changedCount++;
        }


        if (!Application.isPlaying &&
            root.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(
                root.scene
            );
        }


        Debug.Log(
            $"[TMPFontReplacer] " +
            $"{changedCount}개의 TMP Text를 변경했습니다."
        );
    }
}