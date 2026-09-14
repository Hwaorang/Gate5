/// <summary>
/// 한 판이 끝났을 때 결과 화면에 전달할 데이터.
///
/// UI와 게임 로직을 분리하기 위해
/// 결과에 필요한 값만 보관한다.
///
/// 이후 킬 수, 획득 골드, 점수 등이 추가되면
/// 이 클래스에 항목을 추가하면 된다.
/// </summary>
public class GameResultData
{
    /// <summary>
    /// 이번 게임에서 생존한 시간
    /// </summary>
    public float SurvivalTime { get; private set; }

    /// <summary>
    /// 게임 종료 시 플레이어 레벨
    /// </summary>
    public int Level { get; private set; }

    public GameResultData(
        float survivalTime,
        int level)
    {
        SurvivalTime = survivalTime;
        Level = level;
    }
}