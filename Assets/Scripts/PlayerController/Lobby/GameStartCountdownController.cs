using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 인게임 Scene 진입 직후
/// 3, 2, 1, GO! 카운트다운을 진행한다.
///
/// 카운트다운 중에는 게임을 멈추고,
/// 메뉴가 열려 있는 상태에서 카운트다운이 끝나면
/// 메뉴를 닫을 때까지 Pause 상태를 유지한다.
/// </summary>
[DefaultExecutionOrder(-1000)]
public sealed class GameStartCountdownController : MonoBehaviour
{
    // ========================================================
    // Pause UI
    // ========================================================

    [Header("Pause UI")]

    [SerializeField]
    private InGameMenuPresenter inGameMenuPresenter;


    // ========================================================
    // Countdown
    // ========================================================

    [Header("Countdown")]

    [SerializeField]
    [Min(1)]
    private int countdownSeconds = 3;

    [SerializeField]
    [Min(0f)]
    private float goDisplaySeconds = 0.6f;


    // ========================================================
    // UI
    // ========================================================

    [Header("UI")]

    [SerializeField]
    private GameStartCountdownUI countdownUI;


    // ========================================================
    // State
    // ========================================================

    public bool IsCountingDown
    {
        get;
        private set;
    }


    public event Action OnCountdownStarted;
    public event Action<int> OnCountChanged;
    public event Action OnGo;
    public event Action OnCountdownFinished;


    private Coroutine countdownRoutine;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        ResolveReferences();

        // 다른 Start보다 먼저
        // 게임이 진행되지 않도록 막는다.
        Time.timeScale = 0f;
    }


    private void Start()
    {
        countdownRoutine =
            StartCoroutine(
                RunCountdown()
            );
    }


    private void Update()
    {
        // 카운트다운 중 다른 시스템에서
        // Resume을 호출하더라도 다시 멈춘다.
        if (IsCountingDown &&
            Time.timeScale != 0f)
        {
            Time.timeScale = 0f;
        }
    }


    private void OnDisable()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(
                countdownRoutine
            );

            countdownRoutine = null;
        }


        if (!IsCountingDown)
        {
            return;
        }


        IsCountingDown = false;


        // 메뉴가 열린 상태라면
        // CountdownController가 비활성화되어도
        // 강제로 게임을 재개하지 않는다.
        if (inGameMenuPresenter != null &&
            inGameMenuPresenter.IsMenuPauseActive)
        {
            Time.timeScale = 0f;
            return;
        }


        Time.timeScale = 1f;
    }


    // ========================================================
    // Countdown
    // ========================================================

    private IEnumerator RunCountdown()
    {
        IsCountingDown = true;

        Time.timeScale = 0f;

        OnCountdownStarted?.Invoke();


        // =========================
        // 3, 2, 1
        // =========================

        for (int count = countdownSeconds;
             count >= 1;
             count--)
        {
            OnCountChanged?.Invoke(
                count
            );


            if (countdownUI != null)
            {
                countdownUI.ShowNumber(
                    count
                );
            }

            // HUD / Menu가 열려 있으면
            // 이 1초도 진행되지 않는다.
            yield return WaitCountdownTime(
                1f
            );
        }


        // =========================
        // GO
        // =========================

        OnGo?.Invoke();


        if (countdownUI != null)
        {
            countdownUI.ShowGo();
        }


        if (goDisplaySeconds > 0f)
        {
            yield return WaitCountdownTime(
                goDisplaySeconds
            );
        }


        if (countdownUI != null)
        {
            countdownUI.Hide();
        }


        // =========================
        // Countdown 완료
        // =========================

        IsCountingDown = false;


        // HUD / Menu / Settings가 열려 있다면
        // 사용자가 의도적으로 Pause한 것이므로
        // 게임을 시작하지 않는다.
        if (inGameMenuPresenter != null &&
            inGameMenuPresenter.IsMenuPauseActive)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }


        countdownRoutine = null;

        OnCountdownFinished?.Invoke();


        Debug.Log(
            "[GameStartCountdown] 카운트다운 완료"
        );
    }

    /// <summary>
    /// 실제 시간을 기준으로 기다리되,
    /// HUD / Menu / Settings가 열려 있는 동안에는
    /// 카운트다운 시간을 진행시키지 않는다.
    /// </summary>
    private IEnumerator WaitCountdownTime(
        float seconds)
    {
        float elapsedTime = 0f;


        while (elapsedTime < seconds)
        {
            // 메뉴가 열려 있지 않을 때만
            // 카운트다운 시간을 진행한다.
            bool isMenuPaused =
                inGameMenuPresenter != null &&
                inGameMenuPresenter.IsMenuPauseActive;


            if (!isMenuPaused)
            {
                elapsedTime +=
                    Time.unscaledDeltaTime;
            }


            yield return null;
        }
    }

    // ========================================================
    // Reference
    // ========================================================

    private void ResolveReferences()
    {
        // =========================
        // Countdown UI
        // =========================

        if (countdownUI == null)
        {
            countdownUI =
                FindFirstObjectByType<GameStartCountdownUI>(
                    FindObjectsInactive.Include
                );


            if (countdownUI == null)
            {
                Debug.LogWarning(
                    "[GameStartCountdown] " +
                    "GameStartCountdownUI를 찾지 못했습니다. " +
                    "카운트다운은 진행되지만 화면에는 표시되지 않습니다."
                );
            }
        }


        // =========================
        // InGame Menu
        // =========================

        if (inGameMenuPresenter == null)
        {
            inGameMenuPresenter =
                FindFirstObjectByType<InGameMenuPresenter>(
                    FindObjectsInactive.Include
                );
        }
    }
}