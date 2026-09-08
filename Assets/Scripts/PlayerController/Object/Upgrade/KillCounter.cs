using System;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    private int totalKills;

    public int TotalKills => totalKills;

    public event Action<int> OnKillCountChanged;

    /// <summary>
    /// 적이 처치됐을 때 호출
    /// </summary>
    public void AddKill()
    {
        totalKills++;

        OnKillCountChanged?.Invoke(totalKills);
    }

    /// <summary>
    /// 새로운 게임 시작 시 킬 수 초기화
    /// </summary>
    public void ResetKills()
    {
        totalKills = 0;

        OnKillCountChanged?.Invoke(totalKills);
    }
}