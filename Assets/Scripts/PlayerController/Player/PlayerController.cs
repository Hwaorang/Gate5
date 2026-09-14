using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerRoot의 좌우 이동을 담당한다.
///
/// 현재 게임 구조에서는 Player가 앞으로 직접 이동하지 않고,
/// X축 방향으로만 이동한다.
///
/// 이동 속도는 PlayerStats의 MoveSpeed를 사용하며,
/// xLimit 범위를 벗어나지 않도록 위치를 제한한다.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 스탯")]

    // 이동 속도 등의 Player 기본/강화 스탯을 관리하는 컴포넌트
    [SerializeField] private PlayerStats playerStats;


    [Header("좌우 이동 범위")]

    // Player가 이동할 수 있는 X축 최대 범위
    //
    // 예:
    // xLimit = 4
    // 이동 가능 범위 = -4 ~ +4
    [SerializeField] private float xLimit = 4f;


    /// <summary>
    /// 외부 시스템에서 Player의 이동 가능 범위를 확인할 때 사용한다.
    ///
    /// DamageLine이 자신의 Collider 폭을 계산할 때도 사용한다.
    /// </summary>
    public float XLimit => xLimit;


    private void Awake()
    {
        // Inspector에서 PlayerStats가 연결되지 않았을 경우
        // 같은 GameObject에서 자동으로 찾아본다.
        if (playerStats == null)
        {
            playerStats =
                GetComponent<PlayerStats>();
        }

        // 자동 탐색 이후에도 없다면
        // 이동 처리에서 NullReference가 발생할 수 있으므로 경고
        if (playerStats == null)
        {
            Debug.LogWarning(
                "[PlayerController] PlayerStats를 찾을 수 없습니다."
            );
        }
    }


    private void Update()
    {
        Move();
    }


    /// <summary>
    /// A / D 키 입력을 받아 Player를 X축 방향으로 이동시킨다.
    ///
    /// A = 왼쪽
    /// D = 오른쪽
    ///
    /// 이동 후에는 Mathf.Clamp를 이용해
    /// xLimit 범위를 벗어나지 않도록 제한한다.
    /// </summary>
    private void Move()
    {
        // PlayerStats가 없다면
        // 이동 속도를 가져올 수 없으므로 종료
        if (playerStats == null)
        {
            return;
        }

        // Keyboard 장치가 없는 경우 방어
        if (Keyboard.current == null)
        {
            return;
        }


        // -1 = 왼쪽
        //  0 = 이동 없음
        //  1 = 오른쪽
        float horizontal = 0f;


        // A 키를 누르고 있으면 왼쪽 이동
        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
        }


        // D 키를 누르고 있으면 오른쪽 이동
        if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
        }


        // X축 이동 방향과 현재 MoveSpeed를 이용해
        // 이번 프레임의 이동 방향을 계산한다.
        Vector3 direction =
            Vector3.right *
            horizontal *
            playerStats.MoveSpeed;


        // 프레임 속도에 영향을 덜 받도록
        // Time.deltaTime을 곱해서 실제 위치를 변경한다.
        transform.position +=
            direction *
            Time.deltaTime;


        // 이동 후 현재 위치 가져오기
        Vector3 position =
            transform.position;


        // Player가 지정된 좌우 이동 범위를
        // 벗어나지 못하도록 X 위치를 제한한다.
        position.x =
            Mathf.Clamp(
                position.x,
                -xLimit,
                xLimit
            );


        // 제한된 위치를 최종 적용
        transform.position =
            position;
    }
}