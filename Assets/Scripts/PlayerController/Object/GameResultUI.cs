using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [Header("Result")]
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text levelText;

    [Header("Button")]
    [SerializeField] private Button retryButton;

    public event Action OnRetryClicked;


    private void Awake()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(
                HandleRetryButtonClicked
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
    }


    private void HandleRetryButtonClicked()
    {
        OnRetryClicked?.Invoke();
    }


    public void Show(GameResultData data)
    {
        gameObject.SetActive(true);

        survivalTimeText.text =
            FormatTime(data.SurvivalTime);

        levelText.text =
            $"Level {data.Level}";
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }


    private string FormatTime(float time)
    {
        int minutes =
            Mathf.FloorToInt(time / 60f);

        int seconds =
            Mathf.FloorToInt(time % 60f);

        return $"{minutes:00}:{seconds:00}";
    }
}