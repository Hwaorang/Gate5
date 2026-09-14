using UnityEngine;

/// <summary>
/// 투사체 강화에 필요한 수치를 관리하는 ScriptableObject.
///
/// SoldierAttack에 수치를 직접 하드코딩하지 않고
/// Inspector에서 밸런스를 수정할 수 있도록 분리했다.
///
/// 관리 항목
/// - 기본 투사체 개수
/// - 최대 투사체 개수
/// - 최대 개수 이후 크기 증가량
/// - 다중 투사체 퍼짐 각도
/// </summary>
[CreateAssetMenu(
    fileName = "ProjectileUpgradeData",
    menuName = "Game/Upgrade/Projectile Upgrade Data"
)]
public class ProjectileUpgradeData : ScriptableObject
{
    [Header("투사체 개수")]

    // 아무 강화도 없을 때 발사하는 투사체 개수
    [Min(1)]
    public int baseProjectileCount = 1;

    // 투사체 개수가 증가할 수 있는 최대치
    //
    // 예:
    // 기본 1발 / 최대 5발
    //
    // Lv.0 = 1발
    // Lv.1 = 2발
    // ...
    // Lv.4 = 5발
    [Min(1)]
    public int maxProjectileCount = 5;


    [Header("투사체 크기")]

    // 아무 크기 강화도 없을 때 기본 Scale 배율
    [Min(0.1f)]
    public float baseScaleMultiplier = 1f;

    // 최대 투사체 개수에 도달한 이후
    // 강화 1회당 증가할 Scale 배율
    //
    // 실제 FX 크기 증가 기능을 사용할지는
    // 팀 회의 후 결정한다.
    [Min(0f)]
    public float scaleIncreasePerLevel = 0.2f;


    [Header("발사 형태")]

    // 다중 투사체 발사 시
    // 각각의 투사체 사이에 적용할 각도
    //
    // 예:
    // 3발 / spreadAngle = 5
    // → -5도 / 0도 / +5도
    [Min(0f)]
    public float spreadAngle = 5f;
}