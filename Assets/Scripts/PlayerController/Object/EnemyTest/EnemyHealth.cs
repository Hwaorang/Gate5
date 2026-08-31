using UnityEngine;

// 테스트용 Enemy 체력 스크립트
// 나중에 ObjectManager / ObjectPool이 완성되면
// Destroy 대신 풀로 반환하는 방식으로 변경 예정
public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]

    // 적의 최대 체력
    [SerializeField] private float maxHp = 30f;

    // 현재 체력
    private float currentHp;


    [Header("처치 카운트")]

    // 적 사망 시 처치 수를 증가시키기 위한 KillCounter
    private KillCounter killCounter;


    private void Awake()
    {
        // 게임 시작 시 최대 체력으로 초기화
        currentHp = maxHp;

        // 현재 씬에 있는 KillCounter를 찾아서 참조
        killCounter = FindFirstObjectByType<KillCounter>();
    }


    /// <summary>
    /// 적이 공격을 받았을 때 호출
    /// </summary>
    public void TakeDamage(float damage)
    {
        // 받은 데미지만큼 현재 체력 감소
        currentHp -= damage;

        // 체력이 0 이하가 되면 사망 처리
        if (currentHp <= 0f)
        {
            Die();
        }
    }


    /// <summary>
    /// 적 사망 처리
    /// </summary>
    private void Die()
    {
        // 적 처치 수 증가
        if (killCounter != null)
        {
            killCounter.AddKill();
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning(
                "KillCounter를 찾을 수 없습니다."
            );
        }
#endif

        // 현재는 테스트용이기 때문에 오브젝트 삭제
        // 추후 ObjectManager가 구현되면
        // Destroy 대신 ObjectPool로 반환할 예정
        Destroy(gameObject);
    }
}