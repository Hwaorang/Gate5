using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lobby 전용 Cheat UI.
///
/// 담당 역할
/// - F11로 치트창 On / Off
/// - 골드 추가
/// - 골드 초기화
/// - 업그레이드 초기화
///
/// Lobby Scene에만 존재하며
/// DontDestroyOnLoad를 사용하지 않는다.
/// </summary>
public class LobbyCheatUI : MonoBehaviour
{
    // =========================
    // UI
    // =========================

    [Header("Cheat UI")]

    [SerializeField]
    private GameObject cheatPanel;


    // =========================
    // Cheat 설정
    // =========================

    [Header("Gold Cheat")]

    [SerializeField]
    private int addGoldAmount = 10000;


    // =========================
    // 초기화
    // =========================

    private void Awake()
    {
#if !UNITY_EDITOR
    if (cheatPanel != null)
    {
        cheatPanel.SetActive(false);
    }

    enabled = false;
#endif
    }

    private void Start()
    {
        if (cheatPanel != null)
        {
            cheatPanel.SetActive(false);
        }
    }


    // =========================
    // 입력
    // =========================

    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            ToggleCheatPanel();
        }
#endif
    }


    // =========================
    // Panel
    // =========================

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


    // =========================
    // Gold Add
    // =========================

    public void AddGold()
    {
#if !UNITY_EDITOR
    return;
#endif

        if (!LobbyCheatDataBridge.AddGold(
                addGoldAmount))
        {
            return;
        }

        Debug.Log(
            $"[Cheat] Gold +{addGoldAmount}"
        );
    }


    // =========================
    // Gold Reset
    // =========================

    public void ResetGold()
    {
        if (!LobbyCheatDataBridge.ResetGold())
        {
            return;
        }

        Debug.Log(
            "[Cheat] Gold Reset"
        );
    }


    // =========================
    // Upgrade Reset
    // =========================

    public void ResetUpgrades()
    {
        if (!LobbyCheatDataBridge.ResetUpgrades())
        {
            return;
        }

        // 비활성화된 UpgradePanel 안의
        // UpgradeManager까지 찾아서 UI 갱신
        UpgradeManager upgradeManager =
            FindFirstObjectByType<UpgradeManager>(
                FindObjectsInactive.Include
            );

        if (upgradeManager != null)
        {
            upgradeManager.RefreshUI();
        }

#if UNITY_EDITOR
        Debug.Log(
            "[Cheat] Upgrade Reset 완료"
        );
#endif
    }
}