using UnityEngine;

/// <summary>
/// Player의 기본 스탯과 이동속도 강화 상태를 관리한다.
///
/// 주요 역할
/// - 기본 이동속도 저장
/// - 시작 병사 수 저장
/// - 로비 영구 이동속도 강화 배율 관리
/// - 인게임 EXP 이동속도 강화 배율 관리
/// - 최종 이동속도 계산
///
/// 실제 이동 처리는 PlayerController가 담당하고,
/// PlayerController는 이 클래스의 MoveSpeed 값을 사용한다.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]

    // 아무 강화도 적용되지 않았을 때 기본 이동속도
    [SerializeField] private float baseMoveSpeed = 5f;

    // 게임 시작 시 생성할 기본 병사 수
    [SerializeField] private int startSoldierCount = 1;


    // =========================
    // 이동속도 강화 상태
    // =========================

    // 로비에서 구매한 영구 이동속도 강화 배율
    //
    // 기본값 1 = 강화 없음
    private float lobbyMoveSpeedMultiplier = 1f;

    // 플레이 도중 EXP 보상으로 획득한
    // 인게임 이동속도 강화 배율
    private float inGameMoveSpeedMultiplier = 1f;


    /// <summary>
    /// 현재 실제 이동속도.
    ///
    /// 기본 이동속도
    /// × 로비 강화 배율
    /// × 인게임 강화 배율
    ///
    /// 예:
    /// Base = 5
    /// Lobby = 1.1
    /// InGame = 1.1
    ///
    /// 최종 이동속도 = 6.05
    /// </summary>
    public float MoveSpeed =>
        baseMoveSpeed *
        lobbyMoveSpeedMultiplier *
        inGameMoveSpeedMultiplier;


    /// <summary>
    /// 게임 시작 시 생성할 병사 수.
    /// SquadManager가 시작 병사를 생성할 때 사용한다.
    /// </summary>
    public int StartSoldierCount =>
        startSoldierCount;


    /// <summary>
    /// 인게임 EXP 보상으로 획득한
    /// 이동속도 증가량을 누적한다.
    ///
    /// 예:
    /// percent = 0.1
    /// → 인게임 이동속도 +10%
    /// </summary>
    public void UpgradeMoveSpeed(
        float percent)
    {
        // 인게임 강화 배율에만 추가한다.
        //
        // 로비 강화 값과 별도로 관리하기 때문에
        // 영구 강화 상태를 덮어쓰지 않는다.
        inGameMoveSpeedMultiplier +=
            percent;
    }


    /// <summary>
    /// 로비에서 저장된 영구 이동속도 강화 배율을 설정한다.
    ///
    /// 예:
    /// 로비 이동속도 +20%
    /// multiplier = 1.2
    ///
    /// 기존 값에 더하는 방식이 아니라
    /// 저장된 최종 로비 배율을 그대로 설정한다.
    /// </summary>
    public void SetLobbyMoveSpeedMultiplier(
        float multiplier)
    {
        // 1보다 작은 값이 들어와
        // 기본 이동속도보다 느려지는 상황을 방지한다.
        lobbyMoveSpeedMultiplier =
            Mathf.Max(
                1f,
                multiplier
            );
    }
}