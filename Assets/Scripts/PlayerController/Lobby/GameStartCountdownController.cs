using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 인게임 Scene 진입 직후 3, 2, 1, GO! 카운트다운을 진행한다.
///
/// 이 스크립트는 Player / Enemy / Stage / GameManager_KHM을 직접 참조하지 않는다.
/// 카운트다운 동안 Time.timeScale을 0으로 유지해 전체 게임 진행을 잠시 멈추고,
/// 완료되면 다시 1로 복구한다.
///
/// 다른 시스템이 "실제 게임 시작 시점"을 알아야 한다면
/// OnCountdownFinished 이벤트만 구독하면 된다.
/// </summary>
[DefaultExecutionOrder(-1000)]
public sealed class GameStartCountdownController : MonoBehaviour
{
    [Header("Countdown")]

    [SerializeField]
    [Min(1)]
    private int countdownSeconds = 3;

    [SerializeField]
    [Min(0f)]
    private float goDisplaySeconds = 0.6f;


    [Header("UI")]

    [SerializeField]
    private GameStartCountdownUI countdownUI;


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


    private void Awake()
    {
        ResolveReferences();

        // 다른 Start()보다 먼저 게임 진행을 막는다.
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
        // 다른 시스템이 실수로 Resume()을 호출하더라도
        // 카운트다운이 끝나기 전에는 게임이 진행되지 않게 한다.
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


        // Scene 전환 등으로 중간에 비활성화되어도
        // Time.timeScale이 0으로 남지 않게 한다.
        if (IsCountingDown)
        {
            IsCountingDown = false;
            Time.timeScale = 1f;
        }
    }


    private IEnumerator RunCountdown()
    {
        IsCountingDown = true;

        Time.timeScale = 0f;

        OnCountdownStarted?.Invoke();


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

            // timeScale == 0에서도 진행되어야 하므로
            // WaitForSecondsRealtime을 사용한다.
            yield return new WaitForSecondsRealtime(
                1f
            );
        }


        OnGo?.Invoke();


        if (countdownUI != null)
        {
            countdownUI.ShowGo();
        }


        if (goDisplaySeconds > 0f)
        {
            yield return new WaitForSecondsRealtime(
                goDisplaySeconds
            );
        }


        if (countdownUI != null)
        {
            countdownUI.Hide();
        }


        IsCountingDown = false;

        Time.timeScale = 1f;

        countdownRoutine = null;

        OnCountdownFinished?.Invoke();

        Debug.Log(
            "[GameStartCountdown] 게임 시작"
        );
    }


    private void ResolveReferences()
    {
        if (countdownUI != null)
        {
            return;
        }


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
}
