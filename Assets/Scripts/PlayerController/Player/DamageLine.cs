using UnityEngine;

public class DamageLine : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SquadManager squadManager;
    [SerializeField] private BoxCollider boxCollider;

    [Header("방어선 설정")]
    // 좌우 각각 추가할 여유 공간
    [SerializeField] private float extraWidth = 1f;

    private void Start()
    {
        // 게임이 시작되면 PlayerRoot와 부모 관계를 끊는다.
        // 현재 월드 위치는 그대로 유지된다.
        transform.SetParent(null, true);

        // 플레이어 이동 범위에 맞춰 방어선 폭 설정
        UpdateLineWidth();
    }

    /// <summary>
    /// 플레이어 좌우 이동 범위를 기준으로
    /// DamageLine Collider의 폭을 설정한다.
    /// </summary>
    private void UpdateLineWidth()
    {
        if (playerController == null ||
            boxCollider == null)
        {
            return;
        }

        // Player가 이동할 수 있는 전체 가로 폭
        float playerMoveWidth =
            playerController.XLimit * 2f;

        // 좌우에 각각 extraWidth만큼 여유 추가
        float totalWidth =
            playerMoveWidth + (extraWidth * 2f);

        Vector3 size = boxCollider.size;

        size.x = totalWidth;

        boxCollider.size = size;
    }

    /// <summary>
    /// 몬스터가 방어선을 통과했을 때 처리
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }

        Debug.Log("[DamageLine] 몬스터 방어선 통과");

        // 병사 한 명 감소
        if (squadManager != null)
        {
            squadManager.RemoveOneSoldier();
        }

        // 몬스터 제거는 임시 처리
        enemy.gameObject.SetActive(false);
    }
}