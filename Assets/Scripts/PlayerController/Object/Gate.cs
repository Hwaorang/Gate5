using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어가 통과했을 때
/// 병사 수를 증가시키거나 감소시키는 Gate.
///
/// GateType
/// - Add      : 병사 수 증가
/// - Subtract : 병사 수 감소
///
/// 총알에 맞으면 Gate의 value가 변경될 수 있으며,
/// Player가 실제로 Gate를 통과했을 때
/// 현재 value만큼 SquadManager에 적용한다.
/// </summary>
public class Gate : MonoBehaviour
{
    /// <summary>
    /// Gate가 병사 수를 증가시키는지
    /// 감소시키는지 구분한다.
    /// </summary>
    public enum GateType
    {
        Add,
        Subtract
    }


    [Header("게이트 설정")]

    // 현재 Gate의 종류
    [SerializeField] private GateType gateType;

    // Player가 통과했을 때 적용할 현재 값
    [SerializeField, Min(0)]
    private int value = 1;

    // 총알 1발에 의해 Gate value가 변하는 양
    [SerializeField, Min(0)]
    private int changePerHit = 1;


    [Header("UI")]

    // Gate 위에 현재 값을 표시할 TextMeshPro 텍스트
    [SerializeField] private TMP_Text valueText;


    [Header("충돌 설정")]

    // Gate를 사용할 수 있는 Player Layer
    [SerializeField] private LayerMask playerLayer;


    // 한 Gate가 여러 번 적용되는 것을 방지
    private bool isUsed;


    private void Start()
    {
        // 게임 시작 시 현재 Gate 값을 UI에 표시
        UpdateText();
    }


    /// <summary>
    /// 총알이 Gate를 맞았을 때 호출한다.
    ///
    /// Add Gate는 value가 증가하고,
    /// Subtract Gate는 value가 감소한다.
    ///
    /// 변경 후 UI도 즉시 갱신한다.
    /// </summary>
    public void HitByBullet()
    {
        // 이미 사용된 Gate라면
        // 더 이상 값을 변경하지 않도록 한다.
        if (isUsed)
        {
            return;
        }

        switch (gateType)
        {
            case GateType.Add:

                // + Gate는 총알을 맞을 때마다
                // 병사 증가량이 커진다.
                value +=
                    changePerHit;

                break;


            case GateType.Subtract:

                // - Gate는 총알을 맞을 때마다
                // 병사 감소량이 줄어든다.
                value -=
                    changePerHit;

                // 0 아래로 내려가지 않도록 제한
                value =
                    Mathf.Max(
                        0,
                        value
                    );

                break;
        }

        // 변경된 값을 UI에 반영
        UpdateText();
    }


    /// <summary>
    /// 현재 GateType과 value를
    /// 화면에 표시한다.
    ///
    /// Add      -> +5
    /// Subtract -> -5
    /// </summary>
    private void UpdateText()
    {
        if (valueText == null)
        {
            return;
        }

        switch (gateType)
        {
            case GateType.Add:

                valueText.text =
                    $"+{value}";

                break;


            case GateType.Subtract:

                valueText.text =
                    $"-{value}";

                break;
        }
    }


    /// <summary>
    /// Player가 Gate Trigger에 들어왔을 때
    /// 현재 Gate 값을 Squad에 적용한다.
    /// </summary>
    private void OnTriggerEnter(
        Collider other)
    {
        // 이미 사용한 Gate라면
        // 중복 적용하지 않는다.
        if (isUsed)
        {
            return;
        }


        // 충돌한 오브젝트가
        // Player Layer가 아니라면 무시
        if ((playerLayer.value &
            (1 << other.gameObject.layer)) == 0)
        {
            return;
        }


        // PlayerRoot에 붙어있는
        // SquadManager를 찾는다.
        SquadManager squadManager =
            other.GetComponent<SquadManager>();


        if (squadManager == null)
        {
            return;
        }


        // 이 시점부터 Gate는 한 번 사용된 상태
        isUsed =
            true;


        switch (gateType)
        {
            case GateType.Add:

                // 현재 value만큼 병사 추가
                squadManager.AddUnit(
                    value
                );

                break;


            case GateType.Subtract:

                // 현재 value만큼 병사 제거
                squadManager.RemoveUnits(
                    value
                );

                break;
        }
    }
}