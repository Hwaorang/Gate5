using UnityEngine;

/// <summary>
/// 게임 종료 시 필요한 데이터를 수집하고
/// GameResultUI에 전달하는 중간 관리자.
///
/// 역할:
/// SquadManager
///     → GameOver 발생 알림
///
/// GameResultPresenter
///     → 생존 시간 / 레벨 수집
///
/// GameResultUI
///     → 화면에 표시
///
/// Observer Pattern을 사용하여
/// SquadManager와 결과 UI 사이의 직접적인 의존성을 줄인다.
/// </summary>
public class GameResultPresenter : MonoBehaviour
{
    [Header("GameOver 감지")]

    // GameOver 이벤트를 발생시키는 SquadManager
    [SerializeField]
    private SquadManager squadManager;


    [Header("결과 데이터")]

    // 플레이 시간을 가지고 있는 GameManager
    [SerializeField]
    private GameManager_KHM gameManager;

    // 현재 레벨을 가지고 있는 PlayerExperience
    [SerializeField]
    private PlayerExperience playerExperience;


    [Header("결과 UI")]

    // 결과를 실제 화면에 표시하는 UI
    [SerializeField]
    private GameResultUI gameResultUI;


    /// <summary>
    /// 오브젝트가 활성화될 때
    /// SquadManager의 GameOver 이벤트를 구독한다.
    /// </summary>
    private void OnEnable()
    {
        if (squadManager != null)
        {
            squadManager.OnGameOver +=
                HandleGameOver;
        }
    }


    /// <summary>
    /// 오브젝트가 비활성화되거나 제거될 때
    /// 이벤트 구독을 해제한다.
    ///
    /// 구독 해제를 하지 않으면
    /// 오브젝트가 사라진 뒤에도 이벤트가 호출되는 등의
    /// 문제가 발생할 수 있다.
    /// </summary>
    private void OnDisable()
    {
        if (squadManager != null)
        {
            squadManager.OnGameOver -=
                HandleGameOver;
        }
    }


    /// <summary>
    /// SquadManager에서 GameOver가 발생했을 때 호출된다.
    ///
    /// 필요한 데이터를 각 시스템에서 가져와
    /// GameResultData로 묶은 뒤
    /// GameResultUI에 전달한다.
    /// </summary>
    private void HandleGameOver()
    {
        int level = 1;

        if (playerExperience != null)
        {
            level = playerExperience.CurrentLevel;
        }

        // GameManager 담당자가 GameTime 프로퍼티 추가 후 연결
        float survivalTime = 0f;

        if (gameManager != null)
        {
            survivalTime = gameManager.GameTime;
        }

        GameResultData resultData =
            new GameResultData(
                survivalTime,
                level
            );

        if (gameResultUI != null)
        {
            gameResultUI.Show(resultData);
        }
    }
}