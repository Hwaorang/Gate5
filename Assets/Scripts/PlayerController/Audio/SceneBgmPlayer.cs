using UnityEngine;

/// <summary>
/// 현재 Scene에서 사용할 BGM을
/// AudioManager에게 요청한다.
///
/// AudioManager 자체는 Scene에 의존하지 않고,
/// 각 Scene이 자신의 BGM만 결정한다.
/// </summary>
public class SceneBgmPlayer : MonoBehaviour
{
    [Header("Scene BGM")]
    [SerializeField]
    private AudioClip bgm;


    private void Start()
    {
        PlaySceneBgm();
    }


    private void PlaySceneBgm()
    {
        AudioManager_PlayerController audioManager =
            AudioManager_PlayerController.Instance;


        if (audioManager == null)
        {
            Debug.LogWarning(
                "[SceneBgmPlayer] " +
                "AudioManager_PlayerController가 없습니다."
            );

            return;
        }


        if (bgm == null)
        {
            Debug.LogWarning(
                "[SceneBgmPlayer] " +
                "BGM AudioClip이 연결되지 않았습니다."
            );

            return;
        }


        audioManager.PlayBgm(
            bgm
        );
    }
}