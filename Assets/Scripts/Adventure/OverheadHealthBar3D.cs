using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Core;

namespace CrystalForge.Adventure
{
    public enum HealthBarOwner
    {
        Player,         // Chú Capybara thợ rèn hộ vệ
        Monster,        // Quái vật xâm lăng đảo
        CrystalCore     // Trụ Tinh Thể trung tâm
    }

    [DefaultExecutionOrder(-40)]
    public class OverheadHealthBar3D : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private HealthBarOwner barOwner = HealthBarOwner.Monster;
        [SerializeField] private bool isHero = false; // Backward compatibility

        [Header("UI Rects & Fills")]
        [SerializeField] private RectTransform mainFillRect;
        [SerializeField] private RectTransform delayedFillRect;
        [SerializeField] private Image mainFillImage;
        [SerializeField] private TMP_Text nameLabelText;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Appearance & Colors")]
        [SerializeField] private Color healthColor = Color.green;
        [SerializeField] private Color delayedDrainColor = new Color(1f, 0.75f, 0.2f, 0.9f);

        private Camera mainCam;
        private Coroutine delayedDrainCoroutine;
        private Coroutine shakeCoroutine;
        private Vector3 initialLocalPos;
        private bool isSubscribed = false;

        public HealthBarOwner BarOwner
        {
            get => barOwner;
            set => barOwner = value;
        }

        private void Awake()
        {
            initialLocalPos = transform.localPosition;
            mainCam = Camera.main;

            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            // Đồng bộ isHero nếu có
            if (isHero) barOwner = HealthBarOwner.Player;

            ApplyDefaultTheme();
        }

        private void Start()
        {
            ApplyDefaultTheme();
            SubscribeEvents();
            RefreshInitial();
            SetVisible(true);
        }

        private void OnEnable()
        {
            SubscribeEvents();
            RefreshInitial();
            SetVisible(true);
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
                // Luôn xoay mặt vuông góc trực diện với Camera (Billboard)
                transform.rotation = mainCam.transform.rotation;
            }
        }

        private void ApplyDefaultTheme()
        {
            switch (barOwner)
            {
                case HealthBarOwner.Player:
                    healthColor = new Color(0.1f, 0.95f, 0.45f); // Xanh ngọc tươi sáng
                    if (nameLabelText != null && string.IsNullOrEmpty(nameLabelText.text))
                    {
                        nameLabelText.text = "THO MO";
                        nameLabelText.color = new Color(0.6f, 1f, 0.7f);
                    }
                    break;

                case HealthBarOwner.CrystalCore:
                    healthColor = new Color(0.1f, 0.85f, 1f); // Xanh Cyan ma thuật
                    if (nameLabelText != null && string.IsNullOrEmpty(nameLabelText.text))
                    {
                        nameLabelText.text = "TRỤ TINH THỂ";
                        nameLabelText.color = new Color(0.6f, 0.95f, 1f);
                    }
                    break;

                case HealthBarOwner.Monster:
                    healthColor = new Color(1f, 0.25f, 0.35f); // Đỏ rực chiến đấu
                    if (nameLabelText != null && string.IsNullOrEmpty(nameLabelText.text))
                    {
                        nameLabelText.text = "QUÁI VẬT";
                        nameLabelText.color = new Color(1f, 0.65f, 0.65f);
                    }
                    break;
            }

            if (mainFillImage != null)
            {
                mainFillImage.color = healthColor;
            }
        }

        public void SetupBar(string ownerName, int currentHp, int maxHp, Color barColor)
        {
            if (nameLabelText != null)
            {
                nameLabelText.text = ownerName;
            }

            healthColor = barColor;
            if (mainFillImage != null)
            {
                mainFillImage.color = healthColor;
            }

            HandleHpUpdated(currentHp, maxHp);
            SetVisible(true);
        }

        private void SubscribeEvents()
        {
            if (isSubscribed) return;
            isSubscribed = true;

            if (CrystalDefenseManager.Instance != null)
            {
                if (barOwner == HealthBarOwner.Player)
                {
                    CrystalDefenseManager.Instance.OnHeroHpChanged += HandleHpUpdated;
                }
                else if (barOwner == HealthBarOwner.CrystalCore)
                {
                    CrystalDefenseManager.Instance.OnCoreShieldChanged += HandleHpUpdated;
                }
                else if (barOwner == HealthBarOwner.Monster)
                {
                    CrystalDefenseManager.Instance.OnMonsterHpChanged += HandleHpUpdated;
                }
            }
        }

        private void UnsubscribeEvents()
        {
            if (!isSubscribed) return;
            isSubscribed = false;

            if (CrystalDefenseManager.Instance != null)
            {
                if (barOwner == HealthBarOwner.Player)
                {
                    CrystalDefenseManager.Instance.OnHeroHpChanged -= HandleHpUpdated;
                }
                else if (barOwner == HealthBarOwner.CrystalCore)
                {
                    CrystalDefenseManager.Instance.OnCoreShieldChanged -= HandleHpUpdated;
                }
                else if (barOwner == HealthBarOwner.Monster)
                {
                    CrystalDefenseManager.Instance.OnMonsterHpChanged -= HandleHpUpdated;
                }
            }
        }

        private void RefreshInitial()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                if (barOwner == HealthBarOwner.Player)
                {
                    HandleHpUpdated(CrystalDefenseManager.Instance.HeroCurrentHp, CrystalDefenseManager.Instance.HeroMaxHp);
                }
                else if (barOwner == HealthBarOwner.CrystalCore)
                {
                    HandleHpUpdated(CrystalDefenseManager.Instance.CoreCurrentShield, CrystalDefenseManager.Instance.CoreMaxShield);
                }
            }
            else
            {
                // Fallback nếu đang ở Editor chưa play
                HandleHpUpdated(100, 100);
            }
        }

        public void SetVisible(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }

        public void HandleHpUpdated(int current, int max)
        {
            if (max <= 0) return;
            float ratio = Mathf.Clamp01((float)current / max);

            if (mainFillRect != null)
            {
                SetFill(mainFillRect, ratio);
            }

            if (hpText != null)
            {
                string suffix = (barOwner == HealthBarOwner.CrystalCore) ? "Khiên HP" : "HP";
                hpText.text = $"{current}/{max} {suffix}";
            }

            // Hiệu ứng rung nhẹ khi bị dính đòn mất máu
            if (gameObject.activeInHierarchy && current < max)
            {
                if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeRoutine());
            }

            // Hiệu ứng thanh máu delayed trượt dần bắt kịp (fighting game style)
            if (delayedFillRect != null && gameObject.activeInHierarchy)
            {
                if (delayedDrainCoroutine != null) StopCoroutine(delayedDrainCoroutine);
                delayedDrainCoroutine = StartCoroutine(DelayedDrainRoutine(ratio));
            }
        }

        private void SetFill(RectTransform rt, float ratio)
        {
            if (rt == null) return;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
            rt.offsetMin = new Vector2(1.5f, 1.5f);
            rt.offsetMax = new Vector2(-1.5f, -1.5f);
        }

        private IEnumerator DelayedDrainRoutine(float targetRatio)
        {
            yield return new WaitForSeconds(0.18f);

            float startRatio = (delayedFillRect != null) ? delayedFillRect.anchorMax.x : targetRatio;
            float elapsed = 0f;
            float duration = 0.35f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                if (delayedFillRect != null)
                {
                    SetFill(delayedFillRect, Mathf.Lerp(startRatio, targetRatio, t));
                }
                yield return null;
            }

            if (delayedFillRect != null) SetFill(delayedFillRect, targetRatio);
        }

        private IEnumerator ShakeRoutine()
        {
            float elapsed = 0f;
            float duration = 0.2f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration);
                float offsetX = Mathf.Sin(elapsed * 40f) * 0.04f * decay;
                transform.localPosition = initialLocalPos + new Vector3(offsetX, 0f, 0f);
                yield return null;
            }
            transform.localPosition = initialLocalPos;
        }
    }
}
