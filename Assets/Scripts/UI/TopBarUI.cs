using TMPro;
using UnityEngine;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class TopBarUI : MonoBehaviour
    {
        [Header("Text Displays (TextMeshPro)")]
        [SerializeField] private TMP_Text currentCrystalText;
        [SerializeField] private TMP_Text lifetimeCrystalText;
        [SerializeField] private TMP_Text productionRateText;
        [SerializeField] private TMP_Text tapPowerText;

        [Header("Buff Indicator")]
        [SerializeField] private GameObject buffBadgeObj;
        [SerializeField] private TMP_Text buffMultiplierText;

        [Header("Fever Rush Indicator")]
        [SerializeField] private GameObject feverBadgeObj;
        [SerializeField] private TMP_Text feverText;

        private void Start()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCrystalChanged += UpdateCrystalDisplay;
                ResourceManager.Instance.OnLifetimeCrystalChanged += UpdateLifetimeDisplay;
                ResourceManager.Instance.OnProductionRateChanged += UpdateProductionDisplay;
                ResourceManager.Instance.OnTapPowerChanged += UpdateTapPowerDisplay;
                ResourceManager.Instance.OnTemporaryBuffUpdated += UpdateBuffBadge;

                // Initial refresh
                UpdateCrystalDisplay(ResourceManager.Instance.CurrentCrystal);
                UpdateLifetimeDisplay(ResourceManager.Instance.LifetimeCrystal);
                UpdateProductionDisplay(ResourceManager.Instance.EffectiveProductionPerSec);
                UpdateTapPowerDisplay(ResourceManager.Instance.TapPower);
                UpdateBuffBadge(ResourceManager.Instance.TemporaryBuffMultiplier);
            }

            ClickSystem.OnFeverStateChanged += UpdateFeverBadge;
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCrystalChanged -= UpdateCrystalDisplay;
                ResourceManager.Instance.OnLifetimeCrystalChanged -= UpdateLifetimeDisplay;
                ResourceManager.Instance.OnProductionRateChanged -= UpdateProductionDisplay;
                ResourceManager.Instance.OnTapPowerChanged -= UpdateTapPowerDisplay;
                ResourceManager.Instance.OnTemporaryBuffUpdated -= UpdateBuffBadge;
            }

            ClickSystem.OnFeverStateChanged -= UpdateFeverBadge;
        }

        private void UpdateCrystalDisplay(double crystal)
        {
            if (currentCrystalText != null)
            {
                currentCrystalText.text = $"{CrystalFormat.Format(crystal)} CR";
            }
        }

        private void UpdateLifetimeDisplay(double lifetime)
        {
            if (lifetimeCrystalText != null)
            {
                lifetimeCrystalText.text = $"Tổng: {CrystalFormat.Format(lifetime)}";
            }
        }

        private void UpdateProductionDisplay(double ratePerSec)
        {
            if (productionRateText != null)
            {
                productionRateText.text = CrystalFormat.FormatRate(ratePerSec);
            }
        }

        private void UpdateTapPowerDisplay(double tapPower)
        {
            if (tapPowerText != null)
            {
                tapPowerText.text = $"Lực gõ: +{CrystalFormat.Format(tapPower)}";
            }
        }

        private void UpdateBuffBadge(float buffMultiplier)
        {
            if (buffBadgeObj != null)
            {
                buffBadgeObj.SetActive(buffMultiplier > 1.0f);
            }
            if (buffMultiplierText != null && buffMultiplier > 1.0f)
            {
                buffMultiplierText.text = $"x{buffMultiplier:F1} BUFF";
            }
        }

        private void UpdateFeverBadge(bool isActive, float remainingTimer, float progress)
        {
            if (feverBadgeObj == null) return;

            if (isActive)
            {
                feverBadgeObj.SetActive(true);
                if (feverText != null)
                {
                    feverText.text = $"FEVER x2 ({remainingTimer:F1}s)";
                }
            }
            else if (progress > 0.45f)
            {
                feverBadgeObj.SetActive(true);
                if (feverText != null)
                {
                    feverText.text = $"RUSH {progress * 100f:F0}%";
                }
            }
            else
            {
                feverBadgeObj.SetActive(false);
            }
        }
    }
}
