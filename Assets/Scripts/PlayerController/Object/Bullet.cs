using UnityEngine;

/// <summary>
/// 전방으로 이동하는 Bullet
/// 사용이 끝나면 Destroy하지 않고 BulletPool로 반환한다.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Bullet 설정")]

    // 총알 이동 속도
    [SerializeField]
    private float speed = 15f;

    // 최대 생존 시간
    [SerializeField]
    private float lifeTime = 3f;


    // 발사 방향
    private Vector3 direction;

    // 현재 총알의 데미지
    private float damage;

    // 현재 총알이 활성화된 시간
    private float lifeTimer;

    // 자신을 관리하는 BulletPool
    private BulletPool bulletPool;


    /// <summary>
    /// BulletPool에서 총알을 꺼낼 때 호출한다.
    /// </summary>
    public void Init(
        Vector3 fireDirection,
        float bulletDamage,
        BulletPool pool)
    {
        direction =
            fireDirection.normalized;

        damage =
            bulletDamage;

        bulletPool =
            pool;

        // 재사용될 때 타이머 초기화
        lifeTimer = 0f;
    }


    private void Update()
    {
        Move();
        CheckLifeTime();
    }


    /// <summary>
    /// 지정된 방향으로 Bullet 이동
    /// </summary>
    private void Move()
    {
        transform.position +=
            direction *
            speed *
            Time.deltaTime;
    }


    /// <summary>
    /// 일정 시간이 지나면 Pool로 반환
    /// </summary>
    private void CheckLifeTime()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            ReturnToPool();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // Enemy 충돌 확인
        EnemyHealth enemy =
            other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            ReturnToPool();

            return;
        }

        // Gate 기능을 다시 사용하게 된다면
        // 여기서 Gate 충돌도 추가하면 됨
    }


    /// <summary>
    /// Bullet을 삭제하지 않고 Pool로 반환
    /// </summary>
    private void ReturnToPool()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else
        {
            // Pool 참조가 없는 예외 상황
            gameObject.SetActive(false);
        }
    }
}