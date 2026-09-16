using UnityEngine;

/// <summary>
/// 생성된 GateChoiceGroup 전체를
/// 플레이어 방향으로 이동시킨다.
/// </summary>
public class GateMover : MonoBehaviour
{
    private Vector3 moveDirection;

    private float moveSpeed;
    private float maxTravelDistance;

    private float traveledDistance;

    private bool initialized;


    /// <summary>
    /// GateSpawnManager가 생성 직후 호출한다.
    /// </summary>
    public void Initialize(
        Vector3 direction,
        float speed,
        float travelDistance)
    {
        moveDirection =
            direction.normalized;

        moveSpeed =
            Mathf.Max(
                0f,
                speed
            );

        maxTravelDistance =
            Mathf.Max(
                1f,
                travelDistance
            );

        traveledDistance =
            0f;

        initialized =
            true;
    }


    private void Update()
    {
        if (!initialized)
        {
            return;
        }


        float moveAmount =
            moveSpeed *
            Time.deltaTime;


        transform.position +=
            moveDirection *
            moveAmount;


        traveledDistance +=
            moveAmount;


        // 플레이어를 지나 충분히 멀리 간 Gate 제거
        if (traveledDistance >=
            maxTravelDistance)
        {
            Destroy(
                gameObject
            );
        }
    }
}