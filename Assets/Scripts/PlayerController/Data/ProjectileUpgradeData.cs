using UnityEngine;

/// <summary>
/// 투사체 강화에 필요한 수치를 관리하는 데이터
///
/// 밸런스 변경 시 SoldierAttack 코드를 수정하지 않고
/// Inspector에서 값만 수정할 수 있도록 ScriptableObject로 분리했다.
/// </summary>
[CreateAssetMenu(
    fileName = "ProjectileUpgradeData",
    menuName = "Game/Upgrade/Projectile Upgrade Data"
)]
public class ProjectileUpgradeData : ScriptableObject
{
    [Header("투사체 개수")]

    // 기본 투사체 개수
    public int baseProjectileCount = 1;

    // 증가할 수 있는 최대 투사체 개수
    public int maxProjectileCount = 5;


    [Header("투사체 크기")]

    // 기본 크기 배율
    public float baseScaleMultiplier = 1f;

    // 최대 투사체 개수 이후
    // 강화 1회당 증가할 크기
    public float scaleIncreasePerLevel = 0.2f;


    [Header("발사 형태")]

    // 다중 투사체 발사 시
    // 각 투사체 사이의 각도
    public float spreadAngle = 5f;
}