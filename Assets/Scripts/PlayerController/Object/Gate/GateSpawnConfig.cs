using UnityEngine;

/// <summary>
/// Gate 출현 규칙을 저장한다.
///
/// Scene에 종속되지 않기 때문에
/// 여러 Scene에서 같은 설정을 재사용할 수 있다.
/// </summary>
[CreateAssetMenu(
    fileName = "GateSpawnConfig",
    menuName = "Game/Gate/Gate Spawn Config"
)]
public class GateSpawnConfig : ScriptableObject
{
    [Header("Spawn Timing")]

    [Tooltip("게임 시작 후 첫 Gate가 등장하기까지의 시간")]
    [SerializeField]
    [Min(0f)]
    private float initialDelay = 5f;


    [Tooltip("Gate 최소 등장 간격")]
    [SerializeField]
    [Min(0.1f)]
    private float minSpawnInterval = 8f;


    [Tooltip("Gate 최대 등장 간격")]
    [SerializeField]
    [Min(0.1f)]
    private float maxSpawnInterval = 12f;

    [Header("Movement")]

    [SerializeField]
    [Min(0f)]
    private float moveSpeed =
    5f;


    [SerializeField]
    [Min(1f)]
    private float maxTravelDistance =
        60f;


    public float MoveSpeed =>
        moveSpeed;


    public float MaxTravelDistance =>
        maxTravelDistance;


    public float InitialDelay =>
        initialDelay;


    public float GetRandomSpawnInterval()
    {
        float min =
            Mathf.Min(
                minSpawnInterval,
                maxSpawnInterval
            );


        float max =
            Mathf.Max(
                minSpawnInterval,
                maxSpawnInterval
            );


        return Random.Range(
            min,
            max
        );
    }
}