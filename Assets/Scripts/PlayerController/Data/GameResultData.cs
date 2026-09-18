/// <summary>
/// GameOver 결과 화면에 전달할 데이터.
///
/// UI가 GameManager나 PlayerExperience 같은
/// 게임 시스템을 직접 참조하지 않도록
/// 필요한 결과값만 묶어서 전달한다.
/// </summary>
public class GameResultData
{
    public float SurvivalTime
    {
        get;
        private set;
    }


    public int Level
    {
        get;
        private set;
    }


    public int EarnedGold
    {
        get;
        private set;
    }


    public GameResultData(
        float survivalTime,
        int level,
        int earnedGold)
    {
        SurvivalTime =
            survivalTime;

        Level =
            level;

        EarnedGold =
            earnedGold;
    }

    public GameResultData(
    float survivalTime,
    int level)
    {
        SurvivalTime = survivalTime;
        Level = level;
        EarnedGold = 0;
    }
}