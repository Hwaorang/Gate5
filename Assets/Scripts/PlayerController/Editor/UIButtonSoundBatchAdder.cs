using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class UIButtonSoundBatchAdder
{
    [MenuItem("Tools/UI/Add Button Sounds")]
    private static void AddButtonSounds()
    {
        GameObject root =
            Selection.activeGameObject;

        if (root == null)
        {
            EditorUtility.DisplayDialog(
                "Button Sound",
                "Hierarchy에서 UI Root를 먼저 선택해주세요.",
                "확인"
            );

            return;
        }


        // 비활성화된 버튼까지 전부 찾는다.
        Button[] buttons =
            root.GetComponentsInChildren<Button>(
                true
            );


        if (buttons.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Button Sound",
                $"'{root.name}' 아래에서 Button을 찾지 못했습니다.",
                "확인"
            );

            return;
        }


        int addedCount = 0;
        int skippedCount = 0;


        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }


            // 이미 UIButtonSound가 있으면 중복 추가하지 않는다.
            if (button.GetComponent<UIButtonSound>() != null)
            {
                skippedCount++;

                continue;
            }


            Undo.AddComponent<UIButtonSound>(
                button.gameObject
            );

            addedCount++;
        }


        // Scene 변경사항 표시
        if (root.scene.IsValid())
        {
            EditorSceneManager.MarkSceneDirty(
                root.scene
            );
        }


        Debug.Log(
            $"[UIButtonSoundBatchAdder] 완료 | " +
            $"추가 : {addedCount} / " +
            $"기존 존재 : {skippedCount}"
        );


        EditorUtility.DisplayDialog(
            "Button Sound",
            $"완료했습니다.\n\n" +
            $"UIButtonSound 추가 : {addedCount}개\n" +
            $"이미 있어서 건너뜀 : {skippedCount}개",
            "확인"
        );
    }
}