
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject debugPanel;

    // 테스트 빌드: true
    // 최종 배포 빌드: false
    private const bool AllowCheatsInBuild = true;

    private bool CanUseCheats =>
        Application.isEditor || AllowCheatsInBuild;

    private void Awake()
    {
        if (debugPanel == null)
            return;

        debugPanel.SetActive(false);

        // 최종 빌드에서는 치트 패널 자체를 제거
        if (!CanUseCheats)
        {
            Destroy(debugPanel);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!CanUseCheats || Keyboard.current == null)
            return;

        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (!CanUseCheats || debugPanel == null)
            return;

        debugPanel.SetActive(!debugPanel.activeSelf);
    }
}