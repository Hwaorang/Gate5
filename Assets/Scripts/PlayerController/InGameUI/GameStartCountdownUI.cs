using TMPro;
using UnityEngine;

/// <summary>
/// 게임 시작 카운트다운의 화면 표시만 담당한다.
///
/// 게임 상태나 Player / Enemy 등의 시스템은 알지 않는다.
/// GameStartCountdownController가 전달하는 숫자와 GO!만 표시한다.
/// </summary>
public sealed class GameStartCountdownUI : MonoBehaviour
{
    [Header("UI")]

    [SerializeField]
    private GameObject contentRoot;

    [SerializeField]
    private TMP_Text countdownText;


    private void Awake()
    {
        ResolveReferences();

        Hide();
    }


    public void ShowNumber(
        int number)
    {
        Show();

        if (countdownText != null)
        {
            countdownText.text =
                number.ToString();
        }
    }


    public void ShowGo()
    {
        Show();

        if (countdownText != null)
        {
            countdownText.text =
                "GO!";
        }
    }


    public void Show()
    {
        if (contentRoot != null)
        {
            contentRoot.SetActive(
                true
            );
        }
    }


    public void Hide()
    {
        if (contentRoot != null)
        {
            contentRoot.SetActive(
                false
            );
        }
    }


    private void ResolveReferences()
    {
        if (contentRoot == null)
        {
            contentRoot =
                gameObject;
        }


        if (countdownText == null)
        {
            countdownText =
                GetComponentInChildren<TMP_Text>(
                    true
                );
        }
    }
}
