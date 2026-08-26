using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("좌우 이동")]
    [SerializeField] private float sideSpeed = 5f;

    [Header("이동 범위 제한")]
    [SerializeField] private float xLimit = 4f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // 좌우 입력값
        // A : -1
        // D : 1
        // 아무 입력 없음 : 0
        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }

        // X축 방향으로만 이동
        Vector3 direction = Vector3.right * horizontal;

        // 프레임에 상관없이 일정한 속도로 이동
        transform.position += direction * sideSpeed * Time.deltaTime;

        // 플레이어가 정해진 좌우 범위를 벗어나지 못하도록 제한
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            -xLimit,
            xLimit
        );

        transform.position = position;
    }
}