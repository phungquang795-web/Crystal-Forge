using System;
using UnityEngine;
using CrystalForge.Audio;
using CrystalForge.Core;
using CrystalForge.Economy;
using CrystalForge.UI;
using CrystalForge.Utils;
using CrystalForge.VFX;

namespace CrystalForge.Economy
{
    [DefaultExecutionOrder(-35)]
    public class RelicManager : MonoBehaviour
    {
        public static RelicManager Instance { get; private set; }

        [Header("Relic Levels")]
        [SerializeField] private int hammerLevel = 0;
        [SerializeField] private int aegisLevel = 0;
        [SerializeField] private int ignisLevel = 0;

        [Header("Base Configs")]
        private const double HAMMER_BASE_COST = 5000.0;
        private const double HAMMER_GROWTH = 1.22;

        private const double AEGIS_BASE_COST = 12000.0;
        private const double AEGIS_GROWTH = 1.25;

        private const double IGNIS_BASE_COST = 35000.0;
        private const double IGNIS_GROWTH = 1.28;

        [Header("Thunder Hammer Auto Strike Timer")]
        [SerializeField] private float hammerTimer = 0f;
        private const float HAMMER_INTERVAL = 10.0f;

        // Events
        public event Action<RelicType, int, double> OnRelicUpgraded; // type, newLevel, nextCost

        public int HammerLevel => hammerLevel;
        public int AegisLevel => aegisLevel;
        public int IgnisLevel => ignisLevel;

        public double CurrentHammerCost => Math.Floor(HAMMER_BASE_COST * Math.Pow(HAMMER_GROWTH, hammerLevel));
        public double CurrentAegisCost => Math.Floor(AEGIS_BASE_COST * Math.Pow(AEGIS_GROWTH, aegisLevel));
        public double CurrentIgnisCost => Math.Floor(IGNIS_BASE_COST * Math.Pow(IGNIS_GROWTH, ignisLevel));

        // Synergies
        public float OfflineBonusRatio => aegisLevel * 0.05f; // Tăng hiệu suất offline (+5%/cấp)
        public float IgnisProductionMultiplier => 1.0f + (ignisLevel * 0.05f); // Tăng +5%/cấp sản lượng toàn cầu
        public int AegisShieldAmount => aegisLevel * 200; // Khiên chống chịu đầu trận
        public float AegisDamageReduction => Mathf.Min(0.40f, aegisLevel * 0.03f); // Giảm tới 40% sát thương
        public int IgnisBurnDamage => ignisLevel * 35; // Sát thương thiêu đốt mỗi lượt

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Vòng lặp Búa Sấm Sét tự động gõ phụ đe rèn
            if (hammerLevel > 0)
            {
                hammerTimer += Time.deltaTime;
                if (hammerTimer >= HAMMER_INTERVAL)
                {
                    hammerTimer = 0f;
                    TriggerHammerAutoStrike();
                }
            }
        }

        private void TriggerHammerAutoStrike()
        {
            if (ResourceManager.Instance == null) return;

            double multiplier = 1.0 + (0.35 * hammerLevel);
            double gain = ResourceManager.Instance.TapCore(multiplier);

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayClickSpark(new Vector3(0f, 1.8f, 0f));
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(
                    new Vector3(0f, 2.2f, 0f),
                    $"⚡ THUNDER STRIKE! +{CrystalFormat.Format(gain)}",
                    new Color(0.35f, 0.95f, 1.0f)
                );
            }
        }

        public bool TryUpgrade(RelicType type)
        {
            if (ResourceManager.Instance == null) return false;

            double cost = GetCost(type);
            if (ResourceManager.Instance.TrySpend(cost))
            {
                switch (type)
                {
                    case RelicType.ThunderHammer:
                        hammerLevel++;
                        OnRelicUpgraded?.Invoke(type, hammerLevel, CurrentHammerCost);
                        break;
                    case RelicType.GlacialAegis:
                        aegisLevel++;
                        OnRelicUpgraded?.Invoke(type, aegisLevel, CurrentAegisCost);
                        break;
                    case RelicType.IgnisCore:
                        ignisLevel++;
                        ApplyIgnisMultiplier();
                        OnRelicUpgraded?.Invoke(type, ignisLevel, CurrentIgnisCost);
                        break;
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayUpgradeSfx();
                }

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(
                        new Vector3(0f, 2.4f, 0f),
                        $"✨ RÈN CỔ VẬT THÀNH CÔNG! ✨",
                        new Color(1.0f, 0.85f, 0.2f)
                    );
                }

                return true;
            }

            return false;
        }

        public double GetCost(RelicType type)
        {
            switch (type)
            {
                case RelicType.ThunderHammer: return CurrentHammerCost;
                case RelicType.GlacialAegis: return CurrentAegisCost;
                case RelicType.IgnisCore: return CurrentIgnisCost;
                default: return double.MaxValue;
            }
        }

        public int GetLevel(RelicType type)
        {
            switch (type)
            {
                case RelicType.ThunderHammer: return hammerLevel;
                case RelicType.GlacialAegis: return aegisLevel;
                case RelicType.IgnisCore: return ignisLevel;
                default: return 0;
            }
        }

        private void ApplyIgnisMultiplier()
        {
            // Tác động lên sản lượng thông qua MilestoneManager hoặc ResourceManager
            if (MilestoneManager.Instance != null)
            {
                // Trigger recalculation if needed
            }
        }

        public void LoadLevels(int hammer, int aegis, int ignis)
        {
            hammerLevel = hammer;
            aegisLevel = aegis;
            ignisLevel = ignis;
        }
    }
}
