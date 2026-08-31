using UnityEngine;

// 테스트용 Enemy 체력 스크립트
public class EnemyHealth : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float maxHp = 30f;

    [Header("경험치")]
    // 테스트용 경험치 보상
    // 나중에는 EnemyData 같은 ScriptableObject로 분리하는 것을 추천
    [SerializeField] private int expReward = 5;

    private float currentHp;

    // 플레이어 경험치 시스템 참조
    private PlayerExperience playerExperience;

    private void Awake()
    {
        currentHp = maxHp;

        // 현재 씬에서 PlayerExperience를 찾아 참조
        playerExperience =
            FindFirstObjectByType<PlayerExperience>();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // 적을 처치했을 때 경험치 지급
        if (playerExperience != null)
        {
            playerExperience.AddExp(expReward);

            Debug.Log($"EXP +{expReward}");
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning(
                "PlayerExperience를 찾을 수 없습니다."
            );
        }
#endif

        // 현재는 테스트용
        // ObjectPool 구현 후 풀 반환으로 변경 예정
        Destroy(gameObject);
    }
}