
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전체적인 흐름을 담당한다.
///
/// - GameOver
/// - Pause / Resume
/// - Retry
/// - Lobby 이동
/// - 게임 시간 관리
/// - 적 HP 배율 관리
/// - 생존 시간에 따른 골드 보상
///
/// UI를 직접 제어하지 않는다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }


    [Header("Scene")]

    [SerializeField]
    private string lobbySceneName = "LobbyScene";


    // =========================
    // 게임 상태
    // =========================

    private bool isGameOver;
    private bool isGameStarted;

    public bool IsGameOver =>
        isGameOver;

    public bool IsGameStarted =>
        isGameStarted;


    // =========================
    // 게임 시간
    // =========================

    /// <summary>
    /// 현재 게임의 경과 시간
    /// </summary>
    public float GameTime
    {
        get;
        private set;
    }


    // =========================
    // 적 HP 배율
    // =========================

    [Header("Enemy HP Scaling")]

    [SerializeField]
    private float hpIncreasePerMinute = 0.2f;


    /// <summary>
    /// 현재 적 HP 배율
    /// </summary>
    public float EnemyHpMultiplier
    {
        get;
        private set;
    } = 1f;


    // =========================
    // 골드 보상
    // =========================

    [Header("Gold Reward")]

    /// <summary>
    /// 1초당 기본 골드
    /// </summary>
    [SerializeField]
    private float goldPerSecond = 2f;


    /// <summary>
    /// 60초마다 증가하는 골드 배율
    /// </summary>
    [SerializeField]
    private float goldMultiplierIncreasePerMinute = 0.1f;


    private bool rewardGiven;


    // =========================
    // Singleton
    // =========================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================
    // Update
    // =========================

    private void Update()
    {
        // 게임이 시작되고 게임 오버가 아닐 때만
        // 게임 시간을 증가시킨다.
        if (isGameStarted &&
            !isGameOver)
        {
            UpdateGameTime();
            UpdateEnemyHpMultiplier();
        }
    }


    // =========================
    // 게임 시작
    // =========================

    /// <summary>
    /// 게임을 시작하고
    /// 게임 시간과 적 HP 배율을 초기화한다.
    /// </summary>
    public void StartGame()
    {
        isGameStarted = true;
        isGameOver = false;
        rewardGiven = false;

        GameTime = 0f;
        EnemyHpMultiplier = 1f;

        Time.timeScale = 1f;

        Debug.Log("게임 시작");
    }


    // =========================
    // 게임 시간 계산
    // =========================

    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }


    // =========================
    // 적 HP 배율 계산
    // =========================

    private void UpdateEnemyHpMultiplier()
    {
        int minutes =
            Mathf.FloorToInt(GameTime / 60f);

        EnemyHpMultiplier =
            1f + minutes * hpIncreasePerMinute;
    }


    // =========================
    // 골드 보상 계산
    // =========================

    private int CalculateGoldReward()
    {
        // 기본 골드
        // 예: 180초 × 2 = 360골드
        float baseGold =
            GameTime * goldPerSecond;


        // 60초마다 배율 +0.1
        //
        // 0~59초     = 1.0배
        // 60~119초   = 1.1배
        // 120~179초  = 1.2배
        // 180~239초  = 1.3배
        int minutes =
            Mathf.FloorToInt(GameTime / 60f);


        float goldMultiplier =
            1f +
            minutes * goldMultiplierIncreasePerMinute;


        // 최종 골드
        float finalGold =
            baseGold * goldMultiplier;


        return Mathf.FloorToInt(finalGold);
    }


    /// <summary>
    /// 현재 생존 시간을 기준으로
    /// 획득 가능한 골드를 반환한다.
    /// </summary>
    public int GetGoldReward()
    {
        return CalculateGoldReward();
    }


    // =========================
    // 게임 오버
    // =========================

    /// <summary>
    /// 게임 종료 상태로 변경한다.
    /// 실제 GameOver UI 표시는 GameResultPresenter가 담당한다.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;


        // =========================
        // 골드 보상
        // =========================

        if (!rewardGiven)
        {
            rewardGiven = true;

            int rewardGold =
                CalculateGoldReward();


            // SaveManager를 통해
            // PlayerData.gold에 골드를 추가하고 저장한다.
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AddGold(rewardGold);
            }


            Debug.Log("게임 오버!");
            Debug.Log($"생존 시간 : {GameTime:F1}초");
            Debug.Log($"획득 골드 : {rewardGold}G");
        }


        // 기존 GameManager의
        // 게임 오버 처리 유지
        Pause();
    }


    // =========================
    // 게임 일시정지
    // =========================

    /// <summary>
    /// 게임을 일시정지한다.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
    }


    // =========================
    // 게임 재개
    // =========================

    /// <summary>
    /// 게임을 다시 진행한다.
    /// </summary>
    public void Resume()
    {
        if (isGameOver)
        {
            return;
        }

        Time.timeScale = 1f;
    }


    // =========================
    // Retry
    // =========================

    /// <summary>
    /// 현재 게임 Scene을 다시 시작한다.
    /// </summary>
    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    // =========================
    // Lobby 이동
    // =========================

    /// <summary>
    /// Lobby Scene으로 이동한다.
    /// </summary>
    public void GoLobby()
    {
        // Scene 이동 전에
        // 일시정지 상태를 정상적으로 복구
        Time.timeScale = 1f;


        // =========================
        // InGame BGM 정지
        // =========================

        // AudioManager는 DontDestroyOnLoad이므로
        // Scene을 이동해도 살아있다.
        //
        // 따라서 Lobby로 넘어가기 전에
        // 현재 재생 중인 InGame BGM을 직접 정지한다.
        if (AudioManager_PlayerController.Instance != null)
        {
            AudioManager_PlayerController.Instance.StopBgm();
        }


        // 게임 플레이 중 Lobby로 돌아가는 것임을 기록
        LobbyNavigationContext
            .SetReturnFromGame();


        // =========================
        // Lobby Scene 이동
        // =========================

        SceneManager.LoadScene(
            lobbySceneName
        );
    }
}

