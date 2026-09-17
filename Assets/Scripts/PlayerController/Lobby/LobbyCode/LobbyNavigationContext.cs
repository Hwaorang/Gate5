/// <summary>
/// Lobby Scene에 어떤 이유로 진입했는지
/// Scene 간에 잠깐 전달하기 위한 세션 상태.
///
/// 저장 데이터가 아니므로
/// PlayerPrefs / SaveManager에는 저장하지 않는다.
/// </summary>
public static class LobbyNavigationContext
{
    public enum EntryReason
    {
        InitialLaunch,
        ReturnFromGame
    }


    private static EntryReason entryReason =
        EntryReason.InitialLaunch;


    public static EntryReason CurrentReason =>
        entryReason;


    /// <summary>
    /// 게임 Scene에서 Lobby로 돌아가기 전에 호출한다.
    /// </summary>
    public static void SetReturnFromGame()
    {
        entryReason =
            EntryReason.ReturnFromGame;
    }


    /// <summary>
    /// Lobby에서 진입 이유를 사용한 뒤
    /// 기본 상태로 되돌린다.
    /// </summary>
    public static void Consume()
    {
        entryReason =
            EntryReason.InitialLaunch;
    }
}