using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject debugPanel;

    private void Start()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            TogglePanel();
        }
#endif
    }

    public void TogglePanel()
    {
        if (debugPanel == null)
            return;

        debugPanel.SetActive(
            !debugPanel.activeSelf
        );
    }
}