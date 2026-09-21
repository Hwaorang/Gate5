using UnityEngine;

/// <summary>
/// 로비 스킨 미리보기 캐릭터가
/// 활성화될 때 Run 애니메이션을 재생한다.
/// </summary>
public sealed class PreviewSkinAutoRun : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string runStateName = "Run";


    private void Awake()
    {
        ResolveAnimator();
    }


    private void OnEnable()
    {
        ResolveAnimator();
        PlayRunPreview();
    }


    private void ResolveAnimator()
    {
        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>(
                    true
                );
        }


        if (animator != null)
        {
            // 미리보기 캐릭터가 앞으로 이동하지 않도록
            animator.applyRootMotion = false;
        }
    }


    private void PlayRunPreview()
    {
        if (animator == null)
        {
            Debug.LogWarning(
                $"[PreviewSkinAutoRun] " +
                $"{gameObject.name}에서 Animator를 찾지 못했습니다."
            );

            return;
        }


        int runHash =
            Animator.StringToHash(
                runStateName
            );


        // Base Layer에 Run 상태가 있는지 확인
        if (!animator.HasState(
                0,
                runHash
            ))
        {
            Debug.LogWarning(
                $"[PreviewSkinAutoRun] " +
                $"Animator에 '{runStateName}' 상태가 없습니다."
            );

            return;
        }


        animator.Play(
            runHash,
            0,
            0f
        );

        animator.Update(
            0f
        );
    }
}