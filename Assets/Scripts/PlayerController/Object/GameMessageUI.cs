using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 진행 중 발생하는 안내 메시지를
/// 화면에 잠깐 표시하는 UI
/// </summary>
public class GameMessageUI : MonoBehaviour
{
    public static GameMessageUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text messageText;

    [Header("설정")]
    [SerializeField] private float displayTime = 2f;

    private Coroutine messageCoroutine;

    private void Awake()
    {
        Instance = this;

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        ShowMessage("test message");
    }

    /// <summary>
    /// 화면에 메시지를 출력한다.
    /// </summary>
    public void ShowMessage(string message)
    {
        if (messageText == null)
        {
            return;
        }

        // 이전 메시지 Coroutine이 실행 중이면 중지
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine =
            StartCoroutine(
                ShowMessageCoroutine(message)
            );
    }

    private IEnumerator ShowMessageCoroutine(
        string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Time.timeScale = 0인 강화창에서도
        // 시간이 흐르도록 Realtime 사용
        yield return new WaitForSecondsRealtime(
            displayTime
        );

        messageText.gameObject.SetActive(false);
    }
}