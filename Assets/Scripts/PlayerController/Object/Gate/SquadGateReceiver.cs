using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SquadManager를 Gate 시스템에서
/// 사용할 수 있도록 연결하는 Adapter.
///
/// Gate는 SquadManager를 직접 알지 않고
/// IGateReceiver만 사용한다.
/// </summary>
[RequireComponent(typeof(SquadManager))]
public class SquadGateReceiver :
    MonoBehaviour,
    IGateReceiver
{
    [SerializeField]
    private SquadManager squadManager;


    public int CurrentUnitCount
    {
        get
        {
            if (squadManager == null)
            {
                return 0;
            }


            IReadOnlyList<SoldierAttack> attacks =
                squadManager.SoldierAttacks;


            if (attacks == null)
            {
                return 0;
            }


            int aliveCount = 0;


            for (int i = 0;
                 i < attacks.Count;
                 i++)
            {
                SoldierAttack attack =
                    attacks[i];


                if (attack != null &&
                    attack.CanAttack)
                {
                    aliveCount++;
                }
            }


            return aliveCount;
        }
    }


    private void Awake()
    {
        if (squadManager == null)
        {
            squadManager =
                GetComponent<SquadManager>();
        }
    }


    public void AddUnits(
        int amount)
    {
        if (squadManager == null ||
            amount <= 0)
        {
            return;
        }


        squadManager.AddUnit(
            amount
        );
    }


    public void RemoveUnits(
        int amount)
    {
        if (squadManager == null ||
            amount <= 0)
        {
            return;
        }


        squadManager.RemoveUnits(
            amount
        );
    }
}