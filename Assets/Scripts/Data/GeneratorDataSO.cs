using UnityEngine;
using CrystalForge.Core;

namespace CrystalForge.Data
{
    [CreateAssetMenu(fileName = "NewGeneratorData", menuName = "CrystalForge/Data/Generator Data")]
    public class GeneratorDataSO : ScriptableObject
    {
        [Header("Identity")]
        public GeneratorType generatorType;
        public string generatorName = "Crystal Drill";
        [TextArea(2, 4)]
        public string description = "Tự động khai thác tinh thể ngọc theo thời gian.";
        public Sprite icon;

        [Header("Economy Formula")]
        [Tooltip("Giá mua ban đầu cấp 1")]
        public double baseCost = 100.0;
        [Tooltip("Hệ số tăng giá theo cấp: Cost = BaseCost * (Growth ^ Level)")]
        [Range(1.05f, 2.0f)]
        public float costGrowthRate = 1.15f;
        [Tooltip("Sản lượng cơ bản mỗi giây tại cấp 1")]
        public double baseProductionPerSec = 1.0;

        [Header("Unlock Condition")]
        [Tooltip("Lượng Lifetime Crystal cần tích lũy để mở khóa")]
        public double unlockLifetimeRequired = 100.0;

        public double CalculateCost(int currentLevel)
        {
            if (currentLevel <= 0) return baseCost;
            return baseCost * System.Math.Pow(costGrowthRate, currentLevel);
        }

        public double CalculateProduction(int currentLevel)
        {
            if (currentLevel <= 0) return 0.0;
            return baseProductionPerSec * currentLevel;
        }
    }
}
