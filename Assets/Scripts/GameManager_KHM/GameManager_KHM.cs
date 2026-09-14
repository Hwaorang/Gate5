using UnityEngine;

public class GameManager_KHM : MonoBehaviour
{
    public static GameManager_KHM Instance { get; private set; }

<<<<<<< Updated upstream
    [Header("Game Time")]
    public float gameTime;

    [Header("Enemy Scaling")]
    public float enemyHpMultiplier = 1f;

    // 외부에서는 게임 시간을 읽기만 가능
    public float GameTime => gameTime;

    private void Awake()
    {
=======
    // 현재 게임 상태
    public enum GameState
    {
        Ready,
        Playing,
        GameOver,
        Pause
    }

    public GameState CurrentState { get; private set; }

    // 게임 경과 시간
    public float GameTime { get; private set; }

    // 적 HP 배율
    public float EnemyHpMultiplier { get; private set; } = 1f;

    [Header("Enemy HP Scaling")]
    [SerializeField] private float hpIncreasePerMinute = 0.2f;

    private void Awake()
    {
        // Singleton
>>>>>>> Stashed changes
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
<<<<<<< Updated upstream
=======
            return;
>>>>>>> Stashed changes
        }
    }

    private void Update()
    {
<<<<<<< Updated upstream
        gameTime += Time.deltaTime;

        UpdateEnemyHpMultiplier();
    }

    private void UpdateEnemyHpMultiplier()
    {
        // 1분마다 HP 20% 증가
        enemyHpMultiplier = 1f + Mathf.Floor(gameTime / 60f) * 0.2f;
=======
        // 게임 중일 때만 시간 흐름
        if (CurrentState == GameState.Playing)
        {
            UpdateGameTime();
            UpdateEnemyHpMultiplier();
        }
    }

    // 게임 시작
    public void StartGame()
    {
        GameTime = 0f;
        EnemyHpMultiplier = 1f;

        CurrentState = GameState.Playing;

        Debug.Log("게임 시작");
    }

    // 게임 시간 계산
    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    // 시간에 따른 적 HP 배율 계산
    private void UpdateEnemyHpMultiplier()
    {
        int minutes = Mathf.FloorToInt(GameTime / 60f);

        EnemyHpMultiplier =
            1f + minutes * hpIncreasePerMinute;
    }

    // 게임 오버
    public void GameOver()
    {
        CurrentState = GameState.GameOver;

        Debug.Log("게임 오버");
    }

    // 게임 일시정지
    public void PauseGame()
    {
        CurrentState = GameState.Pause;
    }

    // 게임 재개
    public void ResumeGame()
    {
        CurrentState = GameState.Playing;
>>>>>>> Stashed changes
    }
}