using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// GameOverPanel 내부 요소의 순차 등장 연출.
///
/// UIPanelTransition:
///     전체 패널 등장 담당
///
/// GameResultSequenceAnimator:
///     GAME OVER / 생존시간 / 레벨 / 버튼의
///     순차 등장과 숫자 Count Up 담당
/// </summary>
public sealed class GameResultSequenceAnimator : MonoBehaviour
{
    // ========================================================
    // UI Group
    // ========================================================

    [Header("UI Groups")]

    [SerializeField]
    private CanvasGroup titleGroup;

    [SerializeField]
    private CanvasGroup survivalGroup;

    [SerializeField]
    private CanvasGroup levelGroup;

    [SerializeField]
    private CanvasGroup retryButtonGroup;

    [SerializeField]
    private CanvasGroup lobbyButtonGroup;

    [SerializeField]
    private CanvasGroup goldGroup;


    // ========================================================
    // Value Text
    // ========================================================

    [Header("Value Text")]

    [SerializeField]
    private TMP_Text survivalTimeText;

    [SerializeField]
    private TMP_Text levelText;


    // ========================================================
    // Timing
    // ========================================================

    [Header("Timing")]

    [SerializeField]
    [Min(0f)]
    private float startDelay = 0.15f;

    [SerializeField]
    [Min(0.01f)]
    private float elementDuration = 0.18f;

    [SerializeField]
    [Min(0f)]
    private float elementDelay = 0.08f;

    [SerializeField]
    [Min(0.01f)]
    private float survivalCountDuration = 0.65f;

    [SerializeField]
    [Min(0.01f)]
    private float levelCountDuration = 0.45f;

    [SerializeField]
    [Range(0.5f, 1f)]
    private float startScale = 0.9f;


    // ========================================================
    // Runtime
    // ========================================================

    private Coroutine sequenceRoutine;


    // ========================================================
    // Public
    // ========================================================

    public void Play(
        GameResultData data)
    {
        if (sequenceRoutine != null)
        {
            StopCoroutine(
                sequenceRoutine
            );
        }


        PrepareInitialState();


        sequenceRoutine =
            StartCoroutine(
                PlaySequence(data)
            );
    }


    public void Stop()
    {
        if (sequenceRoutine == null)
        {
            return;
        }


        StopCoroutine(
            sequenceRoutine
        );

        sequenceRoutine = null;
    }


    // ========================================================
    // Sequence
    // ========================================================

    private IEnumerator PlaySequence(
        GameResultData data)
    {
        if (startDelay > 0f)
        {
            yield return
                new WaitForSecondsRealtime(
                    startDelay
                );
        }


        // GAME OVER
        yield return
            AnimateGroup(
                titleGroup
            );


        yield return
            WaitElementDelay();


        // Survival
        yield return
            AnimateGroup(
                survivalGroup
            );


        yield return
            CountSurvivalTime(
                data.SurvivalTime
            );


        yield return
            WaitElementDelay();


        // Level
        yield return
            AnimateGroup(
                levelGroup
            );


        yield return
            CountLevel(
                data.Level
            );


        yield return
            WaitElementDelay();

        // Gold
        yield return
            AnimateGroup(
                goldGroup
            );


        yield return
            WaitElementDelay();

        // Buttons
        StartCoroutine(
            AnimateGroup(
                retryButtonGroup
            )
        );

        yield return
            AnimateGroup(
                lobbyButtonGroup
            );


        sequenceRoutine = null;
    }


    // ========================================================
    // Initial State
    // ========================================================

    private void PrepareInitialState()
    {
        SetHidden(
            titleGroup
        );

        SetHidden(
            survivalGroup
        );

        SetHidden(
            levelGroup
        );

        SetHidden(
            goldGroup
        );

        SetHidden(
            retryButtonGroup
        );

        SetHidden(
            lobbyButtonGroup
        );


        if (survivalTimeText != null)
        {
            survivalTimeText.text =
                "00:00";
        }


        if (levelText != null)
        {
            levelText.text =
                "Lv. 1";
        }
    }

    private void SetHidden(
    CanvasGroup group)
    {
        if (group == null)
        {
            return;
        }


        group.alpha = 0f;

        // 등장하기 전에는 클릭 불가능
        group.interactable = false;
        group.blocksRaycasts = false;


        RectTransform rectTransform =
            group.transform as RectTransform;


        if (rectTransform != null)
        {
            rectTransform.localScale =
                Vector3.one *
                startScale;
        }
    }


    // ========================================================
    // Element Animation
    // ========================================================

    private IEnumerator AnimateGroup(
        CanvasGroup group)
    {
        if (group == null)
        {
            yield break;
        }


        RectTransform rectTransform =
            group.transform as RectTransform;


        float elapsed = 0f;


        while (elapsed < elementDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    elementDuration
                );


            float eased =
                EaseOutBack(
                    t
                );


            group.alpha =
                t;


            if (rectTransform != null)
            {
                rectTransform.localScale =
                    Vector3.LerpUnclamped(
                        Vector3.one *
                        startScale,
                        Vector3.one,
                        eased
                    );
            }


            yield return null;
        }


        group.alpha = 1f;


        if (rectTransform != null)
        {
            rectTransform.localScale =
                Vector3.one;
        }

        group.interactable = true;
        group.blocksRaycasts = true;
    }


    // ========================================================
    // Survival Count
    // ========================================================

    private IEnumerator CountSurvivalTime(
        float targetTime)
    {
        if (survivalTimeText == null)
        {
            yield break;
        }


        float elapsed = 0f;


        while (elapsed < survivalCountDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    survivalCountDuration
                );


            float currentTime =
                Mathf.Lerp(
                    0f,
                    targetTime,
                    t
                );


            survivalTimeText.text =
                FormatTime(
                    currentTime
                );


            yield return null;
        }


        survivalTimeText.text =
            FormatTime(
                targetTime
            );
    }


    // ========================================================
    // Level Count
    // ========================================================

    private IEnumerator CountLevel(
        int targetLevel)
    {
        if (levelText == null)
        {
            yield break;
        }


        targetLevel =
            Mathf.Max(
                1,
                targetLevel
            );


        float elapsed = 0f;


        while (elapsed < levelCountDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    levelCountDuration
                );


            int currentLevel =
                Mathf.RoundToInt(
                    Mathf.Lerp(
                        1f,
                        targetLevel,
                        t
                    )
                );


            levelText.text =
                $"Lv. {currentLevel}";


            yield return null;
        }


        levelText.text =
            $"Lv. {targetLevel}";
    }


    // ========================================================
    // Utility
    // ========================================================

    private IEnumerator WaitElementDelay()
    {
        if (elementDelay <= 0f)
        {
            yield break;
        }


        yield return
            new WaitForSecondsRealtime(
                elementDelay
            );
    }


    private string FormatTime(
        float time)
    {
        int minutes =
            Mathf.FloorToInt(
                time / 60f
            );


        int seconds =
            Mathf.FloorToInt(
                time % 60f
            );


        return
            $"{minutes:00}:{seconds:00}";
    }


    private float EaseOutBack(
        float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;


        return
            1f +
            c3 *
            Mathf.Pow(
                t - 1f,
                3f
            ) +
            c1 *
            Mathf.Pow(
                t - 1f,
                2f
            );
    }
}