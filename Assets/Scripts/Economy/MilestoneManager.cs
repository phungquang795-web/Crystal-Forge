using System;
using System.Collections.Generic;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.Data;

namespace CrystalForge.Economy
{
    [DefaultExecutionOrder(-30)]
    public class MilestoneManager : MonoBehaviour
    {
        public static MilestoneManager Instance { get; private set; }

        [Header("Milestone Configurations (Bronze -> Guardian)")]
        [SerializeField] private List<MilestoneDataSO> milestoneConfigs = new List<MilestoneDataSO>();

        private readonly HashSet<MilestoneTier> completedTiers = new HashSet<MilestoneTier>();

        public event Action<MilestoneDataSO> OnMilestoneAchieved;
        public event Action<float> OnGlobalMultiplierUpdated;

        public IReadOnlyCollection<MilestoneTier> CompletedTiers => completedTiers;

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
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged += EvaluateMilestones;
            }
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged -= EvaluateMilestones;
            }
        }

        public void EvaluateMilestones(double lifetime)
        {
            foreach (var milestone in milestoneConfigs)
            {
                if (milestone == null) continue;

                if (!completedTiers.Contains(milestone.tier) && lifetime >= milestone.requiredLifetimeCrystal)
                {
                    ClaimMilestone(milestone);
                }
            }
        }

        private void ClaimMilestone(MilestoneDataSO milestone)
        {
            completedTiers.Add(milestone.tier);

            // Mở khóa Generator nếu cột mốc yêu cầu
            if (milestone.unlocksGenerator && GeneratorSystem.Instance != null)
            {
                GeneratorSystem.Instance.UnlockGenerator(milestone.unlockedGeneratorType);
            }

            // Tính toán lại hệ số sản xuất toàn cục
            RecalculateGlobalMultiplier();

            if (CrystalForge.UI.FloatingTextSpawner.Instance != null)
            {
                string rName = !string.IsNullOrEmpty(milestone.relicName) ? milestone.relicName : milestone.milestoneTitle;
                CrystalForge.UI.FloatingTextSpawner.Instance.SpawnFloatingText(
                    new Vector3(0f, 2.5f, 0f), 
                    CrystalForge.Utils.FontSanitizer.Sanitize($"✨ KHAI QUAT: {rName.ToUpper()}! ✨"), 
                    new Color(1.0f, 0.88f, 0.25f)
                );
            }

            OnMilestoneAchieved?.Invoke(milestone);
        }

        private void RecalculateGlobalMultiplier()
        {
            float totalMultiplier = 1.0f;
            foreach (var milestone in milestoneConfigs)
            {
                if (milestone != null && completedTiers.Contains(milestone.tier))
                {
                    if (milestone.globalProductionMultiplier > 1.0f)
                    {
                        totalMultiplier *= milestone.globalProductionMultiplier;
                    }
                }
            }

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.SetGlobalMultiplier(totalMultiplier);
            }
            OnGlobalMultiplierUpdated?.Invoke(totalMultiplier);
        }

        public MilestoneDataSO GetNextPendingMilestone()
        {
            foreach (var milestone in milestoneConfigs)
            {
                if (milestone != null && !completedTiers.Contains(milestone.tier))
                {
                    return milestone;
                }
            }
            return null; // Đã hoàn thành hết
        }

        public bool IsMilestoneCompleted(MilestoneTier tier)
        {
            return completedTiers.Contains(tier);
        }

        public void LoadCompletedMilestones(IEnumerable<MilestoneTier> loadedTiers)
        {
            completedTiers.Clear();
            if (loadedTiers != null)
            {
                foreach (var tier in loadedTiers)
                {
                    completedTiers.Add(tier);
                }
            }
            RecalculateGlobalMultiplier();
        }
    }
}
