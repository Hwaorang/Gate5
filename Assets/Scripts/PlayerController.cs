using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float sideSpeed = 5f;
    [SerializeField] private float xLimit = 4f;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }

        Vector3 direction = Vector3.right * horizontal;

        transform.position += direction * sideSpeed * Time.deltaTime;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -xLimit, xLimit);
        transform.position = position;
    }
}