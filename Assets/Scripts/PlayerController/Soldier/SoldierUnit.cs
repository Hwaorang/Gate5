using System.Collections;
using UnityEngine;

/// <summary>
/// 병사 한 명의 생명주기와 상태를 관리한다.
///
/// 주요 역할
/// - SquadManager 참조 저장
/// - 선택된 병사 스킨 적용
/// - 병사 상태 관리
/// - 중복 사망 처리 방지
/// - Death 애니메이션 재생
/// - 사망 연출이 끝난 뒤 SquadManager에 제거 요청
///
/// 실제 병사 수 관리와 Object Pool 반환은
/// SquadManager가 담당한다.
/// </summary>
public class SoldierUnit : MonoBehaviour
{
    // =========================
    // 분대 참조
    // =========================

    private SquadManager squadManager;


    // =========================
    // 애니메이션
    // =========================

    [SerializeField]
    private SoldierAnimationController animationController;


    // =========================
    // 사망 연출
    // =========================

    [Header("사망 연출")]

    [SerializeField]
    private float deathDelay = 0.8f;


    // =========================
    // 상태
    // =========================

    /// <summary>
    /// 현재 병사의 상태.
    ///
    /// Alive
    /// - 정상 상태
    /// - 공격 가능
    ///
    /// Dying
    /// - Death 애니메이션 재생 중
    /// - 공격 불가
    ///
    /// Dead
    /// - 실제 제거 직전 상태
    /// </summary>
    public SoldierState CurrentState
    {
        get;
        private set;
    }


    /// <summary>
    /// 공격 가능한 상태인지 외부에서 쉽게 확인할 수 있도록 제공한다.
    /// </summary>
    public bool CanAttack =>
        CurrentState == SoldierState.Alive;


    /// <summary>
    /// 병사가 분대에 추가되거나
    /// Object Pool에서 다시 꺼내졌을 때 호출한다.
    /// </summary>
    public void Init(SquadManager manager)
    {
        squadManager = manager;

        // Pool에서 재사용될 수 있으므로
        // 항상 정상 상태로 초기화
        ChangeState(
            SoldierState.Alive
        );


        // =========================
        // 저장된 스킨 적용
        // =========================

        SoldierVisualController visualController =
            GetComponent<SoldierVisualController>();

        if (visualController != null)
        {
            visualController.ApplySavedSkin();
        }


        // =========================
        // 기본 애니메이션
        // =========================

        if (animationController != null)
        {
            animationController.PlayRun();
        }
    }


    /// <summary>
    /// 병사 사망 처리 시작.
    ///
    /// 이미 Dying 또는 Dead 상태라면
    /// 중복으로 사망 처리를 실행하지 않는다.
    /// </summary>
    public void Die()
    {
        // Alive 상태가 아니라면
        // 이미 사망 처리 중이거나 제거된 상태
        if (CurrentState != SoldierState.Alive)
        {
            return;
        }

        if (squadManager == null)
        {
            return;
        }

        // =========================
        // Alive → Dying
        // =========================

        ChangeState(
            SoldierState.Dying
        );

        StartCoroutine(
            DeathRoutine()
        );
    }


    /// <summary>
    /// 사망 연출 처리.
    ///
    /// Dying
    /// → Death 애니메이션
    /// → 대기
    /// → Dead
    /// → SquadManager.RemoveUnit()
    /// </summary>
    private IEnumerator DeathRoutine()
    {
        // =========================
        // Death 애니메이션
        // =========================

        if (animationController != null)
        {
            animationController.PlayDeath();
        }


        // =========================
        // 사망 연출 대기
        // =========================

        yield return new WaitForSeconds(
            deathDelay
        );


        // =========================
        // Dying → Dead
        // =========================

        ChangeState(
            SoldierState.Dead
        );


        // =========================
        // 실제 병사 제거
        // =========================

        squadManager.RemoveUnit(
            this
        );
    }


    /// <summary>
    /// 병사의 상태 변경을 한 곳에서 처리한다.
    ///
    /// 추후 Stunned, Reloading 같은 상태가 추가되더라도
    /// 상태 변경 흐름을 이 메서드에서 관리할 수 있다.
    /// </summary>
    private void ChangeState(
        SoldierState newState)
    {
        CurrentState =
            newState;
    }
}