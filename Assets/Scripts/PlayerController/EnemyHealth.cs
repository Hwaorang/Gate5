using UnityEngine;
//테스트용입니다.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHp = 30f;

    private float currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        Debug.Log($"{gameObject.name} HP : {currentHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망");

        Destroy(gameObject);
    }
}