using UnityEngine;

/// <summary>
/// PlayerRoot의 대표 캐릭터 외형을 생성한다.
///
/// 별도의 Player 전용 캐릭터 Prefab을 만들지 않고,
/// SoldierPool이 사용하는 Soldier Prefab의 Skin을 원본으로 사용한다.
///
/// 따라서 로비에서 선택한 selectedSkin과
/// 게임의 Soldier / Player 대표 캐릭터가 같은 외형을 사용한다.
///
/// 이 오브젝트는 Squad 병사 수에 포함되지 않으며
/// SoldierAttack / SoldierUnit도 가지지 않는다.
/// </summary>
public class PlayerCharacterVisual : MonoBehaviour
{
    // ========================================================
    // Source
    // ========================================================

    [Header("Skin Source")]

    [Tooltip(
        "비어 있으면 Scene의 SoldierPool을 자동 탐색합니다."
    )]
    [SerializeField]
    private SoldierPool soldierPool;


    // ========================================================
    // Player Visual
    // ========================================================

    [Header("Player Visual")]

    [Tooltip(
        "PlayerRoot 기준 대표 캐릭터 위치. " +
        "병사 대형과 겹치면 Z 값을 조금 앞으로 조절하세요."
    )]
    [SerializeField]
    private Vector3 visualLocalPosition =
        new Vector3(0f, 0f, 0.8f);

    [SerializeField]
    private Vector3 visualLocalEulerAngles =
        Vector3.zero;

    [SerializeField]
    private Vector3 visualLocalScale =
        Vector3.one;


    // ========================================================
    // Animation
    // ========================================================

    [Header("Run Animation")]

    [Tooltip(
        "현재 Soldier Animator에서 사용하는 Run State 이름. " +
        "해당 State가 없으면 Animator의 기본 State를 그대로 사용합니다."
    )]
    [SerializeField]
    private string runStateName = "Run";


    // ========================================================
    // Player Marker
    // ========================================================

    [Header("Player Marker")]

    [SerializeField]
    private bool createPlayerMarker = true;

    [SerializeField]
    private Vector3 markerLocalPosition =
        new Vector3(0f, 2.4f, 0f);

    [SerializeField]
    [Min(0.05f)]
    private float markerWidth = 0.35f;

    [SerializeField]
    [Min(0.05f)]
    private float markerHeight = 0.45f;

    [SerializeField]
    private Color markerColor =
        new Color(1f, 0.85f, 0.1f, 1f);


    // ========================================================
    // Runtime
    // ========================================================

    private Transform visualRoot;
    private GameObject currentSkinInstance;

    private Transform markerTransform;
    private Camera mainCamera;

    private Mesh markerMesh;
    private Material markerMaterial;


    // ========================================================
    // Unity
    // ========================================================

    private void Start()
    {
        BuildPlayerCharacter();
    }


    private void LateUpdate()
    {
        UpdateMarkerRotation();
    }


    private void OnDestroy()
    {
        if (markerMesh != null)
        {
            Destroy(markerMesh);
        }

        if (markerMaterial != null)
        {
            Destroy(markerMaterial);
        }
    }


    // ========================================================
    // Build
    // ========================================================

    [ContextMenu("Build Player Character")]
    public void BuildPlayerCharacter()
    {
        ResolveSoldierPool();

        if (soldierPool == null)
        {
            Debug.LogWarning(
                "[PlayerCharacterVisual] SoldierPool을 찾을 수 없습니다."
            );

            return;
        }


        GameObject soldierPrefab =
            soldierPool.SoldierPrefab;

        if (soldierPrefab == null)
        {
            Debug.LogWarning(
                "[PlayerCharacterVisual] SoldierPool에 SoldierPrefab이 없습니다."
            );

            return;
        }


        SoldierVisualController sourceVisual =
            soldierPrefab.GetComponent<SoldierVisualController>();

        if (sourceVisual == null)
        {
            Debug.LogWarning(
                "[PlayerCharacterVisual] Soldier Prefab에 " +
                "SoldierVisualController가 없습니다."
            );

            return;
        }


        GameObject selectedSkinSource =
            sourceVisual.GetSavedSkinObject();

        if (selectedSkinSource == null)
        {
            Debug.LogWarning(
                "[PlayerCharacterVisual] 현재 선택된 Soldier Skin을 찾지 못했습니다."
            );

            return;
        }


        EnsureVisualRoot();
        ClearCurrentSkin();


        currentSkinInstance =
            Instantiate(
                selectedSkinSource,
                visualRoot,
                false
            );

        currentSkinInstance.name =
            $"PlayerSkin_{selectedSkinSource.name}";


        // Soldier Prefab 안에서 Skin에 설정되어 있던
        // Local Transform을 그대로 사용한다.
        currentSkinInstance.transform.localPosition =
            selectedSkinSource.transform.localPosition;

        currentSkinInstance.transform.localRotation =
            selectedSkinSource.transform.localRotation;

        currentSkinInstance.transform.localScale =
            selectedSkinSource.transform.localScale;


        // 원본 Skin이 Prefab에서 비활성 상태여도
        // Player에서는 선택된 Skin이므로 반드시 활성화한다.
        currentSkinInstance.SetActive(true);


        PlayRunAnimation();


        if (createPlayerMarker)
        {
            EnsurePlayerMarker();
        }


        Debug.Log(
            $"[PlayerCharacterVisual] Player Skin 적용 완료 | " +
            $"Skin : {selectedSkinSource.name} | " +
            $"Index : {sourceVisual.GetSavedSkinIndex()}"
        );
    }


    /// <summary>
    /// 로비 선택값 등이 런타임에 변경된 경우
    /// 다시 호출하면 현재 스킨으로 갱신할 수 있다.
    /// </summary>
    public void RefreshSkin()
    {
        BuildPlayerCharacter();
    }


    // ========================================================
    // Visual Root
    // ========================================================

    private void EnsureVisualRoot()
    {
        if (visualRoot == null)
        {
            Transform existing =
                transform.Find("PlayerVisualRoot");

            if (existing != null)
            {
                visualRoot = existing;
            }
            else
            {
                GameObject rootObject =
                    new GameObject("PlayerVisualRoot");

                visualRoot =
                    rootObject.transform;

                visualRoot.SetParent(
                    transform,
                    false
                );
            }
        }


        visualRoot.localPosition =
            visualLocalPosition;

        visualRoot.localRotation =
            Quaternion.Euler(
                visualLocalEulerAngles
            );

        visualRoot.localScale =
            visualLocalScale;
    }


    private void ClearCurrentSkin()
    {
        if (currentSkinInstance != null)
        {
            Destroy(
                currentSkinInstance
            );

            currentSkinInstance = null;
        }
    }


    // ========================================================
    // Run Animation
    // ========================================================

    private void PlayRunAnimation()
    {
        if (currentSkinInstance == null)
        {
            return;
        }


        Animator animator =
            currentSkinInstance.GetComponent<Animator>();

        if (animator == null)
        {
            animator =
                currentSkinInstance
                    .GetComponentInChildren<Animator>(true);
        }

        if (animator == null ||
            animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning(
                "[PlayerCharacterVisual] 선택된 Skin에서 Animator를 찾지 못했습니다."
            );

            return;
        }


        animator.enabled = true;
        animator.speed = 1f;


        // Run State 이름을 알고 있을 경우 강제로 Run을 재생한다.
        // State 이름이 다르면 기본 State를 그대로 사용한다.
        if (!string.IsNullOrWhiteSpace(
                runStateName))
        {
            int fullPathHash =
                Animator.StringToHash(
                    $"Base Layer.{runStateName}"
                );

            int shortNameHash =
                Animator.StringToHash(
                    runStateName
                );


            if (animator.HasState(
                    0,
                    fullPathHash))
            {
                animator.Play(
                    fullPathHash,
                    0,
                    0f
                );
            }
            else if (animator.HasState(
                         0,
                         shortNameHash))
            {
                animator.Play(
                    shortNameHash,
                    0,
                    0f
                );
            }
            else
            {
                Debug.LogWarning(
                    $"[PlayerCharacterVisual] Animator에 " +
                    $"'{runStateName}' State가 없어 " +
                    "기본 State를 사용합니다."
                );
            }
        }
    }


    // ========================================================
    // Marker
    // ========================================================

    private void EnsurePlayerMarker()
    {
        if (markerTransform != null)
        {
            return;
        }


        Transform existing =
            transform.Find("PlayerMarker");

        if (existing != null)
        {
            markerTransform = existing;
            return;
        }


        GameObject markerObject =
            new GameObject("PlayerMarker");

        markerTransform =
            markerObject.transform;

        markerTransform.SetParent(
            transform,
            false
        );

        markerTransform.localPosition =
            markerLocalPosition;


        MeshFilter meshFilter =
            markerObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer =
            markerObject.AddComponent<MeshRenderer>();


        markerMesh =
            CreateTriangleMesh();

        meshFilter.sharedMesh =
            markerMesh;


        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
        {
            shader =
                Shader.Find("Unlit/Color");
        }

        if (shader == null)
        {
            shader =
                Shader.Find("Standard");
        }


        if (shader != null)
        {
            markerMaterial =
                new Material(shader);

            if (markerMaterial.HasProperty(
                    "_BaseColor"))
            {
                markerMaterial.SetColor(
                    "_BaseColor",
                    markerColor
                );
            }

            if (markerMaterial.HasProperty(
                    "_Color"))
            {
                markerMaterial.SetColor(
                    "_Color",
                    markerColor
                );
            }

            meshRenderer.sharedMaterial =
                markerMaterial;
        }


        mainCamera =
            Camera.main;
    }


    private Mesh CreateTriangleMesh()
    {
        Mesh mesh =
            new Mesh();

        mesh.name =
            "PlayerMarkerTriangle";


        float halfWidth =
            markerWidth * 0.5f;

        float halfHeight =
            markerHeight * 0.5f;


        // 아래쪽을 가리키는 삼각형
        mesh.vertices =
            new Vector3[]
            {
                new Vector3(
                    -halfWidth,
                    halfHeight,
                    0f
                ),

                new Vector3(
                    0f,
                    -halfHeight,
                    0f
                ),

                new Vector3(
                    halfWidth,
                    halfHeight,
                    0f
                )
            };


        mesh.triangles =
            new int[]
            {
                0,
                1,
                2
            };


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }


    private void UpdateMarkerRotation()
    {
        if (markerTransform == null)
        {
            return;
        }


        if (mainCamera == null)
        {
            mainCamera =
                Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }


        Vector3 directionToCamera =
            mainCamera.transform.position -
            markerTransform.position;

        if (directionToCamera.sqrMagnitude <=
            0.0001f)
        {
            return;
        }


        markerTransform.rotation =
            Quaternion.LookRotation(
                directionToCamera.normalized,
                mainCamera.transform.up
            );
    }


    // ========================================================
    // Resolve
    // ========================================================

    private void ResolveSoldierPool()
    {
        if (soldierPool != null)
        {
            return;
        }


        soldierPool =
            Object.FindFirstObjectByType<SoldierPool>();
    }
}
