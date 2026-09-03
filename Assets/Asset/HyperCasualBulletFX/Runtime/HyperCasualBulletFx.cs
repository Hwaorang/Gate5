using UnityEngine;

namespace GptAsset.HyperCasualBulletFX
{
    /// <summary>
    /// Visual-only gunfire. This component never performs hit detection or physics queries.
    /// All renderers are allocated once and then reused.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HyperCasualBulletFx : MonoBehaviour
    {
        [Header("Pooling")]
        [SerializeField, Min(1)] private int poolSize = 20;
        [SerializeField, Range(1, 12)] private int visualBulletCount = 6;

        [Header("Tracer")]
        [SerializeField, Min(1f)] private float tracerSpeed = 115f;
        [SerializeField, Min(0.001f)] private float tracerLength = 2.2f;
        [SerializeField, Min(0.001f)] private float tracerWidth = 0.035f;
        [SerializeField, Min(0f)] private float visualSpread = 0.22f;
        [SerializeField] private Color tracerColor = new Color(1f, 0.58f, 0.12f, 1f);
        [SerializeField] private Color tracerTipColor = new Color(1f, 0.95f, 0.66f, 0.05f);

        [Header("Muzzle Flash")]
        [SerializeField, Min(0f)] private float muzzleSize = 0.32f;
        [SerializeField, Min(0f)] private float muzzleDuration = 0.055f;
        [SerializeField] private Color muzzleColor = new Color(1f, 0.72f, 0.2f, 1f);

        [Header("Impact")]
        [SerializeField, Range(0, 8)] private int impactSparkCount = 5;
        [SerializeField, Min(0f)] private float impactSparkLength = 0.28f;
        [SerializeField, Min(0f)] private float impactDuration = 0.12f;
        [SerializeField] private Color impactColor = new Color(1f, 0.46f, 0.08f, 1f);

        private const int MaxTracers = 12;
        private const int MaxSparks = 8;
        private const int MuzzleLines = 3;

        private ShotVisual[] pool;
        private int nextSlot;
        private Material sharedMaterial;

        private sealed class ShotVisual
        {
            public GameObject root;
            public LineRenderer[] tracers;
            public LineRenderer[] muzzle;
            public LineRenderer[] sparks;
            public Vector3 origin;
            public Vector3 end;
            public Vector3 normal;
            public float distance;
            public float age;
            public bool showTracers;
            public bool showMuzzle;
            public bool showImpact;
            public bool active;
            public uint seed;
        }

        private void Awake()
        {
            BuildPool();
        }

        private void OnValidate()
        {
            poolSize = Mathf.Max(1, poolSize);
            visualBulletCount = Mathf.Clamp(visualBulletCount, 1, MaxTracers);
            impactSparkCount = Mathf.Clamp(impactSparkCount, 0, MaxSparks);
        }

        private void Update()
        {
            if (pool == null) return;

            float dt = Time.deltaTime;
            for (int i = 0; i < pool.Length; i++)
            {
                if (pool[i].active)
                    Tick(pool[i], dt);
            }
        }

        /// <summary>Plays muzzle flash and several cosmetic tracers in the supplied direction.</summary>
        public void Play(Vector3 origin, Vector3 direction, float visualDistance = 25f)
        {
            EnsurePool();
            ShotVisual shot = pool[nextSlot];
            nextSlot = (nextSlot + 1) % pool.Length;

            ResetLines(shot);
            Vector3 safeDirection = direction.sqrMagnitude > 0.001f ? direction.normalized : transform.forward;
            shot.origin = origin;
            shot.distance = Mathf.Max(0.01f, visualDistance);
            shot.end = origin + safeDirection * shot.distance;
            shot.normal = -safeDirection;
            shot.age = 0f;
            shot.showTracers = true;
            shot.showMuzzle = true;
            shot.showImpact = false;
            shot.active = true;
            shot.seed = (uint)(Time.frameCount * 747796405u + (uint)nextSlot * 2891336453u + 1u);
            shot.root.SetActive(true);

            ConfigureStaticShape(shot);
            Tick(shot, 0f);
        }

        /// <summary>Plays only the cosmetic impact sparks at a supplied position and orientation.</summary>
        public void PlayImpact(Vector3 position, Vector3 surfaceNormal)
        {
            EnsurePool();
            ShotVisual shot = pool[nextSlot];
            nextSlot = (nextSlot + 1) % pool.Length;

            ResetLines(shot);
            shot.origin = position;
            shot.end = position;
            shot.normal = surfaceNormal.sqrMagnitude > 0.001f ? surfaceNormal.normalized : Vector3.up;
            shot.distance = 0.01f;
            shot.age = 0f;
            shot.showTracers = false;
            shot.showMuzzle = false;
            shot.showImpact = true;
            shot.active = true;
            shot.seed = (uint)(Time.frameCount * 747796405u + (uint)nextSlot * 2891336453u + 1u);
            shot.root.SetActive(true);

            ConfigureStaticShape(shot);
            Tick(shot, 0f);
        }

        private void BuildPool()
        {
            if (pool != null) return;

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null) shader = Shader.Find("Unlit/Color");

            sharedMaterial = new Material(shader)
            {
                name = "Bullet FX Runtime Material",
                hideFlags = HideFlags.HideAndDontSave
            };
            if (sharedMaterial.HasProperty("_Surface")) sharedMaterial.SetFloat("_Surface", 1f);
            if (sharedMaterial.HasProperty("_Blend")) sharedMaterial.SetFloat("_Blend", 1f);
            if (sharedMaterial.HasProperty("_SrcBlend")) sharedMaterial.SetFloat("_SrcBlend", 5f);
            if (sharedMaterial.HasProperty("_DstBlend")) sharedMaterial.SetFloat("_DstBlend", 1f);
            if (sharedMaterial.HasProperty("_ZWrite")) sharedMaterial.SetFloat("_ZWrite", 0f);
            sharedMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            sharedMaterial.SetOverrideTag("RenderType", "Transparent");
            sharedMaterial.renderQueue = 3000;

            pool = new ShotVisual[poolSize];
            for (int i = 0; i < pool.Length; i++)
            {
                var shot = new ShotVisual
                {
                    root = new GameObject("Bullet FX Slot " + i),
                    tracers = new LineRenderer[MaxTracers],
                    muzzle = new LineRenderer[MuzzleLines],
                    sparks = new LineRenderer[MaxSparks]
                };
                shot.root.transform.SetParent(transform, false);

                for (int n = 0; n < MaxTracers; n++)
                    shot.tracers[n] = CreateLine(shot.root.transform, "Tracer " + n);
                for (int n = 0; n < MuzzleLines; n++)
                    shot.muzzle[n] = CreateLine(shot.root.transform, "Muzzle " + n);
                for (int n = 0; n < MaxSparks; n++)
                    shot.sparks[n] = CreateLine(shot.root.transform, "Impact Spark " + n);

                shot.root.SetActive(false);
                pool[i] = shot;
            }
        }

        private LineRenderer CreateLine(Transform parent, string objectName)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            var line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = sharedMaterial;
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.numCapVertices = 2;
            line.alignment = LineAlignment.View;
            line.textureMode = LineTextureMode.Stretch;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.enabled = false;
            return line;
        }

        private void ConfigureStaticShape(ShotVisual shot)
        {
            Vector3 forward = (shot.end - shot.origin).normalized;
            BuildBasis(forward, out Vector3 right, out Vector3 up);

            for (int i = 0; i < MaxTracers; i++)
            {
                LineRenderer line = shot.tracers[i];
                bool enabled = shot.showTracers && i < visualBulletCount;
                line.enabled = enabled;
                if (!enabled) continue;

                float scale = i == 0 ? 1f : RandomRange(ref shot.seed, 0.58f, 0.9f);
                line.widthMultiplier = tracerWidth * scale;
                line.startColor = tracerColor;
                line.endColor = tracerTipColor;
            }

            for (int i = 0; i < MuzzleLines; i++)
            {
                LineRenderer line = shot.muzzle[i];
                line.enabled = shot.showMuzzle && muzzleSize > 0f && muzzleDuration > 0f;
                line.widthMultiplier = tracerWidth * (2.6f - i * 0.45f);
                line.startColor = muzzleColor;
                line.endColor = new Color(muzzleColor.r, muzzleColor.g, muzzleColor.b, 0f);
                Vector3 radial = i == 0 ? right : (i == 1 ? up : (right + up).normalized);
                line.SetPosition(0, shot.origin - radial * muzzleSize * 0.32f);
                line.SetPosition(1, shot.origin + radial * muzzleSize);
            }

            for (int i = 0; i < MaxSparks; i++)
            {
                LineRenderer line = shot.sparks[i];
                bool enabled = shot.showImpact && i < impactSparkCount;
                line.enabled = enabled;
                if (!enabled) continue;

                Vector3 tangent = right * RandomRange(ref shot.seed, -1f, 1f) + up * RandomRange(ref shot.seed, -1f, 1f);
                Vector3 direction = (shot.normal * RandomRange(ref shot.seed, 0.25f, 0.9f) + tangent).normalized;
                float length = impactSparkLength * RandomRange(ref shot.seed, 0.45f, 1.15f);
                line.widthMultiplier = tracerWidth * 0.45f;
                line.startColor = impactColor;
                line.endColor = new Color(impactColor.r, impactColor.g, impactColor.b, 0f);
                line.SetPosition(0, shot.end + shot.normal * 0.012f);
                line.SetPosition(1, shot.end + direction * length);
            }
        }

        private void Tick(ShotVisual shot, float dt)
        {
            shot.age += dt;
            Vector3 direction = (shot.end - shot.origin).normalized;
            BuildBasis(direction, out Vector3 right, out Vector3 up);
            for (int i = 0; i < visualBulletCount && shot.showTracers; i++)
            {
                LineRenderer line = shot.tracers[i];
                float phase = i == 0 ? 0f : Hash01(shot.seed + (uint)i * 1013u) * 0.075f;
                float localHead = Mathf.Clamp((shot.age - phase) * tracerSpeed, 0f, shot.distance);
                float localTail = Mathf.Max(0f, localHead - tracerLength * (i == 0 ? 1f : 0.72f));
                float angle = Hash01(shot.seed + (uint)i * 3571u) * Mathf.PI * 2f;
                float radius = i == 0 ? 0f : visualSpread * Mathf.Sqrt(Hash01(shot.seed + (uint)i * 7919u));
                Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
                float normalizedHead = localHead / shot.distance;
                line.SetPosition(0, shot.origin + direction * localTail + offset * (localTail / shot.distance));
                line.SetPosition(1, shot.origin + direction * localHead + offset * normalizedHead);
                line.enabled = localHead > 0f && (shot.age - phase) < (shot.distance / tracerSpeed + 0.08f);
            }

            float muzzleFade = 1f - Mathf.Clamp01(shot.age / Mathf.Max(0.001f, muzzleDuration));
            for (int i = 0; i < MuzzleLines; i++)
            {
                if (shot.muzzle[i].enabled)
                {
                    Color c = muzzleColor;
                    c.a *= muzzleFade;
                    shot.muzzle[i].startColor = c;
                    shot.muzzle[i].enabled = muzzleFade > 0f;
                }
            }

            float impactStart = shot.showTracers ? shot.distance / tracerSpeed : 0f;
            float impactT = (shot.age - impactStart) / Mathf.Max(0.001f, impactDuration);
            for (int i = 0; i < impactSparkCount; i++)
            {
                if (!shot.showImpact) break;
                shot.sparks[i].enabled = impactT >= 0f && impactT < 1f;
                if (shot.sparks[i].enabled)
                {
                    Color c = impactColor;
                    c.a *= 1f - impactT;
                    shot.sparks[i].startColor = c;
                    shot.sparks[i].widthMultiplier = tracerWidth * 0.45f * (1f - impactT);
                }
            }

            float tracerFinish = shot.showTracers ? shot.distance / tracerSpeed + 0.08f : 0f;
            float muzzleFinish = shot.showMuzzle ? muzzleDuration : 0f;
            float impactFinish = shot.showImpact ? impactDuration : 0f;
            float finishAt = Mathf.Max(tracerFinish, Mathf.Max(muzzleFinish, impactFinish));
            if (shot.age >= finishAt)
            {
                shot.active = false;
                shot.root.SetActive(false);
            }
        }

        private static void BuildBasis(Vector3 forward, out Vector3 right, out Vector3 up)
        {
            Vector3 reference = Mathf.Abs(Vector3.Dot(forward, Vector3.up)) > 0.95f ? Vector3.right : Vector3.up;
            right = Vector3.Cross(forward, reference).normalized;
            up = Vector3.Cross(right, forward).normalized;
        }

        private static float RandomRange(ref uint state, float min, float max)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return Mathf.Lerp(min, max, (state & 0x00FFFFFFu) / 16777215f);
        }

        private static float Hash01(uint value)
        {
            value ^= value >> 16;
            value *= 0x7feb352du;
            value ^= value >> 15;
            value *= 0x846ca68bu;
            value ^= value >> 16;
            return (value & 0x00FFFFFFu) / 16777215f;
        }

        private static void ResetLines(ShotVisual shot)
        {
            for (int i = 0; i < shot.tracers.Length; i++) shot.tracers[i].enabled = false;
            for (int i = 0; i < shot.muzzle.Length; i++) shot.muzzle[i].enabled = false;
            for (int i = 0; i < shot.sparks.Length; i++) shot.sparks[i].enabled = false;
        }

        private void EnsurePool()
        {
            if (pool == null) BuildPool();
        }

        private void OnDestroy()
        {
            if (sharedMaterial == null) return;
            if (Application.isPlaying) Destroy(sharedMaterial);
            else DestroyImmediate(sharedMaterial);
        }
    }
}
