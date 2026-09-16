using UnityEngine;

/// <summary>
/// 무한 Stage를 구성하는 하나의 Chunk.
///
/// RoadLeftEdge / RoadRightEdge를 이용해서
/// 실제 플레이 가능한 도로 영역을 제공한다.
/// </summary>
public class StageChunk : MonoBehaviour
{
    [Header("Chunk 설정")]

    [SerializeField]
    private float chunkLength = 30f;


    [Header("Road Area")]

    [SerializeField]
    private Transform roadLeftEdge;

    [SerializeField]
    private Transform roadRightEdge;


    [Header("Points")]

    [SerializeField]
    private Transform endPoint;


    public float ChunkLength =>
        chunkLength;

    public Transform EndPoint =>
        endPoint;


    /// <summary>
    /// 실제 도로의 월드 X 중심과 폭을 반환한다.
    /// </summary>
    public bool TryGetRoadArea(
        out float centerX,
        out float width)
    {
        centerX = 0f;
        width = 0f;


        if (roadLeftEdge == null ||
            roadRightEdge == null)
        {
            Debug.LogWarning(
                $"[StageChunk] {name} | " +
                $"Road Edge가 연결되지 않았습니다."
            );

            return false;
        }


        float leftX =
            roadLeftEdge.position.x;

        float rightX =
            roadRightEdge.position.x;


        centerX =
            (leftX + rightX) * 0.5f;


        width =
            Mathf.Abs(
                rightX - leftX
            );


        return width > 0.01f;
    }


    private void OnDrawGizmosSelected()
    {
        if (roadLeftEdge == null ||
            roadRightEdge == null)
        {
            return;
        }


        Gizmos.DrawLine(
            roadLeftEdge.position,
            roadRightEdge.position
        );
    }
}