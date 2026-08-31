using UnityEngine;

public enum UpgradeType
{
    ProjectileCount,
    AttackSpeed,
    Damage,
    MoveSpeed
}

[CreateAssetMenu(
    fileName = "UpgradeData",
    menuName = "Game/Upgrade Data"
)]
public class UpgradeData : ScriptableObject
{
    [Header("UI")]
    public string upgradeName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("강화 설정")]
    public UpgradeType upgradeType;

    // 한 번 선택했을 때 증가량
    public float value;

    // 최대 강화 횟수
    public int maxLevel = 5;
}