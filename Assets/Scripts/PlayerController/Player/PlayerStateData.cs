using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerStatData",
    menuName = "Game/Player Stat Data"
)]
public class PlayerStatData : ScriptableObject
{
    [Header("기본 능력치")]
    public float attackDamage = 10f;
    public float attackDelay = 1f;
    public float moveSpeed = 5f;

    [Header("시작 설정")]
    public int startSoldierCount = 1;
}