using UnityEngine;

public class DamageLine : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SquadManager squadManager;
    [SerializeField] private BoxCollider boxCollider;

    [Header("설정")]
    [SerializeField] private float extraWidth = 1f;

    private void Start()
    {
        UpdateLineWidth();
    }

    /// <summary>
    /// PlayerController의 좌우 이동 범위를 기준으로
    /// DamageLine의 가로 길이를 자동으로 설정
    /// </summary>
    private void UpdateLineWidth()
    {
        if (playerController == null || boxCollider == null)
        {
            return;
        }

        // 플레이어 이동 범위 전체 폭
        float width =
            playerController.XLimit * 2f;

        // 좌우 여유 공간 추가
        width += extraWidth;

        Vector3 size = boxCollider.size;

        size.x = width;

        boxCollider.size = size;
    }

    /// <summary>
    /// 몬스터가 DamageLine을 통과했을 때 처리
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
        {
            return;
        }


        if (squadManager != null)
        {
            squadManager.RemoveOneSoldier();
        }

        // 임시 몬스터 제거
        enemy.gameObject.SetActive(false);
    }
}