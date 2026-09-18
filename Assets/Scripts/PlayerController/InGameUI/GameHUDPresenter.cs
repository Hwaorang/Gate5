using UnityEngine;

/// <summary>
/// Player / GameManager / UpgradeManager의 데이터를
/// GameHUDUI에 전달한다.
///
/// 이 오브젝트는 HUD가 꺼져도 항상 활성화되어 있어야 한다.
/// </summary>
public class GameHUDPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField]
    private GameHUDUI hudUI;


    private PlayerExperience playerExperience;

    // GameManager_KHM → GameManager
    private GameManager gameManager;

    private UpgradeManager_PlayerController upgradeManager;


    private int lastLevel = -1;


    private void Awake()
    {
        ResolveHUD();
        ResolveGameManager();
    }


    /// <summary>
    /// PlayerRoot가 생성된 뒤 Bootstrapper에서 호출한다.
    /// </summary>
    public void Initialize(
        PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[GameHUDPresenter] PlayerContext가 없습니다."
            );

            return;
        }

        playerExperience =
            context.PlayerExperience;

        ResolveHUD();
        ResolveGameManager();

        RefreshHUD();
    }


    /// <summary>
    /// UpgradeManager 연결.
    /// </summary>
    public void SetUpgradeManager(
        UpgradeManager_PlayerController manager)
    {
        // 기존 이벤트 해제
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeApplied -=
                HandleUpgradeApplied;
        }

        upgradeManager =
            manager;

        // 새로운 이벤트 연결
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeApplied +=
                HandleUpgradeApplied;
        }
    }


    private void Update()
    {
        if (hudUI == null)
        {
            return;
        }

        UpdateTimer();
        UpdateLevel();
    }


    private void ResolveHUD()
    {
        if (hudUI != null)
        {
            return;
        }

        // 비활성화된 HUD도 찾을 수 있도록 true
        GameUIRoot root =
            GetComponentInParent<GameUIRoot>();

        if (root != null)
        {
            hudUI =
                root.GetComponentInChildren<GameHUDUI>(
                    true
                );
        }
    }


    private void ResolveGameManager()
    {
        if (gameManager != null)
        {
            return;
        }

        // 기존 GameManager_KHM → 통합된 GameManager
        if (GameManager.Instance != null)
        {
            gameManager =
                GameManager.Instance;

            return;
        }

        gameManager =
            FindFirstObjectByType<GameManager>();
    }


    private void UpdateTimer()
    {
        // Scene 변경 등으로 Manager가 아직 없으면 다시 탐색
        if (gameManager == null)
        {
            ResolveGameManager();
        }

        if (gameManager == null)
        {
            return;
        }

        hudUI.SetTime(
            gameManager.GameTime
        );
    }


    private void UpdateLevel()
    {
        if (playerExperience == null)
        {
            return;
        }

        int currentLevel =
            playerExperience.CurrentLevel;

        if (currentLevel == lastLevel)
        {
            return;
        }

        lastLevel =
            currentLevel;

        hudUI.SetLevel(
            currentLevel
        );
    }


    private void HandleUpgradeApplied(
        UpgradeType type,
        int level)
    {
        if (hudUI == null)
        {
            return;
        }

        switch (type)
        {
            case UpgradeType.Damage:

                hudUI.SetDamageLevel(
                    level
                );

                break;


            case UpgradeType.AttackSpeed:

                hudUI.SetAttackSpeedLevel(
                    level
                );

                break;


            case UpgradeType.ProjectileCount:

                hudUI.SetProjectileLevel(
                    level
                );

                break;


            case UpgradeType.MoveSpeed:

                hudUI.SetMoveSpeedLevel(
                    level
                );

                break;
        }
    }


    private void RefreshHUD()
    {
        if (hudUI == null)
        {
            Debug.LogWarning(
                "[GameHUDPresenter] GameHUDUI를 찾을 수 없습니다."
            );

            return;
        }

        // Level 강제 갱신
        lastLevel = -1;

        UpdateLevel();
        UpdateTimer();

        // 게임 시작 시 Skill HUD 숨김
        hudUI.SetDamageLevel(0);
        hudUI.SetAttackSpeedLevel(0);
        hudUI.SetProjectileLevel(0);
        hudUI.SetMoveSpeedLevel(0);
    }


    private void OnDestroy()
    {
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeApplied -=
                HandleUpgradeApplied;
        }
    }
}
