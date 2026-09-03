using GptAsset.HyperCasualBulletFX;
using UnityEngine;

namespace GptAsset.HyperCasualBulletFX.Samples
{
    /// <summary>Optional example for independently triggering the fire and impact visuals.</summary>
    public sealed class BulletFxVisualOnlyExample : MonoBehaviour
    {
        [SerializeField] private HyperCasualBulletFx bulletFx;
        [SerializeField] private Transform muzzle;
        [SerializeField, Min(0.1f)] private float visualDistance = 25f;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                PlayFireVisual();
        }

        public void PlayFireVisual()
        {
            if (bulletFx == null || muzzle == null) return;
            bulletFx.Play(muzzle.position, muzzle.forward, visualDistance);
        }

        public void PlayImpactVisual(Vector3 position, Vector3 normal)
        {
            if (bulletFx == null) return;
            bulletFx.PlayImpact(position, normal);
        }
    }
}
