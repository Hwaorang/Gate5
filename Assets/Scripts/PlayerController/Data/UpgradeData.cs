using UnityEngine;

/// <summary>
/// 게임에서 사용하는 강화 종류.
/// UpgradeManager와 StrategyFactory에서
/// 어떤 강화 로직을 적용할지 구분할 때 사용한다.
/// </summary>
public enum UpgradeType
{
    ProjectileCount,
    AttackSpeed,
    Damage,
    MoveSpeed
}


/// <summary>
/// 강화 하나의 정보를 저장하는 ScriptableObject.
///
/// UI 정보
/// - 이름
/// - 설명
/// - 아이콘
///
/// 강화 정보
/// - 강화 종류
/// - 1회 선택 시 증가량
/// - 최대 강화 레벨
/// </summary>
[CreateAssetMenu(
    fileName = "UpgradeData",
    menuName = "Game/Upgrade Data"
)]
public class UpgradeData : ScriptableObject
{
    [Header("UI")]

    // 강화 선택창에 표시할 이름
    public string upgradeName;

    // 강화에 대한 설명
    [TextArea]
    public string description;

    // 강화 선택창에 표시할 아이콘
    public Sprite icon;


    [Header("강화 설정")]

    // 실제 적용할 강화 종류
    public UpgradeType upgradeType;

    // 한 번 선택했을 때 적용되는 증가량
    //
    // 예:
    // Damage = 0.1 → +10%
    // AttackSpeed = 0.1 → +10%
    // MoveSpeed = 0.1 → +10%
    //
    // ProjectileCount는 현재 Strategy에서
    // value를 직접 사용하지 않고 강화 레벨을 1 증가시킨다.
    [Min(0f)]
    public float value;

    // 이 강화가 선택될 수 있는 최대 횟수
    [Min(1)]
    public int maxLevel = 5;
}