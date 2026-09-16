using TMPro;
using UnityEngine;

/// <summary>
/// Gate의 실제 3D 크기를 변경한다.
///
/// Gate 전체 Transform Scale을 늘리는 대신
/// Panel / Frame / Trigger를 각각 조절하여
/// 프레임 두께나 Text가 찌그러지는 것을 방지한다.
/// </summary>
public class GateSizeController : MonoBehaviour
{
    [Header("Visual")]

    [SerializeField]
    private Transform glassPanel;

    [SerializeField]
    private Transform leftPost;

    [SerializeField]
    private Transform rightPost;

    [SerializeField]
    private Transform topRail;

    [SerializeField]
    private Transform bottomRail;


    [Header("UI")]

    [SerializeField]
    private TMP_Text valueText;


    [Header("Trigger")]

    [SerializeField]
    private BoxCollider triggerCollider;


    [Header("Frame")]

    [SerializeField]
    private float postThickness =
        0.25f;

    [SerializeField]
    private float railThickness =
        0.22f;

    [SerializeField]
    private float frameDepth =
        0.3f;

    [SerializeField]
    private float panelDepth =
        0.08f;

    [SerializeField]
    private float triggerDepth =
        1f;

    private void Awake()
    {
        ResolveReferences();
    }


    private void ResolveReferences()
    {
        if (glassPanel == null)
        {
            glassPanel =
                transform.Find(
                    "Visual/GlassPanel"
                );
        }


        if (leftPost == null)
        {
            leftPost =
                transform.Find(
                    "Visual/LeftPost"
                );
        }


        if (rightPost == null)
        {
            rightPost =
                transform.Find(
                    "Visual/RightPost"
                );
        }


        if (topRail == null)
        {
            topRail =
                transform.Find(
                    "Visual/TopRail"
                );
        }


        if (bottomRail == null)
        {
            bottomRail =
                transform.Find(
                    "Visual/BottomRail"
                );
        }


        if (valueText == null)
        {
            Transform textTransform =
                transform.Find(
                    "ValueText"
                );


            if (textTransform != null)
            {
                valueText =
                    textTransform
                        .GetComponent<TMPro.TMP_Text>();
            }
        }


        if (triggerCollider == null)
        {
            Transform triggerTransform =
                transform.Find(
                    "Trigger"
                );


            if (triggerTransform != null)
            {
                triggerCollider =
                    triggerTransform
                        .GetComponent<BoxCollider>();
            }
        }
    }

    /// <summary>
    /// Gate의 Width / Height를 설정한다.
    /// </summary>
    public void SetSize(
        float width,
        float height)
    {
        ResolveReferences();

        width =
            Mathf.Max(
                0.5f,
                width
            );

        height =
            Mathf.Max(
                1f,
                height
            );


        float innerWidth =
            Mathf.Max(
                0.1f,
                width -
                postThickness * 2f
            );


        float innerHeight =
            Mathf.Max(
                0.1f,
                height -
                railThickness * 2f
            );


        // =========================
        // Glass Panel
        // =========================

        if (glassPanel != null)
        {
            glassPanel.localPosition =
                new Vector3(
                    0f,
                    height * 0.5f,
                    0f
                );


            glassPanel.localScale =
                new Vector3(
                    innerWidth,
                    innerHeight,
                    panelDepth
                );
        }


        // =========================
        // Posts
        // =========================

        if (leftPost != null)
        {
            leftPost.localPosition =
                new Vector3(
                    -width * 0.5f +
                    postThickness * 0.5f,
                    height * 0.5f,
                    0f
                );


            leftPost.localScale =
                new Vector3(
                    postThickness,
                    height,
                    frameDepth
                );
        }


        if (rightPost != null)
        {
            rightPost.localPosition =
                new Vector3(
                    width * 0.5f -
                    postThickness * 0.5f,
                    height * 0.5f,
                    0f
                );


            rightPost.localScale =
                new Vector3(
                    postThickness,
                    height,
                    frameDepth
                );
        }


        // =========================
        // Rails
        // =========================

        if (topRail != null)
        {
            topRail.localPosition =
                new Vector3(
                    0f,
                    height -
                    railThickness * 0.5f,
                    0f
                );


            topRail.localScale =
                new Vector3(
                    width,
                    railThickness,
                    frameDepth
                );
        }


        if (bottomRail != null)
        {
            bottomRail.localPosition =
                new Vector3(
                    0f,
                    railThickness * 0.5f,
                    0f
                );


            bottomRail.localScale =
                new Vector3(
                    width,
                    railThickness,
                    frameDepth
                );
        }


        // =========================
        // Trigger
        // =========================

        if (triggerCollider != null)
        {
            triggerCollider.transform.localPosition =
                new Vector3(
                    0f,
                    height * 0.5f,
                    0f
                );


            triggerCollider.transform.localScale =
                Vector3.one;


            triggerCollider.center =
                Vector3.zero;


            triggerCollider.size =
                new Vector3(
                    width,
                    height,
                    triggerDepth
                );
        }


        // =========================
        // Text
        // =========================

        if (valueText != null)
        {
            valueText.transform.localPosition =
                new Vector3(
                    0f,
                    height * 0.5f,
                    -0.2f
                );
        }
    }


    // 컴포넌트를 처음 붙였을 때
    // 자동으로 기존 Gate 구조를 찾아준다.
    private void Reset()
    {
        glassPanel =
            transform.Find(
                "Visual/GlassPanel"
            );

        leftPost =
            transform.Find(
                "Visual/LeftPost"
            );

        rightPost =
            transform.Find(
                "Visual/RightPost"
            );

        topRail =
            transform.Find(
                "Visual/TopRail"
            );

        bottomRail =
            transform.Find(
                "Visual/BottomRail"
            );


        Transform textTransform =
            transform.Find(
                "ValueText"
            );


        if (textTransform != null)
        {
            valueText =
                textTransform.GetComponent<TMP_Text>();
        }


        Transform triggerTransform =
            transform.Find(
                "Trigger"
            );


        if (triggerTransform != null)
        {
            triggerCollider =
                triggerTransform
                    .GetComponent<BoxCollider>();
        }
    }
}