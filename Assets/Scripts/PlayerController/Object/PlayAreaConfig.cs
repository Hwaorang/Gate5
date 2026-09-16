using UnityEngine;

/// <summary>
/// Player 이동 범위와 Gate 폭이 함께 사용하는 플레이 영역 설정.
///
/// 하나의 값을 Player와 Gate가 공유하므로
/// 도로 폭 / Player 이동 범위 / Gate 폭이 서로 어긋나는 것을 방지한다.
/// </summary>
[CreateAssetMenu(
    fileName = "PlayAreaConfig",
    menuName = "Game/Stage/Play Area Config"
)]
public class PlayAreaConfig : ScriptableObject
{
    [Header("Play Area")]

    [Tooltip("플레이 영역의 월드 X 중심")]
    [SerializeField]
    private float centerX = 0f;

    [Tooltip("Gate가 채울 실제 플레이 가능 전체 폭")]
    [SerializeField]
    [Min(1f)]
    private float playableWidth = 9f;

    [Tooltip(
        "Player 중심이 도로 가장자리까지 가지 않도록 확보할 여백. " +
        "Player 반폭 정도를 권장한다."
    )]
    [SerializeField]
    [Min(0f)]
    private float edgeClearance = 0.5f;


    public float CenterX =>
        centerX;

    public float PlayableWidth =>
        playableWidth;

    /// <summary>
    /// Player 중심점이 이동할 수 있는 좌우 한계.
    ///
    /// 예:
    /// playableWidth = 9
    /// edgeClearance = 0.5
    /// -> 4.5 - 0.5
    /// -> XLimit = 4
    /// </summary>
    public float PlayerXLimit =>
        Mathf.Max(
            0f,
            playableWidth * 0.5f -
            edgeClearance
        );

    public float MinPlayerX =>
        centerX - PlayerXLimit;

    public float MaxPlayerX =>
        centerX + PlayerXLimit;
}
