using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerRoot의 좌우 이동을 담당한다.
///
/// 실제 이동 범위는 StageChunk의
/// RoadLeftEdge / RoadRightEdge를 기준으로 계산한다.
///
/// StageChunk를 아직 찾지 못한 경우에만
/// 기존 xLimit 값을 fallback으로 사용한다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========================================================
    // Player Stats
    // ========================================================

    [Header("플레이어 스탯")]

    [SerializeField]
    private PlayerStats playerStats;


    // ========================================================
    // Road Area
    // ========================================================

    [Header("도로 이동 범위")]

    [Tooltip(
        "Player 중심이 도로 끝까지 붙지 않도록 확보할 여백. " +
        "Player 반폭 정도를 권장합니다."
    )]
    [SerializeField]
    [Min(0f)]
    private float edgeClearance = 0.5f;


    [Header("Fallback 이동 범위")]

    [Tooltip(
        "StageChunk의 RoadLeftEdge / RoadRightEdge를 아직 찾지 못했을 때만 사용합니다. " +
        "예: 4 -> -4 ~ +4"
    )]
    [SerializeField]
    [Min(0f)]
    private float xLimit = 4f;


    // Runtime에 생성되는 StageChunk를 한 번 찾은 뒤 캐싱한다.
    private StageChunk roadAreaSource;


    // ========================================================
    // Public Properties
    // ========================================================

    /// <summary>
    /// Player 중심이 도로 중앙을 기준으로
    /// 좌우로 이동할 수 있는 거리.
    ///
    /// 도로 폭 9 / edgeClearance 0.5
    /// -> XLimit = 4
    /// </summary>
    public float XLimit
    {
        get
        {
            if (TryGetRoadArea(
                    out _,
                    out float roadWidth))
            {
                return Mathf.Max(
                    0f,
                    roadWidth * 0.5f -
                    edgeClearance
                );
            }


            return xLimit;
        }
    }


    /// <summary>
    /// Gate가 채워야 하는 실제 도로 전체 폭.
    /// </summary>
    public float GateWidth
    {
        get
        {
            if (TryGetRoadArea(
                    out _,
                    out float roadWidth))
            {
                return roadWidth;
            }


            return xLimit * 2f;
        }
    }


    /// <summary>
    /// 실제 도로의 월드 X 중심.
    /// </summary>
    public float MovementCenterX
    {
        get
        {
            if (TryGetRoadArea(
                    out float centerX,
                    out _))
            {
                return centerX;
            }


            return 0f;
        }
    }


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats =
                GetComponent<PlayerStats>();
        }


        if (playerStats == null)
        {
            Debug.LogWarning(
                "[PlayerController] PlayerStats를 찾을 수 없습니다."
            );
        }
    }


    private void Update()
    {
        Move();
    }


    // ========================================================
    // Road Area
    // ========================================================

    /// <summary>
    /// 현재 StageChunk에 배치된
    /// RoadLeftEdge / RoadRightEdge를 기준으로
    /// 실제 도로 중심과 폭을 가져온다.
    ///
    /// PlayerRoot가 Runtime에 생성되기 때문에
    /// StageChunk 참조도 Runtime에 자동 탐색한다.
    /// </summary>
    public bool TryGetRoadArea(
        out float centerX,
        out float width)
    {
        centerX = 0f;
        width = 0f;


        // 아직 StageChunk를 찾지 않았거나
        // 기존 참조가 사라졌다면 다시 탐색한다.
        if (roadAreaSource == null)
        {
            roadAreaSource =
                Object.FindFirstObjectByType<StageChunk>();
        }


        if (roadAreaSource == null)
        {
            return false;
        }


        return roadAreaSource.TryGetRoadArea(
            out centerX,
            out width
        );
    }


    // ========================================================
    // Movement
    // ========================================================

    /// <summary>
    /// A / D 키 입력으로 PlayerRoot를 좌우 이동시킨다.
    ///
    /// Clamp 기준은 실제 도로 중심과 폭이다.
    /// </summary>
    private void Move()
    {
        if (playerStats == null)
        {
            return;
        }


        if (Keyboard.current == null)
        {
            return;
        }


        float horizontal = 0f;


        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }


        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }


        Vector3 direction =
            Vector3.right *
            horizontal *
            playerStats.MoveSpeed;


        transform.position +=
            direction *
            Time.deltaTime;


        Vector3 position =
            transform.position;


        float minX;
        float maxX;


        // =============================================
        // 실제 도로 기준 Clamp
        // =============================================

        if (TryGetRoadArea(
                out float roadCenterX,
                out float roadWidth))
        {
            float currentXLimit =
                Mathf.Max(
                    0f,
                    roadWidth * 0.5f -
                    edgeClearance
                );


            minX =
                roadCenterX -
                currentXLimit;


            maxX =
                roadCenterX +
                currentXLimit;
        }
        else
        {
            // StageChunk가 아직 준비되지 않은 경우에만
            // 기존 값으로 동작한다.
            minX =
                -xLimit;

            maxX =
                xLimit;
        }


        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX
            );


        transform.position =
            position;
    }
}
