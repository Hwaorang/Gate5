using UnityEngine;

[CreateAssetMenu(
    fileName = "ExperienceProgressionData",
    menuName = "Game/Experience Progression Data"
)]
public class ExperienceProgressionData : ScriptableObject
{
    [Header("레벨별 필요 경험치")]
    public int[] requiredExp;
}