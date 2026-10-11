using System;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.Data;

namespace CrystalForge.Economy
{
    [DefaultExecutionOrder(-40)]
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }

        [Header("Tap Power Upgrade Config")]
        [SerializeField] private UpgradeDataSO tapPowerData;
        [SerializeField] private int currentTapLevel = 0;

        [Header("Player Hero Stats Upgrades")]
        [SerializeField] private int playerHpLevel = 0;
        [SerializeField] private int playerAtkLevel = 0;
        [SerializeField] private int playerSpdLevel = 0;
        [SerializeField] private int playerSkillLevel = 0;

        // Events
        public event Action<int, double, double> OnTapPowerUpgraded; // newLevel, currentBonusPower, nextCost
        public event Action<PlayerStatType, int, double> OnPlayerStatUpgraded; // stat, newLevel, nextCost

        public int CurrentTapLevel => currentTapLevel;
        public double CurrentTapPowerCost => tapPowerData != null ? tapPowerData.CalculateCost(currentTapLevel) : 25.0;

        // Player Stats Levels & Costs
        public int PlayerHpLevel => playerHpLevel;
        public int PlayerAtkLevel => playerAtkLevel;
        public int PlayerSpdLevel => playerSpdLevel;
        public int PlayerSkillLevel => playerSkillLevel;

        public double CurrentHpCost => Math.Floor(40.0 * Math.Pow(1.15, playerHpLevel));
        public double CurrentAtkCost => Math.Floor(60.0 * Math.Pow(1.16, playerAtkLevel));
        public double CurrentSpeedCost => Math.Floor(80.0 * Math.Pow(1.18, playerSpdLevel));
        public double CurrentSkillCost => Math.Floor(120.0 * Math.Pow(1.20, playerSkillLevel));

        // Player Stats Bonuses
        public int BonusMaxHp => playerHpLevel * 150;
        public int BonusAtk => playerAtkLevel * 25;
        public float BonusSpeedPercent => playerSpdLevel * 5f;
        public int BonusSkillDamage => playerSkillLevel * 30;
        public float BonusSkillCritRate => playerSkillLevel * 0.02f;

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
            // Cập nhật giá trị ban đầu vào ResourceManager
            ApplyTapPowerBonus();
        }

        /// <summary>
        /// Nâng cấp Tap Power theo công thức lũy tiến BaseCost * (Growth ^ Level)
        /// </summary>
        public bool TryUpgradeTapPower()
        {
            if (tapPowerData == null || ResourceManager.Instance == null) return false;

            double cost = CurrentTapPowerCost;
            if (ResourceManager.Instance.TrySpend(cost))
            {
                currentTapLevel++;
                ApplyTapPowerBonus();
                OnTapPowerUpgraded?.Invoke(currentTapLevel, currentTapLevel * tapPowerData.powerGainPerLevel, CurrentTapPowerCost);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Nâng cấp Máu Dũng Sĩ (HP)
        /// </summary>
        public bool TryUpgradeHp()
        {
            if (ResourceManager.Instance == null) return false;
            double cost = CurrentHpCost;
            if (ResourceManager.Instance.TrySpend(cost))
            {
                playerHpLevel++;
                OnPlayerStatUpgraded?.Invoke(PlayerStatType.Hp, playerHpLevel, CurrentHpCost);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Nâng cấp Công Kích Dũng Sĩ (Attack / Damage)
        /// </summary>
        public bool TryUpgradeAtk()
        {
            if (ResourceManager.Instance == null) return false;
            double cost = CurrentAtkCost;
            if (ResourceManager.Instance.TrySpend(cost))
            {
                playerAtkLevel++;
                OnPlayerStatUpgraded?.Invoke(PlayerStatType.Attack, playerAtkLevel, CurrentAtkCost);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Nâng cấp Tốc Độ Dũng Sĩ (Speed: Đánh & Tuần tra nhanh hơn)
        /// </summary>
        public bool TryUpgradeSpeed()
        {
            if (ResourceManager.Instance == null) return false;
            double cost = CurrentSpeedCost;
            if (ResourceManager.Instance.TrySpend(cost))
            {
                playerSpdLevel++;
                OnPlayerStatUpgraded?.Invoke(PlayerStatType.Speed, playerSpdLevel, CurrentSpeedCost);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Nâng cấp Uy Lực Kỹ Năng Cổ Đại (Skill Damage & Crit)
        /// </summary>
        public bool TryUpgradeSkill()
        {
            if (ResourceManager.Instance == null) return false;
            double cost = CurrentSkillCost;
            if (ResourceManager.Instance.TrySpend(cost))
            {
                playerSkillLevel++;
                OnPlayerStatUpgraded?.Invoke(PlayerStatType.Skill, playerSkillLevel, CurrentSkillCost);
                return true;
            }
            return false;
        }

        public bool TryUpgradePlayerStat(PlayerStatType stat)
        {
            switch (stat)
            {
                case PlayerStatType.Hp: return TryUpgradeHp();
                case PlayerStatType.Attack: return TryUpgradeAtk();
                case PlayerStatType.Speed: return TryUpgradeSpeed();
                case PlayerStatType.Skill: return TryUpgradeSkill();
                default: return false;
            }
        }

        public double GetPlayerStatCost(PlayerStatType stat)
        {
            switch (stat)
            {
                case PlayerStatType.Hp: return CurrentHpCost;
                case PlayerStatType.Attack: return CurrentAtkCost;
                case PlayerStatType.Speed: return CurrentSpeedCost;
                case PlayerStatType.Skill: return CurrentSkillCost;
                default: return 0.0;
            }
        }

        public int GetPlayerStatLevel(PlayerStatType stat)
        {
            switch (stat)
            {
                case PlayerStatType.Hp: return playerHpLevel;
                case PlayerStatType.Attack: return playerAtkLevel;
                case PlayerStatType.Speed: return playerSpdLevel;
                case PlayerStatType.Skill: return playerSkillLevel;
                default: return 0;
            }
        }

        /// <summary>
        /// Proxy tiện ích để UI gọi nâng cấp Generator trực tiếp qua UpgradeManager
        /// </summary>
        public bool TryUpgradeGenerator(GeneratorType type)
        {
            if (GeneratorSystem.Instance != null)
            {
                return GeneratorSystem.Instance.TryUpgrade(type);
            }
            return false;
        }

        private void ApplyTapPowerBonus()
        {
            if (ResourceManager.Instance != null && tapPowerData != null)
            {
                double bonus = currentTapLevel * tapPowerData.powerGainPerLevel;
                ResourceManager.Instance.SetBonusTapPower(bonus);
            }
        }

        public void LoadTapLevel(int savedLevel)
        {
            currentTapLevel = savedLevel;
            ApplyTapPowerBonus();
            OnTapPowerUpgraded?.Invoke(currentTapLevel, currentTapLevel * (tapPowerData != null ? tapPowerData.powerGainPerLevel : 1.0), CurrentTapPowerCost);
        }

        public void LoadPlayerStats(int hpLvl, int atkLvl, int spdLvl, int skillLvl)
        {
            playerHpLevel = hpLvl;
            playerAtkLevel = atkLvl;
            playerSpdLevel = spdLvl;
            playerSkillLevel = skillLvl;

            OnPlayerStatUpgraded?.Invoke(PlayerStatType.Hp, playerHpLevel, CurrentHpCost);
            OnPlayerStatUpgraded?.Invoke(PlayerStatType.Attack, playerAtkLevel, CurrentAtkCost);
            OnPlayerStatUpgraded?.Invoke(PlayerStatType.Speed, playerSpdLevel, CurrentSpeedCost);
            OnPlayerStatUpgraded?.Invoke(PlayerStatType.Skill, playerSkillLevel, CurrentSkillCost);
        }

        public void GetPlayerStats(out int hpLvl, out int atkLvl, out int spdLvl, out int skillLvl)
        {
            hpLvl = playerHpLevel;
            atkLvl = playerAtkLevel;
            spdLvl = playerSpdLevel;
            skillLvl = playerSkillLevel;
        }
    }
}
