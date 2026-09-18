using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Gate의 런타임 동작을 관리한다.
///
/// Gate 자체는 SquadManager를 알지 않는다.
/// IGateReceiver와 Strategy를 통해 효과를 적용한다.
///
/// 다른 Scene에서도 동일한 Gate Prefab을
/// 그대로 재사용할 수 있다.
/// </summary>
public class Gate : MonoBehaviour
{
    // ========================================================
    // Progressive Random Value
    // ========================================================

    [Header("Progressive Random Value")]

    [Tooltip(
        "게임 진행 시간에 따라 Gate 숫자 범위를 증가시킬지 여부"
    )]
    [SerializeField]
    private bool useProgressiveRandomValue = true;


    [Header("Early Game")]

    [Tooltip("게임 초반 최소 Gate 값")]
    [SerializeField]
    [Min(1)]
    private int earlyMinValue = 1;

    [Tooltip("게임 초반 최대 Gate 값")]
    [SerializeField]
    [Min(1)]
    private int earlyMaxValue = 4;


    [Header("Late Game")]

    [Tooltip("후반 최소 Gate 값")]
    [SerializeField]
    [Min(1)]
    private int lateMinValue = 5;

    [Tooltip("후반 최대 Gate 값")]
    [SerializeField]
    [Min(1)]
    private int lateMaxValue = 12;


    [Header("Progression")]

    [Tooltip(
        "몇 초 후 Late Game 범위에 도달할지 설정"
    )]
    [SerializeField]
    [Min(1f)]
    private float progressionDuration = 120f;

    // ========================================================
    // Data
    // ========================================================

    [Header("Gate Data")]

    [SerializeField]
    private GateData gateData;


    // ========================================================
    // UI
    // ========================================================

    [Header("UI")]

    [SerializeField]
    private TMP_Text valueText;


    // ========================================================
    // Collision
    // ========================================================

    [Header("Collision")]

    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField]
    private Collider triggerCollider;


    // ========================================================
    // Runtime
    // ========================================================

    private IGateOperationStrategy strategy;

    private int currentValue;

    private bool isUsed;


    // ========================================================
    // Event
    // ========================================================

    /// <summary>
    /// Gate가 실제로 사용되었을 때 발생한다.
    ///
    /// GateChoiceGroup,
    /// VFX,
    /// SFX 등이 이 이벤트를 사용할 수 있다.
    /// </summary>
    public event Action<Gate> OnUsed;


    public bool IsUsed =>
        isUsed;

    public int CurrentValue =>
        currentValue;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider =
                GetComponentInChildren<Collider>(
                    true
                );
        }
    }


    private void OnEnable()
    {
        ResetGate();
    }


    // ========================================================
    // Initialize
    // ========================================================

    /// <summary>
    /// Gate를 초기 상태로 되돌린다.
    ///
    /// Scene 재사용이나
    /// Object Pool에서도 사용할 수 있다.
    /// </summary>
    public void ResetGate()
    {
        if (gateData == null)
        {
            Debug.LogWarning(
                $"[Gate] {gameObject.name}의 " +
                $"GateData가 없습니다."
            );

            return;
        }


        strategy =
            GateStrategyFactory.GetStrategy(
                gateData.OperationType
            );


        currentValue = GetInitialValue();


        isUsed =
            false;


        if (triggerCollider != null)
        {
            triggerCollider.enabled =
                true;
        }


        RefreshView();
    }

    /// <summary>
    /// Gate가 생성될 때 사용할 초기값을 결정한다.
    ///
    /// Add / Subtract Gate는
    /// 게임 진행 시간에 따라 랜덤 범위가 증가한다.
    ///
    /// Multiply Gate는 기존 GateData 값을 유지한다.
    /// </summary>
    private int GetInitialValue()
    {
        if (gateData == null)
        {
            return 0;
        }


        // =============================================
        // Multiply Gate
        // =============================================

        // ×2 같은 곱하기 Gate는
        // 랜덤값 시스템의 영향을 받지 않는다.
        if (gateData.OperationType ==
            GateOperationType.Multiply)
        {
            return gateData.InitialValue;
        }


        // =============================================
        // Random 사용 안 함
        // =============================================

        if (!useProgressiveRandomValue)
        {
            return gateData.InitialValue;
        }


        // =============================================
        // Game Progress
        // =============================================

        float elapsedTime =
            Time.timeSinceLevelLoad;


        float progress =
            Mathf.Clamp01(
                elapsedTime /
                progressionDuration
            );


        // =============================================
        // Current Range
        // =============================================

        int currentMin =
            Mathf.RoundToInt(
                Mathf.Lerp(
                    earlyMinValue,
                    lateMinValue,
                    progress
                )
            );


        int currentMax =
            Mathf.RoundToInt(
                Mathf.Lerp(
                    earlyMaxValue,
                    lateMaxValue,
                    progress
                )
            );


        // Inspector에서 값 순서를 잘못 넣어도
        // 정상 동작하도록 보정한다.
        int min =
            Mathf.Min(
                currentMin,
                currentMax
            );


        int max =
            Mathf.Max(
                currentMin,
                currentMax
            );


        // int Random.Range의 max는 포함되지 않으므로
        // +1 해준다.
        return UnityEngine.Random.Range(
            min,
            max + 1
        );
    }

    // ========================================================
    // Bullet
    // ========================================================

    /// <summary>
    /// 총알 또는 Raycast가 Gate에 맞았을 때 호출한다.
    /// </summary>
    public void HitByBullet()
    {
        if (isUsed ||
            gateData == null ||
            strategy == null)
        {
            return;
        }


        currentValue =
            strategy.ModifyValueByBullet(
                currentValue,
                gateData.ChangePerHit
            );


        RefreshView();
    }


    // ========================================================
    // Use
    // ========================================================

    private void UseGate(
        IGateReceiver receiver)
    {
        if (isUsed ||
            strategy == null)
        {
            return;
        }


        // 적용 전에 사용 상태로 변경해서
        // 같은 프레임 중복 Trigger를 방지
        isUsed =
            true;


        // Trigger만 끈다.
        // Gate Visual은 그대로 유지된다.
        if (triggerCollider != null)
        {
            triggerCollider.enabled =
                false;
        }


        strategy.Apply(
            receiver,
            currentValue
        );


        OnUsed?.Invoke(
            this
        );
    }


    // ========================================================
    // Lock
    // ========================================================

    /// <summary>
    /// 다른 Gate가 선택됐을 때
    /// 이 Gate를 사용 불가능하게 만든다.
    ///
    /// Visual은 유지한다.
    /// </summary>
    public void Lock()
    {
        if (isUsed)
        {
            return;
        }


        isUsed =
            true;


        if (triggerCollider != null)
        {
            triggerCollider.enabled =
                false;
        }
    }


    // ========================================================
    // Receiver
    // ========================================================

    /// <summary>
    /// Collider 자신 또는 부모에서
    /// IGateReceiver 구현체를 찾는다.
    ///
    /// PlayerRoot Collider 구조가 달라져도
    /// 재사용할 수 있도록 한다.
    /// </summary>
    private IGateReceiver FindGateReceiver(
        Collider other)
    {
        MonoBehaviour[] behaviours =
            other.GetComponentsInParent<MonoBehaviour>(
                true
            );


        for (int i = 0;
             i < behaviours.Length;
             i++)
        {
            if (behaviours[i] is
                IGateReceiver receiver)
            {
                return receiver;
            }
        }


        return null;
    }


    // ========================================================
    // UI
    // ========================================================

    private void RefreshView()
    {
        if (valueText == null ||
            strategy == null)
        {
            return;
        }


        valueText.text =
            strategy.GetDisplayText(
                currentValue
            );
    }

    public void TryUseFromTrigger(
    Collider other)
    {
        if (isUsed)
        {
            return;
        }


        IGateReceiver receiver =
            FindGateReceiver(
                other
            );


        if (receiver == null)
        {
            Debug.LogWarning(
                $"[Gate] {other.name}에서 " +
                $"IGateReceiver를 찾지 못했습니다."
            );

            return;
        }


        Debug.Log(
            $"[Gate] Gate 적용 | " +
            $"Value : {currentValue}"
        );


        UseGate(
            receiver
        );
    }
}