using UnityEngine;

/// <summary>
/// 개발 중 테스트를 빠르게 하기 위한 치트 관리자.
///
/// 실제 데이터를 직접 수정하지 않고
/// 기존 게임 시스템의 public 메서드를 호출한다.
/// </summary>
public class DebugCheatManager : MonoBehaviour
{
    [Header("시스템 참조")]
    [SerializeField] private SquadManager squadManager;
    [SerializeField] private PlayerExperience playerExperience;
    [SerializeField] private UpgradeManager_PlayerController upgradeManager;

    /// <summary>
    /// 병사 10명 추가
    /// </summary>
    public void AddSoldier10()
    {
        if (squadManager == null)
            return;

        squadManager.AddUnit(10);
    }

    /// <summary>
    /// 병사 100명 추가
    /// </summary>
    public void AddSoldier100()
    {
        if (squadManager == null)
            return;

        squadManager.AddUnit(100);
    }

    /// <summary>
    /// 병사 1000명 추가
    /// 성능 테스트 용도
    /// </summary>
    public void AddSoldier1000()
    {
        if (squadManager == null)
            return;

        squadManager.AddUnit(1000);
    }

    /// <summary>
    /// EXP 100 추가
    /// </summary>
    public void AddExp100()
    {
        if (playerExperience == null)
            return;

        playerExperience.AddExp(100);
    }

    /// <summary>
    /// 강화창 강제로 열기
    /// </summary>
    public void ForceUpgrade()
    {
        if (upgradeManager == null)
            return;

        upgradeManager.OpenUpgradePanel();
    }

    /// <summary>
    /// 현재 병사를 모두 사망 처리하여
    /// 실제 GameOver 흐름 테스트
    /// </summary>
    public void ForceGameOver()
    {
        if (squadManager == null)
            return;

        squadManager.RemoveUnits(
            squadManager.CurrentCount
        );
    }
}