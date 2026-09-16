using UnityEngine;

/// <summary>
/// Player의 좌우 이동 가능 폭을 기준으로
/// Left / Right Gate를 자동 배치하고 크기를 조절한다.
/// </summary>
public class GateGroupLayout : MonoBehaviour
{
    [Header("Gate")]

    [SerializeField]
    private GateSizeController leftGate;

    [SerializeField]
    private GateSizeController rightGate;


    [Header("Layout")]

    [Tooltip("Player 이동 범위 양 끝에서 확보할 여백")]
    [SerializeField]
    [Min(0f)]
    private float sidePadding = 0.1f;


    [Tooltip("두 Gate 사이의 간격")]
    [SerializeField]
    [Min(0f)]
    private float centerGap = 0.15f;


    [Tooltip("Gate 높이")]
    [SerializeField]
    [Min(1f)]
    private float gateHeight = 4f;


    private void Awake()
    {
        ResolveGates();
    }


    /// <summary>
    /// Inspector 연결이 없어도
    /// 자식 GateSizeController를 자동으로 찾는다.
    /// </summary>
    private void ResolveGates()
    {
        if (leftGate != null &&
            rightGate != null)
        {
            return;
        }


        GateSizeController[] gates =
            GetComponentsInChildren<GateSizeController>(
                true
            );


        if (gates.Length < 2)
        {
            Debug.LogError(
                $"[GateGroupLayout] {name} 아래에서 " +
                $"GateSizeController를 2개 찾지 못했습니다. " +
                $"현재 개수 : {gates.Length}"
            );

            return;
        }


        // Hierarchy 순서 기준
        leftGate = gates[0];
        rightGate = gates[1];


        Debug.Log(
            $"[GateGroupLayout] Gate 자동 연결 | " +
            $"Left : {leftGate.name} / " +
            $"Right : {rightGate.name}"
        );
    }


    /// <summary>
    /// Player 이동 가능 전체 폭을 기준으로
    /// 좌우 Gate를 절반씩 배치한다.
    /// </summary>
    public void Configure(
        float movementWidth)
    {
        ResolveGates();


        if (leftGate == null ||
            rightGate == null)
        {
            Debug.LogError(
                "[GateGroupLayout] " +
                "Left / Right Gate가 없습니다."
            );

            return;
        }


        movementWidth =
            Mathf.Max(
                1f,
                movementWidth
            );


        float usableWidth =
            movementWidth -
            sidePadding * 2f -
            centerGap;


        if (usableWidth <= 0f)
        {
            Debug.LogError(
                $"[GateGroupLayout] " +
                $"사용 가능한 폭이 없습니다. " +
                $"MovementWidth : {movementWidth}"
            );

            return;
        }


        // 두 Gate가 절반씩 사용
        float gateWidth =
            usableWidth * 0.5f;


        // Gate 중심 위치
        float gateCenterX =
            centerGap * 0.5f +
            gateWidth * 0.5f;


        // 중요:
        // Prefab에서 Scale을 건드렸어도
        // Runtime 계산 전에 1로 되돌린다.
        leftGate.transform.localScale =
            Vector3.one;

        rightGate.transform.localScale =
            Vector3.one;


        // =========================
        // Left Gate
        // =========================

        leftGate.transform.localPosition =
            new Vector3(
                -gateCenterX,
                0f,
                0f
            );


        leftGate.SetSize(
            gateWidth,
            gateHeight
        );


        // =========================
        // Right Gate
        // =========================

        rightGate.transform.localPosition =
            new Vector3(
                gateCenterX,
                0f,
                0f
            );


        rightGate.SetSize(
            gateWidth,
            gateHeight
        );

#if UNITY_EDITOR
        Debug.Log(
            $"[GateGroupLayout] Layout 적용 완료 | " +
            $"전체 폭 : {movementWidth:0.00} | " +
            $"Gate 1개 폭 : {gateWidth:0.00} | " +
            $"Left X : {-gateCenterX:0.00} | " +
            $"Right X : {gateCenterX:0.00}"
        );
#endif
    }
}