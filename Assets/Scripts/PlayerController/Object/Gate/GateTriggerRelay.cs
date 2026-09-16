using UnityEngine;

/// <summary>
/// Gate의 자식 Trigger에서 충돌을 감지하고
/// 실제 Gate 로직으로 전달한다.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class GateTriggerRelay : MonoBehaviour
{
    [SerializeField]
    private Gate gate;


    private void Awake()
    {
        if (gate == null)
        {
            gate =
                GetComponentInParent<Gate>();
        }


        BoxCollider trigger =
            GetComponent<BoxCollider>();


        trigger.isTrigger =
            true;
    }


    private void OnTriggerEnter(
        Collider other)
    {
        Debug.Log(
            $"[GateTriggerRelay] Trigger 진입 : {other.name}"
        );


        if (gate == null)
        {
            Debug.LogWarning(
                "[GateTriggerRelay] Gate를 찾지 못했습니다."
            );

            return;
        }


        gate.TryUseFromTrigger(
            other
        );
    }
}