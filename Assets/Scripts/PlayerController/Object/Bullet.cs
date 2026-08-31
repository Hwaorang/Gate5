using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    private Vector3 direction;
    private float damage;

    /// <summary>
    /// 총알 생성 시 이동 방향과 데미지를 전달받는다.
    /// </summary>
    public void Init(Vector3 fireDirection, float bulletDamage)
    {
        direction = fireDirection.normalized;
        damage = bulletDamage;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 지정된 방향으로 계속 직진
        transform.position +=
            direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy =
            other.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}