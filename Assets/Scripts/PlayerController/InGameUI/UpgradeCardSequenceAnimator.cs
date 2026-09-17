using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UpgradePanel에 생성된 강화 카드들을
/// 왼쪽부터 순서대로 Pop + Fade In 시킨다.
///
/// 강화 기능 자체에는 관여하지 않고
/// 시각적인 등장 연출만 담당한다.
/// </summary>
public sealed class UpgradeCardSequenceAnimator : MonoBehaviour
{
    [Header("Animation")]

    [SerializeField]
    [Min(0f)]
    private float startDelay = 0.12f;

    [SerializeField]
    [Min(0f)]
    private float cardDelay = 0.08f;

    [SerializeField]
    [Min(0.01f)]
    private float cardDuration = 0.22f;

    [SerializeField]
    [Range(0.5f, 1f)]
    private float startScale = 0.85f;


    private Coroutine sequenceRoutine;

    [Header("Selection")]

    [SerializeField]
    [Min(0.01f)]
    private float selectPopDuration = 0.18f;

    [SerializeField]
    [Range(1f, 1.3f)]
    private float selectedScale = 1.1f;

    [SerializeField]
    [Min(0.01f)]
    private float unselectedFadeDuration = 0.15f;

    [SerializeField]
    [Range(0.5f, 1f)]
    private float unselectedScale = 0.9f;


    /// <summary>
    /// 현재 생성된 UpgradeButton들을
    /// 순서대로 등장시킨다.
    /// </summary>
    public void Play(
        IReadOnlyList<UpgradeButton> buttons)
    {
        if (buttons == null ||
            buttons.Count == 0)
        {
            return;
        }


        if (sequenceRoutine != null)
        {
            StopCoroutine(
                sequenceRoutine
            );
        }


        PrepareButtons(
            buttons
        );


        sequenceRoutine =
            StartCoroutine(
                PlaySequence(
                    buttons
                )
            );
    }


    private void PrepareButtons(
        IReadOnlyList<UpgradeButton> buttons)
    {
        foreach (UpgradeButton button in buttons)
        {
            if (button == null)
            {
                continue;
            }


            CanvasGroup group =
                GetOrAddCanvasGroup(
                    button.gameObject
                );


            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;


            RectTransform rectTransform =
                button.transform as RectTransform;


            if (rectTransform != null)
            {
                rectTransform.localScale =
                    Vector3.one *
                    startScale;
            }
        }
    }


    private IEnumerator PlaySequence(
        IReadOnlyList<UpgradeButton> buttons)
    {
        if (startDelay > 0f)
        {
            yield return
                new WaitForSecondsRealtime(
                    startDelay
                );
        }


        for (int i = 0;
             i < buttons.Count;
             i++)
        {
            UpgradeButton button =
                buttons[i];


            if (button == null)
            {
                continue;
            }


            yield return
                AnimateCard(
                    button
                );


            if (cardDelay > 0f &&
                i < buttons.Count - 1)
            {
                yield return
                    new WaitForSecondsRealtime(
                        cardDelay
                    );
            }
        }


        sequenceRoutine = null;
    }


    private IEnumerator AnimateCard(
        UpgradeButton button)
    {
        CanvasGroup group =
            GetOrAddCanvasGroup(
                button.gameObject
            );


        RectTransform rectTransform =
            button.transform as RectTransform;


        float elapsed = 0f;


        while (elapsed < cardDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    cardDuration
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
        group.interactable = true;
        group.blocksRaycasts = true;


        if (rectTransform != null)
        {
            rectTransform.localScale =
                Vector3.one;
        }
    }


    private CanvasGroup GetOrAddCanvasGroup(
        GameObject target)
    {
        CanvasGroup group =
            target.GetComponent<CanvasGroup>();


        if (group == null)
        {
            group =
                target.AddComponent<CanvasGroup>();
        }


        return group;
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

    /// <summary>
    /// 강화 카드 선택 피드백.
    ///
    /// 선택한 카드는 Pop,
    /// 나머지 카드는 Fade Out 한다.
    /// </summary>
    public IEnumerator PlaySelection(
        IReadOnlyList<UpgradeButton> buttons,
        UpgradeButton selectedButton)
    {
        // 등장 애니메이션이 아직 실행 중이면 중지
        if (sequenceRoutine != null)
        {
            StopCoroutine(
                sequenceRoutine
            );

            sequenceRoutine = null;
        }


        // 모든 버튼 입력 차단
        foreach (UpgradeButton button in buttons)
        {
            if (button == null)
            {
                continue;
            }


            CanvasGroup group =
                GetOrAddCanvasGroup(
                    button.gameObject
                );


            group.interactable = false;
            group.blocksRaycasts = false;
        }


        // 선택한 카드 강조와
        // 나머지 카드 Fade Out을 동시에 실행
        Coroutine selectedRoutine = null;


        if (selectedButton != null)
        {
            selectedRoutine =
                StartCoroutine(
                    AnimateSelectedCard(
                        selectedButton
                    )
                );
        }


        foreach (UpgradeButton button in buttons)
        {
            if (button == null ||
                button == selectedButton)
            {
                continue;
            }


            StartCoroutine(
                AnimateUnselectedCard(
                    button
                )
            );
        }


        // 선택 카드 애니메이션이 끝날 때까지 대기
        if (selectedRoutine != null)
        {
            yield return selectedRoutine;
        }
        else
        {
            yield return
                new WaitForSecondsRealtime(
                    selectPopDuration
                );
        }
    }

    private IEnumerator AnimateSelectedCard(
    UpgradeButton button)
    {
        RectTransform rectTransform =
            button.transform as RectTransform;


        if (rectTransform == null)
        {
            yield break;
        }


        Vector3 normalScale =
            Vector3.one;

        Vector3 popScale =
            Vector3.one *
            selectedScale;


        // -------------------------
        // 커지기
        // -------------------------

        float halfDuration =
            selectPopDuration *
            0.5f;

        float elapsed = 0f;


        while (elapsed < halfDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    halfDuration
                );


            rectTransform.localScale =
                Vector3.Lerp(
                    normalScale,
                    popScale,
                    t
                );


            yield return null;
        }


        // -------------------------
        // 원래 크기로 복귀
        // -------------------------

        elapsed = 0f;


        while (elapsed < halfDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    halfDuration
                );


            rectTransform.localScale =
                Vector3.Lerp(
                    popScale,
                    normalScale,
                    t
                );


            yield return null;
        }


        rectTransform.localScale =
            normalScale;
    }

    private IEnumerator AnimateUnselectedCard(
    UpgradeButton button)
    {
        CanvasGroup group =
            GetOrAddCanvasGroup(
                button.gameObject
            );


        RectTransform rectTransform =
            button.transform as RectTransform;


        float startAlpha =
            group.alpha;

        Vector3 startCardScale =
            rectTransform != null
                ? rectTransform.localScale
                : Vector3.one;

        Vector3 endCardScale =
            Vector3.one *
            unselectedScale;


        float elapsed = 0f;


        while (elapsed <
               unselectedFadeDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    elapsed /
                    unselectedFadeDuration
                );


            group.alpha =
                Mathf.Lerp(
                    startAlpha,
                    0f,
                    t
                );


            if (rectTransform != null)
            {
                rectTransform.localScale =
                    Vector3.Lerp(
                        startCardScale,
                        endCardScale,
                        t
                    );
            }


            yield return null;
        }


        group.alpha = 0f;


        if (rectTransform != null)
        {
            rectTransform.localScale =
                endCardScale;
        }
    }
}