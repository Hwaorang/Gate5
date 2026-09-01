using UnityEngine;

/// <summary>
/// 게이트를 플레이어 방향으로 이동시키는 스크립트
/// 플레이어가 앞으로 달리는 것처럼 보이도록
/// 게이트가 -Z 방향으로 이동한다.
/// </summary>
public class GateMover : MonoBehaviour
{
    [Header("게이트 이동 속도")]
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// 게이트를 -Z 방향으로 이동
    /// </summary>
    private void Move()
    {
        transform.position +=
            Vector3.back *
            moveSpeed *
            Time.deltaTime;
    }

    /// <summary>
    /// 외부에서 게이트 이동 속도를 변경할 때 사용
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
}