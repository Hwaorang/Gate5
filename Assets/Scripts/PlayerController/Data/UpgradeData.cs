using UnityEngine;

public enum UpgradeType
{
    Damage,
    AttackSpeed,
    SoldierCount,
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

    public float value;
}