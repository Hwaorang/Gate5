using UnityEngine;

public class GameManager_KHM : MonoBehaviour
{
    public static GameManager_KHM Instance { get; private set; }

    [Header("Game Time")]
    public float gameTime;

    [Header("Enemy Scaling")]
    public float enemyHpMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        UpdateEnemyHpMultiplier();
    }

    private void UpdateEnemyHpMultiplier()
    {
        // 1분마다 HP 20% 증가
        enemyHpMultiplier = 1f + Mathf.Floor(gameTime / 60f) * 0.2f;
    }
}