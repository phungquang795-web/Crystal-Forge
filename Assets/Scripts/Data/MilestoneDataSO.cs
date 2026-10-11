using UnityEngine;
using CrystalForge.Core;

namespace CrystalForge.Data
{
    [CreateAssetMenu(fileName = "NewMilestoneData", menuName = "CrystalForge/Data/Milestone Data")]
    public class MilestoneDataSO : ScriptableObject
    {
        [Header("Milestone Tier")]
        public MilestoneTier tier;
        public string milestoneTitle = "Bronze Milestone";
        public string relicName = "Bua Ren Runic";
        [TextArea(2, 4)]
        public string relicLore = "Di vat co xua tiep them linh luc khai khoang.";
        [TextArea(2, 4)]
        public string description = "Đạt 100 Lifetime Crystal để mở khóa.";

        [Header("Requirement")]
        public double requiredLifetimeCrystal = 100.0;

        [Header("Rewards")]
        [Tooltip("Hệ số nhân toàn cục vào sản lượng (VD: 1.25x = +25% Global Production)")]
        public float globalProductionMultiplier = 1.0f;
        [Tooltip("Có mở khóa Generator mới hay không")]
        public bool unlocksGenerator = false;
        public GeneratorType unlockedGeneratorType;
        [Tooltip("Mô tả phần thưởng hiển thị trên UI")]
        public string rewardSummaryText = "Unlock Crystal Drill";
    }
}
