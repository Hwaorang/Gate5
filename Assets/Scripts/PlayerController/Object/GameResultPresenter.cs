using UnityEngine;

/// <summary>
/// 게임 종료 시 필요한 데이터를 수집하고
/// GameResultUI에 전달하는 중간 관리자.
///
/// 역할:
///
/// SquadManager
///     → GameOver 발생 알림
///
/// GameResultPresenter
///     → 생존 시간 / 레벨 수집
///
/// GameResultUI
///     → 화면에 표시
///
/// 현재는 Inspector 참조를 사용할 수 있고,
/// 이후 PlayerRoot가 동적으로 생성되면
/// PlayerContext를 통해 Player 관련 참조를 전달받는다.
/// </summary>
public class GameResultPresenter : MonoBehaviour
{
    [Header("GameOver 감지")]

    // GameOver 이벤트를 발생시키는 SquadManager
    // 현재 단계에서는 기존 Inspector 연결도 유지한다.
    [SerializeField]
    private SquadManager squadManager;


    [Header("결과 데이터")]

    // 플레이 시간을 가지고 있는 GameManager
    // PlayerRoot와 관계없는 Scene 시스템이므로 그대로 유지
    [SerializeField]
    private GameManager_KHM gameManager_KHM;

    [SerializeField]
    private GameManager gameManager;

    // 현재 Player의 레벨 정보
    [SerializeField]
    private PlayerExperience playerExperience;


    [Header("결과 UI")]

    // 결과를 실제 화면에 표시하는 UI
    [SerializeField]
    private GameResultUI gameResultUI;

    private void Awake()
    {
        ResolveSceneReferences();
    }

    private void OnEnable()
    {
        if (gameResultUI != null)
        {
            gameResultUI.OnRetryClicked +=
                HandleRetryClicked;
        }
    }


    private void OnDisable()
    {
        if (gameResultUI != null)
        {
            gameResultUI.OnRetryClicked -=
                HandleRetryClicked;
        }
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

    /// <summary>
    /// 동적으로 생성된 Player의 정보를 전달받는다.
    /// </summary>
    public void Initialize(PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[GameResultPresenter] PlayerContext가 없습니다."
            );

            return;
        }


        // =========================
        // 기존 Player 이벤트 해제
        // =========================

        if (squadManager != null)
        {
            squadManager.OnGameOver -= HandleGameOver;
        }


        // =========================
        // 새 Runtime Player 연결
        // =========================

        squadManager =
            context.SquadManager;

        playerExperience =
            context.PlayerExperience;


        // =========================
        // 새 Player 이벤트 등록
        // =========================

        if (squadManager != null)
        {
            squadManager.OnGameOver -= HandleGameOver;
            squadManager.OnGameOver += HandleGameOver;
        }
        else
        {
            Debug.LogWarning(
                "[GameResultPresenter] SquadManager를 찾지 못했습니다."
            );
        }
    }



    /// <summary>
    /// SquadManager의 GameOver 이벤트를 구독한다.
    /// </summary>
    private void SubscribeGameOver()
    {
        if (squadManager == null)
        {
            return;
        }

        // 중복 구독 방지
        squadManager.OnGameOver -=
            HandleGameOver;

        squadManager.OnGameOver +=
            HandleGameOver;
    }


    /// <summary>
    /// 현재 SquadManager의 GameOver 이벤트 구독을 해제한다.
    /// </summary>
    private void UnsubscribeGameOver()
    {
        if (squadManager == null)
        {
            return;
        }

        squadManager.OnGameOver -=
            HandleGameOver;
    }


    /// <summary>
    /// SquadManager에서 GameOver가 발생했을 때 호출된다.
    ///
    /// 필요한 데이터를 각 시스템에서 가져와
    /// GameResultData로 묶은 뒤
    /// GameResultUI에 전달한다.
    /// </summary>
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
        // Result Data 생성
        // =========================

        GameResultData resultData =
            new GameResultData(
                survivalTime,
                level
            );


        // =========================
        // UI 표시
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
        // 생존 시간 담당
        if (gameManager_KHM == null)
        {
            gameManager_KHM =
                FindFirstObjectByType<GameManager_KHM>();
        }

        // Retry 담당
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }
    }
}