using System;
using UnityEngine;

/// <summary>
/// 현재 한 판에서 획득한 Gold만 관리한다.
///
/// 영구 저장 골드는 SaveManager가 담당하고,
/// 이 클래스는 이번 플레이에서 얻은 양만 기록한다.
/// </summary>
public sealed class GameRunGoldTracker : MonoBehaviour
{
    public int EarnedGold
    {
        get;
        private set;
    }


    public event Action<int> OnGoldChanged;


    private void Awake()
    {
        EarnedGold = 0;
    }


    /// <summary>
    /// 이번 플레이에서 골드를 획득했을 때 호출한다.
    /// </summary>
    public void AddGold(
        int amount)
    {
        if (amount <= 0)
        {
            return;
        }


        EarnedGold += amount;


        OnGoldChanged?.Invoke(
            EarnedGold
        );
    }
}