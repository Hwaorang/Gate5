using UnityEngine;

/// <summary>
/// 적이 플레이어의 방어선을 통과했을 때
/// 병사 수를 감소시키는 역할을 담당한다.
///
/// 주요 역할
/// - 게임 시작 시 PlayerRoot와 부모 관계를 끊어
///   플레이어의 좌우 이동을 따라가지 않도록 한다.
/// - PlayerController의 이동 범위를 기준으로
///   DamageLine Collider 폭을 자동 설정한다.
/// - Enemy가 Trigger를 통과하면
///   SquadManager를 통해 병사 1명을 제거한다.
///
/// Enemy 판별은
/// - 팀 Enemy 시스템 : Mon_Ctrl
/// - 기존 테스트 Enemy 시스템 : EnemyHealth
/// 두 구조를 모두 지원한다.
/// </summary>
public class DamageLine : MonoBehaviour
{
    [Header("참조")]

    // 플레이어의 좌우 이동 제한값(XLimit)을 가져오기 위한 참조
    [SerializeField] private PlayerController playerController;

    // Enemy가 DamageLine을 통과했을 때
    // 병사 수를 감소시키기 위해 사용하는 SquadManager
    [SerializeField] private SquadManager squadManager;

    // DamageLine의 실제 Trigger 영역
    [SerializeField] private BoxCollider boxCollider;


    [Header("방어선 설정")]

    // 플레이어 이동 가능 범위보다
    // 좌우 각각 조금 더 넓게 방어선을 만들기 위한 여유 값
    [SerializeField] private float extraWidth = 1f;


    private void Start()
    {
        // DamageLine을 PlayerRoot의 자식으로 배치하면
        // 에디터에서 위치를 맞추기 쉽다.
        //
        // 하지만 게임 중에는 Player가 좌우로 움직여도
        // DamageLine은 월드에 고정되어 있어야 하므로
        // 시작 시 부모 관계를 끊는다.
        //
        // true를 사용하면 부모를 해제해도
        // 현재 월드 위치/회전/크기가 그대로 유지된다.
        transform.SetParent(null, true);

        // Player 이동 범위를 기준으로
        // DamageLine의 가로 폭을 한 번 설정한다.
        UpdateLineWidth();
    }


    /// <summary>
    /// PlayerController의 XLimit을 기준으로
    /// DamageLine Collider의 가로 폭을 계산한다.
    ///
    /// 예:
    /// XLimit = 4
    /// extraWidth = 1
    ///
    /// Player 이동 범위 = -4 ~ +4 = 총 8
    /// 좌우 여유 = 1 + 1
    /// 최종 DamageLine 폭 = 10
    /// </summary>
    private void UpdateLineWidth()
    {
        // 필요한 참조가 없으면
        // Collider 크기를 계산할 수 없으므로 종료
        if (playerController == null ||
            boxCollider == null)
        {
            return;
        }

        // XLimit은 한쪽 방향의 최대 이동 거리이므로
        // 전체 가로 범위를 구하려면 2를 곱한다.
        float playerMoveWidth =
            playerController.XLimit * 2f;

        // 플레이어 최대 이동 범위보다
        // 좌우 각각 extraWidth만큼 넓게 설정한다.
        float totalWidth =
            playerMoveWidth +
            (extraWidth * 2f);

        // 기존 Collider의 Y/Z 크기는 유지하고
        // X 크기만 변경한다.
        Vector3 size =
            boxCollider.size;

        size.x =
            totalWidth;

        boxCollider.size =
            size;
    }


    /// <summary>
    /// 어떤 Collider가 DamageLine Trigger에 들어왔을 때 호출된다.
    ///
    /// Enemy인지 확인한 뒤
    /// Enemy가 맞으면 SquadManager를 통해
    /// 병사 한 명을 제거한다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 팀 프로젝트에서 사용하는 Enemy 구조
        Mon_Ctrl monCtrl =
            other.GetComponentInParent<Mon_Ctrl>();

        // 기존 테스트 씬에서 사용하는 Enemy 구조
        EnemyHealth enemyHealth =
            other.GetComponentInParent<EnemyHealth>();

        // 두 Enemy 컴포넌트 모두 없다면
        // DamageLine이 처리할 대상이 아니므로 종료
        if (monCtrl == null &&
            enemyHealth == null)
        {
            return;
        }

        // SquadManager가 정상적으로 연결되어 있다면
        // 병사 한 명 감소
        if (squadManager != null)
        {
            squadManager.RemoveOneSoldier();
        }
    }
}