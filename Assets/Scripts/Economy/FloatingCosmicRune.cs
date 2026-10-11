using System;
using UnityEngine;
using UnityEngine.EventSystems;
using CrystalForge.Audio;
using CrystalForge.Economy;
using CrystalForge.UI;
using CrystalForge.Utils;
using CrystalForge.VFX;

namespace CrystalForge.Economy
{
    public enum SkyRewardType
    {
        CrystalSurge,   // Thưởng 60s sản lượng
        HyperSpeed,     // Nhân đôi x2.0 sản lượng trong 20s
        LuckyForge      // Thưởng x25 lực gõ TapPower
    }

    public class FloatingCosmicRune : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 targetPos;
        [SerializeField] private float flightDuration = 9.0f;
        [SerializeField] private float waveAmplitude = 0.45f;
        [SerializeField] private float waveFrequency = 3.2f;

        private float elapsedFlightTime = 0f;
        private bool isCollected = false;
        private Light runeLight;
        private Transform innerGlow;

        public void Initialize(Vector3 start, Vector3 target, float duration = 9.0f)
        {
            startPos = start;
            targetPos = target;
            flightDuration = duration;
            elapsedFlightTime = 0f;
            isCollected = false;
            transform.position = start;

            SetupVisualsIfMissing();
        }

        private void SetupVisualsIfMissing()
        {
            // Đảm bảo có Collider để bắt click chuột
            var col = GetComponent<SphereCollider>();
            if (col == null)
            {
                col = gameObject.AddComponent<SphereCollider>();
                col.radius = 0.75f; // Bán kính bấm chuột rộng rãi, dễ click
            }

            // Đèn phát quang huyền ảo
            if (runeLight == null)
            {
                runeLight = GetComponentInChildren<Light>();
                if (runeLight == null)
                {
                    var lightObj = new GameObject("Rune_Light");
                    lightObj.transform.SetParent(transform, false);
                    runeLight = lightObj.AddComponent<Light>();
                    runeLight.type = LightType.Point;
                    runeLight.range = 3.5f;
                    runeLight.intensity = 2.8f;
                    runeLight.color = new Color(0.3f, 0.95f, 1.0f); // Xanh ngọc phát sáng
                }
            }

            innerGlow = transform.Find("InnerGlow");
        }

        private void Update()
        {
            if (isCollected) return;

            elapsedFlightTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedFlightTime / flightDuration);

            // Chuyển động lướt ngang hình sin (Sine Wave)
            Vector3 currentBase = Vector3.Lerp(startPos, targetPos, progress);
            float waveOffsetY = Mathf.Sin(elapsedFlightTime * waveFrequency) * waveAmplitude;
            transform.position = currentBase + new Vector3(0f, waveOffsetY, 0f);

            // Tự xoay lơ lửng tạo cảm giác ma thuật
            transform.Rotate(Vector3.up, 120f * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.forward, 60f * Time.deltaTime, Space.Self);

            // Hết thời gian bay mà không được nhấp -> tự biến mất
            if (progress >= 1.0f)
            {
                Despawn();
            }
        }

        private void OnMouseDown()
        {
            // Tránh click xuyên qua UI Button/Panel
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            CollectRune();
        }

        public void CollectRune()
        {
            if (isCollected) return;
            isCollected = true;

            // 1. Âm thanh chuông báu vật ngân vang
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayTreasureChimeSfx();
            }

            // 2. Hiệu ứng hạt bụi sao bùng nổ
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayClickSpark(transform.position);
                VFXManager.Instance.PlayMilestoneBurst();
            }

            // 3. Roll phần thưởng ngẫu nhiên (50% Surge, 30% HyperSpeed, 20% LuckyForge)
            float roll = UnityEngine.Random.value;
            Vector3 textSpawnPos = transform.position + Vector3.up * 0.4f;

            if (roll < 0.50f)
            {
                // Thưởng Mưa Pha Lê (Crystal Surge): 60 giây sản lượng
                double rate = (ResourceManager.Instance != null) ? ResourceManager.Instance.EffectiveProductionPerSec : 0.0;
                double reward = Math.Max(60.0, rate * 60.0);

                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddDirectBonus(reward);
                }

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(
                        textSpawnPos,
                        $"CRYSTAL SURGE! +{CrystalFormat.Format(reward)}",
                        new Color(1.0f, 0.88f, 0.2f) // Vàng kim rực rỡ
                    );
                }
            }
            else if (roll < 0.80f)
            {
                // Cơn Lốc Tốc Độ (Hyper Speed): x2.0 sản lượng trong 20s
                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.ApplyTemporaryBuff(2.0f, 20f);
                }

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(
                        textSpawnPos,
                        "HYPER SPEED x2.0 (20s)!",
                        new Color(0.2f, 1.0f, 0.9f) // Xanh ngọc điện
                    );
                }
            }
            else
            {
                // Phước Lành Thợ Rèn (Lucky Forge): Thưởng 25 lần lực gõ
                double tapPower = (ResourceManager.Instance != null) ? ResourceManager.Instance.TapPower : 1.0;
                double reward = tapPower * 25.0;

                if (ResourceManager.Instance != null)
                {
                    ResourceManager.Instance.AddDirectBonus(reward);
                }

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(
                        textSpawnPos,
                        $"LUCKY FORGE! +{CrystalFormat.Format(reward)}",
                        new Color(1.0f, 0.4f, 0.95f) // Hồng tím huyền bí
                    );
                }
            }

            Despawn();
        }

        private void Despawn()
        {
            Destroy(gameObject);
        }
    }
}
