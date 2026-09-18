using UnityEngine;

/// <summary>
/// GameStartCountdownController의 이벤트를 받아
/// 카운트다운 효과음을 재생한다.
///
/// 역할:
/// - 숫자가 바뀔 때 Tick SFX 재생
/// - GO!가 표시될 때 Go SFX 재생
///
/// CountdownController는 AudioManager를 알지 않고,
/// AudioManager도 CountdownController를 알지 않는다.
///
/// 두 시스템 사이를 연결하는 Adapter 역할만 담당한다.
/// </summary>
public sealed class GameStartCountdownSound
    : MonoBehaviour
{
    // ========================================================
    // Reference
    // ========================================================

    [Header("Countdown")]

    [SerializeField]
    private GameStartCountdownController countdownController;


    // ========================================================
    // SFX Type
    // ========================================================

    [Header("SFX")]

    [SerializeField]
    private PlayerSfxType tickSfxType =
        PlayerSfxType.CountdownTick;

    [SerializeField]
    private PlayerSfxType goSfxType =
        PlayerSfxType.CountdownGo;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        ResolveReferences();
    }


    private void OnEnable()
    {
        ResolveReferences();
        Subscribe();
    }


    private void OnDisable()
    {
        Unsubscribe();
    }


    // ========================================================
    // Event
    // ========================================================

    /// <summary>
    /// 3 / 2 / 1이 표시될 때 호출된다.
    ///
    /// 현재 숫자 값 자체는 사운드 재생에 필요하지 않으므로
    /// 사용하지 않는다.
    /// </summary>
    private void HandleCountChanged(
        int count)
    {
        PlaySfx(
            tickSfxType
        );
    }


    /// <summary>
    /// GO!가 표시될 때 호출된다.
    /// </summary>
    private void HandleGo()
    {
        PlaySfx(
            goSfxType
        );
    }


    // ========================================================
    // Audio
    // ========================================================

    private void PlaySfx(
        PlayerSfxType type)
    {
        AudioManager_PlayerController audio =
            AudioManager_PlayerController.Instance;


        if (audio == null)
        {
            Debug.LogWarning(
                "[GameStartCountdownSound] " +
                "AudioManager_PlayerController가 없습니다."
            );

            return;
        }


        audio.PlaySfx(
            type
        );
    }


    // ========================================================
    // Subscribe
    // ========================================================

    private void Subscribe()
    {
        if (countdownController == null)
        {
            return;
        }


        // 중복 구독 방지
        countdownController.OnCountChanged -=
            HandleCountChanged;

        countdownController.OnGo -=
            HandleGo;


        countdownController.OnCountChanged +=
            HandleCountChanged;

        countdownController.OnGo +=
            HandleGo;
    }


    private void Unsubscribe()
    {
        if (countdownController == null)
        {
            return;
        }


        countdownController.OnCountChanged -=
            HandleCountChanged;

        countdownController.OnGo -=
            HandleGo;
    }


    // ========================================================
    // Reference
    // ========================================================

    private void ResolveReferences()
    {
        if (countdownController != null)
        {
            return;
        }


        // 같은 GameObject에 Controller가 있다면
        // 우선 그것을 사용한다.
        countdownController =
            GetComponent<GameStartCountdownController>();


        if (countdownController != null)
        {
            return;
        }


        // 다른 GameObject에 있다면 Scene에서 한 번 탐색한다.
        countdownController =
            FindFirstObjectByType<
                GameStartCountdownController
            >();


        if (countdownController == null)
        {
            Debug.LogWarning(
                "[GameStartCountdownSound] " +
                "GameStartCountdownController를 찾지 못했습니다."
            );
        }
    }
}