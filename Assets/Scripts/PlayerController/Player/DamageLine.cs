using UnityEngine;

/// <summary>
/// Enemy가 방어선을 통과했을 때
/// Player의 병사를 감소시키는 영역.
///
/// PlayerRoot를 Scene에서 직접 참조하지 않고
/// PlayerContext를 통해 필요한 컴포넌트를 전달받는다.
/// </summary>
public class DamageLine : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField]
    private BoxCollider boxCollider;

    [Header("크기 설정")]
    [SerializeField]
    private float extraWidth = 1f;


    [Header("Player 참조")]
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private SquadManager squadManager;

    [Header("위치 설정")]

    // Player보다 얼마나 앞에 DamageLine을 둘지
    [SerializeField]
    private float forwardOffset = 3f;

    // 동적으로 생성된 PlayerRoot
    private Transform playerTransform;

    // DamageLine의 원래 X / Y 위치
    private float fixedX;
    private float fixedY;

    private void Awake()
    {
        // DamageLine은 좌우로 움직이지 않도록
        // 현재 Scene 위치의 X / Y를 기억한다.
        fixedX = transform.position.x;
        fixedY = transform.position.y;
    }

    /// <summary>
    /// 런타임에 생성된 Player의 정보를 전달받는다.
    ///
    /// Dependency Injection 방식으로
    /// DamageLine이 Scene의 PlayerRoot를 직접 참조하지 않게 한다.
    /// </summary>
    public void Initialize(PlayerContext context)
    {
        if (context == null)
        {
            Debug.LogWarning(
                "[DamageLine] PlayerContext가 없습니다."
            );

            return;
        }

        playerController =
            context.PlayerController;

        squadManager =
            context.SquadManager;

        // 동적으로 생성된 PlayerRoot 위치
        playerTransform =
            context.transform;

        // Player 정보를 받은 뒤
        // 실제 이동 범위에 맞춰 DamageLine 크기를 계산한다.
        UpdateLineWidth();

        // 처음 연결되는 순간에도 위치 즉시 적용
        UpdateLinePosition();
    }

    /// <summary>
    /// DamageLine을 Player보다 일정 거리 앞에 배치한다.
    ///
    /// Player의 좌우 이동(X)은 따라가지 않고,
    /// 전후 위치(Z)만 Player 기준으로 계산한다.
    /// </summary>
    private void UpdateLinePosition()
    {
        if (playerTransform == null)
        {
            return;
        }

        Vector3 position =
            transform.position;

        // 좌우 중앙 위치는 고정
        position.x = fixedX;

        // 높이도 기존 값 유지
        position.y = fixedY;

        // Player보다 일정 거리 앞
        position.z =
            playerTransform.position.z +
            forwardOffset;

        transform.position =
            position;
    }


    /// <summary>
    /// Player의 좌우 이동 가능 범위에 맞춰
    /// DamageLine Collider의 가로 길이를 설정한다.
    /// </summary>
    private void UpdateLineWidth()
    {
        if (playerController == null ||
            boxCollider == null)
        {
            return;
        }

        float playerMoveWidth =
            playerController.XLimit * 2f;

        float totalWidth =
            playerMoveWidth +
            extraWidth * 2f;

        Vector3 size =
            boxCollider.size;

        size.x =
            totalWidth;

        boxCollider.size =
            size;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (squadManager == null)
        {
            return;
        }

        // 팀 Enemy
        Mon_Ctrl monCtrl =
            other.GetComponentInParent<Mon_Ctrl>();

        // 테스트/레거시 Enemy
        EnemyHealth enemyHealth =
            other.GetComponentInParent<EnemyHealth>();

        if (monCtrl == null &&
            enemyHealth == null)
        {
            return;
        }

        squadManager.RemoveOneSoldier();
    }

    private void LateUpdate()
    {
        UpdateLinePosition();
    }
}