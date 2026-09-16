using TMPro;
using UnityEditor;
using UnityEngine;

public static class Gate3DBuilder
{
    // ========================================================
    // Menu
    // ========================================================

    [MenuItem("Tools/Gate/Create Blue Gate")]
    public static void CreateBlueGate()
    {
        CreateGate(
            "Gate_Blue",
            new Color(
                0.05f,
                0.45f,
                1f,
                0.35f
            ),
            new Color(
                0.02f,
                0.20f,
                0.85f,
                1f
            )
        );
    }


    [MenuItem("Tools/Gate/Create Red Gate")]
    public static void CreateRedGate()
    {
        CreateGate(
            "Gate_Red",
            new Color(
                1f,
                0.10f,
                0.10f,
                0.35f
            ),
            new Color(
                0.85f,
                0.02f,
                0.02f,
                1f
            )
        );
    }


    // ========================================================
    // Create Gate
    // ========================================================

    private static void CreateGate(
        string gateName,
        Color panelColor,
        Color frameColor)
    {
        // =========================
        // Root
        // =========================

        GameObject root =
            new GameObject(
                gateName
            );


        Undo.RegisterCreatedObjectUndo(
            root,
            "Create Gate"
        );


        // =========================
        // Visual Root
        // =========================

        GameObject visual =
            new GameObject(
                "Visual"
            );


        visual.transform.SetParent(
            root.transform,
            false
        );


        // =========================
        // Materials
        // =========================

        Material panelMaterial =
            CreateTransparentMaterial(
                gateName + "_Panel",
                panelColor
            );


        Material frameMaterial =
            CreateOpaqueMaterial(
                gateName + "_Frame",
                frameColor
            );


        // =========================
        // Glass Panel
        // =========================

        GameObject panel =
            CreateCube(
                "GlassPanel",
                visual.transform
            );


        panel.transform.localPosition =
            new Vector3(
                0f,
                1.8f,
                0f
            );


        panel.transform.localScale =
            new Vector3(
                3.8f,
                3.6f,
                0.10f
            );


        Renderer panelRenderer =
            panel.GetComponent<Renderer>();


        panelRenderer.sharedMaterial =
            panelMaterial;


        // GlassPanel에는
        // 충돌이 필요하지 않다.
        Object.DestroyImmediate(
            panel.GetComponent<BoxCollider>()
        );


        // =========================
        // Left Post
        // =========================

        GameObject leftPost =
            CreateCube(
                "LeftPost",
                visual.transform
            );


        leftPost.transform.localPosition =
            new Vector3(
                -2.05f,
                1.8f,
                0f
            );


        leftPost.transform.localScale =
            new Vector3(
                0.30f,
                4.2f,
                0.30f
            );


        leftPost
            .GetComponent<Renderer>()
            .sharedMaterial =
                frameMaterial;


        Object.DestroyImmediate(
            leftPost.GetComponent<BoxCollider>()
        );


        // =========================
        // Right Post
        // =========================

        GameObject rightPost =
            CreateCube(
                "RightPost",
                visual.transform
            );


        rightPost.transform.localPosition =
            new Vector3(
                2.05f,
                1.8f,
                0f
            );


        rightPost.transform.localScale =
            new Vector3(
                0.30f,
                4.2f,
                0.30f
            );


        rightPost
            .GetComponent<Renderer>()
            .sharedMaterial =
                frameMaterial;


        Object.DestroyImmediate(
            rightPost.GetComponent<BoxCollider>()
        );


        // =========================
        // Top Rail
        // =========================

        GameObject topRail =
            CreateCube(
                "TopRail",
                visual.transform
            );


        topRail.transform.localPosition =
            new Vector3(
                0f,
                3.75f,
                0f
            );


        topRail.transform.localScale =
            new Vector3(
                4.1f,
                0.22f,
                0.30f
            );


        topRail
            .GetComponent<Renderer>()
            .sharedMaterial =
                frameMaterial;


        Object.DestroyImmediate(
            topRail.GetComponent<BoxCollider>()
        );


        // =========================
        // Bottom Rail
        // =========================

        GameObject bottomRail =
            CreateCube(
                "BottomRail",
                visual.transform
            );


        bottomRail.transform.localPosition =
            new Vector3(
                0f,
                -0.15f,
                0f
            );


        bottomRail.transform.localScale =
            new Vector3(
                4.1f,
                0.22f,
                0.30f
            );


        bottomRail
            .GetComponent<Renderer>()
            .sharedMaterial =
                frameMaterial;


        Object.DestroyImmediate(
            bottomRail.GetComponent<BoxCollider>()
        );


        // =========================
        // TMP Text
        // =========================

        GameObject textObject =
            new GameObject(
                "ValueText"
            );


        textObject.transform.SetParent(
            root.transform,
            false
        );


        TextMeshPro text =
            textObject.AddComponent<TextMeshPro>();


        text.text =
            "+10";


        text.fontSize =
            8f;


        text.fontStyle =
            FontStyles.Bold;


        text.alignment =
            TextAlignmentOptions.Center;


        text.color =
            Color.white;


        textObject.transform.localPosition =
            new Vector3(
                0f,
                1.8f,
                -0.12f
            );


        textObject.transform.localScale =
            Vector3.one;


        RectTransform textRect =
            text.GetComponent<RectTransform>();


        textRect.sizeDelta =
            new Vector2(
                4f,
                2f
            );


        // =========================
        // Trigger
        // =========================

        GameObject trigger =
            new GameObject(
                "Trigger"
            );


        trigger.transform.SetParent(
            root.transform,
            false
        );


        trigger.transform.localPosition =
            new Vector3(
                0f,
                1.8f,
                0f
            );


        BoxCollider triggerCollider =
            trigger.AddComponent<BoxCollider>();


        triggerCollider.isTrigger =
            true;


        triggerCollider.size =
            new Vector3(
                4f,
                3.8f,
                0.8f
            );


        // =========================
        // Selection
        // =========================

        Selection.activeGameObject =
            root;


        SceneView.lastActiveSceneView?.FrameSelected();


        Debug.Log(
            $"[Gate3DBuilder] " +
            $"{gateName} 생성 완료"
        );
    }


    // ========================================================
    // Cube
    // ========================================================

    private static GameObject CreateCube(
        string objectName,
        Transform parent)
    {
        GameObject cube =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );


        cube.name =
            objectName;


        cube.transform.SetParent(
            parent,
            false
        );


        return cube;
    }


    // ========================================================
    // Transparent Material
    // ========================================================

    private static Material CreateTransparentMaterial(
        string materialName,
        Color color)
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Lit"
            );


        if (shader == null)
        {
            shader =
                Shader.Find(
                    "Standard"
                );
        }


        Material material =
            new Material(
                shader
            );


        material.name =
            materialName;


        material.color =
            color;


        // =========================
        // URP Transparent
        // =========================

        if (material.HasProperty(
                "_Surface"))
        {
            material.SetFloat(
                "_Surface",
                1f
            );


            material.SetFloat(
                "_Blend",
                0f
            );


            material.SetFloat(
                "_ZWrite",
                0f
            );


            material.EnableKeyword(
                "_SURFACE_TYPE_TRANSPARENT"
            );


            material.renderQueue =
                3000;
        }


        // =========================
        // Standard Transparent
        // =========================

        if (material.HasProperty(
                "_Mode"))
        {
            material.SetFloat(
                "_Mode",
                3f
            );


            material.SetInt(
                "_SrcBlend",
                (int)UnityEngine.Rendering.BlendMode.SrcAlpha
            );


            material.SetInt(
                "_DstBlend",
                (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
            );


            material.SetInt(
                "_ZWrite",
                0
            );


            material.DisableKeyword(
                "_ALPHATEST_ON"
            );


            material.EnableKeyword(
                "_ALPHABLEND_ON"
            );


            material.renderQueue =
                3000;
        }


        return material;
    }


    // ========================================================
    // Frame Material
    // ========================================================

    private static Material CreateOpaqueMaterial(
        string materialName,
        Color color)
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Lit"
            );


        if (shader == null)
        {
            shader =
                Shader.Find(
                    "Standard"
                );
        }


        Material material =
            new Material(
                shader
            );


        material.name =
            materialName;


        material.color =
            color;


        return material;
    }
}