using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// StageChunk를 일정 개수 생성하고,
/// Player 뒤로 넘어간 Chunk를 다시 맨 앞으로 이동시켜
/// 무한 맵처럼 보이게 만든다.
/// </summary>
public class StageMapManager : MonoBehaviour
{
    [Header("Chunk")]

    [SerializeField]
    private StageChunk chunkPrefab;

    // 동시에 유지할 Chunk 개수
    [SerializeField]
    private int chunkCount = 4;


    [Header("Movement")]

    // 맵이 Player 쪽으로 내려오는 속도
    [SerializeField]
    private float moveSpeed = 5f;


    [Header("Recycle")]

    // 이 Z보다 뒤로 가면 맨 앞으로 이동시킨다.
    [SerializeField]
    private float recycleZ = -30f;


    private readonly List<StageChunk>
        activeChunks = new();

    [Header("Recycle")]

    [SerializeField]
    private Transform recycleAnchor;

    // Player 뒤로 얼마나 더 지나간 뒤 재사용할지
    [SerializeField]
    private float recycleBehindDistance = 15f;

    [Header("Initial Placement")]

    // 시작할 때 Player 뒤쪽에 미리 생성할 Chunk 개수
    [SerializeField]
    private int behindChunkCount = 1;

    private void Start()
    {
        CreateChunks();
    }


    private void Update()
    {
        MoveChunks();
        RecycleChunks();
    }


    /// <summary>
    /// 게임 시작 시 필요한 만큼 Chunk를 배치한다.
    /// </summary>
    private void CreateChunks()
    {
        if (chunkPrefab == null)
        {
            Debug.LogError(
                "[StageMapManager] Chunk Prefab이 없습니다."
            );

            return;
        }

        float chunkLength =
            chunkPrefab.ChunkLength;


        // =========================
        // 시작 위치 계산
        // =========================

        float startZ = 0f;

        if (recycleAnchor != null)
        {
            startZ =
                recycleAnchor.position.z -
                chunkLength * behindChunkCount;
        }
        else
        {
            startZ =
                -chunkLength * behindChunkCount;
        }


        // Player 뒤쪽 Chunk까지 포함해서 생성
        int totalChunkCount =
            chunkCount +
            behindChunkCount;


        // =========================
        // Chunk 생성
        // =========================

        for (int i = 0;
             i < totalChunkCount;
             i++)
        {
            Vector3 spawnPosition =
                new Vector3(
                    0f,
                    0f,
                    startZ +
                    i * chunkLength
                );


            StageChunk chunk =
                Instantiate(
                    chunkPrefab,
                    spawnPosition,
                    Quaternion.identity,
                    transform
                );


            activeChunks.Add(chunk);
        }
    }


    /// <summary>
    /// 모든 Chunk를 Player 방향으로 이동시킨다.
    /// </summary>
    private void MoveChunks()
    {
        float moveAmount =
            moveSpeed *
            Time.deltaTime;


        for (int i = 0;
             i < activeChunks.Count;
             i++)
        {
            activeChunks[i]
                .transform.position +=
                Vector3.back *
                moveAmount;
        }
    }


    /// <summary>
    /// 뒤로 넘어간 Chunk를
    /// 가장 앞쪽으로 다시 이동시킨다.
    /// </summary>
    private void RecycleChunks()
    {
        if (activeChunks.Count == 0)
        {
            return;
        }


        for (int i = 0;
             i < activeChunks.Count;
             i++)
        {
            StageChunk chunk =
                activeChunks[i];


            if (recycleAnchor == null ||
            chunk.EndPoint == null)
            {
                continue;
            }

            // Chunk의 끝까지 Player 뒤로 충분히 지나갔을 때만 재활용
            float recycleLine =
                recycleAnchor.position.z -
                recycleBehindDistance;

            if (chunk.EndPoint.position.z >
                recycleLine)
            {
                continue;
            }

            MoveChunkToFront(chunk);
        }
    }


    /// <summary>
    /// 현재 가장 앞에 있는 Chunk를 찾고
    /// 그 다음 위치로 재배치한다.
    /// </summary>
    private void MoveChunkToFront(
        StageChunk chunk)
    {
        float farthestZ =
            float.MinValue;


        for (int i = 0;
             i < activeChunks.Count;
             i++)
        {
            StageChunk other =
                activeChunks[i];


            if (other == chunk)
            {
                continue;
            }


            if (other.transform.position.z >
                farthestZ)
            {
                farthestZ =
                    other.transform.position.z;
            }
        }


        Vector3 position =
            chunk.transform.position;


        position.z =
            farthestZ +
            chunk.ChunkLength;


        chunk.transform.position =
            position;
    }
}