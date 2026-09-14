using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전체적인 흐름을 담당한다.
///
/// - GameOver
/// - Pause / Resume
/// - Retry
/// - Lobby 이동
///
/// UI를 직접 제어하지 않는다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }


    [Header("Scene")]

    [SerializeField]
    private string lobbySceneName = "LobbyScene";


    private bool isGameOver;


    public bool IsGameOver =>
        isGameOver;


    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    /// <summary>
    /// 게임 종료 상태로 변경한다.
    /// 실제 GameOver UI 표시는 GameResultPresenter가 담당한다.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        Pause();
    }


    /// <summary>
    /// 게임을 일시정지한다.
    /// </summary>
    public void Pause()
    {
        Time.timeScale = 0f;
    }


    /// <summary>
    /// 게임을 다시 진행한다.
    /// </summary>
    public void Resume()
    {
        if (isGameOver)
        {
            return;
        }

        Time.timeScale = 1f;
    }


    /// <summary>
    /// 현재 게임 Scene을 다시 시작한다.
    /// </summary>
    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    /// <summary>
    /// Lobby Scene으로 이동한다.
    /// </summary>
    public void GoLobby()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            lobbySceneName
        );
    }
}