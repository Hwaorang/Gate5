using UnityEngine;

public class StageChunk : MonoBehaviour
{
    [Header("Chunk 설정")]
    [SerializeField] private float chunkLength = 30f;

    [Header("Points")]
    [SerializeField] private Transform endPoint;

    public float ChunkLength => chunkLength;
    public Transform EndPoint => endPoint;
}