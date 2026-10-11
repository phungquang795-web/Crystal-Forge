using System;
using System.Collections.Generic;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.UI;
using CrystalForge.Utils;

namespace CrystalForge.Economy
{
    [Serializable]
    public class GameSaveData
    {
        public double currentCrystal = 0.0;
        public double lifetimeCrystal = 0.0;
        public int tapPowerLevel = 0;
        public int drillLevel = 0;
        public int furnaceLevel = 0;
        public int extractorLevel = 0;
        public int playerHpLevel = 0;
        public int playerAtkLevel = 0;
        public int playerSpdLevel = 0;
        public int playerSkillLevel = 0;
        public List<int> completedMilestoneTiers = new List<int>();
        public long lastSaveTimestampUtc = 0;

        // Relic Data (Ý tưởng 2)
        public int relicHammerLevel = 0;
        public int relicAegisLevel = 0;
        public int relicIgnisLevel = 0;

        // Prestige Data (Ý tưởng 3)
        public int totalStardust = 0;
        public int prestigeCount = 0;
        public int astralEfficiencyLevel = 0;
        public int royalInheritanceLevel = 0;
        public int cosmicCapybaraLevel = 0;
    }

    [DefaultExecutionOrder(-20)]
    public class SaveLoadSystem : MonoBehaviour
    {
        public static SaveLoadSystem Instance { get; private set; }

        private const string SAVE_KEY = "CrystalForge_SaveData_v1";
        private const double MAX_OFFLINE_SECONDS = 28800.0; // 8 tiếng
        private const double OFFLINE_EFFICIENCY = 0.5; // 50% sản lượng

        [Header("Auto Save")]
        [SerializeField] private float autoSaveInterval = 15f;
        private float autoSaveTimer = 0f;

        public event Action<double, double> OnOfflineEarningsCalculated; // (earnedAmount, offlineSeconds)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            LoadGame();
        }

        private void Update()
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0f;
                SaveGame(silent: true);
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame(silent: true);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGame(silent: true);
            }
        }

        public void SaveGame(bool silent = false)
        {
            if (ResourceManager.Instance == null) return;

            var data = new GameSaveData
            {
                currentCrystal = ResourceManager.Instance.CurrentCrystal,
                lifetimeCrystal = ResourceManager.Instance.LifetimeCrystal,
                tapPowerLevel = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentTapLevel : 0,
                lastSaveTimestampUtc = DateTime.UtcNow.Ticks
            };

            if (GeneratorSystem.Instance != null)
            {
                var drill = GeneratorSystem.Instance.GetState(GeneratorType.CrystalDrill);
                var furnace = GeneratorSystem.Instance.GetState(GeneratorType.ManaFurnace);
                var extractor = GeneratorSystem.Instance.GetState(GeneratorType.AutoExtractor);

                data.drillLevel = drill != null ? drill.currentLevel : 0;
                data.furnaceLevel = furnace != null ? furnace.currentLevel : 0;
                data.extractorLevel = extractor != null ? extractor.currentLevel : 0;
            }

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.GetPlayerStats(out data.playerHpLevel, out data.playerAtkLevel, out data.playerSpdLevel, out data.playerSkillLevel);
            }

            if (MilestoneManager.Instance != null)
            {
                foreach (var tier in MilestoneManager.Instance.CompletedTiers)
                {
                    data.completedMilestoneTiers.Add((int)tier);
                }
            }

            if (RelicManager.Instance != null)
            {
                data.relicHammerLevel = RelicManager.Instance.HammerLevel;
                data.relicAegisLevel = RelicManager.Instance.AegisLevel;
                data.relicIgnisLevel = RelicManager.Instance.IgnisLevel;
            }

            if (PrestigeManager.Instance != null)
            {
                data.totalStardust = PrestigeManager.Instance.TotalStardust;
                data.prestigeCount = PrestigeManager.Instance.PrestigeCount;
                data.astralEfficiencyLevel = PrestigeManager.Instance.AstralEfficiencyLevel;
                data.royalInheritanceLevel = PrestigeManager.Instance.RoyalInheritanceLevel;
                data.cosmicCapybaraLevel = PrestigeManager.Instance.CosmicCapybaraLevel;
            }

            string json = JsonUtility.ToJson(data, true);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();

            if (!silent)
            {
                Debug.Log("<color=#55FF88>[SaveLoadSystem] Đã lưu tiến trình thành công!</color>");
            }
        }

        public void LoadGame()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                Debug.Log("[SaveLoadSystem] Tạo dữ liệu trò chơi mới.");
                return;
            }

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var data = JsonUtility.FromJson<GameSaveData>(json);
                if (data == null) return;

                // 1. Nạp cấp độ Generator
                if (GeneratorSystem.Instance != null)
                {
                    var levels = new Dictionary<GeneratorType, int>
                    {
                        { GeneratorType.CrystalDrill, data.drillLevel },
                        { GeneratorType.ManaFurnace, data.furnaceLevel },
                        { GeneratorType.AutoExtractor, data.extractorLevel }
                    };
                    GeneratorSystem.Instance.LoadLevels(levels);
                }

                // 2. Nạp cấp độ Tap Power & Player Stats
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.LoadTapLevel(data.tapPowerLevel);
                    UpgradeManager.Instance.LoadPlayerStats(data.playerHpLevel, data.playerAtkLevel, data.playerSpdLevel, data.playerSkillLevel);
                }

                // 3. Nạp Cột mốc Milestones
                if (MilestoneManager.Instance != null && data.completedMilestoneTiers != null)
                {
                    var tiers = new List<MilestoneTier>();
                    foreach (var i in data.completedMilestoneTiers)
                    {
                        tiers.Add((MilestoneTier)i);
                    }
                    MilestoneManager.Instance.LoadCompletedMilestones(tiers);
                }

                // 3b. Nạp cấp độ Cổ Vật (Relics)
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.LoadLevels(data.relicHammerLevel, data.relicAegisLevel, data.relicIgnisLevel);
                }

                // 3c. Nạp dữ liệu Đại Nung Chảy (Prestige)
                if (PrestigeManager.Instance != null)
                {
                    PrestigeManager.Instance.LoadPrestigeData(
                        data.totalStardust,
                        data.prestigeCount,
                        data.astralEfficiencyLevel,
                        data.royalInheritanceLevel,
                        data.cosmicCapybaraLevel
                    );
                }

                // 4. Tính toán Thu Nhập Treo Máy Offline (Offline Income)
                double offlineEarnings = 0.0;
                double offlineDuration = 0.0;

                if (data.lastSaveTimestampUtc > 0)
                {
                    var lastTime = new DateTime(data.lastSaveTimestampUtc, DateTimeKind.Utc);
                    var elapsed = DateTime.UtcNow - lastTime;
                    offlineDuration = elapsed.TotalSeconds;

                    if (offlineDuration >= 10.0 && ResourceManager.Instance != null)
                    {
                        double effectiveRate = ResourceManager.Instance.EffectiveProductionPerSec;
                        double clampedSeconds = Math.Min(offlineDuration, MAX_OFFLINE_SECONDS);
                        double bonusOffline = (RelicManager.Instance != null) ? RelicManager.Instance.OfflineBonusRatio : 0.0;
                        offlineEarnings = effectiveRate * clampedSeconds * (OFFLINE_EFFICIENCY + bonusOffline);
                    }
                }

                // 5. Nạp tài nguyên vào ResourceManager
                if (ResourceManager.Instance != null)
                {
                    double totalLoadedCrystal = data.currentCrystal + offlineEarnings;
                    double totalLoadedLifetime = data.lifetimeCrystal + offlineEarnings;

                    ResourceManager.Instance.LoadData(
                        totalLoadedCrystal,
                        totalLoadedLifetime,
                        data.tapPowerLevel,
                        GeneratorSystem.Instance != null ? GeneratorSystem.Instance.GetState(GeneratorType.CrystalDrill).CurrentProduction : 0.0
                    );
                }

                // 6. Phát sự kiện thu nhập offline nếu có
                if (offlineEarnings > 0.0)
                {
                    Debug.Log($"<color=#FFCC00>[Offline Progress] Chào mừng trở lại! Bạn nhận được +{CrystalFormat.Format(offlineEarnings)} Crystal sau {offlineDuration:F0}s treo máy.</color>");
                    OnOfflineEarningsCalculated?.Invoke(offlineEarnings, offlineDuration);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveLoadSystem] Lỗi khi nạp dữ liệu: {ex.Message}");
            }
        }

        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("<color=#FF4444>[SaveLoadSystem] Đã xóa toàn bộ tiến trình game!</color>");
        }
    }
}
