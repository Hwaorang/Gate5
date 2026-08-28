using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 3f;

    private Transform target;
    private float damage;

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 적이 이미 죽었거나 사라졌으면 총알 제거
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 적 방향 계산
        Vector3 direction =
            (target.position - transform.position).normalized;

        // 적을 향해 이동
        transform.position +=
            direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        //이 함수는 추후 Enemy담당자분께 여쭈어봐서 해결할것
        EnemyHealth enemyHealth =
            other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            return;
        }

        // 적에게 데미지 전달
        enemyHealth.TakeDamage(damage);

        // 총알 제거
        Destroy(gameObject);
    }
}