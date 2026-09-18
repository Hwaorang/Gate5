using UnityEngine;

public class GameManager_KHM : MonoBehaviour
{
    public static GameManager_KHM Instance { get; private set; }

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

    [Header("Gold Reward")]
    [SerializeField] private float goldPerSecond = 2f;
    [SerializeField] private float goldMultiplierIncreasePerMinute = 0.1f;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
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

    // 생존 시간에 따른 골드 계산
    private int CalculateGoldReward()
    {
        // 1초당 2골드
        float baseGold = GameTime * goldPerSecond;

        // 60초마다 배율 +0.1
        int minutes = Mathf.FloorToInt(GameTime / 60f);

        float goldMultiplier =
            1f + minutes * goldMultiplierIncreasePerMinute;

        // 최종 골드
        int finalGold =
            Mathf.FloorToInt(baseGold * goldMultiplier);

        return finalGold;
    }

    // 게임 오버
    public void GameOver()
    {
        CurrentState = GameState.GameOver;

        int rewardGold = CalculateGoldReward();

        // 로비의 실제 보유 골드에 지급
        SaveManager.Instance.AddGold(rewardGold);

        Debug.Log($"게임 오버!");
        Debug.Log($"생존 시간 : {GameTime:F1}초");
        Debug.Log($"획득 골드 : {rewardGold}G");
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
    }
}