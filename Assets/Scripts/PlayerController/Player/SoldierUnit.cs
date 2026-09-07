using UnityEngine;

/// <summary>
/// 병사 한 명의 생명주기를 관리한다.
///
/// 주요 역할
/// - SquadManager 참조 저장
/// - 병사가 제거되어야 할 때 SquadManager에 제거 요청
///
/// 실제 병사 수 관리와 Pool 반환은
/// SquadManager가 담당한다.
/// </summary>
public class SoldierUnit : MonoBehaviour
{
    // 이 병사가 속해 있는 SquadManager
    private SquadManager squadManager;

    // 같은 병사에 대해 Die()가 여러 번 호출되는 것을 방지
    private bool isDead;


    /// <summary>
    /// 병사가 분대에 추가될 때 SquadManager에서 호출한다.
    ///
    /// Object Pool에서 다시 꺼낸 병사도
    /// 현재 SquadManager 참조를 다시 연결하고
    /// 사망 상태를 초기화한다.
    /// </summary>
    public void Init(
        SquadManager manager)
    {
        squadManager =
            manager;

        isDead =
            false;
    }


    /// <summary>
    /// 병사를 제거해야 할 때 호출한다.
    ///
    /// 직접 Destroy하지 않고
    /// SquadManager에 제거를 요청하며,
    /// 이후 Pool 반환도 SquadManager가 처리한다.
    /// </summary>
    public void Die()
    {
        // 이미 제거 처리된 병사라면
        // 중복 처리하지 않는다.
        if (isDead)
        {
            return;
        }

        if (squadManager == null)
        {
            return;
        }

        isDead =
            true;

        squadManager.RemoveUnit(
            this
        );
    }
}