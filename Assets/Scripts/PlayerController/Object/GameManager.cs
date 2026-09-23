```csharp
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
/// - 로비 골드 강화 적용
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

    /// <summary>
    /// 60초마다 증가하는 적 HP 배율.
    ///
    /// 0분 = x1.0
    /// 1분 = x1.2
    /// 2분 = x1.4
    /// 3분 = x1.6
    /// </summary>
    [SerializeField]
    private float hpIncreasePerMinute = 0.2f;


    /// <summary>
    /// 현재 적 HP 배율.
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
    /// 1초당 기본 골드.
    ///
    /// 기본값:
    /// 1초 = 2골드
    /// </summary>
    [SerializeField]
    private float goldPerSecond = 2f;


    /// <summary>
    /// 60초마다 증가하는 생존 시간 골드 배율.
    ///
    /// 0분 = x1.0
    /// 1분 = x1.1
    /// 2분 = x1.2
    /// 3분 = x1.3
    /// </summary>
    [SerializeField]
    private float goldMultiplierIncreasePerMinute = 0.1f;


    /// <summary>
    /// 로비 골드 강화 1레벨당
    /// 골드 획득량 증가율.
    ///
    /// 1레벨 = +10%
    /// 2레벨 = +20%
    /// 3레벨 = +30%
    /// </summary>
    [SerializeField]
    private float goldRewardIncreasePerLevel = 0.1f;


    /// <summary>
    /// 게임오버 보상을 이미 지급했는지 여부.
    ///
    /// GameOver가 여러 번 호출되어도
    /// 골드가 중복 지급되지 않도록 한다.
    /// </summary>
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
        // 게임이 시작되고
        // 게임오버가 아닐 때만 시간 진행
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
    /// 게임을 시작한다.
    ///
    /// 새로운 게임 시작 시
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
        // 60초마다 1분 증가
        int minutes =
            Mathf.FloorToInt(
                GameTime / 60f
            );


        // 기본 x1.0
        // 1분마다 +0.2
        EnemyHpMultiplier =
            1f +
            minutes * hpIncreasePerMinute;
    }


    // =========================
    // 골드 보상 계산
    // =========================

    private int CalculateGoldReward()
    {
        // =========================
        // 1. 기본 생존 골드
        // =========================

        // 예:
        // 180초 × 2골드
        // = 360골드
        float baseGold =
            GameTime * goldPerSecond;


        // =========================
        // 2. 생존 시간 골드 배율
        // =========================

        // 0~59초     = x1.0
        // 60~119초   = x1.1
        // 120~179초  = x1.2
        // 180~239초  = x1.3

        int minutes =
            Mathf.FloorToInt(
                GameTime / 60f
            );


        float timeGoldMultiplier =
            1f +
            minutes *
            goldMultiplierIncreasePerMinute;


        // =========================
        // 3. 생존 시간 보상
        // =========================

        float survivalGold =
            baseGold *
            timeGoldMultiplier;


        // =========================
        // 4. 로비 골드 강화
        // =========================

        int goldRewardLevel = 0;

        if (SaveManager.Instance != null &&
            SaveManager.Instance.Data != null)
        {
            goldRewardLevel =
                SaveManager.Instance
                    .Data
                    .goldRewardLevel;
        }


        // 로비 강화 1레벨 = +10%
        //
        // 0레벨 = x1.0
        // 1레벨 = x1.1
        // 2레벨 = x1.2
        // 3레벨 = x1.3

        float lobbyGoldMultiplier =
            1f +
            goldRewardLevel *
            goldRewardIncreasePerLevel;


        // =========================
        // 5. 최종 골드
        // =========================

        float finalGold =
            survivalGold *
            lobbyGoldMultiplier;


        return Mathf.FloorToInt(
            finalGold
        );
    }


    /// <summary>
    /// 현재 생존 시간과
    /// 로비 골드 강화 레벨을 기준으로
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
    ///
    /// 게임오버 시 생존 시간에 따른 골드를
    /// SaveManager를 통해 지급한다.
    ///
    /// 실제 GameOver UI 표시는
    /// GameResultPresenter가 담당한다.
    /// </summary>
    public void GameOver()
    {
        // 이미 게임오버 처리된 경우
        // 중복 처리하지 않는다.
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
                SaveManager.Instance.AddGold(
                    rewardGold
                );
            }


            Debug.Log("게임 오버!");

            Debug.Log(
                $"생존 시간 : " +
                $"{GameTime:F1}초"
            );


            if (SaveManager.Instance != null &&
                SaveManager.Instance.Data != null)
            {
                Debug.Log(
                    $"골드 강화 레벨 : " +
                    $"{SaveManager.Instance.Data.goldRewardLevel}"
                );
            }


            Debug.Log(
                $"획득 골드 : " +
                $"{rewardGold}G"
            );
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
```
