using UnityEngine;

/// <summary>
/// 한 Gate 그룹에서 하나의 Gate만
/// 선택 가능하도록 관리한다.
///
/// 한 Gate가 사용되면
/// 같은 그룹의 나머지 Gate를 잠근다.
/// </summary>
public class GateChoiceGroup : MonoBehaviour
{
    [SerializeField]
    private Gate[] gates;


    private void Awake()
    {
        if (gates == null ||
            gates.Length == 0)
        {
            gates =
                GetComponentsInChildren<Gate>(
                    true
                );
        }
    }


    private void OnEnable()
    {
        Subscribe();
    }


    private void OnDisable()
    {
        Unsubscribe();
    }


    private void Subscribe()
    {
        if (gates == null)
        {
            return;
        }


        foreach (Gate gate in gates)
        {
            if (gate == null)
            {
                continue;
            }


            gate.OnUsed +=
                HandleGateUsed;
        }
    }


    private void Unsubscribe()
    {
        if (gates == null)
        {
            return;
        }


        foreach (Gate gate in gates)
        {
            if (gate == null)
            {
                continue;
            }


            gate.OnUsed -=
                HandleGateUsed;
        }
    }


    private void HandleGateUsed(
        Gate selectedGate)
    {
        foreach (Gate gate in gates)
        {
            if (gate == null ||
                gate == selectedGate)
            {
                continue;
            }


            gate.Lock();
        }
    }
}