using UnityEngine;

namespace CrystalForge.Visuals
{
    /// <summary>
    /// Điều khiển chuyển động xoay tròn ma thuật của trận đồ Cổ ngữ dưới sàn đảo nổi lò rèn.
    /// Giúp môi trường 3D luôn chuyển động sống động và thần bí.
    /// </summary>
    public class RunicCircleRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 8f;
        [SerializeField] private bool rotateClockwise = true;
        [SerializeField] private bool subtleBobbing = false;
        [SerializeField] private float bobAmplitude = 0.005f;
        [SerializeField] private float bobFrequency = 1.5f;

        private Vector3 initialLocalPos;
        private float seed;

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            seed = Random.Range(0f, 50f);
        }

        private void Update()
        {
            float direction = rotateClockwise ? 1f : -1f;
            transform.Rotate(Vector3.up, direction * rotationSpeed * Time.deltaTime, Space.Self);

            if (subtleBobbing)
            {
                float bobY = Mathf.Sin((Time.time + seed) * bobFrequency) * bobAmplitude;
                transform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y + bobY, initialLocalPos.z);
            }
        }
    }
}
