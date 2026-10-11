using UnityEngine;

namespace CrystalForge.Visuals
{
    /// <summary>
    /// Tạo hiệu ứng ánh sáng bập bùng tự nhiên (Perlin noise flickering) cho các ngọn đuốc cổ ngữ và lò nung ma thuật.
    /// Giúp xưởng rèn ấm cúng, lung linh và có chiều sâu ánh sáng chân thực.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class MysticFlickerLight : MonoBehaviour
    {
        [Header("Flicker Parameters")]
        [SerializeField] private float baseIntensity = 1.8f;
        [SerializeField] private float flickerAmplitude = 0.45f;
        [SerializeField] private float flickerSpeed = 3.5f;

        private Light targetLight;
        private float noiseOffset;

        private void Awake()
        {
            targetLight = GetComponent<Light>();
            noiseOffset = Random.Range(0f, 100f);
            if (baseIntensity <= 0f && targetLight != null)
            {
                baseIntensity = targetLight.intensity;
            }
        }

        private void Update()
        {
            if (targetLight == null) return;
            float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);
            targetLight.intensity = baseIntensity + (noise - 0.5f) * 2f * flickerAmplitude;
        }
    }
}
