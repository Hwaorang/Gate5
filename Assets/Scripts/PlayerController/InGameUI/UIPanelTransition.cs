using System.Collections;
using UnityEngine;

/// <summary>
/// 여러 UI Panel에서 공통으로 사용할 등장 / 퇴장 애니메이션.
///
/// 역할
/// - Fade
/// - Scale
/// - Fade + Scale
/// - Slide
///
/// Pause / GameOver처럼 Time.timeScale == 0 상태에서도
/// 동작할 수 있도록 Unscaled Time을 지원한다.
/// </summary>
public sealed class UIPanelTransition : MonoBehaviour
{
    public enum TransitionType
    {
        Fade,
        Scale,
        FadeAndScale,
        Slide
    }


    // ========================================================
    // Reference
    // ========================================================

    [Header("Reference")]

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private RectTransform contentRoot;


    // ========================================================
    // Animation
    // ========================================================

    [Header("Animation")]

    [SerializeField]
    private TransitionType transitionType =
        TransitionType.FadeAndScale;

    [SerializeField]
    [Min(0.01f)]
    private float duration = 0.22f;

    public float Duration =>
    duration;

    [SerializeField]
    [Range(0.5f, 1f)]
    private float startScale = 0.92f;

    [SerializeField]
    private Vector2 slideOffset =
        new Vector2(
            0f,
            -80f
        );

    [SerializeField]
    private bool useUnscaledTime = true;


    // ========================================================
    // Runtime
    // ========================================================

    private Coroutine transitionRoutine;

    private Vector3 originalScale;
    private Vector2 originalPosition;

    private bool initialized;


    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        Initialize();
    }


    // ========================================================
    // Initialize
    // ========================================================

    private void Initialize()
    {
        if (initialized)
        {
            return;
        }


        if (contentRoot == null)
        {
            contentRoot =
                transform as RectTransform;
        }


        if (canvasGroup == null)
        {
            canvasGroup =
                GetComponent<CanvasGroup>();
        }


        if (canvasGroup == null)
        {
            canvasGroup =
                gameObject.AddComponent<CanvasGroup>();
        }


        if (contentRoot != null)
        {
            originalScale =
                contentRoot.localScale;

            originalPosition =
                contentRoot.anchoredPosition;
        }


        initialized = true;
    }


    // ========================================================
    // Show
    // ========================================================

    public void Show()
    {
        Initialize();


        gameObject.SetActive(
            true
        );


        StopCurrentTransition();


        transitionRoutine =
            StartCoroutine(
                PlayShow()
            );
    }


    private IEnumerator PlayShow()
    {
        PrepareShowState();


        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed +=
                GetDeltaTime();


            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );


            float eased =
                EaseOutCubic(
                    t
                );


            ApplyShowState(
                eased
            );


            yield return null;
        }


        ApplyShowState(
            1f
        );


        transitionRoutine = null;
    }


    // ========================================================
    // Hide
    // ========================================================

    public void Hide()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }


        Initialize();

        StopCurrentTransition();


        transitionRoutine =
            StartCoroutine(
                PlayHide()
            );
    }


    private IEnumerator PlayHide()
    {
        float elapsed = 0f;


        while (elapsed < duration)
        {
            elapsed +=
                GetDeltaTime();


            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );


            float eased =
                EaseInCubic(
                    t
                );


            ApplyHideState(
                eased
            );


            yield return null;
        }


        ApplyHideState(
            1f
        );


        transitionRoutine = null;


        gameObject.SetActive(
            false
        );
    }


    // ========================================================
    // Immediate
    // ========================================================

    /// <summary>
    /// 애니메이션 없이 즉시 숨긴다.
    ///
    /// Scene 시작 시 처음부터 숨겨야 하는 UI 등에 사용한다.
    /// </summary>
    public void HideImmediate()
    {
        Initialize();

        StopCurrentTransition();

        RestoreOriginalState();

        canvasGroup.alpha = 0f;

        gameObject.SetActive(
            false
        );
    }


    /// <summary>
    /// 애니메이션 없이 즉시 표시한다.
    /// </summary>
    public void ShowImmediate()
    {
        Initialize();

        StopCurrentTransition();

        gameObject.SetActive(
            true
        );

        RestoreOriginalState();

        canvasGroup.alpha = 1f;
    }


    // ========================================================
    // Animation State
    // ========================================================

    private void PrepareShowState()
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                canvasGroup.alpha = 0f;
                break;


            case TransitionType.Scale:
                canvasGroup.alpha = 1f;

                contentRoot.localScale =
                    originalScale *
                    startScale;

                break;


            case TransitionType.FadeAndScale:
                canvasGroup.alpha = 0f;

                contentRoot.localScale =
                    originalScale *
                    startScale;

                break;


            case TransitionType.Slide:
                canvasGroup.alpha = 0f;

                contentRoot.anchoredPosition =
                    originalPosition +
                    slideOffset;

                break;
        }
    }


    private void ApplyShowState(
        float t)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:

                canvasGroup.alpha =
                    t;

                break;


            case TransitionType.Scale:

                contentRoot.localScale =
                    Vector3.LerpUnclamped(
                        originalScale * startScale,
                        originalScale,
                        t
                    );

                break;


            case TransitionType.FadeAndScale:

                canvasGroup.alpha =
                    t;

                contentRoot.localScale =
                    Vector3.LerpUnclamped(
                        originalScale * startScale,
                        originalScale,
                        t
                    );

                break;


            case TransitionType.Slide:

                canvasGroup.alpha =
                    t;

                contentRoot.anchoredPosition =
                    Vector2.LerpUnclamped(
                        originalPosition + slideOffset,
                        originalPosition,
                        t
                    );

                break;
        }
    }


    private void ApplyHideState(
        float t)
    {
        switch (transitionType)
        {
            case TransitionType.Fade:

                canvasGroup.alpha =
                    1f - t;

                break;


            case TransitionType.Scale:

                contentRoot.localScale =
                    Vector3.LerpUnclamped(
                        originalScale,
                        originalScale * startScale,
                        t
                    );

                break;


            case TransitionType.FadeAndScale:

                canvasGroup.alpha =
                    1f - t;

                contentRoot.localScale =
                    Vector3.LerpUnclamped(
                        originalScale,
                        originalScale * startScale,
                        t
                    );

                break;


            case TransitionType.Slide:

                canvasGroup.alpha =
                    1f - t;

                contentRoot.anchoredPosition =
                    Vector2.LerpUnclamped(
                        originalPosition,
                        originalPosition + slideOffset,
                        t
                    );

                break;
        }
    }


    // ========================================================
    // Utility
    // ========================================================

    private void RestoreOriginalState()
    {
        if (contentRoot != null)
        {
            contentRoot.localScale =
                originalScale;

            contentRoot.anchoredPosition =
                originalPosition;
        }
    }


    private void StopCurrentTransition()
    {
        if (transitionRoutine == null)
        {
            return;
        }


        StopCoroutine(
            transitionRoutine
        );

        transitionRoutine = null;
    }


    private float GetDeltaTime()
    {
        return useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;
    }


    private float EaseOutCubic(
        float t)
    {
        return
            1f -
            Mathf.Pow(
                1f - t,
                3f
            );
    }


    private float EaseInCubic(
        float t)
    {
        return
            t * t * t;
    }
}