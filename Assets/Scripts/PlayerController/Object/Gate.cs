using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public enum GateType
    {
        Add,
        Subtract
    }

    [Header("게이트 설정")]
    [SerializeField] private GateType gateType;

    // 현재 게이트 값
    [SerializeField] private int value = 1;

    // 총알 1발 맞을 때 변화할 값
    [SerializeField] private int changePerHit = 1;

    [Header("UI")]
    // 게이트 위에 표시되는 숫자 텍스트
    [SerializeField] private TMP_Text valueText;

    // 게이트 중복 적용 방지
    private bool isUsed;

    [SerializeField] private LayerMask playerLayer;

    private void Start()
    {
        // 시작할 때 현재 값 표시
        UpdateText();
    }


    /// <summary>
    /// 총알이 게이트를 맞았을 때 호출
    /// 게이트 종류에 따라 숫자를 변경한다.
    /// </summary>
    public void HitByBullet()
    {
        switch (gateType)
        {
            case GateType.Add:

                // + 게이트는 총알을 맞으면 숫자 증가
                value += changePerHit;
                break;


            case GateType.Subtract:

                // - 게이트는 총알을 맞으면
                // 페널티 숫자를 줄여준다.
                value -= changePerHit;

                // -0이나 음수까지 내려가지 않게 최소 0으로 제한
                value = Mathf.Max(0, value);
                break;
        }

        // 변경된 숫자를 화면에 바로 반영
        UpdateText();
    }


    /// <summary>
    /// 현재 GateType과 value를
    /// TextMeshPro에 표시한다.
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
                valueText.text = $"+{value}";
                break;

            case GateType.Subtract:
                valueText.text = $"-{value}";
                break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isUsed)
        {
            return;
        }

        // Player Layer가 아니면 무시
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        // PlayerRoot에 붙어있는 SquadManager 찾기
        SquadManager squadManager =
            other.GetComponent<SquadManager>();

        if (squadManager == null)
        {
            return;
        }

        isUsed = true;

        switch (gateType)
        {
            case GateType.Add:
                squadManager.AddUnit(value);
                break;

            case GateType.Subtract:
                squadManager.RemoveUnits(value);
                break;
        }
    }
}