using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임 진행 중 발생하는 안내 메시지를
/// 화면에 일정 시간 동안 표시하는 UI.
///
/// 예:
/// - 모든 강화가 최대 레벨입니다.
/// - 특정 기능 사용 불가
/// - 간단한 상태 안내
///
/// Time.timeScale = 0 상태에서도 메시지가 사라질 수 있도록
/// WaitForSecondsRealtime을 사용한다.
/// </summary>
public class GameMessageUI : MonoBehaviour
{
    /// <summary>
    /// 다른 시스템에서 쉽게 메시지를 표시할 수 있도록
    /// 현재 GameMessageUI 인스턴스를 제공한다.
    /// </summary>
    public static GameMessageUI Instance
    {
        get;
        private set;
    }


    [Header("UI")]

    // 실제 안내 메시지를 표시할 TextMeshPro 텍스트
    [SerializeField] private TMP_Text messageText;


    [Header("설정")]

    // 메시지가 화면에 표시되는 시간
    [SerializeField] private float displayTime = 2f;


    // 현재 실행 중인 메시지 Coroutine
    //
    // 새로운 메시지가 들어오면
    // 기존 Coroutine을 중지하고 새 메시지를 표시한다.
    private Coroutine messageCoroutine;


    private void Awake()
    {
        // 동일한 GameMessageUI가 여러 개 생성되는 것을 방지
        if (Instance != null &&
            Instance != this)
        {
            Debug.LogWarning(
                "[GameMessageUI] 중복된 GameMessageUI가 존재합니다."
            );

            Destroy(gameObject);
            return;
        }

        Instance =
            this;


        // 게임 시작 시 메시지는 숨긴 상태로 시작
        if (messageText != null)
        {
            messageText.gameObject.SetActive(
                false
            );
        }
    }


    /// <summary>
    /// 화면에 안내 메시지를 표시한다.
    ///
    /// 이전 메시지가 아직 표시 중이라면
    /// 기존 Coroutine을 중지하고 새 메시지로 교체한다.
    /// </summary>
    public void ShowMessage(
        string message)
    {
        if (messageText == null)
        {
            return;
        }


        // 빈 메시지는 표시하지 않는다.
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }


        // 이전 메시지 Coroutine이 실행 중이면 중지
        if (messageCoroutine != null)
        {
            StopCoroutine(
                messageCoroutine
            );
        }


        // 새로운 메시지 출력 시작
        messageCoroutine =
            StartCoroutine(
                ShowMessageCoroutine(
                    message
                )
            );
    }


    /// <summary>
    /// 메시지를 화면에 표시하고
    /// 지정된 시간이 지나면 다시 숨긴다.
    ///
    /// WaitForSecondsRealtime을 사용하기 때문에
    /// 강화창처럼 Time.timeScale = 0인 상태에서도
    /// 메시지 표시 시간이 정상적으로 흐른다.
    /// </summary>
    private IEnumerator ShowMessageCoroutine(
        string message)
    {
        messageText.text =
            message;

        messageText.gameObject.SetActive(
            true
        );


        // 게임 시간이 멈춰 있어도
        // 실제 시간 기준으로 대기
        yield return new WaitForSecondsRealtime(
            Mathf.Max(
                0f,
                displayTime
            )
        );


        messageText.gameObject.SetActive(
            false
        );

        messageCoroutine =
            null;
    }


    private void OnDestroy()
    {
        // 현재 Instance가 자신일 때만 초기화
        if (Instance == this)
        {
            Instance =
                null;
        }
    }
}