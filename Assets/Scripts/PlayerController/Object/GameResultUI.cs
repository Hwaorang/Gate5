using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameOver 결과 화면을 표시한다.
///
/// 역할
/// - 생존 시간 / 레벨 표시
/// - Retry / Lobby 버튼 이벤트 전달
/// - Time.timeScale == 0 상태에서도 동작하는 간단한 등장 애니메이션
/// </summary>
public class GameResultUI : MonoBehaviour
{
    [Header("Result")]
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text levelText;

    [Header("Button")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button lobbyButton;

    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform contentRoot;

    [SerializeField]
    [Range(0.05f, 1f)]
    private float showDuration = 0.22f;

    public event Action OnRetryClicked;
    public event Action OnLobbyClicked;

    private Coroutine showRoutine;


    private void Awake()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(
                HandleRetryButtonClicked
            );
        }

        if (lobbyButton != null)
        {
            lobbyButton.onClick.AddListener(
                HandleLobbyButtonClicked
            );
        }
    }


    private void OnDestroy()
    {
        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(
                HandleRetryButtonClicked
            );
        }

        if (lobbyButton != null)
        {
            lobbyButton.onClick.RemoveListener(
                HandleLobbyButtonClicked
            );
        }
    }


    private void HandleRetryButtonClicked()
    {
        OnRetryClicked?.Invoke();
    }


    private void HandleLobbyButtonClicked()
    {
        OnLobbyClicked?.Invoke();
    }


    public void Show(GameResultData data)
    {
        gameObject.SetActive(true);

        if (survivalTimeText != null)
        {
            survivalTimeText.text =
                FormatTime(data.SurvivalTime);
        }

        if (levelText != null)
        {
            levelText.text =
                $"Lv. {data.Level}";
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        showRoutine =
            StartCoroutine(
                PlayShowAnimation()
            );
    }


    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        gameObject.SetActive(false);
    }


    private IEnumerator PlayShowAnimation()
    {
        if (canvasGroup == null ||
            contentRoot == null)
        {
            yield break;
        }

        canvasGroup.alpha = 0f;

        Vector3 startScale =
            Vector3.one * 0.92f;

        Vector3 endScale =
            Vector3.one;

        contentRoot.localScale =
            startScale;

        float elapsed = 0f;

        while (elapsed < showDuration)
        {
            elapsed +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / showDuration
                );

            // 부드러운 EaseOut
            float eased =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            canvasGroup.alpha =
                eased;

            contentRoot.localScale =
                Vector3.LerpUnclamped(
                    startScale,
                    endScale,
                    eased
                );

            yield return null;
        }

        canvasGroup.alpha = 1f;
        contentRoot.localScale = Vector3.one;

        showRoutine = null;
    }


    private string FormatTime(float time)
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
}
