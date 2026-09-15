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

    private void Awake()
    {
        SetupMenuLayout();
    }

    private void SetupMenuLayout()
    {
        Transform frameTransform = transform.Find("MenuFrame");

        if (frameTransform == null)
        {
            return;
        }

        RectTransform frame =
            frameTransform.GetComponent<RectTransform>();

        Image frameImage =
            frameTransform.GetComponent<Image>();


        // 중앙 고정
        frame.anchorMin =
            new Vector2(0.5f, 0.5f);

        frame.anchorMax =
            new Vector2(0.5f, 0.5f);

        frame.pivot =
            new Vector2(0.5f, 0.5f);

        frame.anchoredPosition =
            new Vector2(
                0f,
                -20f
            );


        // Sprite 원본 비율을 유지하면서 크게
        ResizeFrameBySprite(
            frameImage,
            1050f
        );


        // =========================
        // MenuFrame 이미지
        // =========================

        if (frameImage != null)
        {
            frameImage.type =
                Image.Type.Simple;

            frameImage.preserveAspect =
                true;
        }


        // =========================
        // Title
        // =========================

        Transform titleBarTransform = frameTransform.Find("TitleBar");

        if (titleBarTransform != null)
        {
            RectTransform titleBar =
                titleBarTransform.GetComponent<RectTransform>();

            titleBar.anchorMin =
                new Vector2(0.5f, 1f);

            titleBar.anchorMax =
                new Vector2(0.5f, 1f);

            titleBar.pivot =
                new Vector2(0.5f, 0.5f);

            // X = 0
            // Y를 더 낮게 내려주기
            titleBar.anchoredPosition =
                new Vector2(0f, -150f);
        }


        // =========================
        // ButtonList
        // =========================

        Transform buttonListTransform =
            frameTransform.Find("ButtonList");

        if (buttonListTransform == null)
        {
            return;
        }

        RectTransform buttonList =
            buttonListTransform.GetComponent<RectTransform>();

        buttonList.anchorMin =
            new Vector2(0.5f, 0.5f);

        buttonList.anchorMax =
            new Vector2(0.5f, 0.5f);

        buttonList.pivot =
            new Vector2(0.5f, 0.5f);

        buttonList.sizeDelta =
            new Vector2(
                520f,
                650f
            );

        buttonList.anchoredPosition =
            new Vector2(
                0f,
                -40f
            );


        VerticalLayoutGroup layout =
            buttonListTransform
                .GetComponent<VerticalLayoutGroup>();

        if (layout != null)
        {
            layout.padding =
                new RectOffset(
                    20,
                    20,
                    20,
                    20
                );

            layout.spacing = 30f;

            layout.childAlignment =
                TextAnchor.MiddleCenter;

            layout.childControlWidth = true;
            layout.childControlHeight = true;

            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }


        SetupButtonSize(
            continueButton,
            480f,
            105f
        );

        SetupButtonSize(
            settingsButton,
            480f,
            105f
        );

        SetupButtonSize(
            retryButton,
            480f,
            105f
        );

        SetupButtonSize(
            lobbyButton,
            480f,
            105f
        );


        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            buttonList
        );
    }


    private void SetupButtonSize(
        Button button,
        float width,
        float height)
    {
        if (button == null)
        {
            return;
        }

        LayoutElement layout =
            button.GetComponent<LayoutElement>();

        if (layout == null)
        {
            layout =
                button.gameObject
                    .AddComponent<LayoutElement>();
        }

        layout.preferredWidth =
            width;

        layout.preferredHeight =
            height;

        layout.minHeight =
            height;
    }


    private void OnEnable()
    {
        SetupMenuLayout();

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

        // Canvas에서 가장 위에 렌더링
        transform.SetAsLastSibling();

        SetupMenuLayout();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ResizeFrameBySprite(
    Image image,
    float targetHeight)
    {
        if (image == null ||
            image.sprite == null)
        {
            return;
        }

        RectTransform rect =
            image.rectTransform;

        float spriteWidth =
            image.sprite.rect.width;

        float spriteHeight =
            image.sprite.rect.height;

        if (spriteHeight <= 0f)
        {
            return;
        }

        // 원본 이미지 비율
        float aspect =
            spriteWidth / spriteHeight;

        // 높이를 기준으로 가로 자동 계산
        float targetWidth =
            targetHeight * aspect;

        rect.sizeDelta =
            new Vector2(
                targetWidth,
                targetHeight
            );

        image.type =
            Image.Type.Simple;

        image.preserveAspect =
            true;
    }
}