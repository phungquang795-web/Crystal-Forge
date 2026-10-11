using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using CrystalForge.Adventure;

namespace CrystalForge.Economy
{
    public class ClickSystem : MonoBehaviour, IPointerClickHandler
    {
        [Header("Target & Feedback")]
        [Tooltip("Transform của khối ngọc sẽ bị co giãn nhẹ khi click")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private Vector3 punchScaleVector = new Vector3(0.12f, -0.1f, 0.12f);
        [SerializeField] private float bounceDuration = 0.15f;

        [Header("Fever Rush System (Sốt Gõ Pha Lê)")]
        [SerializeField] private float feverGainPerTap = 0.08f;
        [SerializeField] private float feverDecayRate = 0.15f;
        [SerializeField] private float feverDuration = 5.5f;
        [SerializeField] private float feverMultiplier = 2.0f;

        private float currentFeverProgress = 0f;
        private bool isFeverActive = false;
        private float feverTimer = 0f;

        public bool IsFeverActive => isFeverActive;
        public float FeverProgress => isFeverActive ? Mathf.Clamp01(feverTimer / feverDuration) : currentFeverProgress;
        public float FeverRemainingTime => feverTimer;

        [Header("Layer & Camera")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask clickableLayer;

        private Vector3 originalScale;
        private Coroutine bounceCoroutine;

        public static event Action<Vector3, double> OnCoreClicked; // Position & Gain amount
        public static event Action<bool, float, float> OnFeverStateChanged; // isActive, remainingTimer, progress (0..1)

        private void Awake()
        {
            if (visualTransform == null)
            {
                visualTransform = transform;
            }
            originalScale = visualTransform.localScale;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            UpdateFeverState();
            // Bắt sự kiện Click / Tap trên màn hình 3D thông qua New Input System hoặc Fallback
            CheckDirectInputRaycast();
        }

        private void UpdateFeverState()
        {
            if (isFeverActive)
            {
                feverTimer -= Time.deltaTime;
                if (feverTimer <= 0f)
                {
                    isFeverActive = false;
                    feverTimer = 0f;
                    currentFeverProgress = 0f;
                    OnFeverStateChanged?.Invoke(false, 0f, 0f);
                }
                else
                {
                    OnFeverStateChanged?.Invoke(true, feverTimer, Mathf.Clamp01(feverTimer / feverDuration));
                }
            }
            else
            {
                if (currentFeverProgress > 0f)
                {
                    currentFeverProgress = Mathf.Max(0f, currentFeverProgress - feverDecayRate * Time.deltaTime);
                    OnFeverStateChanged?.Invoke(false, 0f, currentFeverProgress);
                }
            }
        }

        private void CheckDirectInputRaycast()
        {
            // Kiểm tra con trỏ có đang đè lên UI (Button, Panel) hay không
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            bool wasClicked = false;
            Vector2 screenPos = Vector2.zero;

            // Hỗ trợ New Input System
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                wasClicked = true;
                screenPos = Mouse.current.position.ReadValue();
            }
            else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                wasClicked = true;
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }

            if (wasClicked && mainCamera != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    // 1. Kiểm tra click trúng Quái vật đang tấn công
                    if (CrystalDefenseManager.Instance != null && CrystalDefenseManager.Instance.IsMonsterActive)
                    {
                        if (CrystalDefenseManager.Instance.IsActiveMonsterTransform(hit.transform))
                        {
                            CrystalDefenseManager.Instance.HitMonsterByPlayer(hit.point);
                            return;
                        }

                        var combatUnit = hit.transform.GetComponentInParent<CombatUnit3D>();
                        if (combatUnit != null && combatUnit == CrystalDefenseManager.Instance.CurrentActiveMonster)
                        {
                            CrystalDefenseManager.Instance.HitMonsterByPlayer(hit.point);
                            return;
                        }
                    }

                    // 2. Click trúng Trụ Tinh Thể (khai thác Crystal)
                    if (hit.transform == transform || hit.transform.IsChildOf(transform))
                    {
                        TriggerCoreTap(hit.point);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý click qua chuẩn IPointerClickHandler nếu có PhysicsRaycaster trên Camera
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerCoreTap(eventData.pointerCurrentRaycast.worldPosition);
        }

        public void TriggerCoreTap(Vector3 clickWorldPos)
        {
            if (ResourceManager.Instance == null) return;

            // Nạp thanh sốt gõ Fever nếu chưa kích hoạt
            if (!isFeverActive)
            {
                currentFeverProgress = Mathf.Clamp01(currentFeverProgress + feverGainPerTap);
                if (currentFeverProgress >= 1.0f)
                {
                    isFeverActive = true;
                    feverTimer = feverDuration;
                    OnFeverStateChanged?.Invoke(true, feverTimer, 1.0f);

                    if (CrystalForge.UI.FloatingTextSpawner.Instance != null)
                    {
                        CrystalForge.UI.FloatingTextSpawner.Instance.SpawnFloatingText(clickWorldPos + Vector3.up * 1.5f, "🔥 SOT GO FEVER x2! 🔥", new Color(1.0f, 0.45f, 0.1f));
                    }

                    if (CrystalForge.Audio.AudioManager.Instance != null)
                    {
                        CrystalForge.Audio.AudioManager.Instance.PlaySpellSfx();
                    }

                    if (CrystalForge.VFX.VFXManager.Instance != null)
                    {
                        CrystalForge.VFX.VFXManager.Instance.PlayMilestoneBurst();
                        CrystalForge.VFX.VFXManager.Instance.PlayShockwave(clickWorldPos);
                    }
                }
            }

            double multiplier = isFeverActive ? feverMultiplier : 1.0;
            double gained = ResourceManager.Instance.TapCore(multiplier);
            OnCoreClicked?.Invoke(clickWorldPos, gained);

            if (isFeverActive && CrystalForge.VFX.VFXManager.Instance != null)
            {
                CrystalForge.VFX.VFXManager.Instance.PlayClickSpark(clickWorldPos);
            }

            if (CrystalDefenseManager.Instance != null)
            {
                float energyBonus = isFeverActive ? 6.0f : 3.0f;
                CrystalDefenseManager.Instance.AddRunicEnergy(energyBonus);
                var hero = CrystalDefenseManager.Instance.HeroUnit;
                if (hero != null && !CrystalDefenseManager.Instance.IsMonsterActive && !CrystalDefenseManager.Instance.IsHeroKnockedDown)
                {
                    hero.PlayMineCrystalStrike(transform.position);
                }
            }

            // Kích hoạt hiệu ứng nảy đàn hồi (Juice Scale Bounce)
            if (bounceCoroutine != null)
            {
                StopCoroutine(bounceCoroutine);
            }
            bounceCoroutine = StartCoroutine(AnimatePunchScale());
        }

        private IEnumerator AnimatePunchScale()
        {
            float elapsed = 0f;
            Vector3 targetScale = originalScale + punchScaleVector;

            // Co lại tạo lực
            while (elapsed < bounceDuration * 0.4f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (bounceDuration * 0.4f);
                visualTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            elapsed = 0f;
            // Bung nảy trở lại vị trí ban đầu
            while (elapsed < bounceDuration * 0.6f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (bounceDuration * 0.6f);
                visualTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            visualTransform.localScale = originalScale;
            bounceCoroutine = null;
        }

        private void OnDisable()
        {
            if (visualTransform != null)
            {
                visualTransform.localScale = originalScale;
            }
        }
    }
}
