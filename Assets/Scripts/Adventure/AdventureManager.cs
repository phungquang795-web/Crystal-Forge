using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrystalForge.Core;
using CrystalForge.Data;
using CrystalForge.Economy;

namespace CrystalForge.Adventure
{
    [DefaultExecutionOrder(-50)]
    public class AdventureManager : MonoBehaviour
    {
        public static AdventureManager Instance { get; private set; }

        [Header("Configuration Database")]
        [SerializeField] private EnemyDataSO[] enemyDatabase;
        [SerializeField] private EventDataSO[] eventDatabase;
        [SerializeField] private SkillDataSO[] skillDatabase;

        [Header("Runtime Hero Stats")]
        [SerializeField] private int heroMaxHp = 1000;
        [SerializeField] private int heroCurrentHp = 1000;
        [SerializeField] private int heroBaseAtk = 50;
        [SerializeField] private int heroBaseDef = 10;
        [SerializeField] private float heroBaseCritRate = 0.10f; // 10%

        [Header("Current Run State")]
        [SerializeField] private AdventureState currentState = AdventureState.None;
        [SerializeField] private int currentFloor = 1;
        [SerializeField] private double accumulatedReward = 0.0;
        [SerializeField] private float bestBuffMultiplier = 1.0f;
        [SerializeField] private float bestBuffDuration = 0f;

        private readonly List<SkillDataSO> acquiredSkills = new List<SkillDataSO>();
        private EnemyDataSO currentEnemy;
        private int currentEnemyHp;
        private int currentEnemyMaxHp;
        private Coroutine combatCoroutine;
        private int heroAttackCount = 0;

        // Events for UI and 3D visual communication
        public event Action<AdventureState> OnStateChanged;
        public event Action<int, int> OnHeroHpUpdated; // currentHp, maxHp
        public event Action<int, int> OnEnemyHpUpdated; // currentHp, maxHp
        public event Action<EnemyDataSO> OnEnemyEncountered;
        public event Action<EventDataSO> OnEventEncountered;
        public event Action<SkillDataSO[]> OnSkillsOffered; // 3 choices
        public event Action<string, Color> OnCombatLogMessage;
        public event Action<double, float, float, bool> OnRewardSummary; // crystals, buffMult, buffDuration, isVictory
        public event Action<int> OnFloorChanged; // current floor
        public event Action OnHeroAttackTriggered;
        public event Action<int, bool> OnHeroDamageDealt; // damage, isCrit
        public event Action OnEnemyAttackTriggered;
        public event Action OnEnemyDefeatedTriggered;
        public event Action OnHeroDefeatedTriggered;

        public AdventureState CurrentState => currentState;
        public int CurrentFloor => currentFloor;
        public int HeroCurrentHp => heroCurrentHp;
        public int HeroMaxHp => heroMaxHp;
        public EnemyDataSO CurrentEnemy => currentEnemy;
        public IReadOnlyList<SkillDataSO> AcquiredSkills => acquiredSkills;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Bắt đầu lượt thám hiểm mới từ tầng 1
        /// </summary>
        public void StartNewAdventureRun()
        {
            StopCombatLoop();

            currentFloor = 1;
            accumulatedReward = 0.0;
            bestBuffMultiplier = 1.0f;
            bestBuffDuration = 0f;
            acquiredSkills.Clear();
            heroAttackCount = 0;

            // Tính toán chỉ số Hero dựa trên Tap Power nâng cấp từ Lò Rèn & Player Stats
            double tapPower = ResourceManager.Instance != null ? ResourceManager.Instance.TapPower : 1.0;
            int bonusAtk = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusAtk : 0;
            int bonusHp = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusMaxHp : 0;
            float bonusCrit = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillCritRate : 0f;

            float cosmicMult = PrestigeManager.Instance != null ? PrestigeManager.Instance.CosmicCombatMultiplier : 1.0f;
            heroBaseAtk = Math.Max(50, (int)(((tapPower * 5) + bonusAtk) * cosmicMult));
            heroBaseCritRate = 0.10f + bonusCrit;

            int aegisShield = RelicManager.Instance != null ? RelicManager.Instance.AegisShieldAmount : 0;
            heroMaxHp = (int)((1000 + bonusHp) * cosmicMult) + aegisShield;
            heroCurrentHp = heroMaxHp;

            OnFloorChanged?.Invoke(currentFloor);
            OnHeroHpUpdated?.Invoke(heroCurrentHp, heroMaxHp);

            AdvanceToFloor(currentFloor);
        }

        /// <summary>
        /// Chuyển sang tầng tiếp theo trong chặng thám hiểm
        /// </summary>
        public void AdvanceToFloor(int floor)
        {
            currentFloor = floor;
            OnFloorChanged?.Invoke(currentFloor);

            // Tầng 2 là Sự kiện bí ẩn (Random Event)
            if (floor == 2 && eventDatabase != null && eventDatabase.Length > 0)
            {
                TriggerRandomEvent();
                return;
            }

            // Các tầng còn lại là Chiến đấu quái vật
            StartCombatEncounter(floor);
        }

        private void TriggerRandomEvent()
        {
            SetState(AdventureState.EventChoice);
            int randomIndex = UnityEngine.Random.Range(0, eventDatabase.Length);
            var chosenEvent = eventDatabase[randomIndex];
            OnEventEncountered?.Invoke(chosenEvent);
        }

        /// <summary>
        /// Người chơi chọn một quyết định trong sự kiện
        /// </summary>
        public void ResolveEventChoice(EventChoice choice)
        {
            if (choice.crystalChange != 0)
            {
                accumulatedReward = Math.Max(0.0, accumulatedReward + choice.crystalChange);
            }

            if (choice.heroHpPercentChange != 0)
            {
                int hpDelta = (int)(heroMaxHp * (choice.heroHpPercentChange / 100f));
                heroCurrentHp = Mathf.Clamp(heroCurrentHp + hpDelta, 1, heroMaxHp);
                OnHeroHpUpdated?.Invoke(heroCurrentHp, heroMaxHp);
            }

            if (choice.temporaryProductionBuff > bestBuffMultiplier)
            {
                bestBuffMultiplier = choice.temporaryProductionBuff;
                bestBuffDuration = choice.buffDurationSeconds;
            }

            if (choice.grantsRandomSkill && skillDatabase != null && skillDatabase.Length > 0)
            {
                var randomSkill = skillDatabase[UnityEngine.Random.Range(0, skillDatabase.Length)];
                acquiredSkills.Add(randomSkill);
                OnCombatLogMessage?.Invoke($"[KY NANG] Nhan duoc: {randomSkill.skillName}!", Color.green);
            }

            if (choice.triggersCombatEncounter)
            {
                // Bước vào trận đánh đặc biệt từ sự kiện
                SetState(AdventureState.AutoCombat);
                SetupEnemy(choice.overrideEnemy != null ? choice.overrideEnemy : GetEnemyForFloor(currentFloor));
                combatCoroutine = StartCoroutine(CombatLoopRoutine());
            }
            else
            {
                // Hoàn thành sự kiện an toàn -> Mời chọn kĩ năng hoặc qua tầng tiếp
                OfferSkillSelection();
            }
        }

        private void StartCombatEncounter(int floor)
        {
            SetState(AdventureState.AutoCombat);
            SetupEnemy(GetEnemyForFloor(floor));
            combatCoroutine = StartCoroutine(CombatLoopRoutine());
        }

        private EnemyDataSO GetEnemyForFloor(int floor)
        {
            if (enemyDatabase == null || enemyDatabase.Length == 0) return null;

            // Sắp xếp tầng tương ứng với độ mạnh quái
            int enemyIndex = Mathf.Clamp(floor - 1, 0, enemyDatabase.Length - 1);
            return enemyDatabase[enemyIndex];
        }

        private void SetupEnemy(EnemyDataSO enemy)
        {
            currentEnemy = enemy;
            if (currentEnemy != null)
            {
                currentEnemyMaxHp = currentEnemy.maxHp;
                currentEnemyHp = currentEnemyMaxHp;
            }
            else
            {
                currentEnemyMaxHp = 300;
                currentEnemyHp = 300;
            }

            OnEnemyEncountered?.Invoke(currentEnemy);
            OnEnemyHpUpdated?.Invoke(currentEnemyHp, currentEnemyMaxHp);
            OnCombatLogMessage?.Invoke($"Gap quai vat: {currentEnemy?.enemyName ?? "Quai Vat"}! Chuan bi giao chien...", Color.yellow);
        }

        private IEnumerator CombatLoopRoutine()
        {
            yield return new WaitForSeconds(0.6f);

            while (heroCurrentHp > 0 && currentEnemyHp > 0)
            {
                // 1. Lượt tấn công của Hero (kích hoạt animation trước, gây sát thương tại điểm va chạm)
                OnHeroAttackTriggered?.Invoke();
                yield return new WaitForSeconds(0.25f);
                ExecuteHeroAttack();

                if (currentEnemyHp <= 0)
                {
                    yield return new WaitForSeconds(0.7f);
                    HandleEnemyDefeated();
                    yield break;
                }

                yield return new WaitForSeconds(0.65f);

                // 2. Lượt tấn công của Quái
                OnEnemyAttackTriggered?.Invoke();
                yield return new WaitForSeconds(0.25f);
                ExecuteEnemyAttack();

                if (heroCurrentHp <= 0)
                {
                    yield return new WaitForSeconds(0.7f);
                    HandleHeroDefeated();
                    yield break;
                }

                yield return new WaitForSeconds(0.55f);
            }
        }

        private void ExecuteHeroAttack()
        {
            heroAttackCount++;

            // Tính sát thương cơ bản
            int enemyDef = currentEnemy != null ? currentEnemy.defense : 5;
            int effectiveAtk = heroBaseAtk;

            // Tính bạo kích (Crit)
            float totalCritRate = heroBaseCritRate;
            if (UpgradeManager.Instance != null)
            {
                totalCritRate += UpgradeManager.Instance.BonusSkillCritRate;
            }
            foreach (var skill in acquiredSkills)
            {
                totalCritRate += skill.critRateBonusPercent / 100f;
            }

            bool isCrit = UnityEngine.Random.value <= totalCritRate;
            int bonusSkillDmg = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillDamage : 0;
            int damage = Math.Max(5, effectiveAtk - enemyDef) + bonusSkillDmg;
            if (isCrit)
            {
                damage = (int)(damage * 2.0f);
            }

            // Kỹ năng Fireball: nổ thêm sát thương mỗi N lượt
            foreach (var skill in acquiredSkills)
            {
                if (skill.skillId == "fireball" && skill.triggerIntervalTurns > 0)
                {
                    if (heroAttackCount % skill.triggerIntervalTurns == 0)
                    {
                        damage += skill.flatBonusDamage;
                        OnCombatLogMessage?.Invoke($"[FIREBALL] Bung no! +{skill.flatBonusDamage} dmg", new Color(1f, 0.45f, 0.1f));
                    }
                }
                else if (skill.skillId == "lightning" && skill.procChancePercent > 0)
                {
                    if (UnityEngine.Random.value <= (skill.procChancePercent / 100f))
                    {
                        damage += skill.flatBonusDamage;
                        OnCombatLogMessage?.Invoke($"[LIGHTNING] Set giat te liet! +{skill.flatBonusDamage} dmg", new Color(0.2f, 0.8f, 1f));
                    }
                }
            }

            // Cổ vật Búa Sấm Sét
            if (RelicManager.Instance != null && RelicManager.Instance.HammerLevel > 0)
            {
                float hammerProcChance = 0.15f + (0.04f * RelicManager.Instance.HammerLevel);
                if (UnityEngine.Random.value <= hammerProcChance)
                {
                    int thunderDmg = (int)(damage * (0.4f + 0.15f * RelicManager.Instance.HammerLevel));
                    damage += thunderDmg;
                    OnCombatLogMessage?.Invoke($"[BUA SAM SET] Set giat lan! +{thunderDmg} dmg", new Color(0.35f, 0.95f, 1f));
                }
            }

            // Cổ vật Lõi Lửa Bất Diệt (Burn)
            if (RelicManager.Instance != null && RelicManager.Instance.IgnisLevel > 0)
            {
                int burnDmg = RelicManager.Instance.IgnisBurnDamage;
                damage += burnDmg;
                OnCombatLogMessage?.Invoke($"[LOI LUA] Thieu dot quai vat! +{burnDmg} dmg", new Color(1f, 0.45f, 0.15f));
            }

            currentEnemyHp = Mathf.Max(0, currentEnemyHp - damage);
            OnEnemyHpUpdated?.Invoke(currentEnemyHp, currentEnemyMaxHp);
            OnHeroDamageDealt?.Invoke(damage, isCrit);

            string hitText = isCrit ? $"[BAO KICH] -{damage} HP!" : $"[HERO] Chem: -{damage} HP";
            OnCombatLogMessage?.Invoke(hitText, isCrit ? new Color(1f, 0.85f, 0.2f) : Color.white);

            // Kỹ năng Hút máu (Lifesteal)
            float totalLifesteal = 0f;
            foreach (var skill in acquiredSkills)
            {
                totalLifesteal += skill.lifestealPercent;
            }
            if (totalLifesteal > 0f)
            {
                int heal = Mathf.Max(1, (int)(damage * (totalLifesteal / 100f)));
                heroCurrentHp = Mathf.Clamp(heroCurrentHp + heal, 1, heroMaxHp);
                OnHeroHpUpdated?.Invoke(heroCurrentHp, heroMaxHp);
                OnCombatLogMessage?.Invoke($"[HUT MAU] Hoi phuc: +{heal} HP", new Color(0.9f, 0.3f, 0.4f));
            }
        }

        private void ExecuteEnemyAttack()
        {
            if (currentEnemy == null) return;

            int enemyAtk = currentEnemy.attackPower;
            int effectiveDef = heroBaseDef;

            // Kỹ năng Giáp Hộ Mệnh (Guardian Shield)
            float totalDamageReduction = 0f;
            foreach (var skill in acquiredSkills)
            {
                totalDamageReduction += skill.damageReductionPercent;
            }

            // Cổ vật Khiên Băng Cổ Đại
            if (RelicManager.Instance != null && RelicManager.Instance.AegisLevel > 0)
            {
                totalDamageReduction += RelicManager.Instance.AegisDamageReduction * 100f;
            }

            int damage = Math.Max(5, enemyAtk - effectiveDef);
            if (totalDamageReduction > 0f)
            {
                damage = Mathf.Max(1, (int)(damage * (1f - (Mathf.Min(80f, totalDamageReduction) / 100f))));
            }

            heroCurrentHp = Mathf.Max(0, heroCurrentHp - damage);
            OnHeroHpUpdated?.Invoke(heroCurrentHp, heroMaxHp);

            OnCombatLogMessage?.Invoke($"[{currentEnemy.enemyName.ToUpper()}] Tan cong: -{damage} HP", new Color(1f, 0.5f, 0.5f));
        }

        private void HandleEnemyDefeated()
        {
            StopCombatLoop();
            OnEnemyDefeatedTriggered?.Invoke();

            double reward = currentEnemy != null ? currentEnemy.crystalReward : 50.0;
            accumulatedReward += reward;

            if (currentEnemy != null && currentEnemy.productionBuffMultiplier > bestBuffMultiplier)
            {
                bestBuffMultiplier = currentEnemy.productionBuffMultiplier;
                bestBuffDuration = currentEnemy.buffDurationSeconds;
            }

            OnCombatLogMessage?.Invoke($"[CHIEN THANG] {currentEnemy?.enemyName ?? "Quai"}! Nhan +{reward} Crystal.", Color.green);

            // Nếu đã hạ Boss cuối (hoặc hết tầng), trao thưởng lớn
            if (currentEnemy != null && currentEnemy.isBoss)
            {
                ShowRunSummary(true);
            }
            else
            {
                // Cho chọn kỹ năng nâng cấp Roguelike
                OfferSkillSelection();
            }
        }

        private void HandleHeroDefeated()
        {
            StopCombatLoop();
            OnHeroDefeatedTriggered?.Invoke();
            OnCombatLogMessage?.Invoke("[THAT BAI] Hero da nga xuong! Rut lui an toan...", Color.red);
            ShowRunSummary(false);
        }

        private void OfferSkillSelection()
        {
            SetState(AdventureState.SkillSelection);

            if (skillDatabase == null || skillDatabase.Length == 0)
            {
                // Không có kỹ năng -> qua tầng tiếp
                ProceedAfterSkillSelected();
                return;
            }

            // Chọn 3 kỹ năng ngẫu nhiên không trùng
            List<SkillDataSO> pool = new List<SkillDataSO>(skillDatabase);
            List<SkillDataSO> offered = new List<SkillDataSO>();
            int countToPick = Mathf.Min(3, pool.Count);

            for (int i = 0; i < countToPick; i++)
            {
                int idx = UnityEngine.Random.Range(0, pool.Count);
                offered.Add(pool[idx]);
                pool.RemoveAt(idx);
            }

            OnSkillsOffered?.Invoke(offered.ToArray());
        }

        /// <summary>
        /// Người chơi chọn 1 trong 3 thẻ bài kỹ năng
        /// </summary>
        public void SelectSkill(SkillDataSO chosenSkill)
        {
            if (chosenSkill != null)
            {
                acquiredSkills.Add(chosenSkill);
                OnCombatLogMessage?.Invoke($"[HOC KY NANG] {chosenSkill.skillName}!", Color.cyan);
            }
            ProceedAfterSkillSelected();
        }

        private void ProceedAfterSkillSelected()
        {
            if (currentFloor >= 5)
            {
                // Hoàn thành xuất sắc hành trình
                ShowRunSummary(true);
            }
            else
            {
                AdvanceToFloor(currentFloor + 1);
            }
        }

        private void ShowRunSummary(bool isVictory)
        {
            SetState(isVictory ? AdventureState.AdventureReward : AdventureState.RunDefeat);

            // Thua thì nhận 25% an ủi
            double finalCrystal = isVictory ? accumulatedReward : (accumulatedReward * 0.25);
            float finalBuffMult = isVictory ? bestBuffMultiplier : 1.0f;
            float finalBuffDuration = isVictory ? bestBuffDuration : 0f;

            OnRewardSummary?.Invoke(finalCrystal, finalBuffMult, finalBuffDuration, isVictory);
        }

        /// <summary>
        /// Nhận quà và quay trở lại màn hình chính của Lò Rèn (Loop Tie-Back)
        /// </summary>
        public void ClaimRewardAndReturnToForge()
        {
            StopCombatLoop();

            bool isVictory = (currentState == AdventureState.AdventureReward);
            double finalCrystal = isVictory ? accumulatedReward : (accumulatedReward * 0.25);

            if (ResourceManager.Instance != null)
            {
                if (finalCrystal > 0)
                {
                    ResourceManager.Instance.AddDirectBonus(finalCrystal);
                }

                if (isVictory && bestBuffMultiplier > 1.0f && bestBuffDuration > 0f)
                {
                    ResourceManager.Instance.ApplyTemporaryBuff(bestBuffMultiplier, bestBuffDuration);
                }
            }

            SetState(AdventureState.None);
        }

        private void SetState(AdventureState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        private void StopCombatLoop()
        {
            if (combatCoroutine != null)
            {
                StopCoroutine(combatCoroutine);
                combatCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            StopCombatLoop();
        }
    }
}
