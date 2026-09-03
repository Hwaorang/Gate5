using UnityEngine;

[CreateAssetMenu(
    fileName = "LobbyUpgradeData",
    menuName = "Game/Lobby Upgrade Data"
)]
public class LobbyUpgradeData : ScriptableObject
{
    [Header("레벨당 증가량")]
    public float damageIncreasePerLevel = 0.05f;
    public float moveSpeedIncreasePerLevel = 0.05f;
    public float attackSpeedIncreasePerLevel = 0.05f;
}