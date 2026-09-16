using System.Collections;
using UnityEngine;

/// <summary>
/// GameManager_KHM의 게임 시작 보조.
///
/// 팀원 쪽에서 StartGame()을 호출하면 아무것도 하지 않고,
/// 호출되지 않은 경우에만 대신 StartGame()을 호출한다.
/// </summary>
public class GameManagerKHMBridge_PlayerController
    : MonoBehaviour
{
    private IEnumerator Start()
    {
        // GameManager_KHM 생성 대기
        while (GameManager_KHM.Instance == null)
        {
            yield return null;
        }

        // 한 프레임 기다린다.
        //
        // GameManager_KHM 또는 다른 팀원 코드의 Start()가
        // 먼저 StartGame()을 호출할 기회를 준다.
        yield return null;


        // 아직도 Ready라는 것은
        // 아무도 게임을 시작시키지 않았다는 뜻이다.
        if (GameManager_KHM.Instance.CurrentState ==
            GameManager_KHM.GameState.Ready)
        {
            GameManager_KHM.Instance.StartGame();
#if UNITY_EDITOR
            Debug.Log(
                "[GameManagerKHMBridge] " +
                "StartGame 호출이 없어서 대신 시작했습니다."
            );
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log(
                "[GameManagerKHMBridge] " +
                "이미 게임이 시작되어 있으므로 " +
                "StartGame을 호출하지 않습니다."
            );
#endif
        }
    }
}