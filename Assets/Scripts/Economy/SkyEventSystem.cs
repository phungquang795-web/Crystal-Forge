using System.Collections;
using UnityEngine;
using CrystalForge.Economy;

namespace CrystalForge.Economy
{
    [DefaultExecutionOrder(-45)]
    public class SkyEventSystem : MonoBehaviour
    {
        public static SkyEventSystem Instance { get; private set; }

        [Header("Spawn Interval")]
        [SerializeField] private float minInterval = 40.0f;
        [SerializeField] private float maxInterval = 65.0f;

        [Header("Flight Path (3D World Coords)")]
        [SerializeField] private Vector3 leftSpawn = new Vector3(-5.8f, 3.8f, -0.8f);
        [SerializeField] private Vector3 rightSpawn = new Vector3(5.8f, 3.5f, 0.4f);
        [SerializeField] private float flightDuration = 9.0f;

        private float nextSpawnTimer = 0f;
        private GameObject currentActiveRune = null;
        private Material runeMaterial;
        private Material ringMaterial;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureMaterials();
            ResetTimer();
        }

        private void Update()
        {
            if (currentActiveRune != null) return; // Chỉ có tối đa 1 Cổ Tinh bay cùng lúc

            nextSpawnTimer -= Time.deltaTime;
            if (nextSpawnTimer <= 0f)
            {
                SpawnCosmicRune();
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            nextSpawnTimer = UnityEngine.Random.Range(minInterval, maxInterval);
        }

        public void ForceSpawnNow()
        {
            if (currentActiveRune != null)
            {
                Destroy(currentActiveRune);
            }
            SpawnCosmicRune();
        }

        private void SpawnCosmicRune()
        {
            // Ngẫu nhiên bay từ Trái -> Phải hoặc Phải -> Trái
            bool fromLeft = UnityEngine.Random.value > 0.5f;
            Vector3 start = fromLeft ? leftSpawn : rightSpawn;
            Vector3 target = fromLeft ? rightSpawn : leftSpawn;

            // Chênh lệch độ cao ngẫu nhiên nhẹ
            start.y += UnityEngine.Random.Range(-0.3f, 0.3f);
            target.y += UnityEngine.Random.Range(-0.3f, 0.3f);

            var runeObj = CreateRuneGameObject();
            var runeComp = runeObj.AddComponent<FloatingCosmicRune>();
            runeComp.Initialize(start, target, flightDuration);

            currentActiveRune = runeObj;
        }

        private GameObject CreateRuneGameObject()
        {
            var root = new GameObject("CosmicRune_Floating");

            // 1. Khối ngọc trung tâm (Sphere lõi kim cương)
            var core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            core.name = "RuneCore";
            core.transform.SetParent(root.transform, false);
            core.transform.localScale = new Vector3(0.55f, 0.65f, 0.55f);
            var coreRenderer = core.GetComponent<MeshRenderer>();
            if (coreRenderer != null && runeMaterial != null)
            {
                coreRenderer.sharedMaterial = runeMaterial;
            }

            // Xóa collider của child để collider của root quản lý
            var childCol = core.GetComponent<Collider>();
            if (childCol != null) Destroy(childCol);

            // 2. Vòng hào quang cổ ngữ xoay quanh (Ring)
            var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "RuneHaloRing";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localScale = new Vector3(0.95f, 0.02f, 0.95f);
            ring.transform.localRotation = Quaternion.Euler(35f, 20f, 0f);
            var ringRenderer = ring.GetComponent<MeshRenderer>();
            if (ringRenderer != null && ringMaterial != null)
            {
                ringRenderer.sharedMaterial = ringMaterial;
            }
            var ringCol = ring.GetComponent<Collider>();
            if (ringCol != null) Destroy(ringCol);

            // 3. Collider lớn trên root
            var rootCol = root.AddComponent<SphereCollider>();
            rootCol.radius = 0.85f;

            return root;
        }

        private void EnsureMaterials()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            if (runeMaterial == null && shader != null)
            {
                runeMaterial = new Material(shader)
                {
                    name = "Mat_CosmicRune_Core",
                    color = new Color(0.25f, 0.95f, 1.0f)
                };
                runeMaterial.EnableKeyword("_EMISSION");
                runeMaterial.SetColor("_EmissionColor", new Color(0.2f, 0.9f, 1.0f) * 3.5f);
            }

            if (ringMaterial == null && shader != null)
            {
                ringMaterial = new Material(shader)
                {
                    name = "Mat_CosmicRune_Ring",
                    color = new Color(1.0f, 0.85f, 0.35f)
                };
                ringMaterial.EnableKeyword("_EMISSION");
                ringMaterial.SetColor("_EmissionColor", new Color(1.0f, 0.8f, 0.25f) * 4.0f);
            }
        }
    }
}
