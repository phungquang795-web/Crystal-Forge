using System.Collections;
using TMPro;
using UnityEngine;

namespace CrystalForge.UI
{
    public class FloatingTextItem : MonoBehaviour
    {
        [Header("UI Component")]
        [SerializeField] private TMP_Text textComponent;

        [Header("Animation Settings")]
        [SerializeField] private float floatSpeed = 0.9f;
        [SerializeField] private float lifeTime = 0.75f;
        [SerializeField] private Vector3 randomOffsetRange = new Vector3(0.2f, 0.15f, 0.2f);
        [SerializeField] private float targetFontSize = 2.2f;

        public void Initialize(string text, Color color)
        {
            if (textComponent == null)
            {
                textComponent = GetComponentInChildren<TMP_Text>();
            }

            if (textComponent != null)
            {
                textComponent.text = text;
                textComponent.color = color;
                textComponent.fontSize = targetFontSize;
            }

            // Thu nhỏ kích thước tổng thể vừa vặn, không che khuất màn hình
            transform.localScale = Vector3.one * 0.75f;

            Vector3 randomOffset = new Vector3(
                Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
                Random.Range(-randomOffsetRange.y, randomOffsetRange.y),
                Random.Range(-randomOffsetRange.z, randomOffsetRange.z)
            );
            transform.position += randomOffset;

            StartCoroutine(AnimateAndDestroy());
        }

        private IEnumerator AnimateAndDestroy()
        {
            float elapsed = 0f;
            Color originalColor = textComponent != null ? textComponent.color : Color.white;
            Vector3 startPos = transform.position;

            while (elapsed < lifeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / lifeTime;

                // Bay lên trên nhẹ nhàng
                transform.position = startPos + Vector3.up * (floatSpeed * progress);

                // Mờ dần theo thời gian
                if (textComponent != null)
                {
                    Color c = originalColor;
                    c.a = Mathf.Lerp(1f, 0f, progress * progress);
                    textComponent.color = c;
                }

                // Hướng về Camera
                if (Camera.main != null)
                {
                    transform.rotation = Camera.main.transform.rotation;
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
