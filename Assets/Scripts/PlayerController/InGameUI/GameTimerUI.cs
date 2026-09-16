using TMPro;
using UnityEngine;

/// <summary>
/// HUD의 생존 시간을 표시한다.
///
/// 생존 시간의 원본 데이터는
/// GameManager_KHM.GameTime 하나만 사용한다.
/// </summary>
public class GameTimerUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TMP_Text timeText;


    [Header("Time Source")]
    [SerializeField]
    private GameManager_KHM gameManagerKHM;


    private void Awake()
    {
        if (timeText == null)
        {
            timeText =
                GetComponent<TMP_Text>();
        }
    }


    private void Start()
    {
        ResolveGameManager();

        Refresh();
    }


    private void Update()
    {
        // Scene 초기화 순서 때문에 Start 시점에 못 찾았을 경우
        // 이후 한 번 더 찾을 수 있도록 한다.
        if (gameManagerKHM == null)
        {
            ResolveGameManager();

            if (gameManagerKHM == null)
            {
                return;
            }
        }

        Refresh();
    }


    private void ResolveGameManager()
    {
        if (gameManagerKHM != null)
        {
            return;
        }

        gameManagerKHM =
            FindFirstObjectByType<GameManager_KHM>();
    }


    private void Refresh()
    {
        if (timeText == null ||
            gameManagerKHM == null)
        {
            return;
        }

        timeText.text =
            FormatTime(
                gameManagerKHM.GameTime
            );
    }


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
