using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 좌우 이동만 담당한다.
/// 앞으로 이동하지 않고 X축으로만 움직인다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private PlayerStats playerStats;

    [Header("좌우 이동 범위")]
    [SerializeField] private float xLimit = 4f;

    public float XLimit => xLimit;

    private void Update()
    {
        Move();
    }

    /// <summary>
    /// A / D 키를 이용해서 좌우로만 이동한다.
    /// </summary>
    private void Move()
    {
        float horizontal = 0f;

        // A키 : 왼쪽
        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }

        // D키 : 오른쪽
        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }

        // 좌우 방향만 계산한다.
        Vector3 direction =
            Vector3.right *
            horizontal *
            playerStats.MoveSpeed;

        // 실제 이동
        transform.position +=
            direction *
            Time.deltaTime;

        // 현재 위치 가져오기
        Vector3 position =
            transform.position;

        // 플레이어가 지정된 좌우 범위를 벗어나지 않게 제한
        position.x = Mathf.Clamp(
            position.x,
            -xLimit,
            xLimit
        );

        transform.position = position;
    }
}