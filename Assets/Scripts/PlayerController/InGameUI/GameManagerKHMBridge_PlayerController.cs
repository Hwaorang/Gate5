
using System.Collections;
using UnityEngine;

/// <summary>
/// GameManager의 게임 시작 보조.
///
/// 팀원 쪽에서 StartGame()을 호출하면 아무것도 하지 않고,
/// 호출되지 않은 경우에만 대신 StartGame()을 호출한다.
/// </summary>
public class GameManagerKHMBridge_PlayerController
    : MonoBehaviour
{
    private IEnumerator Start()
    {
        // GameManager 생성 대기
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        // 한 프레임 기다린다.
        //
        // 다른 팀원 코드의 Start()가
        // 먼저 StartGame()을 호출할 기회를 준다.
        yield return null;


        // 아직 게임이 시작되지 않았다면
        // 대신 StartGame()을 호출한다.
        if (!GameManager.Instance.IsGameStarted)
        {
            GameManager.Instance.StartGame();

#if UNITY_EDITOR
            Debug.Log(
                "[GameManagerBridge] " +
                "StartGame 호출이 없어서 대신 시작했습니다."
            );
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log(
                "[GameManagerBridge] " +
                "이미 게임이 시작되어 있으므로 " +
                "StartGame을 호출하지 않습니다."
            );
#endif
        }
    }
}

