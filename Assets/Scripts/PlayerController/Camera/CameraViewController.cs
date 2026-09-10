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


    [Header("현재 시점")]

    // 게임 시작 기본값은 대각선 시점
    [SerializeField]
    private CameraViewType currentView =
        CameraViewType.Diagonal;


    // 마지막으로 확인한 이동 범위
    private float lastXLimit;


    private void Start()
    {
        if (playerController != null)
        {
            lastXLimit =
                playerController.XLimit;
        }

        ApplyView(currentView);
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
                "[CameraViewController] Diagonal 기준점이 연결되지 않았습니다."
            );

            return;
        }


        // =========================
        // 현재 이동 범위 확인
        // =========================

        float currentXLimit =
            referenceXLimit;

        if (playerController != null)
        {
            currentXLimit =
                playerController.XLimit;
        }


        // =========================
        // 이동 범위에 따른 거리 배율 계산
        // =========================

        float distanceScale =
            currentXLimit /
            referenceXLimit;

        distanceScale =
            Mathf.Clamp(
                distanceScale,
                minDistanceScale,
                maxDistanceScale
            );


        // =========================
        // 기준 카메라 Offset 계산
        // =========================

        Vector3 baseOffset =
            diagonalView.position -
            diagonalFocusPoint.position;


        // XLimit이 넓어질수록
        // 같은 각도를 유지하면서 카메라를 멀리 보낸다.
        Vector3 cameraPosition =
            diagonalFocusPoint.position +
            baseOffset * distanceScale;


        transform.position =
            cameraPosition;


        // 항상 플레이 영역 중앙을 바라본다.
        transform.LookAt(
            diagonalFocusPoint.position
        );


        lastXLimit =
            currentXLimit;
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
    public void Initialize(
        PlayerContext context)
    {
        if (context == null)
        {
            return;
        }

        playerController =
            context.PlayerController;

        lastXLimit =
            playerController.XLimit;

        if (currentView ==
            CameraViewType.Diagonal)
        {
            ApplyDiagonalView();
        }
    }
}