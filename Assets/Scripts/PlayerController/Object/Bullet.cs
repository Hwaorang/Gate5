using UnityEngine;

/// <summary>
/// 시각 연출용 Bullet.
///
/// 역할:
/// 1. 지정된 방향으로 빠르게 이동
/// 2. 일정 시간이 지나면 BulletPool로 반환
///
/// 실제 데미지 판정은 SoldierAttack의 Raycast가 담당한다.
/// 따라서 Bullet에는 Collider / Rigidbody가 필요하지 않는다.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]

    // 총알이 화면에 유지되는 시간
    // 시간이 지나면 자동으로 Pool에 반환된다.
    [SerializeField] private float lifeTime = 0.5f;


    // 현재 총알이 이동할 방향
    private Vector3 direction;

    // 현재 총알 이동 속도
    private float speed;

    // 총알이 활성화된 이후 흐른 시간
    private float lifeTimer;

    // 사용이 끝난 총알을 반환할 Object Pool
    private BulletPool bulletPool;


    /// <summary>
    /// BulletPool에서 총알을 꺼냈을 때
    /// 이동 방향과 속도 등을 초기화한다.
    ///
    /// 실제 공격 판정은 이미 Raycast로 처리되었기 때문에
    /// Bullet은 시각적으로 이동하는 역할만 담당한다.
    /// </summary>
    public void InitVisual(
        Vector3 fireDirection,
        float bulletSpeed,
        BulletPool pool)
    {
        // 전달받은 발사 방향을 정규화해서 저장
        direction = fireDirection.normalized;

        // 현재 총알 이동 속도 설정
        speed = bulletSpeed;

        // 사용이 끝났을 때 반환할 Pool 저장
        bulletPool = pool;

        // Pool에서 재사용되는 총알이므로
        // 생존 시간을 반드시 초기화한다.
        lifeTimer = 0f;
    }


    private void Update()
    {
        Move();
        CheckLifeTime();
    }


    /// <summary>
    /// 현재 지정된 방향으로 총알을 이동시킨다.
    ///
    /// Rigidbody를 사용하지 않고
    /// Transform을 직접 이동시킨다.
    /// </summary>
    private void Move()
    {
        transform.position +=
            direction *
            speed *
            Time.deltaTime;
    }


    /// <summary>
    /// 총알의 생존 시간을 확인한다.
    ///
    /// 설정된 lifeTime 이상 살아있었다면
    /// BulletPool로 반환한다.
    /// </summary>
    private void CheckLifeTime()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            ReturnToPool();
        }
    }


    /// <summary>
    /// 사용이 끝난 총알을 Object Pool로 반환한다.
    ///
    /// BulletPool이 연결되지 않은 예외 상황에서는
    /// 단순 비활성화 처리한다.
    /// </summary>
    private void ReturnToPool()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnBullet(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}