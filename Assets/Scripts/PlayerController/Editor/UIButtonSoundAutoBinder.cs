using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UIButtonSoundAutoBinder
{
    [MenuItem(
        "Tools/UI/Add Button Sounds To Selected"
    )]
    public static void AddButtonSounds()
    {
        GameObject selected =
            Selection.activeGameObject;


        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "Button Sound",
                "UI Root를 먼저 선택해주세요.",
                "확인"
            );

            return;
        }


        Button[] buttons =
            selected.GetComponentsInChildren<Button>(
                true
            );


        int addedCount = 0;


        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }


            // 이미 있으면 중복 추가하지 않는다.
            if (button.GetComponent<UIButtonSound>() != null)
            {
                continue;
            }


            Undo.AddComponent<UIButtonSound>(
                button.gameObject
            );


            addedCount++;
        }


        Debug.Log(
            $"[UIButtonSoundAutoBinder] " +
            $"{addedCount}개의 버튼에 " +
            $"UIButtonSound를 추가했습니다."
        );
    }
}