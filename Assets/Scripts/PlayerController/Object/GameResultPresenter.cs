using UnityEngine;

/// <summary>
/// 게임 종료 시 필요한 데이터를 수집하고
/// GameResultUI에 전달하는 중간 관리자.
///
/// 생존 시간의 원본 데이터는
/// GameManager_KHM.GameTime 하나만 사용한다.
/// </summary>
public class GameResultPresenter : MonoBehaviour
{
    [Header("GameOver 감지")]
    [SerializeField]
    private SquadManager squadManager;


    [Header("결과 데이터")]

    [SerializeField]
    private GameManager_KHM gameManager_KHM;

    // Retry / Lobby만 담당
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private PlayerExperience playerExperience;


    [Header("결과 UI")]

    [SerializeField]
    private GameResultUI gameResultUI;

    private bool hasShownGameOver;


    private void Awake()
    {
        ResolveSceneReferences();

        if (gameResultUI != null)
        {
            gameResultUI.Hide();
        }
    }


    private void OnEnable()
    {
        if (gameResultUI != null)
        {
            gameResultUI.OnRetryClicked +=
                HandleRetryClicked;

            gameResultUI.OnLobbyClicked +=
                HandleLobbyClicked;
        }
    }


    private void OnDisable()
    {
        if (gameResultUI != null)
        {
            gameResultUI.OnRetryClicked -=
                HandleRetryClicked;

            gameResultUI.OnLobbyClicked -=
                HandleLobbyClicked;
        }

        UnsubscribeGameOver();
    }


    private void HandleRetryClicked()
    {
        if (gameManager == null)
        {
            Debug.LogWarning(
                "[GameResultPresenter] GameManager가 없습니다."
            );

            return;
        }

        gameManager.Retry();
    }


    private void HandleLobbyClicked()
    {
        if (gameManager == null)
        {
            Debug.LogWarning(
                "[GameResultPresenter] GameManager가 없습니다."
            );

            return;
        }

        gameManager.GoLobby();
    }


    /// <summary>
    /// 동적으로 생성된 Player의 정보를 전달받는다.
    /// </summary>
    public void Initialize(
        PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[GameResultPresenter] PlayerContext가 없습니다."
            );

            return;
        }


        if (squadManager != null)
        {
            squadManager.OnGameOver -=
                HandleGameOver;
        }


        squadManager =
            context.SquadManager;

        playerExperience =
            context.PlayerExperience;


        SubscribeGameOver();
    }


    private void SubscribeGameOver()
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.OnGameOver -=
            HandleGameOver;

        squadManager.OnGameOver +=
            HandleGameOver;
    }


    private void UnsubscribeGameOver()
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.OnGameOver -=
            HandleGameOver;
    }


    private void HandleGameOver()
    {
        // =========================
        // Level
        // =========================

        int level = 1;

        if (playerExperience != null)
        {
            level =
                playerExperience.CurrentLevel;
        }


        // =========================
        // Survival Time
        // =========================

        float survivalTime = 0f;

        if (gameManager_KHM != null)
        {
            survivalTime =
                gameManager_KHM.GameTime;
        }


        // =========================
        // GameOver Audio
        // =========================

        AudioManager_PlayerController audio =
            AudioManager_PlayerController.Instance;

        if (audio != null)
        {
            // 인게임 BGM 정지
            audio.StopBgm();

            // GameOver 효과음 재생
            audio.PlaySfx(
                PlayerSfxType.GameOver
            );
        }


        // =========================
        // Result Data
        // =========================

        GameResultData resultData =
            new GameResultData(
                survivalTime,
                level
            );


        // =========================
        // GameOver UI 표시
        // =========================

        if (gameResultUI != null)
        {
            gameResultUI.Show(
                resultData
            );
        }
    }


    private void ResolveSceneReferences()
    {
        if (gameManager_KHM == null)
        {
            gameManager_KHM =
                FindFirstObjectByType<GameManager_KHM>();
        }

        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }
    }
}
