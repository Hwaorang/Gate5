using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button closeButton;

    [Header("Transition")]

    [SerializeField]
    private UIPanelTransition transition;

    public event Action OnCloseClicked;

    private void Awake()
    {
        ResolveReferences();
    }


    private void ResolveReferences()
    {
        if (transition == null)
        {
            transition =
                GetComponent<UIPanelTransition>();
        }
    }

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
        ResolveReferences();


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
    }


    public void Hide()
    {
        ResolveReferences();


        if (transition != null)
        {
            transition.Hide();
        }
        else
        {
            gameObject.SetActive(
                false
            );
        }
    }


    public void HideImmediate()
    {
        ResolveReferences();


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
}