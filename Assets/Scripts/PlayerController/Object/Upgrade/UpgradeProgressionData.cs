using UnityEngine;

/// <summary>
/// 강화가 등장하는 조건 데이터를 저장하는 ScriptableObject
/// 예: 3킬 → 5킬 → 8킬 → 12킬
/// </summary>
[CreateAssetMenu(
    fileName = "UpgradeProgressionData",
    menuName = "Game/Upgrade Progression Data"
)]
public class UpgradeProgressionData : ScriptableObject
{
    [Header("강화 등장에 필요한 처치 수")]

    // 각 강화 단계마다 필요한 처치 수
    // 예:
    // 0번째 강화 = 3킬
    // 1번째 강화 = 5킬
    // 2번째 강화 = 8킬
    public int[] requiredKills;
}