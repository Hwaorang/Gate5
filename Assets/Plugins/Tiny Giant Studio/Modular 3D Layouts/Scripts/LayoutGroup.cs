using System.Collections.Generic;
using UnityEngine;

#if MODULAR_3D_TEXT

using TinyGiantStudio.Text;

#endif

namespace TinyGiantStudio.Layout
{
    /// <summary>
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [HelpURL("https://ferdowsur.gitbook.io/modular-3d-text/layout-group")]
    public abstract class LayoutGroup : MonoBehaviour
    {
        [Tooltip("This is an experimental feature. In text, this bool is ignored, turn it on from the text itself. \nIf the child element has a Layout Element component attached, this value is derived by that layout element component.")]
        public bool autoItemSize = true;

        [Tooltip("For performance, it's better to leave it to false and call UpdateLayout() after making changes.\n" +
            "Turn this on if you are in a hurry or testing stuff.")]
        public bool alwaysUpdateInPlayMode = false;

        [Tooltip("For performance, it's better to leave it to false and call GetAllChildBounds() when a bound(size of an element) changes")]
        public bool alwaysUpdateBounds = false;

        public LayoutElementModuleContainer elementUpdater;

        [Tooltip("Auto updated with the layout. Visible for debugging purposes.")]
        public Bounds[] bounds;

        public bool showSceneViewGizmo = true;

        [ExecuteInEditMode]
        protected virtual void Update()
        {
            if (Application.isPlaying && !alwaysUpdateInPlayMode)
            {
                this.enabled = false;
                return;
            }

            if (TotalActiveChildCount() == 0)
                return;

#if MODULAR_3D_TEXT
            if (GetComponent<Modular3DText>())
                if (GetComponent<Modular3DText>().combineMeshInEditor)
                    return;
#endif
            UpdateLayout();
        }

        public abstract void UpdateLayout(int startRepositioningFrom = 0);

        /// <summary>
        /// Used to retrieve appropriate positions of meshes without needing to place them on scene for single mesh
        /// </summary>
        public abstract List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts);

        public int TotalActiveChildCount()
        {
            int child = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                    child++;
            }

            return child;
        }

        public Bounds GetBound(Transform target)
        {
            LayoutElement element = target.GetComponent<LayoutElement>();

            if (element)
                if (element.autoCalculateSize)
                    return MeshBaseSize.CheckMeshScaledSize(target);
                else
                    return new Bounds(new Vector3(element.xOffset, element.yOffset, element.zOffset), new Vector3(element.width, element.height, element.depth));

            if (autoItemSize)
                return MeshBaseSize.CheckMeshScaledSize(target);
            else
                return new Bounds(Vector3.zero, Vector3.one);
        }

        public Bounds GetBound(MeshLayout meshLayout)
        {
            return new Bounds(new Vector3(meshLayout.xOffset, meshLayout.yOffset, meshLayout.zOffset), new Vector3(meshLayout.width, meshLayout.height, meshLayout.depth));
        }

        public Bounds[] GetAllChildBounds()
        {
            Bounds[] bounds = new Bounds[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                bounds[i] = GetBound(transform.GetChild(i));
            }
            return bounds;
        }

        public Bounds[] GetAllChildBounds(List<MeshLayout> meshLayouts)
        {
            Bounds[] bounds = new Bounds[meshLayouts.Count];
            for (int i = 0; i < meshLayouts.Count; i++)
            {
                bounds[i] = GetBound(meshLayouts[i]);
            }
            return bounds;
        }

        public bool IgnoreChildBound(Bounds[] bounds, int i)
        {
            //Added the guard clause because someone was having an error with transform.GetChild(i) in the IgnoreChildBoundAndLineBreak() method. Which is weird. Requires further investigation
            if (i < 0 || i >= transform.childCount)
                return true;

            if (transform.GetChild(i).GetComponent<LayoutElement>())
                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement == true)
                    return true;

            //Added this just in case. Not required. Requires further investigation
            if (i >= bounds.Length)
                return true;

            return !transform.GetChild(i).gameObject.activeSelf || bounds[i].size == Vector3.zero;
        }

        public bool IgnoreChildBoundAndLineBreak(Bounds[] bounds, int i)
        {
            //Added the guard clause because someone was having an error with transform.GetChild(i). Which is weird. Requires further investigation
            if (i < 0 || i >= transform.childCount)
                return true;

            if (transform.GetChild(i).GetComponent<LayoutElement>())
                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement == true || transform.GetChild(i).GetComponent<LayoutElement>().lineBreak == true)
                    return true;

            //Added this just in case. Not required. Requires further investigation
            if (i >= bounds.Length)
                return true;

            return !transform.GetChild(i).gameObject.activeSelf || bounds[i].size == Vector3.zero;
        }

        public Vector3 RemoveNaNErrorIfAny(Vector3 vector3)
        {
            if (float.IsNaN(vector3.x))
                vector3.x = 0;
            if (float.IsNaN(vector3.y))
                vector3.y = 0;
            if (float.IsNaN(vector3.z))
                vector3.z = 0;

            return vector3;
        }

        //#if MODULAR_3D_TEXT

        //        /// <summary>
        //        /// Safely dispose of the mesh from memory
        //        /// </summary>
        //        /// <param name="meshLayout"></param>
        //        /// <returns></returns>
        //        public MeshLayout ClearMeshLayout(MeshLayout meshLayout)
        //        {
        //            DestroyMesh(meshLayout.mesh);
        //            meshLayout.mesh = null;
        //            return meshLayout;
        //        }

        //#endif

        ///// <summary>
        ///// This is used to destroy meshes
        ///// </summary>
        ///// <param name="mesh"></param>
        //public void DestroyMesh(Mesh mesh)
        //{
        //    if (mesh == null) return;

        //    string assetPath = AssetDatabase.GetAssetPath(mesh);
        //    if (string.IsNullOrEmpty(assetPath))
        //    {
        //        if (Application.isPlaying) Destroy(mesh);
        //        else DestroyImmediate(mesh);
        //    }
        //}

        /// <summary>
        /// Draws each element bound.
        /// This is public so that this can be optionally called.
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            if (!showSceneViewGizmo)
                return;

            Gizmos.color = new Color(0.25f, 0, 0, 1f);
            Vector3 lossyScale = transform.lossyScale;

            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeSelf)
                    continue;
                if (child.TryGetComponent<LayoutElement>(out var layoutElement))
                {
                    if (layoutElement.space || layoutElement.lineBreak || layoutElement.ignoreElement)
                        continue;
                }
                Bounds bound = GetBound(child);
                Gizmos.matrix = Matrix4x4.TRS(child.position, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Multiply(bound.center, lossyScale), Multiply(bound.size, lossyScale));
            }
        }

        private Vector3 Multiply(Vector3 first, Vector3 second)
        {
            return new Vector3(first.x * second.x, first.y * second.y, first.z * second.z);
        }
    }
}