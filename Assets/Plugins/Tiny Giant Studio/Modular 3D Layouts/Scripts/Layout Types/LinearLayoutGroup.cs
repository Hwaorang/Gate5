using System.Collections.Generic;
using TinyGiantStudio.Text;
using UnityEngine;

#if MODULAR_3D_TEXT
#endif

namespace TinyGiantStudio.Layout
{
    [HelpURL("https://ferdowsur.gitbook.io/layout-system/layout-group/linear-layout-group")]
    [AddComponentMenu("Tiny Giant Studio/Layout/Linear Layout Group (M3D)", 30003)]
    public class LinearLayoutGroup : LayoutGroup
    {
        #region Variable Declaration

        public float spacing = 0;
        public Alignment alignment = Alignment.HorizontalMiddle;
        public Alignment secondaryAlignment = Alignment.VerticleMiddle;

        private bool startLoopFromEnd = true;

        public bool randomizeRotations = false;

        [SerializeField] private Vector3 _minimumRandomRotation = Vector3.zero;

        public Vector3 MinimumRandomRotation
        {
            get { return _minimumRandomRotation; }
            set
            {
                if (_minimumRandomRotation != value)
                {
                    rotationChanged = true;
                }

                _minimumRandomRotation = value;
            }
        }

        public Vector3 maximumRandomRotation = Vector3.zero;
        public bool rotationChanged = false;

        public enum Alignment
        {
            Top,
            VerticleMiddle,
            Bottom,
            Left,
            HorizontalMiddle,
            Right
        }

        public float totalSpaceTaken;

        public Overflow overflow;
        public float width = 20;

        [Tooltip("If enabled, will try to cut off characters that are slightly over the border due to their width")]
        public bool addCharacterWidthWhenCaclulatingOverflow = false;

        #endregion Variable Declaration

        protected override void Update()
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
            {
                if (GetComponent<Modular3DText>().combineMeshInEditor)
                    return;
            }
            else
                UpdateLayout();
#endif
        }

        public override void UpdateLayout(int startRepositioningFrom = 0)
        {
            if (TotalActiveChildCount() == 0)
                return;

            if (!Application.isPlaying || alwaysUpdateBounds || TotalActiveChildCount() != bounds.Length)
                bounds = GetAllChildBounds();

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds, spacing);
            //GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            rotationChanged = false;

            if (alignment == Alignment.HorizontalMiddle)
                totalSpaceTaken = Mathf.Abs(x * 2);
            else
                totalSpaceTaken = Mathf.Abs(x);
        }

        public void UpdateLayout(bool ignoreOverflowSettings)
        {
            int startRepositioningFrom = 0;

            if (TotalActiveChildCount() == 0)
                return;

            if (!Application.isPlaying || alwaysUpdateBounds || TotalActiveChildCount() != bounds.Length)
                bounds = GetAllChildBounds();

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds, spacing, true);
            //GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom, ignoreOverflowSettings);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom, ignoreOverflowSettings);
                }
            }
            rotationChanged = false;

            if (alignment == Alignment.HorizontalMiddle)
                totalSpaceTaken = Mathf.Abs(x * 2);
            else
                totalSpaceTaken = Mathf.Abs(x);
        }

        /// <summary>
        /// This is used by Auto Size feature of the 3D Text
        /// </summary>
        /// <param name="startRepositioningFrom"></param>
        public void UpdateLayoutDoNotUpdateBounds(int startRepositioningFrom = 0)
        {
            if (TotalActiveChildCount() == 0)
                return;

            //if (!Application.isPlaying || alwaysUpdateBounds || TotalActiveChildCount() != bounds.Length)
            //    bounds = GetAllChildBounds();

            float x = 0;
            float y = 0;
            GetStartingPosition(ref x, ref y, bounds, spacing);
            //GetPositionValuesAccordingToSelectedLayout(ref x, ref y, bounds);

            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (IgnoreChildBound(bounds, i))
                        continue;

                    SetChildPosition(ref x, ref y, i, bounds[i], startRepositioningFrom);
                }
            }
            rotationChanged = false;

            if (alignment == Alignment.HorizontalMiddle)
                totalSpaceTaken = Mathf.Abs(x * 2);
            else
                totalSpaceTaken = Mathf.Abs(x);
        }

        /// <summary>
        /// Same as UpdateLayout but for combined meshes
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public override List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts)
        {
            if (meshLayouts.Count == 0)
                return null;

            //Bounds[] bounds = GetAllChildBounds(meshLayouts); //commented out on June, why was it using local variable?
            bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds, spacing);
            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], spacing);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], spacing);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            rotationChanged = false;

            return meshLayouts;
        }

        /// <summary>
        /// This is used by auto size of Text to check the width of text
        /// Same as UpdateLayout but for combined meshes
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts, out float totalSpaceTaken, bool ignoreOverflowSetting = true)
        {
            totalSpaceTaken = 0;

            if (meshLayouts.Count == 0)
                return null;

            //Bounds[] bounds = GetAllChildBounds(meshLayouts); //commented out on June, why was it using local variable?
            bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds, spacing, ignoreOverflowSetting);
            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], spacing, true);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], spacing, true);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            rotationChanged = false;

            if (alignment == Alignment.HorizontalMiddle)
                totalSpaceTaken = Mathf.Abs(x * 2);
            else
                totalSpaceTaken = Mathf.Abs(x);

            return meshLayouts;
        }

        /// <summary>
        /// Same as UpdateLayout but for combined meshes
        /// </summary>
        /// <param name="meshLayouts"></param>
        /// <returns></returns>
        public List<MeshLayout> GetPositions(List<MeshLayout> meshLayouts, float modifiedSpacing)
        {
            if (meshLayouts.Count == 0)
                return null;

            bounds = GetAllChildBounds(meshLayouts);

            float x = 0;
            float y = 0;

            GetStartingPosition(ref x, ref y, bounds, modifiedSpacing);
            startLoopFromEnd = StartLoopFromEnd();

            if (startLoopFromEnd)
            {
                for (int i = meshLayouts.Count - 1; i >= 0; i--)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], modifiedSpacing);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            else
            {
                for (int i = 0; i < meshLayouts.Count; i++)
                {
                    meshLayouts[i] = SetMeshPosition(ref x, ref y, bounds[i], meshLayouts[i], modifiedSpacing);
                    if (randomizeRotations) meshLayouts[i].rotation.eulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
            rotationChanged = false;

            return meshLayouts;
        }

        private MeshLayout SetMeshPosition(ref float x, ref float y, Bounds bound, MeshLayout meshLayout, float modifiedSpacing, bool ignoreOverlow = false)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (modifiedSpacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (modifiedSpacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (modifiedSpacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (modifiedSpacing / 2) + (bound.size.x) / 2;

                x -= bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            if (!ignoreOverlow && overflow == Overflow.wrap)
            {
                if (alignment == Alignment.HorizontalMiddle)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + (meshLayout.width / 2) > width / 2)
                            meshLayout.mesh = null;
                    }
                    else
                    {
                        if (x > width / 2)
                            meshLayout.mesh = null;
                    }
                }
                else if (alignment == Alignment.Right)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + (meshLayout.width / 2f) > 0)
                            meshLayout.mesh = null;
                    }
                    else
                    {
                        if (x > 0)
                            meshLayout.mesh = null;
                    }
                }
                else if (alignment == Alignment.Left)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + (meshLayout.width / 2f) > width)
                            meshLayout.mesh = null;
                    }
                    else
                    {
                        if (x > width)
                            meshLayout.mesh = null;
                    }
                }
            }

            meshLayout.position = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            //transform.GetChild(i).localPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }
            x += toAddX;
            y += toAddY;

            return meshLayout;
        }

        private void SetChildPosition(ref float x, ref float y, int i, Bounds bound, int startRepositioningFrom, bool ignoreOverflowSettings = false)
        {
            float toAddX = 0;
            float toAddY = 0;

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle)
            {
                toAddY -= (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Top)
            {
                toAddY += (spacing / 2) + (bound.size.y) / 2;
                y -= bound.center.y;
            }
            else if (alignment == Alignment.Left)
            {
                toAddX += (spacing / 2) + (bound.size.x) / 2;
                x -= bound.center.x;
            }
            else if (alignment == Alignment.Right || alignment == Alignment.HorizontalMiddle)
            {
                toAddX -= (spacing / 2) + (bound.size.x) / 2;

                x -= bound.center.x;
            }

            x += toAddX;
            y += toAddY;

            if (!ignoreOverflowSettings && overflow == Overflow.wrap)
            {
                if (alignment == Alignment.HorizontalMiddle)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + bound.extents.x > width / 2)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        if (x > width / 2)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else if (alignment == Alignment.Right)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + bound.extents.x > 0)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        if (x > 0)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else if (alignment == Alignment.Left)
                {
                    if (addCharacterWidthWhenCaclulatingOverflow)
                    {
                        if (x + bound.extents.x > width)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        if (x > width)
                            transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            Vector3 targetPosition = RemoveNaNErrorIfAny(new Vector3(x, y, 0));
            if (i >= startRepositioningFrom)
                SetLocalPosition(transform.GetChild(i), targetPosition);

            if (alignment == Alignment.Bottom || alignment == Alignment.VerticleMiddle || alignment == Alignment.Top)
            {
                y += bound.center.y;
            }
            else if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                x += bound.center.x;
            }

            x += toAddX;
            y += toAddY;
        }

        private void SetLocalPosition(Transform target, Vector3 targetPosition)
        {
            if (Application.isPlaying && elementUpdater.module)
            {
                if (!randomizeRotations)
                    elementUpdater.module.UpdateLocalPosition(target, elementUpdater.variableHolders, targetPosition);
                else
                    elementUpdater.module.UpdateLocalTransform(target, elementUpdater.variableHolders, targetPosition, GetRandomQuaternionRotation(MinimumRandomRotation, maximumRandomRotation));
            }
            else
            {
                if (target.localPosition != targetPosition)
                {
                    target.localPosition = targetPosition;

                    if (randomizeRotations)
                    {
                        target.localEulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                    }
                }
                else if (rotationChanged && randomizeRotations)
                {
                    target.localEulerAngles = GetRandomRotation(MinimumRandomRotation, maximumRandomRotation);
                }
            }
        }

        private Vector3 GetRandomRotation(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);

            return new Vector3(x == float.NaN ? 0 : x, y == float.NaN ? 0 : y, z == float.NaN ? 0 : z);
        }

        private Quaternion GetRandomQuaternionRotation(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);

            return Quaternion.Euler(x == float.NaN ? 0 : x, y == float.NaN ? 0 : y, z == float.NaN ? 0 : z);
        }

        private void GetStartingPosition(ref float x, ref float y, Bounds[] bounds, float currentSpacing, bool ignoreOverflowSetting = false)
        {
            switch (alignment)
            {
                case Alignment.Top:
                    y = -currentSpacing / 2; //This neutralizes the spacing for the starting character
                    break;

                case Alignment.VerticleMiddle:
                    for (int i = 0; i < bounds.Length; i++)
                    {
                        if (bounds[i].size == Vector3.zero)
                            continue;

                        y += bounds[i].size.y + currentSpacing;
                    }

                    y /= 2;
                    break;

                case Alignment.Bottom:
                    y = currentSpacing / 2; //This neutralizes the spacing for the last character
                    break;

                case Alignment.Left:
                    x = -currentSpacing / 2;
                    break;

                case Alignment.HorizontalMiddle:
                    for (int i = 0; i < bounds.Length; i++)
                    {
                        if (i < transform.childCount && transform.GetChild(i))
                            if (transform.GetChild(i).GetComponent<LayoutElement>())
                                if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
                                    continue;

                        if (bounds[i].size == Vector3.zero)
                            continue;

                        x += bounds[i].size.x + currentSpacing;
                    }

                    if (!ignoreOverflowSetting && overflow == Overflow.wrap)
                    {
                        if (x > width)
                            x -= (width - x);
                    }

                    x /= 2;
                    break;

                case Alignment.Right:

                    x = (currentSpacing / 2);
                    if (!ignoreOverflowSetting && overflow == Overflow.wrap)
                    {
                        float spaceRequired = 0;

                        for (int i = 0; i < bounds.Length; i++)
                        {
                            if (i < transform.childCount && transform.GetChild(i))
                                if (transform.GetChild(i).GetComponent<LayoutElement>())
                                    if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
                                        continue;

                            if (bounds[i].size == Vector3.zero)
                                continue;

                            spaceRequired += bounds[i].size.x + currentSpacing;
                        }

                        if (spaceRequired > width)
                        {
                            x = (spaceRequired - width);
                        }
                    }

                    break;

                default:
                    break;
            }

            if (alignment == Alignment.Left || alignment == Alignment.HorizontalMiddle || alignment == Alignment.Right)
            {
                if (secondaryAlignment == Alignment.Top)
                    y = (MaxBoundHeight(bounds) / 2);
                //y = (-spacing / 2);
                else if (secondaryAlignment == Alignment.Bottom)
                    y = (-MaxBoundHeight(bounds) / 2);
                //y = spacing / 2;
            }
        }

        private float MaxBoundHeight(Bounds[] allBounds)
        {
            float y = 0;
            foreach (Bounds bounds in allBounds)
                if (bounds.size.y > y)
                    y = bounds.size.y;

            return y;
        }

        ///// <summary>
        ///// Legacy. Use GetStartingPosition
        ///// </summary>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <param name="bounds"></param>
        //private void GetPositionValuesAccordingToSelectedLayout(ref float x, ref float y, Bounds[] bounds)
        //{
        //    if (alignment == Alignment.Bottom)
        //    {
        //        y = spacing / 2;
        //    }
        //    else if (alignment == Alignment.VerticleMiddle)
        //    {
        //        for (int i = 0; i < bounds.Length; i++)
        //        {
        //            if (bounds[i].size == Vector3.zero)
        //                continue;

        //            y += bounds[i].size.y + spacing;
        //        }

        //        y /= 2;
        //    }
        //    else if (alignment == Alignment.Top)
        //    {
        //        y = -spacing / 2;
        //    }
        //    else if (alignment == Alignment.Left)
        //    {
        //        x = -spacing / 2;
        //    }
        //    else if (alignment == Alignment.HorizontalMiddle)
        //    {
        //        for (int i = 0; i < bounds.Length; i++)
        //        {
        //            if (i < transform.childCount && transform.GetChild(i))
        //                if (transform.GetChild(i).GetComponent<LayoutElement>())
        //                    if (transform.GetChild(i).GetComponent<LayoutElement>().ignoreElement)
        //                        continue;

        //            if (bounds[i].size == Vector3.zero)
        //                continue;

        //            x += bounds[i].size.x + spacing;
        //        }

        //        x /= 2;
        //    }
        //    else if (alignment == Alignment.Right)
        //    {
        //        x = (spacing / 2);
        //    }
        //}

        private bool StartLoopFromEnd()
        {
            if (alignment == Alignment.Top) return true;
            else if (alignment == Alignment.VerticleMiddle) return false;
            else if (alignment == Alignment.Bottom) return false;
            else if (alignment == Alignment.Left) return false;
            else if (alignment == Alignment.HorizontalMiddle) return true;
            else if (alignment == Alignment.Right) return true;

            return false; //this never happens
        }

#if UNITY_EDITOR

        /// <summary>
        /// Draws the width and height
        /// </summary>
        //void OnDrawGizmos()
        private new void OnDrawGizmosSelected()
        {
            if (!showSceneViewGizmo)
                return;

            base.OnDrawGizmosSelected();

#if MODULAR_3D_TEXT
            if (!gameObject.GetComponent<Modular3DText>())
            {
                if (overflow == Overflow.overflow)
                    return;
            }
            else
            {
                if (!gameObject.GetComponent<Modular3DText>().autoFontSize && overflow == Overflow.overflow)
                    return;
            }
#else
           if (overflow == Overflow.overflow)
                return;
#endif

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0, 1, 0, 0.75f);

            Vector3 offset = Vector3.zero;
            if (alignment == LinearLayoutGroup.Alignment.Right)
                offset = new Vector3(-width / 2f, 0, 0);
            else if (alignment == LinearLayoutGroup.Alignment.Left)
                offset = new Vector3(width / 2f, 0, 0);

            Gizmos.DrawWireCube(offset, new Vector3(width, 0.1f, 0.001f));
        }

#endif
    }
}