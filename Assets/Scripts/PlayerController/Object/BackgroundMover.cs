using UnityEngine;

/// <summary>
/// 옆 배경처럼 일정한 속도로 뒤쪽(-Z)으로 이동하는 오브젝트에 사용
/// </summary>
public class BackgroundMover : MonoBehaviour
{
    [Header("배경 이동 속도")]
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        transform.position +=
            Vector3.back *
            moveSpeed *
            Time.deltaTime;
    }
}