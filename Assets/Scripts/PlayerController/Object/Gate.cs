using TMPro;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // 게이트가 어떤 연산을 할지 구분
    public enum GateType
    {
        Add,        // +
        Multiply    // ×
    }

    [Header("게이트 설정")]
    [SerializeField] private GateType gateType;

    // +5라면 5
    // ×2라면 2
    [SerializeField] private int value = 5;

    [Header("UI")]
    // 게이트 위에 표시할 숫자
    // 아직 UI를 만들지 않았다면 비워둬도 됨
    [SerializeField] private TMP_Text valueText;

    // 한 번만 작동하도록 확인
    private bool isUsed;

    private void Start()
    {
        UpdateText();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 사용한 게이트라면 다시 실행하지 않는다.
        if (isUsed)
        {
            return;
        }

        // 충돌한 오브젝트 또는 부모에서 SquadManager 검색
        SquadManager squadManager =
            other.GetComponentInParent<SquadManager>();

        // 플레이어가 아니라면 무시
        if (squadManager == null)
        {
            return;
        }

        isUsed = true;

        ApplyGate(squadManager);

        // 사용한 게이트 제거
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// 게이트 종류에 따라 병사 수를 변경한다.
    /// </summary>
    private void ApplyGate(SquadManager squadManager)
    {
        switch (gateType)
        {
            case GateType.Add:

                // +5, +10 등
                squadManager.AddUnit(value);

                break;

            case GateType.Multiply:

                // 현재 병력 × 배율
                //
                // 예)
                // 현재 5명이고 ×2라면
                // 기존 5명 + 추가 5명 = 10명
                int addAmount =
                    squadManager.CurrentCount * (value - 1);

                squadManager.AddUnit(addAmount);

                break;
        }
    }

    /// <summary>
    /// 게이트에 +5, ×2 등의 텍스트 표시
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

            case GateType.Multiply:
                valueText.text = $"×{value}";
                break;
        }
    }
}