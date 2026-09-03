using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TinyGiantStudio.Modules
{
    [CustomEditor(typeof(ModuleApplier))]
    public class ModuleApplierEditor : Editor
    {
        private ModuleApplier myTarget;
        private SerializedObject soTarget;
        SerializedProperty targetObject;
        SerializedProperty modules;
        SerializedProperty applyModule;

        public override void OnInspectorGUI()
        {
            myTarget = (ModuleApplier)target;
            soTarget = new SerializedObject(target);
            targetObject = soTarget.FindProperty(nameof(ModuleApplier.moduleTarget));
            modules = soTarget.FindProperty(nameof(ModuleApplier.modules));
            applyModule = soTarget.FindProperty(nameof(ModuleApplier.applyModules));

            soTarget.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(targetObject);

            ModuleDrawer.BaseModuleContainerList("Modules", "Modules that can be applied. Note that module always have to be applied to enabled game object.", myTarget.modules, modules, soTarget, applyModule);

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();

                EditorUtility.SetDirty(myTarget);
            }
        }
    }
}