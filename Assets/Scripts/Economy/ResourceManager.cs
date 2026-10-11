using System;
using UnityEngine;

namespace CrystalForge.Economy
{
    [DefaultExecutionOrder(-100)]
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Runtime Currency (Crystal)")]
        [SerializeField] private double currentCrystal = 0.0;
        [SerializeField] private double lifetimeCrystal = 0.0;

        [Header("Power & Production")]
        [SerializeField] private double baseTapPower = 1.0;
        [SerializeField] private double bonusTapPower = 0.0;
        [SerializeField] private double baseProductionPerSec = 0.0;

        [Header("Multipliers")]
        [SerializeField] private float globalMultiplier = 1.0f;
        [SerializeField] private float temporaryBuffMultiplier = 1.0f;
        private float buffRemainingDuration = 0.0f;

        // Events for UI update
        public event Action<double> OnCrystalChanged;
        public event Action<double> OnLifetimeCrystalChanged;
        public event Action<double> OnProductionRateChanged;
        public event Action<double> OnTapPowerChanged;
        public event Action<float> OnTemporaryBuffUpdated; // buff multiplier

        public double CurrentCrystal => currentCrystal;
        public double LifetimeCrystal => lifetimeCrystal;
        public double TapPower => (baseTapPower + bonusTapPower);
        public double EffectiveProductionPerSec => baseProductionPerSec * globalMultiplier * temporaryBuffMultiplier 
            * (PrestigeManager.Instance != null ? PrestigeManager.Instance.AstralProductionMultiplier : 1.0f)
            * (RelicManager.Instance != null ? RelicManager.Instance.IgnisProductionMultiplier : 1.0f);
        public float GlobalMultiplier => globalMultiplier;
        public float TemporaryBuffMultiplier => temporaryBuffMultiplier;
        public float BuffRemainingDuration => buffRemainingDuration;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }

        private void Update()
        {
            // Xử lý đếm ngược Buff tạm thời (nếu có từ sự kiện phiêu lưu)
            if (buffRemainingDuration > 0f)
            {
                buffRemainingDuration -= Time.deltaTime;
                if (buffRemainingDuration <= 0f)
                {
                    buffRemainingDuration = 0f;
                    temporaryBuffMultiplier = 1.0f;
                    OnTemporaryBuffUpdated?.Invoke(temporaryBuffMultiplier);
                    OnProductionRateChanged?.Invoke(EffectiveProductionPerSec);
                }
            }

            // Vòng lặp cộng thu nhập thụ động tự động theo thời gian thực (Passive Income)
            double effectiveRate = EffectiveProductionPerSec;
            if (effectiveRate > 0.0)
            {
                double incomeThisFrame = effectiveRate * Time.deltaTime;
                AddIncome(incomeThisFrame);
            }
        }

        private void AddIncome(double amount)
        {
            if (amount <= 0.0) return;
            currentCrystal += amount;
            lifetimeCrystal += amount;

            OnCrystalChanged?.Invoke(currentCrystal);
            OnLifetimeCrystalChanged?.Invoke(lifetimeCrystal);
        }

        /// <summary>
        /// Cộng tài nguyên trực tiếp khi người chơi click/tap vào Crystal Core
        /// </summary>
        public double TapCore(double multiplier = 1.0)
        {
            double gain = TapPower * multiplier;
            AddIncome(gain);
            return gain;
        }

        /// <summary>
        /// Thử chi tiêu Crystal (ví dụ khi mua Upgrade)
        /// </summary>
        public bool TrySpend(double amount)
        {
            if (amount <= 0.0) return true;
            if (currentCrystal >= amount)
            {
                currentCrystal -= amount;
                OnCrystalChanged?.Invoke(currentCrystal);
                return true;
            }
            return false;
        }

        public void AddDirectBonus(double amount)
        {
            AddIncome(amount);
        }

        public void SetBonusTapPower(double bonus)
        {
            bonusTapPower = bonus;
            OnTapPowerChanged?.Invoke(TapPower);
        }

        public void SetBaseProduction(double newBaseRate)
        {
            baseProductionPerSec = Math.Max(0.0, newBaseRate);
            OnProductionRateChanged?.Invoke(EffectiveProductionPerSec);
        }

        public void SetGlobalMultiplier(float multiplier)
        {
            globalMultiplier = Math.Max(1.0f, multiplier);
            OnProductionRateChanged?.Invoke(EffectiveProductionPerSec);
        }

        public void ApplyTemporaryBuff(float multiplier, float durationSeconds)
        {
            temporaryBuffMultiplier = multiplier;
            buffRemainingDuration = durationSeconds;
            OnTemporaryBuffUpdated?.Invoke(temporaryBuffMultiplier);
            OnProductionRateChanged?.Invoke(EffectiveProductionPerSec);
        }

        /// <summary>
        /// Nạp dữ liệu khi Load game hoặc tính thu nhập Offline
        /// </summary>
        public void LoadData(double savedCrystal, double savedLifetime, double savedBonusTapPower, double savedBaseProduction)
        {
            currentCrystal = savedCrystal;
            lifetimeCrystal = savedLifetime;
            bonusTapPower = savedBonusTapPower;
            baseProductionPerSec = savedBaseProduction;

            OnCrystalChanged?.Invoke(currentCrystal);
            OnLifetimeCrystalChanged?.Invoke(lifetimeCrystal);
            OnTapPowerChanged?.Invoke(TapPower);
            OnProductionRateChanged?.Invoke(EffectiveProductionPerSec);
        }
    }
}
