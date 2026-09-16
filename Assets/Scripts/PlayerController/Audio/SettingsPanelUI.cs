using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button closeButton;


    public event Action OnCloseClicked;


    private void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(
                HandleClose
            );
        }
    }


    private void OnDisable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(
                HandleClose
            );
        }
    }


    private void HandleClose()
    {
        // Presenter가 있다면
        // "Settings가 닫혔다"는 사실을 알려준다.
        OnCloseClicked?.Invoke();

        // SettingsPanel 자체는
        // 어느 Scene에서 사용하든 기본적으로 닫는다.
        Hide();
    }


    public void Show()
    {
        gameObject.SetActive(true);

        transform.SetAsLastSibling();
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
}