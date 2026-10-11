using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrystalForge.Audio;
using CrystalForge.Economy;

namespace CrystalForge.VFX
{
    [DefaultExecutionOrder(-50)]
    public class LootGemSpawner : MonoBehaviour
    {
        private static LootGemSpawner instance;
        public static LootGemSpawner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindAnyObjectByType<LootGemSpawner>();
                    if (instance == null)
                    {
                        var go = new GameObject("LootGemSpawner");
                        instance = go.AddComponent<LootGemSpawner>();
                    }
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("Gem Visual Materials")]
        private Material cyanGemMat;
        private Material goldGemMat;
        private Material purpleGemMat;
        private Mesh gemMesh;

        private Camera mainCam;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            mainCam = Camera.main;
            EnsureMaterialsAndMesh();
        }

        private void EnsureMaterialsAndMesh()
        {
            if (gemMesh == null)
            {
                gemMesh = CreateDiamondOctahedronMesh();
            }

            var urpShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Diffuse");

            if (cyanGemMat == null)
            {
                cyanGemMat = new Material(urpShader);
                cyanGemMat.color = new Color(0.2f, 0.95f, 1.0f, 1.0f);
                cyanGemMat.EnableKeyword("_EMISSION");
                cyanGemMat.SetColor("_EmissionColor", new Color(0.1f, 0.85f, 1.0f) * 2.2f);
            }

            if (goldGemMat == null)
            {
                goldGemMat = new Material(urpShader);
                goldGemMat.color = new Color(1.0f, 0.85f, 0.2f, 1.0f);
                goldGemMat.EnableKeyword("_EMISSION");
                goldGemMat.SetColor("_EmissionColor", new Color(1.0f, 0.8f, 0.15f) * 2.5f);
            }

            if (purpleGemMat == null)
            {
                purpleGemMat = new Material(urpShader);
                purpleGemMat.color = new Color(0.85f, 0.35f, 1.0f, 1.0f);
                purpleGemMat.EnableKeyword("_EMISSION");
                purpleGemMat.SetColor("_EmissionColor", new Color(0.75f, 0.25f, 0.95f) * 2.2f);
            }
        }

        private Mesh CreateDiamondOctahedronMesh()
        {
            var mesh = new Mesh { name = "LootGemDiamondMesh" };
            float r = 0.22f;
            float h = 0.34f;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, h, 0),             // 0: Đỉnh trên
                new Vector3(r, 0, 0),              // 1: Phải
                new Vector3(0, 0, r),              // 2: Trước
                new Vector3(-r, 0, 0),             // 3: Trái
                new Vector3(0, 0, -r),             // 4: Sau
                new Vector3(0, -h, 0)             // 5: Đáy dưới
            };

            int[] triangles = new int[]
            {
                // Nón trên
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                // Nón dưới
                5, 2, 1,
                5, 3, 2,
                5, 4, 3,
                5, 1, 4
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public void SpawnLootBurst(Vector3 originWorldPos, int gemCount = 8, double totalReward = 0, bool isTreasureOrBoss = false)
        {
            EnsureMaterialsAndMesh();
            if (mainCam == null) mainCam = Camera.main;

            int count = Mathf.Clamp(gemCount, 5, 20);
            for (int i = 0; i < count; i++)
            {
                Material selectedMat = isTreasureOrBoss
                    ? ((i % 2 == 0) ? goldGemMat : cyanGemMat)
                    : ((i % 3 == 0) ? cyanGemMat : (i % 3 == 1 ? purpleGemMat : goldGemMat));

                StartCoroutine(LootGemFlightRoutine(originWorldPos, selectedMat, isTreasureOrBoss, i));
            }
        }

        private IEnumerator LootGemFlightRoutine(Vector3 startPos, Material mat, bool isSpecial, int index)
        {
            var gemObj = new GameObject($"LootGem_{index}");
            gemObj.transform.position = startPos + Vector3.up * 0.5f;
            gemObj.transform.localScale = Vector3.one * Random.Range(0.7f, 1.15f);

            var filter = gemObj.AddComponent<MeshFilter>();
            filter.sharedMesh = gemMesh;
            var renderer = gemObj.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = mat;

            // 1. Giai đoạn Bùng Nổ Tung Cánh (Arc Explosion Burst)
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(1.8f, 3.8f);
            Vector3 velocity = new Vector3(randomCircle.x, Random.Range(3.2f, 5.2f), randomCircle.y);

            float simTime = 0f;
            float gravity = -18f;
            float floorY = 0.15f;
            Vector3 currentPos = gemObj.transform.position;
            Vector3 rotAxis = Random.insideUnitSphere.normalized;

            while (simTime < 0.45f)
            {
                simTime += Time.deltaTime;
                velocity.y += gravity * Time.deltaTime;
                currentPos += velocity * Time.deltaTime;

                if (currentPos.y <= floorY)
                {
                    currentPos.y = floorY;
                    velocity.y = -velocity.y * 0.35f; // Nảy nhẹ trên sàn đá
                    velocity.x *= 0.6f;
                    velocity.z *= 0.6f;
                }

                if (gemObj != null)
                {
                    gemObj.transform.position = currentPos;
                    gemObj.transform.Rotate(rotAxis, 480f * Time.deltaTime);
                }
                yield return null;
            }

            // Dừng thoáng nhẹ 0.08s
            yield return new WaitForSeconds(Random.Range(0.04f, 0.12f));

            // 2. Giai đoạn Hút Gia Tốc Bay Về HUD TopBar (Magnet Fly to TopBar)
            float flyElapsed = 0f;
            float flyDuration = Random.Range(0.5f, 0.75f);
            Vector3 flyStartPos = gemObj.transform.position;

            while (flyElapsed < flyDuration)
            {
                if (gemObj == null) yield break;

                flyElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(flyElapsed / flyDuration);

                // Đường cong mượt gia tốc (EaseInCubic kết hợp nhô cao parabol)
                float easeT = t * t * t;

                // Điểm đích trên màn hình (Góc trên TopBar Crystal Counter)
                Vector3 targetWorldPos;
                if (mainCam != null)
                {
                    // Viewport (x: 0.18f, y: 0.94f) là vị trí huy hiệu Pha Lê trên TopBar
                    targetWorldPos = mainCam.ViewportToWorldPoint(new Vector3(0.18f, 0.94f, 6.5f));
                }
                else
                {
                    targetWorldPos = new Vector3(-3.2f, 4.2f, 0f);
                }

                // Parallax Arc
                Vector3 straightPos = Vector3.Lerp(flyStartPos, targetWorldPos, easeT);
                float arcHeight = Mathf.Sin(t * Mathf.PI) * 1.5f;
                gemObj.transform.position = straightPos + Vector3.up * arcHeight;

                gemObj.transform.Rotate(rotAxis, 720f * Time.deltaTime);
                gemObj.transform.localScale = Vector3.one * Mathf.Lerp(1.0f, 0.4f, t);

                yield return null;
            }

            // 3. Chạm Đích: Hiệu ứng leng keng thu nhận (Chime & Sparkle)
            if (gemObj != null)
            {
                Destroy(gemObj);
            }

            // Phát âm thanh vui tai ngọc va chạm
            if (AudioManager.Instance != null && (index % 3 == 0))
            {
                AudioManager.Instance.PlayClickSfx();
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayClickSpark(Vector3.up * 3.5f);
            }
        }
    }
}
