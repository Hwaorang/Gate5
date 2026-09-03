using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TinyGiantStudio.EditorHelpers
{
    /// <summary>
    /// This helps draw common stuff shared by editor scripts in a uniform manner for IMGUI
    /// </summary>
    public static class MText_Editor_Methods
    {
        private static readonly float defaultTinyHorizontalFieldSize = 50f;
        private static readonly float defaultSmallHorizontalFieldSize = 72.5f;
        private static readonly float defaultNormalltHorizontalFieldSize = 100;
        private static readonly float defaultLargeHorizontalFieldSize = 120f;
        private static readonly float defaultExtraLargeHorizontalFieldSize = 155f;
        private static readonly float defaultGiganticHorizontalFieldSize = 220;
        private static readonly float defaultMegaHorizontalFieldSize = 300;

        private static GUIStyle defaultLabel;
        private static GUIStyle defaultMultilineLabel;

        public static void HorizontalField(SerializedProperty property, string label, string toolTip = "", FieldSize fieldSize = FieldSize.normal)
        {
            if (property == null)
                return;

            float myMaxWidth = GetMyMaxWidth(fieldSize);
            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = myMaxWidth;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = new GUIContent(label, toolTip);
            EditorGUILayout.PropertyField(property, gUIContent);
            GUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = defaultWidth;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="toolTip"></param>
        /// <param name="fieldSize"></param>
        /// <param name="applySizeToPropertyField">Reduces total width taken by the property</param>
        public static void ItalicHorizontalField(SerializedProperty property, string label, string toolTip = "", FieldSize fieldSize = FieldSize.normal, bool applySizeToPropertyField = false)
        {
            if (property == null)
                return;

            GenerateStyle();

            float myMaxWidth = GetMyMaxWidth(fieldSize);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, toolTip), defaultLabel, GUILayout.MaxWidth(myMaxWidth));
            if (applySizeToPropertyField)
                EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.MaxWidth(myMaxWidth / 2));
            else
                EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="toolTip"></param>
        /// <param name="fieldSize"></param>
        /// <param name="applySizeToPropertyField">Reduces total width taken by the property</param>
        public static void DisabledItalicHorizontalField(SerializedProperty property, string label, string toolTip = "", FieldSize fieldSize = FieldSize.normal)
        {
            if (property == null)
                return;

            GenerateStyle();

            float myMaxWidth = GetMyMaxWidth(fieldSize);
           GUI.enabled = false;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, toolTip), defaultLabel, GUILayout.MaxWidth(myMaxWidth));
            EditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }

        public static void PreviewField(SerializedProperty property, Object targetObject, string label, string toolTip = "")
        {
            if (property == null) return;

            GenerateStyle();

            if (targetObject)
            {
                //Texture2D texture = AssetPreview.GetAssetPreview(targetObject);
                GUILayout.Box(AssetPreview.GetAssetPreview(targetObject), GUIStyle.none, GUILayout.MaxWidth(40), GUILayout.MaxHeight(40));
            }

            try
            {
                GUILayout.BeginVertical();
                GUIContent content = new GUIContent(label, toolTip);
                float minWidth = defaultMultilineLabel.CalcSize(content).x;
                EditorGUILayout.ObjectField(property, new GUIContent(""), GUILayout.MinWidth(minWidth));
                EditorGUILayout.LabelField(content, defaultMultilineLabel);
                GUILayout.EndVertical();
            }
            catch
            {
                //Debug.Log("Error " + property.ToString());
            }
        }

        private static float GetMyMaxWidth(FieldSize fieldSize)
        {
            return fieldSize == FieldSize.tiny ? defaultTinyHorizontalFieldSize : fieldSize == FieldSize.small ? defaultSmallHorizontalFieldSize : fieldSize == FieldSize.normal ? defaultNormalltHorizontalFieldSize : fieldSize == FieldSize.large ? defaultLargeHorizontalFieldSize : fieldSize == FieldSize.extraLarge ? defaultExtraLargeHorizontalFieldSize : fieldSize == FieldSize.gigantic ? defaultGiganticHorizontalFieldSize : fieldSize == FieldSize.mega ? defaultMegaHorizontalFieldSize : defaultNormalltHorizontalFieldSize;
        }

        private static void GenerateStyle()
        {
            if (defaultMultilineLabel == null)
            {
                defaultMultilineLabel = new GUIStyle(EditorStyles.wordWrappedLabel)
                {
                    fontSize = 10,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter,
                };
                if (EditorGUIUtility.isProSkin)
                    defaultMultilineLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
                else
                    defaultMultilineLabel.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);
            }
            if (defaultLabel == null)
            {
                defaultLabel = new GUIStyle(EditorStyles.whiteMiniLabel)
                {
                    //fontStyle = FontStyle.Italic,
                    fontSize = 12
                };
                if (EditorGUIUtility.isProSkin)
                    defaultLabel.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
                else
                    defaultLabel.normal.textColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);
            }
        }

        private delegate bool DelegateExecuteMenuItemWithTemporaryContext(string menuItemPath, UnityEngine.Object[] objects);

        private static DelegateExecuteMenuItemWithTemporaryContext ExecuteMenuItemWithTemporaryContext;

        public static void RemoveRectTransform(this GameObject gameObject)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (ExecuteMenuItemWithTemporaryContext == null)
                {
                    ExecuteMenuItemWithTemporaryContext = typeof(EditorApplication).GetMethod("ExecuteMenuItemWithTemporaryContext", BindingFlags.Static | BindingFlags.NonPublic)
                        .CreateDelegate(typeof(DelegateExecuteMenuItemWithTemporaryContext)) as DelegateExecuteMenuItemWithTemporaryContext;
                }
                ExecuteMenuItemWithTemporaryContext("CONTEXT/Component/Remove Component", new UnityEngine.Object[] { rectTransform });
            }
        }
    }
}