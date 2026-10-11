using UnityEngine;

namespace CrystalForge.Data
{
    [CreateAssetMenu(fileName = "NewUpgradeData", menuName = "CrystalForge/Data/Upgrade Data")]
    public class UpgradeDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string upgradeId = "tap_power";
        public string upgradeName = "Tap Power";
        [TextArea(2, 4)]
        public string description = "Tăng sản lượng Crystal nhận được trên mỗi cú click.";
        public Sprite icon;

        [Header("Pricing Formula")]
        public double baseCost = 25.0;
        [Range(1.05f, 2.0f)]
        public float costGrowthRate = 1.18f;

        [Header("Power Effect")]
        public double basePowerPerClick = 1.0;
        public double powerGainPerLevel = 1.0;

        public double CalculateCost(int currentLevel)
        {
            if (currentLevel <= 0) return baseCost;
            return baseCost * System.Math.Pow(costGrowthRate, currentLevel);
        }

        public double CalculateTapPower(int currentLevel)
        {
            // Level 0 gives base power, higher levels add powerGainPerLevel
            return basePowerPerClick + (currentLevel * powerGainPerLevel);
        }
    }
}
