using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameOver 결과 UI.
///
/// 역할
/// - 결과값 표시
/// - Retry / Lobby 입력 전달
/// - 실제 등장/퇴장 애니메이션은
///   UIPanelTransition에게 맡긴다.
/// </summary>
public class GameResultUI : MonoBehaviour
{
    // ========================================================
    // Result
    // ========================================================

    [Header("Result")]

    [SerializeField]
    private TMP_Text survivalTimeText;

    [SerializeField]
    private TMP_Text levelText;


    // ========================================================
    // Button
    // ========================================================

    [Header("Button")]

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button lobbyButton;


    // ========================================================
    // Transition
    // ========================================================

    [Header("Transition")]

    [SerializeField]
    private UIPanelTransition transition;


    // ========================================================
    // Event
    // ========================================================

    public event Action OnRetryClicked;
    public event Action OnLobbyClicked;

    [Header("Sequence")]

    [SerializeField]
    private GameResultSequenceAnimator sequenceAnimator;

    // ========================================================
    // Unity
    // ========================================================

    private void Awake()
    {
        ResolveReferences();


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


    // ========================================================
    // Button
    // ========================================================

    private void HandleRetryButtonClicked()
    {
        OnRetryClicked?.Invoke();
    }


    private void HandleLobbyButtonClicked()
    {
        OnLobbyClicked?.Invoke();
    }


    // ========================================================
    // Show / Hide
    // ========================================================

    public void Show(
    GameResultData data)
    {
        // 패널 전체 등장
        if (transition != null)
        {
            transition.Show();
        }
        else
        {
            gameObject.SetActive(
                true
            );
        }


        // 내부 결과 순차 연출
        if (sequenceAnimator != null)
        {
            sequenceAnimator.Play(
                data
            );
        }
        else
        {
            // SequenceAnimator가 없는 경우를 위한 fallback
            if (survivalTimeText != null)
            {
                survivalTimeText.text =
                    FormatTime(
                        data.SurvivalTime
                    );
            }


            if (levelText != null)
            {
                levelText.text =
                    $"Lv. {data.Level}";
            }
        }
    }


    /// <summary>
    /// Scene 시작 시 GameOverPanel을
    /// 애니메이션 없이 바로 숨긴다.
    /// </summary>
    public void Hide()
    {
        if (sequenceAnimator != null)
        {
            sequenceAnimator.Stop();
        }

        if (transition != null)
        {
            transition.HideImmediate();
        }
        else
        {
            gameObject.SetActive(
                false
            );
        }
    }


    // ========================================================
    // Reference
    // ========================================================

    private void ResolveReferences()
    {
        if (transition == null)
        {
            transition =
                GetComponent<UIPanelTransition>();
        }

        if (sequenceAnimator == null)
        {
            sequenceAnimator =
                GetComponentInChildren
                    <GameResultSequenceAnimator>(
                        true
                    );
        }
    }


    // ========================================================
    // Format
    // ========================================================

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
}