using UnityEngine;

/// <summary>
/// 레벨별 필요 경험치를 관리하는 ScriptableObject.
///
/// 배열의 인덱스를 PlayerExperience의 level과 매칭해서 사용한다.
///
/// 예:
/// requiredExp[0] = 10
/// requiredExp[1] = 20
/// requiredExp[2] = 30
///
/// → Lv.1 진입에 10 EXP
/// → Lv.2 진입에 20 EXP
/// → Lv.3 진입에 30 EXP 필요
///
/// PlayerExperience에서는 현재 level이 배열 길이를 넘어가도
/// 마지막 값을 계속 사용한다.
/// </summary>
[CreateAssetMenu(
    fileName = "ExperienceProgressionData",
    menuName = "Game/Experience Progression Data"
)]
public class ExperienceProgressionData : ScriptableObject
{
    [Header("레벨별 필요 경험치")]

    // 각 레벨에서 다음 레벨업까지 필요한 EXP
    //
    // 값은 1 이상의 양수로 설정하는 것을 권장한다.
    public int[] requiredExp;
}