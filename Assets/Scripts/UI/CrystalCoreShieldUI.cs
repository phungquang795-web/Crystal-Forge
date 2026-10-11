using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Adventure;

namespace CrystalForge.UI
{
    /// <summary>
    /// UI Thanh Khiên và Độ Bền Trụ Tinh Thể (Runic Aegis Core Shield)
    /// Đặt trực tiếp trong World Space trên đỉnh Lõi Pha Lê hoặc trên HUD trung tâm.
    /// </summary>
    public class CrystalCoreShieldUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform shieldFillRect;
        [SerializeField] private RectTransform delayedFillRect;
        [SerializeField] private Image shieldFillImage;
        [SerializeField] private Image delayedFillImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image borderGlowImage;
        [SerializeField] private TMP_Text shieldText;
        [SerializeField] private TMP_Text titleText;

        [Header("Colors")]
        [SerializeField] private Color normalColor = new Color(0.2f, 0.92f, 1f, 1f);     // Xanh Hextech Cyan ma thuật
        [SerializeField] private Color warningColor = new Color(1f, 0.82f, 0.2f, 1f);    // Vàng chanh cảnh báo
        [SerializeField] private Color damagedColor = new Color(1f, 0.35f, 0.25f, 1f);   // Đỏ cam nguy cấp

        private Camera mainCam;
        private Coroutine shakeCoroutine;
        private Coroutine delayedDrainCoroutine;
        private Coroutine flashCoroutine;
        private Vector3 initialLocalPos;
        private int previousShield = -1;

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            mainCam = Camera.main;
        }

        private void Start()
        {
            if (shieldFillImage != null) shieldFillImage.color = normalColor;
            if (titleText != null)
            {
                titleText.text = "TRỤ TINH THỂ CỔ NGỮ";
                titleText.textWrappingMode = TextWrappingModes.NoWrap;
            }
            if (shieldText != null)
            {
                shieldText.textWrappingMode = TextWrappingModes.NoWrap;
            }

            SubscribeEvents();
            if (CrystalDefenseManager.Instance != null)
            {
                UpdateShield(CrystalDefenseManager.Instance.CoreCurrentShield, CrystalDefenseManager.Instance.CoreMaxShield);
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
            if (CrystalDefenseManager.Instance != null)
            {
                UpdateShield(CrystalDefenseManager.Instance.CoreCurrentShield, CrystalDefenseManager.Instance.CoreMaxShield);
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void LateUpdate()
        {
            if (mainCam == null) mainCam = Camera.main;
            if (mainCam != null)
            {
                // Billboard hướng về Camera để luôn nhìn rõ
                transform.rotation = mainCam.transform.rotation;
            }
        }

        private void SubscribeEvents()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnCoreShieldChanged += UpdateShield;
            }
        }

        private void UnsubscribeEvents()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnCoreShieldChanged -= UpdateShield;
            }
        }

        public void UpdateShield(int current, int max)
        {
            if (max <= 0) return;
            float ratio = Mathf.Clamp01((float)current / max);

            if (shieldFillRect != null)
            {
                shieldFillRect.anchorMin = Vector2.zero;
                shieldFillRect.anchorMax = new Vector2(ratio, 1f);
                shieldFillRect.offsetMin = new Vector2(2f, 2f);
                shieldFillRect.offsetMax = new Vector2(-2f, -2f);
            }

            if (shieldText != null)
            {
                shieldText.text = $"KHIÊN: {current} / {max}";
            }

            if (shieldFillImage != null)
            {
                if (ratio > 0.5f) shieldFillImage.color = normalColor;
                else if (ratio > 0.25f) shieldFillImage.color = warningColor;
                else shieldFillImage.color = damagedColor;
            }

            // Hiệu ứng khi bị mất khiên
            if (previousShield >= 0 && current < previousShield)
            {
                if (delayedFillRect != null)
                {
                    if (delayedDrainCoroutine != null) StopCoroutine(delayedDrainCoroutine);
                    delayedDrainCoroutine = StartCoroutine(DelayedDrainRoutine(ratio));
                }

                if (gameObject.activeInHierarchy)
                {
                    if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
                    shakeCoroutine = StartCoroutine(ShakeRoutine());

                    if (flashCoroutine != null) StopCoroutine(flashCoroutine);
                    flashCoroutine = StartCoroutine(FlashRoutine());
                }
            }
            else
            {
                // Khi khiên hồi đầy hoặc khởi tạo
                if (delayedFillRect != null)
                {
                    delayedFillRect.anchorMin = Vector2.zero;
                    delayedFillRect.anchorMax = new Vector2(ratio, 1f);
                    delayedFillRect.offsetMin = new Vector2(2f, 2f);
                    delayedFillRect.offsetMax = new Vector2(-2f, -2f);
                }
            }

            previousShield = current;
        }

        private IEnumerator DelayedDrainRoutine(float targetRatio)
        {
            yield return new WaitForSeconds(0.25f);
            if (delayedFillRect == null) yield break;

            float startRatio = delayedFillRect.anchorMax.x;
            float elapsed = 0f;
            float duration = 0.45f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curRatio = Mathf.Lerp(startRatio, targetRatio, t);
                delayedFillRect.anchorMax = new Vector2(curRatio, 1f);
                yield return null;
            }
            delayedFillRect.anchorMax = new Vector2(targetRatio, 1f);
            delayedDrainCoroutine = null;
        }

        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;
            float duration = 0.22f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration);
                float offsetX = Mathf.Sin(elapsed * 45f) * 0.06f * decay;
                transform.localPosition = initialLocalPos + new Vector3(offsetX, 0f, 0f);
                yield return null;
            }
            transform.localPosition = initialLocalPos;
            shakeCoroutine = null;
        }

        private IEnumerator FlashRoutine()
        {
            if (borderGlowImage == null) yield break;

            borderGlowImage.color = new Color(1f, 0.4f, 0.3f, 0.95f);
            float elapsed = 0f;
            float duration = 0.3f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                borderGlowImage.color = Color.Lerp(new Color(1f, 0.4f, 0.3f, 0.95f), new Color(0.95f, 0.75f, 0.25f, 0.4f), t);
                yield return null;
            }
            borderGlowImage.color = new Color(0.95f, 0.75f, 0.25f, 0.4f);
            flashCoroutine = null;
        }
    }
}
