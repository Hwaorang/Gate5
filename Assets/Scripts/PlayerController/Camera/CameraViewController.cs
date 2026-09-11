using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 게임 카메라의 시점을 관리한다.
///
/// Diagonal
/// - 기본 시점
/// - 플레이어 이동 범위의 중앙을 바라본다.
/// - XLimit이 넓어지면 자동으로 카메라 거리를 늘린다.
///
/// Follow
/// - 기존 CameraFollow를 이용해 Player를 따라간다.
/// </summary>
public class CameraViewController : MonoBehaviour
{
    public enum CameraViewType
    {
        Diagonal,
        Follow
    }


    [Header("Player")]

    // 이동 가능 범위를 확인하기 위한 PlayerController
    [SerializeField]
    private PlayerController playerController;


    [Header("Camera Follow")]

    // 기존 Player 추적 카메라
    [SerializeField]
    private CameraFollow cameraFollow;


    [Header("Diagonal View")]

    // 기준이 되는 대각선 카메라 위치
    //
    // 예:
    // XLimit = 4일 때 가장 보기 좋은 카메라 위치
    [SerializeField]
    private Transform diagonalView;

    // 카메라가 바라볼 플레이 영역의 중앙
    [SerializeField]
    private Transform diagonalFocusPoint;


    [Header("Diagonal Range Setting")]

    // DiagonalView를 세팅했을 때의 기준 XLimit
    //
    // 현재 Player의 기본 XLimit이 4라면 4로 설정
    [SerializeField]
    private float referenceXLimit = 4f;

    // 너무 가까워지거나 너무 멀어지는 것 방지
    [SerializeField]
    private float minDistanceScale = 0.75f;

    [SerializeField]
    private float maxDistanceScale = 2f;

    // DiagonalFocusPoint에서 DiagonalView까지의
    // 기본 거리와 방향을 저장한다.
    private Vector3 baseDiagonalOffset;


    [Header("현재 시점")]

    // 게임 시작 기본값은 대각선 시점
    [SerializeField]
    private CameraViewType currentView =
        CameraViewType.Diagonal;


    // 마지막으로 확인한 이동 범위
    private float lastXLimit;

    [SerializeField] private Transform cameraRig;

    private void Awake()
    {
        ResolveReferences();

        if (diagonalView != null &&
            diagonalFocusPoint != null)
        {
            // FocusPoint를 기준으로
            // 카메라가 얼마나 떨어져 있는지 저장
            baseDiagonalOffset =
                diagonalView.localPosition -
                diagonalFocusPoint.localPosition;
        }
    }


    private void Start()
    {
        if (playerController != null)
        {
            lastXLimit =
                playerController.XLimit;
        }

        ResolveReferences();
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (Keyboard.current != null &&
            Keyboard.current.f10Key.wasPressedThisFrame)
        {
            ToggleView();
        }
#endif

        // 대각선 시점 사용 중에
        // Player의 XLimit이 변경되었다면 카메라 위치 재계산
        UpdateDiagonalView();
    }


    /// <summary>
    /// 대각선 / Player 추적 시점을 전환한다.
    /// </summary>
    public void ToggleView()
    {
        if (currentView == CameraViewType.Diagonal)
        {
            ApplyView(
                CameraViewType.Follow
            );
        }
        else
        {
            ApplyView(
                CameraViewType.Diagonal
            );
        }
    }


    /// <summary>
    /// 원하는 카메라 시점을 적용한다.
    /// </summary>
    public void ApplyView(
        CameraViewType viewType)
    {
        currentView =
            viewType;

        switch (currentView)
        {
            // =========================
            // 대각선 고정 시점
            // =========================

            case CameraViewType.Diagonal:

                if (cameraFollow != null)
                {
                    cameraFollow.enabled = false;
                }

                ApplyDiagonalView();

                break;


            // =========================
            // Player 추적 시점
            // =========================

            case CameraViewType.Follow:

                if (cameraFollow == null)
                {
                    Debug.LogWarning(
                        "[CameraViewController] CameraFollow가 없습니다."
                    );

                    return;
                }

                cameraFollow.enabled = true;

                break;
        }
    }


    /// <summary>
    /// 현재 Player의 이동 범위에 맞춰
    /// 대각선 카메라 위치를 계산한다.
    /// </summary>
    private void ApplyDiagonalView()
    {
        if (diagonalView == null ||
            diagonalFocusPoint == null)
        {
            Debug.LogWarning(
                "[CameraViewController] Diagonal 기준점이 없습니다."
            );

            return;
        }


        if (cameraFollow != null)
        {
            cameraFollow.enabled = false;
        }


        // =========================
        // XLimit에 따른 거리 계산
        // =========================

        float distanceScale = 1f;

        if (playerController != null &&
            referenceXLimit > 0f)
        {
            distanceScale =
                playerController.XLimit /
                referenceXLimit;

            distanceScale =
                Mathf.Clamp(
                    distanceScale,
                    minDistanceScale,
                    maxDistanceScale
                );
        }


        // =========================
        // DiagonalView 위치 조정
        // =========================

        // FocusPoint는 그대로 두고
        // 카메라와 FocusPoint 사이 거리만 조절한다.
        diagonalView.localPosition =
            diagonalFocusPoint.localPosition +
            baseDiagonalOffset *
            distanceScale;


        // =========================
        // 실제 Camera 적용
        // =========================

        transform.position =
            diagonalView.position;

        transform.LookAt(
            diagonalFocusPoint.position
        );


        // 현재 XLimit 저장
        if (playerController != null)
        {
            lastXLimit =
                playerController.XLimit;
        }
    }


    /// <summary>
    /// 게임 도중 XLimit이 바뀌었는지 확인한다.
    /// 바뀌었다면 대각선 카메라 위치를 다시 계산한다.
    /// </summary>
    private void UpdateDiagonalView()
    {
        if (currentView !=
            CameraViewType.Diagonal)
        {
            return;
        }

        if (playerController == null)
        {
            return;
        }

        float currentXLimit =
            playerController.XLimit;


        // 이동 범위가 그대로라면 계산할 필요 없음
        if (Mathf.Approximately(
            currentXLimit,
            lastXLimit))
        {
            return;
        }


        ApplyDiagonalView();
    }


    /// <summary>
    /// 이후 PlayerRoot 동적 생성 시
    /// PlayerContext를 통해 실제 PlayerController를 받을 수 있다.
    /// </summary>
    public void Initialize(PlayerContext context)
    {
        if (context == null)
        {
            return;
        }

        playerController =
            context.PlayerController;

        // =========================
        // CameraRig 기준 위치 설정
        // =========================

        if (cameraRig != null)
        {
            Vector3 playerPosition =
                context.transform.position;

            cameraRig.position =
                playerPosition;
        }


        // =========================
        // Follow Target 연결
        // =========================

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(
                context.transform
            );
        }


        ApplyView(
            CameraViewType.Diagonal
        );
    }

    /// <summary>
    /// CameraRig 내부에서 필요한 참조를 자동으로 찾는다.
    /// Inspector 연결이 있으면 기존 값을 우선 사용한다.
    /// </summary>
    private void ResolveReferences()
    {
        // =========================
        // CameraRig
        // =========================

        if (cameraRig == null)
        {
            cameraRig =
                transform.root;
        }


        // =========================
        // CameraFollow
        // =========================

        if (cameraFollow == null)
        {
            cameraFollow =
                GetComponent<CameraFollow>();
        }


        // =========================
        // Camera 기준점
        // =========================

        if (diagonalView == null)
        {
            diagonalView =
                cameraRig.Find(
                    "CameraViews/DiagonalView"
                );
        }

        if (diagonalFocusPoint == null)
        {
            diagonalFocusPoint =
                cameraRig.Find(
                    "CameraViews/DiagonalFocusPoint"
                );
        }
    }
}