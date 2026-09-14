using UnityEngine;

/// <summary>
/// 로비 영구 강화의
/// 레벨당 증가량을 관리하는 ScriptableObject.
///
/// LobbyUpgradeApplier에서
/// 저장된 강화 레벨과 이 값을 이용해
/// 실제 적용 배율을 계산한다.
///
/// 예:
/// damageIncreasePerLevel = 0.05
/// Attack Level = 3
///
/// 최종 로비 공격력 배율
/// = 1 + (3 × 0.05)
/// = 1.15배
/// </summary>
[CreateAssetMenu(
    fileName = "LobbyUpgradeData",
    menuName = "Game/Lobby Upgrade Data"
)]
public class LobbyUpgradeData : ScriptableObject
{
    [Header("레벨당 증가량")]

    // 공격력 강화 1레벨당 증가 비율
    //
    // 0.05 = +5%
    [Min(0f)]
    public float damageIncreasePerLevel = 0.05f;

    // 이동속도 강화 1레벨당 증가 비율
    //
    // 0.05 = +5%
    [Min(0f)]
    public float moveSpeedIncreasePerLevel = 0.05f;

    // 공격속도 강화 1레벨당 증가 비율
    //
    // 0.05 = +5%
    [Min(0f)]
    public float attackSpeedIncreasePerLevel = 0.05f;
}