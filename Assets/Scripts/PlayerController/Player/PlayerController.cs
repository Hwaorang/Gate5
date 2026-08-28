using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private PlayerStats playerStats;

    [Header("좌우 이동 범위")]
    [SerializeField] private float xLimit = 4f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = 0f;

        // A : 왼쪽
        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }

        // D : 오른쪽
        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }

        // 앞으로 자동 이동 + 좌우 입력
        Vector3 direction =
            Vector3.forward * forwardSpeed +
            Vector3.right * horizontal * playerStats.MoveSpeed;

        transform.position += direction * Time.deltaTime;

        // 좌우 이동 범위 제한
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            -xLimit,
            xLimit
        );

        transform.position = position;
    }
}