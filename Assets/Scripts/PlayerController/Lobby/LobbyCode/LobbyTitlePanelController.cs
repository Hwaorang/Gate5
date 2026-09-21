using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로비 최초 진입 시 타이틀 화면을 표시하고,
/// 클릭하면 타이틀 화면을 닫는다.
/// </summary>
public sealed class LobbyTitlePanelController : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField]
    private Button enterButton;

    [SerializeField]
    private UIPanelTransition transition;


    private bool entered;


    private void Awake()
    {
        ResolveReferences();
    }


    private void OnEnable()
    {
        ResolveReferences();


        if (enterButton != null)
        {
            enterButton.onClick.AddListener(
                HandleEnterClicked
            );
        }
    }


    private void OnDisable()
    {
        if (enterButton != null)
        {
            enterButton.onClick.RemoveListener(
                HandleEnterClicked
            );
        }
    }


    private void Start()
    {
        entered = false;


        // =========================
        // 게임에서 Lobby로 복귀
        // =========================

        if (LobbyNavigationContext.CurrentReason ==
            LobbyNavigationContext.EntryReason.ReturnFromGame)
        {
            // 진입 이유 사용 완료
            LobbyNavigationContext.Consume();


            // Title 화면을 보여주지 않고
            // 바로 기존 Lobby UI를 보이게 한다.
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


            return;
        }


        // =========================
        // 최초 Lobby 진입
        // =========================

        if (transition != null)
        {
            transition.ShowImmediate();
        }
        else
        {
            gameObject.SetActive(
                true
            );
        }
    }


    private void HandleEnterClicked()
    {
        if (entered)
        {
            return;
        }


        entered = true;


        if (enterButton != null)
        {
            enterButton.interactable =
                false;
        }


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


    private void ResolveReferences()
    {
        if (enterButton == null)
        {
            enterButton =
                GetComponent<Button>();
        }


        if (transition == null)
        {
            transition =
                GetComponent<UIPanelTransition>();
        }
    }
}