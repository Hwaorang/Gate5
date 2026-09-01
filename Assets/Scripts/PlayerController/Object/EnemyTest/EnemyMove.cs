using UnityEngine;

/// <summary>
/// 몬스터가 자신의 라인을 유지하면서
/// 플레이어 방향으로 내려오도록 이동시킨다.
/// </summary>
public class EnemyMove : MonoBehaviour
{
    [Header("몬스터 이동 속도")]
    [SerializeField] private float moveSpeed = 6f;

    private void Update()
    {
        transform.position +=
            Vector3.back *
            moveSpeed *
            Time.deltaTime;
    }
}