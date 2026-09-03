using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private float baseMoveSpeed = 5f;

    [SerializeField] private int startSoldierCount = 1;

    // 로비 영구 강화
    private float lobbyMoveSpeedMultiplier = 1f;

    // 인게임 EXP 강화
    private float inGameMoveSpeedMultiplier = 1f;

    /// <summary>
    /// 실제 현재 이동속도.
    /// 기본값 × 로비 강화 × 인게임 강화
    /// </summary>
    public float MoveSpeed =>
        baseMoveSpeed *
        lobbyMoveSpeedMultiplier *
        inGameMoveSpeedMultiplier;

    public int StartSoldierCount =>
        startSoldierCount;


    /// <summary>
    /// 인게임에서 획득한 이동속도 강화.
    /// </summary>
    public void UpgradeMoveSpeed(float percent)
    {
#if UNITY_EDITOR
        Debug.Log(
            $"[MoveSpeed 강화 진입] percent = {percent}"
        );
#endif

        inGameMoveSpeedMultiplier += percent;
#if UNITY_EDITOR
        Debug.Log(
            $"Lobby x{lobbyMoveSpeedMultiplier:F2} / " +
            $"InGame x{inGameMoveSpeedMultiplier:F2} / " +
            $"Final MoveSpeed = {MoveSpeed:F2}"
        );
#endif
    }


    /// <summary>
    /// 로비 영구 이동속도 강화 배율 설정.
    /// </summary>
    public void SetLobbyMoveSpeedMultiplier(
        float multiplier)
    {
        lobbyMoveSpeedMultiplier =
            Mathf.Max(1f, multiplier);

#if UNITY_EDITOR
        Debug.Log(
            $"Lobby MoveSpeed x{lobbyMoveSpeedMultiplier:F2}" +
            $" / Final : {MoveSpeed:F2}"
        );
#endif
    }
}