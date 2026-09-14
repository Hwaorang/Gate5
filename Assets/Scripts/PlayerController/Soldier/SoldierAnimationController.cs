using UnityEngine;

/// <summary>
/// 병사 애니메이션 재생만 담당하는 클래스.
///
/// SoldierUnit, SoldierAttack 같은 게임 로직과
/// Animator 제어를 분리하기 위해 사용한다.
/// </summary>
public class SoldierAnimationController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField]
    private Animator animator;

    // Animator State 이름과 동일해야 한다.
    private static readonly int IdleHash =
        Animator.StringToHash("Idle");

    private static readonly int RunHash =
        Animator.StringToHash("Run");

    private static readonly int ShootHash =
        Animator.StringToHash("Shoot");

    private static readonly int DeathHash =
        Animator.StringToHash("Death");


    /// <summary>
    /// Pool에서 다시 사용될 때
    /// 기본 Idle 상태로 되돌린다.
    /// </summary>
    public void PlayIdle()
    {
        if (animator == null)
        {
            return;
        }

        animator.Play(IdleHash);
    }


    /// <summary>
    /// 이동 애니메이션 재생
    /// </summary>
    public void PlayRun()
    {
        if (animator == null)
        {
            return;
        }

        animator.Play(RunHash);
    }


    /// <summary>
    /// 총 발사 애니메이션 재생
    /// </summary>
    public void PlayShoot()
    {
        if (animator == null)
        {
            return;
        }

        animator.Play(ShootHash);
    }


    /// <summary>
    /// 사망 애니메이션 재생
    /// </summary>
    public void PlayDeath()
    {
        if (animator == null)
        {
            return;
        }

        animator.Play(DeathHash);
    }


    /// <summary>
    /// 현재 활성화된 스킨의 Animator를
    /// 외부에서 전달받을 때 사용한다.
    /// </summary>
    public void SetAnimator(Animator targetAnimator)
    {
        animator = targetAnimator;
    }
}