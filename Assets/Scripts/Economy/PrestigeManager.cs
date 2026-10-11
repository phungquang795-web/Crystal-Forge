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
    [DefaultExecutionOrder(-25)]
    public class PrestigeManager : MonoBehaviour
    {
        public static PrestigeManager Instance { get; private set; }

        [Header("Prestige Progress")]
        [SerializeField] private int totalStardust = 0;
        [SerializeField] private int prestigeCount = 0;

        [Header("Astral Talents (Chỉ số vĩnh viễn)")]
        [SerializeField] private int astralEfficiencyLevel = 0; // +30% sản lượng toàn cầu/cấp
        [SerializeField] private int royalInheritanceLevel = 0; // Khởi đầu với Crystal
        [SerializeField] private int cosmicCapybaraLevel = 0;   // +30% HP & ATK Hero/cấp

        public const double MIN_LIFETIME_FOR_PRESTIGE = 500000.0;

        // Events
        public event Action<int, int> OnPrestigeExecuted; // newTotalStardust, newPrestigeCount
        public event Action<int> OnStardustChanged;
        public event Action OnTalentUpgraded;

        public int TotalStardust => totalStardust;
        public int PrestigeCount => prestigeCount;
        public int AstralEfficiencyLevel => astralEfficiencyLevel;
        public int RoyalInheritanceLevel => royalInheritanceLevel;
        public int CosmicCapybaraLevel => cosmicCapybaraLevel;

        public float AstralProductionMultiplier => 1.0f + (astralEfficiencyLevel * 0.30f);
        public double RoyalStartingCrystal => royalInheritanceLevel > 0 ? (1000.0 * Math.Pow(3, royalInheritanceLevel - 1)) : 0.0;
        public float CosmicCombatMultiplier => 1.0f + (cosmicCapybaraLevel * 0.30f);

        public int AstralCost => (astralEfficiencyLevel + 1) * 2;
        public int RoyalCost => (royalInheritanceLevel + 1) * 3;
        public int CosmicCost => (cosmicCapybaraLevel + 1) * 4;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public int CalculatePendingStardust()
        {
            if (ResourceManager.Instance == null) return 0;
            double lifetime = ResourceManager.Instance.LifetimeCrystal;

            if (lifetime < MIN_LIFETIME_FOR_PRESTIGE) return 0;

            int earned = (int)Math.Floor(Math.Sqrt(lifetime / 100000.0));
            return Math.Max(1, earned);
        }

        public bool CanMeltdown()
        {
            return CalculatePendingStardust() > 0;
        }

        /// <summary>
        /// Kích hoạt Đại Nung Chảy (Cosmic Meltdown): Reset tiến trình cơ bản, nhận Bụi Tinh Tú vĩnh viễn
        /// </summary>
        public bool PerformCosmicMeltdown()
        {
            int pending = CalculatePendingStardust();
            if (pending <= 0) return false;

            totalStardust += pending;
            prestigeCount++;

            // 1. Reset tài nguyên cơ sở
            double startCrystal = RoyalStartingCrystal;
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.LoadData(startCrystal, startCrystal, 0.0, 0.0);
            }

            // 2. Reset các cỗ máy khai khoáng về cấp 0
            if (GeneratorSystem.Instance != null)
            {
                var emptyLevels = new System.Collections.Generic.Dictionary<GeneratorType, int>
                {
                    { GeneratorType.CrystalDrill, 0 },
                    { GeneratorType.ManaFurnace, 0 },
                    { GeneratorType.AutoExtractor, 0 }
                };
                GeneratorSystem.Instance.LoadLevels(emptyLevels);
            }

            // 3. Reset Lực gõ
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.LoadTapLevel(0);
            }

            // 4. Âm thanh & VFX hoành tráng
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayMilestoneBurst();
                VFXManager.Instance.PlayShockwave(new Vector3(0f, 0.05f, 0f));
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMilestoneSfx();
            }

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(
                    new Vector3(0f, 2.5f, 0f),
                    $"🌌 ĐẠI NUNG CHẢY THÀNH CÔNG! +{pending} ⭐",
                    new Color(0.85f, 0.4f, 1.0f)
                );
            }

            OnPrestigeExecuted?.Invoke(totalStardust, prestigeCount);
            OnStardustChanged?.Invoke(totalStardust);

            // 5. Tự động lưu game ngay lập tức
            if (SaveLoadSystem.Instance != null)
            {
                SaveLoadSystem.Instance.SaveGame(silent: false);
            }

            return true;
        }

        public bool TryUpgradeAstralEfficiency()
        {
            int cost = AstralCost;
            if (totalStardust >= cost)
            {
                totalStardust -= cost;
                astralEfficiencyLevel++;
                OnStardustChanged?.Invoke(totalStardust);
                OnTalentUpgraded?.Invoke();
                if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradeSfx();
                return true;
            }
            return false;
        }

        public bool TryUpgradeRoyalInheritance()
        {
            int cost = RoyalCost;
            if (totalStardust >= cost)
            {
                totalStardust -= cost;
                royalInheritanceLevel++;
                OnStardustChanged?.Invoke(totalStardust);
                OnTalentUpgraded?.Invoke();
                if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradeSfx();
                return true;
            }
            return false;
        }

        public bool TryUpgradeCosmicCapybara()
        {
            int cost = CosmicCost;
            if (totalStardust >= cost)
            {
                totalStardust -= cost;
                cosmicCapybaraLevel++;
                OnStardustChanged?.Invoke(totalStardust);
                OnTalentUpgraded?.Invoke();
                if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradeSfx();
                return true;
            }
            return false;
        }

        public void LoadPrestigeData(int stardust, int count, int astral, int royal, int cosmic)
        {
            totalStardust = stardust;
            prestigeCount = count;
            astralEfficiencyLevel = astral;
            royalInheritanceLevel = royal;
            cosmicCapybaraLevel = cosmic;

            OnStardustChanged?.Invoke(totalStardust);
        }
    }
}
