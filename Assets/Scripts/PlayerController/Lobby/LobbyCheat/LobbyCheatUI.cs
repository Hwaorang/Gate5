using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyCheatUI : MonoBehaviour
{
    [Header("Cheat UI")]
    [SerializeField]
    private GameObject cheatPanel;

    [Header("Gold Cheat")]
    [SerializeField]
    private int addGoldAmount = 10000;


    private void Start()
    {
        if (cheatPanel != null)
        {
            cheatPanel.SetActive(false);
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            ToggleCheatPanel();
        }
    }


    private void ToggleCheatPanel()
    {
        if (cheatPanel == null)
        {
            return;
        }

        cheatPanel.SetActive(
            !cheatPanel.activeSelf
        );
    }


    public void AddGold()
    {
        if (!LobbyCheatDataBridge.AddGold(
                addGoldAmount))
        {
            return;
        }

        Debug.Log(
            $"[Cheat] Gold +{addGoldAmount}"
        );
    }
}