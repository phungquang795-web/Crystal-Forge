using UnityEngine;

namespace CrystalForge.Utils
{
    public class Rotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public Vector3 rotationSpeed = new Vector3(0f, 25f, 0f);

        [Header("Hover / Bobbing Settings")]
        public bool enableBobbing = true;
        public float bobbingSpeed = 1.5f;
        public float bobbingHeight = 0.12f;

        private Vector3 initialLocalPos;

        private void Start()
        {
            initialLocalPos = transform.localPosition;
        }

        private void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);

            if (enableBobbing)
            {
                float newY = initialLocalPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
                transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);
            }
        }
    }
}
