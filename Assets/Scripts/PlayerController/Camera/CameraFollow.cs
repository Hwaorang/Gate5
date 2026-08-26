using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("추적 대상")]
    [SerializeField] private Transform target;

    [Header("카메라 위치")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -10f);

    [Header("추적 속도")]
    [SerializeField] private float followSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 플레이어 기준으로 카메라가 위치할 목표 지점
        Vector3 targetPosition = target.position + offset;

        // 부드럽게 플레이어를 따라간다.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // 플레이어 쪽을 바라보게 한다.
        transform.LookAt(target);
    }
}