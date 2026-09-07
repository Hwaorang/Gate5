using UnityEngine;

/// <summary>
/// 시각 연출용 Bullet.
///
/// 실제 데미지 판정은 SoldierAttack의 Raycast가 담당하고,
/// 이 클래스는 화면에서 총알이 날아가는 것처럼 보이게 하는 역할만 한다.
///
/// 역할
/// - 지정된 방향으로 이동
/// - 일정 시간이 지나면 BulletPool로 반환
///
/// Collider / Rigidbody는 사용하지 않는다.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]

    // 총알이 화면에 유지되는 최대 시간
    [SerializeField] private float lifeTime = 0.5f;


    // 현재 이동 방향
    private Vector3 direction;

    // 현재 이동 속도
    private float speed;

    // 활성화된 이후 흐른 시간
    private float lifeTimer;

    // 사용이 끝난 Bullet을 반환할 Pool
    private BulletPool bulletPool;


    /// <summary>
    /// Pool에서 Bullet을 가져왔을 때
    /// 이동에 필요한 값을 초기화한다.
    ///
    /// Object Pool에서 재사용되기 때문에
    /// lifeTimer도 반드시 0으로 초기화한다.
    /// </summary>
    public void InitVisual(
        Vector3 fireDirection,
        float bulletSpeed,
        BulletPool pool)
    {
        direction =
            fireDirection.normalized;

        speed =
            bulletSpeed;

        bulletPool =
            pool;

        lifeTimer =
            0f;
    }


    private void Update()
    {
        Move();
        CheckLifeTime();
    }


    /// <summary>
    /// 현재 설정된 방향과 속도로 이동한다.
    ///
    /// 물리 연산이 필요하지 않기 때문에
    /// Rigidbody 대신 Transform을 직접 이동시킨다.
    /// </summary>
    private void Move()
    {
        transform.position +=
            direction *
            speed *
            Time.deltaTime;
    }


    /// <summary>
    /// Bullet이 설정된 lifeTime 이상 활성화되어 있으면
    /// Object Pool로 반환한다.
    /// </summary>
    private void CheckLifeTime()
    {
        lifeTimer +=
            Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            ReturnToPool();
        }
    }


    /// <summary>
    /// Bullet 사용이 끝났을 때
    /// BulletPool로 반환한다.
    ///
    /// Pool 참조가 없는 예외 상황에서는
    /// 단순 비활성화 처리한다.
    /// </summary>
    private void ReturnToPool()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(
                gameObject
            );

            return;
        }

        gameObject.SetActive(
            false
        );
    }
}