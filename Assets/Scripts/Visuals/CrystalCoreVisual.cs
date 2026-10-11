using UnityEngine;

namespace CrystalForge.Visuals
{
    /// <summary>
    /// Điều khiển hiệu ứng thị giác của Khối Pha Lê Ma Thuật trung tâm (Crystal Core):
    /// - Hiệu ứng lơ lửng nhấp nhô nhẹ nhàng (Floating Hover Bobbing)
    /// - Xoay tròn uyển chuyển (Graceful Rotation)
    /// - Ánh sáng lõi nhịp thở ma thuật (Pulsing Magic Light)
    /// - Xoay các mảnh vệ tinh pha lê xung quanh (Orbiting Satellite Shards)
    /// </summary>
    public class CrystalCoreVisual : MonoBehaviour
    {
        [Header("Hover & Rotation")]
        [SerializeField] private float hoverAmplitude = 0.08f;
        [SerializeField] private float hoverFrequency = 1.8f;
        [SerializeField] private float rotationSpeed = 24f;

        [Header("Pulsing Light")]
        [SerializeField] private Light corePointLight;
        [SerializeField] private float baseLightIntensity = 2.6f;
        [SerializeField] private float lightPulseAmplitude = 0.7f;
        [SerializeField] private float lightPulseFrequency = 2.4f;

        [Header("Orbiting Satellites")]
        [SerializeField] private Transform satellitesContainer;
        [SerializeField] private float satelliteOrbitSpeed = -35f;

        private Vector3 initialLocalPos;
        private float seed;

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            seed = Random.Range(0f, 100f);

            if (corePointLight == null)
            {
                var lightObj = GameObject.Find("CrystalCore_PointLight");
                if (lightObj != null) corePointLight = lightObj.GetComponent<Light>();
            }
        }

        private void Update()
        {
            float t = Time.time + seed;

            // 1. Nhấp nhô bồng bềnh êm ái
            float hoverY = Mathf.Sin(t * hoverFrequency) * hoverAmplitude;
            transform.localPosition = initialLocalPos + new Vector3(0f, hoverY, 0f);

            // 2. Tự xoay quanh trục thẳng đứng
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

            // 3. Vệ tinh xoay quanh quỹ đạo ngược chiều
            if (satellitesContainer != null)
            {
                satellitesContainer.Rotate(Vector3.up, satelliteOrbitSpeed * Time.deltaTime, Space.World);
            }

            // 4. Ánh sáng ma thuật nhịp đập phập phồng (Breathing Glow)
            if (corePointLight != null)
            {
                float pulse = Mathf.Sin(t * lightPulseFrequency) * lightPulseAmplitude;
                corePointLight.intensity = baseLightIntensity + pulse;
            }
        }
    }
}
