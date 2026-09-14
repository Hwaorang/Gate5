using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button lobbyButton;


    public event Action OnContinueClicked;
    public event Action OnSettingsClicked;
    public event Action OnRetryClicked;
    public event Action OnLobbyClicked;


    private void OnEnable()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(
                HandleContinue
            );
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(
                HandleSettings
            );
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(
                HandleRetry
            );
        }

        if (lobbyButton != null)
        {
            lobbyButton.onClick.AddListener(
                HandleLobby
            );
        }
    }


    private void OnDisable()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(
                HandleContinue
            );
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(
                HandleSettings
            );
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(
                HandleRetry
            );
        }

        if (lobbyButton != null)
        {
            lobbyButton.onClick.RemoveListener(
                HandleLobby
            );
        }
    }


    private void HandleContinue()
    {
        OnContinueClicked?.Invoke();
    }

    private void HandleSettings()
    {
        OnSettingsClicked?.Invoke();
    }

    private void HandleRetry()
    {
        OnRetryClicked?.Invoke();
    }

    private void HandleLobby()
    {
        OnLobbyClicked?.Invoke();
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}