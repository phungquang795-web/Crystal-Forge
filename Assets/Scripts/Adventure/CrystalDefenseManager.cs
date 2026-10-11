using System;
using System.Collections;
using UnityEngine;
using CrystalForge.Audio;
using CrystalForge.Core;
using CrystalForge.Economy;
using CrystalForge.UI;
using CrystalForge.Utils;
using CrystalForge.VFX;

namespace CrystalForge.Adventure
{
    public enum DefenseState
    {
        PeacefulIntermission, // Nghỉ cực ngắn giữa các đợt (1.5s) để nhận thưởng & ăn mừng
        Approaching,          // Quái vật xuất hiện từ mép đảo nổi và đang lao vào Trụ & Player
        InCombat,             // Quái đang áp sát và giao chiến dồn dập với Player (Capybara) & Trụ Tinh Thể
        Defeat                // Thất thủ: Trụ vỡ hoặc Dũng sĩ ngã xuống, dừng toàn bộ quái và hồi phục
    }

    public enum MonsterArchetype
    {
        FastBat,        // Dơi Pha Lê: Tốc độ cao, bay lượn nhấp nhô, máu mỏng, đánh nhanh
        ArmoredGolem,   // Thạch Nham Golem: Giáp sắt giảm 25% sát thương thường, đi chậm nện nặng
        TreasureBat,    // Quái Thần Tài Pha Lê: Vàng óng, không đánh, rơi x4 Crystal & Siêu Buff
        TitanBoss       // Đại Boss Hộ Vệ Titan: To lớn 1.35x, có chiêu dậm đất rung chuyển hòn đảo
    }

    [DefaultExecutionOrder(-45)]
    public class CrystalDefenseManager : MonoBehaviour
    {
        private static CrystalDefenseManager instance;
        public static CrystalDefenseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindAnyObjectByType<CrystalDefenseManager>(FindObjectsInactive.Include);
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("Key References")]
        [SerializeField] private CombatUnit3D heroUnit;
        [SerializeField] private Transform enemyContainer;
        [SerializeField] private Transform crystalCoreTransform;

        [Header("Monster 3D Models")]
        [SerializeField] private CombatUnit3D slimePrefab;
        [SerializeField] private CombatUnit3D minionPrefab;
        [SerializeField] private CombatUnit3D golemPrefab;
        [SerializeField] private CombatUnit3D dragonBossPrefab;

        [Header("New Image-Based Monster Models")]
        [SerializeField] private CombatUnit3D crystalSpireGolemPrefab;
        [SerializeField] private CombatUnit3D prismBatPrefab;
        [SerializeField] private CombatUnit3D mossyGuardianPrefab;

        [Header("Core Shield & Health")]
        [SerializeField] private int coreMaxShield = 100;
        [SerializeField] private int coreCurrentShield = 100;
        [SerializeField] private float shieldRegenRate = 8f; // Hồi 8 khiên/s khi nghỉ ngơi

        [Header("Hero (Player) Health & Defense")]
        [SerializeField] private int heroMaxHp = 100;
        [SerializeField] private int heroCurrentHp = 100;
        private OverheadHealthBar3D heroHealthBar;
        private bool isHeroKnockedDown = false;
        private bool isGameOver = false;

        [Header("Continuous Siege Settings")]
        [SerializeField] private float intermissionDuration = 1.6f;
        [SerializeField] private float monsterSpeed = 1.15f;
        [SerializeField] private float attackRangeFromTarget = 1.65f;
        [SerializeField] private float monsterAttackInterval = 1.15f;
        [SerializeField] private int baseMonsterDamage = 4; // Cân bằng lại: sát thương khởi điểm hợp lý

        [Header("Key Positions")]
        [SerializeField] private Vector3 coreCenterPos = new Vector3(0f, 0.05f, 0f);
        [SerializeField] private float islandRadius = 3.9f;

        [Header("Runic Overcharge (Ultimate Feature)")]
        [SerializeField] private float currentRunicEnergy = 0f;
        [SerializeField] private float maxRunicEnergy = 100f;
        [SerializeField] private float overdriveDuration = 7.5f;
        private bool isOverdriveActive = false;
        private float overdriveRemainingTimer = 0f;

        [Header("Combo Tap System")]
        private int currentComboCount = 0;
        private float comboResetTimer = 0f;
        private const float ComboTimeout = 1.25f;

        [Header("Boss & Archetype Special Mechanics")]
        private bool isBossChargingStomp = false;
        private int stompParryTapCount = 0;
        private const int ParryTapThreshold = 4; // Người chơi tap 4 lần trong 0.9s để giảm 50% sát thương Dậm Đất
        private bool isBossEnraged = false;
        private float treasureEscapeTimer = 22f;
        private float lastBossTimerAlert = 99f;

        private DefenseState currentState = DefenseState.PeacefulIntermission;
        private CombatUnit3D currentActiveMonster;
        private OverheadHealthBar3D currentMonsterHealthBar;
        private MonsterArchetype currentArchetype = MonsterArchetype.FastBat;

        private int currentMonsterMaxHp = 100;
        private int currentMonsterHp = 100;
        private int currentWave = 1;
        private float nextMonsterAttackTime = 0f;
        private float nextHeroAutoAttackTime = 0f;
        private int bossAttackCounter = 0;
        private float bossSiegeTimer = 45f;
        private bool isBossSiege = false;

        private Coroutine coreShakeCoroutine;
        private Coroutine camShakeCoroutine;
        private Vector3 originalCoreLocalPos;
        private Camera mainCamera;
        private Vector3 originalCamPos;

        public DefenseState CurrentState => currentState;
        public CombatUnit3D CurrentActiveMonster => currentActiveMonster;
        public bool IsMonsterActive => currentActiveMonster != null && currentActiveMonster.gameObject.activeInHierarchy && (currentState == DefenseState.Approaching || currentState == DefenseState.InCombat);
        public int CoreCurrentShield => coreCurrentShield;
        public int CoreMaxShield => coreMaxShield;
        public CombatUnit3D HeroUnit => heroUnit;
        public int HeroCurrentHp => heroCurrentHp;
        public int HeroMaxHp => heroMaxHp;
        public int CurrentWave => currentWave;
        public bool IsHeroKnockedDown => isHeroKnockedDown;
        public bool IsGameOver => isGameOver;
        public MonsterArchetype CurrentArchetype => currentArchetype;
        public float CurrentRunicEnergy => currentRunicEnergy;
        public float MaxRunicEnergy => maxRunicEnergy;
        public bool IsOverdriveActive => isOverdriveActive;
        public float OverdriveRemainingTimer => overdriveRemainingTimer;
        public int CurrentComboCount => currentComboCount;

        // Events
        public event Action<DefenseState> OnDefenseStateChanged;
        public event Action<int, int> OnCoreShieldChanged; // current, max
        public event Action<int, int> OnHeroHpChanged;     // current, max
        public event Action<int, int> OnMonsterHpChanged;  // current, max
        public event Action<int> OnWaveStarted;            // wave index
        public event Action<string> OnSiegeAlertMessage;
        public event Action<int, string> OnGameOverTriggered; // waveReached, causeOfDeath
        public event Action<float, float, bool, float> OnOverchargeChanged; // current, max, isActive, remainingTime
        public event Action<string, int> OnBossWarningTriggered; // bossTitle, wave
        public event Action<int> OnComboUpdated; // combo count

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            AutoFindReferences();
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                originalCamPos = mainCamera.transform.position;
            }
        }

        private void Start()
        {
            if (crystalCoreTransform != null)
            {
                originalCoreLocalPos = crystalCoreTransform.localPosition;
            }

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnPlayerStatUpgraded += HandlePlayerStatUpgraded;
                heroMaxHp = 100 + UpgradeManager.Instance.BonusMaxHp;
            }
            else
            {
                heroMaxHp = 100;
            }

            coreCurrentShield = coreMaxShield;
            heroCurrentHp = heroMaxHp;

            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
            OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);
            OnOverchargeChanged?.Invoke(currentRunicEnergy, maxRunicEnergy, isOverdriveActive, 0f);

            // Cấu hình thanh máu trên đầu Người Bảo Vệ
            if (heroUnit != null)
            {
                heroUnit.gameObject.SetActive(true);
                heroHealthBar = heroUnit.GetComponentInChildren<OverheadHealthBar3D>(true);
                if (heroHealthBar != null)
                {
                    heroHealthBar.BarOwner = HealthBarOwner.Player;
                    heroHealthBar.SetupBar("NGƯỜI BẢO VỆ", heroCurrentHp, heroMaxHp, new Color(0.1f, 0.95f, 0.45f));
                }

                // Cho Capybara đi tuần bảo vệ lò rèn ban đầu
                float speedBonus = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSpeedPercent : 0f;
                float patrolSpeed = 0.75f * (1f + speedBonus / 100f);
                heroUnit.StartIdlePatrol(coreCenterPos, 2.15f, patrolSpeed);
            }

            // BẮT ĐẦU VÒNG LẶP LIÊN TỤC: Đợt quái đầu tiên xuất hiện sau 1.0s
            StartCoroutine(InitialSpawnRoutine());
        }

        private void OnDestroy()
        {
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnPlayerStatUpgraded -= HandlePlayerStatUpgraded;
            }
        }

        private void HandlePlayerStatUpgraded(PlayerStatType stat, int level, double nextCost)
        {
            if (stat == PlayerStatType.Hp)
            {
                int oldMax = heroMaxHp;
                heroMaxHp = 100 + (UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusMaxHp : 0);
                int hpGain = heroMaxHp - oldMax;
                if (hpGain > 0)
                {
                    heroCurrentHp = Mathf.Min(heroMaxHp, heroCurrentHp + hpGain);
                }
                OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);
                if (heroHealthBar != null)
                {
                    heroHealthBar.HandleHpUpdated(heroCurrentHp, heroMaxHp);
                }

                if (FloatingTextSpawner.Instance != null && heroUnit != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(heroUnit.transform.position + Vector3.up * 1.5f, $"+{hpGain} MAX HP!", new Color(0.2f, 1f, 0.4f));
                }
            }
            else if (stat == PlayerStatType.Speed)
            {
                if (heroUnit != null && currentState == DefenseState.PeacefulIntermission)
                {
                    float speedBonus = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSpeedPercent : 0f;
                    float patrolSpeed = 0.75f * (1f + speedBonus / 100f);
                    heroUnit.StartIdlePatrol(coreCenterPos, 2.15f, patrolSpeed);
                }

                if (FloatingTextSpawner.Instance != null && heroUnit != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(heroUnit.transform.position + Vector3.up * 1.5f, "TĂNG TỐC ĐỘ!", new Color(0.4f, 0.8f, 1f));
                }
            }
            else if (stat == PlayerStatType.Attack)
            {
                if (FloatingTextSpawner.Instance != null && heroUnit != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(heroUnit.transform.position + Vector3.up * 1.5f, "+25 SÁT THƯƠNG!", new Color(1f, 0.5f, 0.2f));
                }
            }
            else if (stat == PlayerStatType.Skill)
            {
                if (FloatingTextSpawner.Instance != null && heroUnit != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(heroUnit.transform.position + Vector3.up * 1.5f, "KỸ NĂNG CỔ NGỮ +!", new Color(0.8f, 0.4f, 1f));
                }
            }
        }

        private IEnumerator InitialSpawnRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            SpawnNextMonsterWave();
        }

        private void AutoFindReferences()
        {
            if (heroUnit == null)
            {
                var heroObj = GameObject.Find("Player_CapybaraHero");
                if (heroObj != null) heroUnit = heroObj.GetComponent<CombatUnit3D>();
            }

            if (crystalCoreTransform == null)
            {
                var coreObj = GameObject.Find("CrystalCore");
                if (coreObj != null) crystalCoreTransform = coreObj.transform;
            }

            if (enemyContainer == null)
            {
                var containerObj = GameObject.Find("EnemyContainer");
                if (containerObj != null) enemyContainer = containerObj.transform;
            }

            if (enemyContainer != null)
            {
                if (crystalSpireGolemPrefab == null)
                {
                    var spireGolem = enemyContainer.Find("Monster_CrystalSpireGolem");
                    if (spireGolem != null) crystalSpireGolemPrefab = spireGolem.GetComponent<CombatUnit3D>();
                }
                if (prismBatPrefab == null)
                {
                    var bat = enemyContainer.Find("Monster_PrismBat");
                    if (bat != null) prismBatPrefab = bat.GetComponent<CombatUnit3D>();
                }
                if (mossyGuardianPrefab == null)
                {
                    var guardian = enemyContainer.Find("Monster_MossyTitanGuardian");
                    if (guardian != null) mossyGuardianPrefab = guardian.GetComponent<CombatUnit3D>();
                }
            }

            HideAllMonsters();
        }

        private void HideAllMonsters()
        {
            if (crystalSpireGolemPrefab != null) crystalSpireGolemPrefab.gameObject.SetActive(false);
            if (prismBatPrefab != null) prismBatPrefab.gameObject.SetActive(false);
            if (mossyGuardianPrefab != null) mossyGuardianPrefab.gameObject.SetActive(false);
            if (slimePrefab != null) slimePrefab.gameObject.SetActive(false);
            if (minionPrefab != null) minionPrefab.gameObject.SetActive(false);
            if (golemPrefab != null) golemPrefab.gameObject.SetActive(false);
            if (dragonBossPrefab != null) dragonBossPrefab.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateOverdriveTimer();
            UpdateComboTimer();

            // 0. Fail-safe kiểm tra tức thì điều kiện Thất thủ: Trụ vỡ hoặc Dũng Sĩ gục ngã
            if (!isGameOver && (currentState == DefenseState.Approaching || currentState == DefenseState.InCombat))
            {
                if (coreCurrentShield <= 0)
                {
                    TriggerCoreDestroyedGameOver();
                    return;
                }
                if (heroCurrentHp <= 0)
                {
                    TriggerHeroDeathGameOver();
                    return;
                }
            }

            // Kiểm tra giới hạn thời gian khiêu chiến Boss (Lose condition 3)
            if (isBossSiege && (currentState == DefenseState.Approaching || currentState == DefenseState.InCombat) && !isGameOver)
            {
                bossSiegeTimer -= Time.deltaTime;
                if (bossSiegeTimer <= 10f && bossSiegeTimer > 0f)
                {
                    int secCeil = Mathf.CeilToInt(bossSiegeTimer);
                    if (secCeil < lastBossTimerAlert)
                    {
                        lastBossTimerAlert = secCeil;
                        TriggerCameraShake(0.1f, 0.04f);
                        if (FloatingTextSpawner.Instance != null && currentActiveMonster != null)
                        {
                            FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 2.2f, $"⚠️ CÒN {secCeil}S ĐỂ HẠ BOSS!", new Color(1f, 0.3f, 0.2f));
                        }
                    }
                }

                if (bossSiegeTimer <= 0f)
                {
                    TriggerBossEnrageGameOver();
                    return;
                }
            }

            switch (currentState)
            {
                case DefenseState.PeacefulIntermission:
                    if (!isGameOver && coreCurrentShield < coreMaxShield)
                    {
                        coreCurrentShield = Mathf.Min(coreMaxShield, coreCurrentShield + Mathf.CeilToInt(shieldRegenRate * Time.deltaTime));
                        OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
                    }
                    break;

                case DefenseState.Approaching:
                    UpdateApproachingState();
                    break;

                case DefenseState.InCombat:
                    UpdateCombatState();
                    break;

                case DefenseState.Defeat:
                    // Đang ở trạng thái Thất thủ: Tạm ngưng toàn bộ logic chiến đấu cho đến khi bấm Chơi Lại
                    return;
            }
        }

        private void UpdateOverdriveTimer()
        {
            if (isOverdriveActive)
            {
                overdriveRemainingTimer -= Time.deltaTime;
                OnOverchargeChanged?.Invoke(currentRunicEnergy, maxRunicEnergy, isOverdriveActive, overdriveRemainingTimer);

                if (overdriveRemainingTimer <= 0f)
                {
                    EndOverdrive();
                }
            }
        }

        private void UpdateComboTimer()
        {
            if (currentComboCount > 0)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0f)
                {
                    currentComboCount = 0;
                    OnComboUpdated?.Invoke(currentComboCount);
                }
            }
        }

        private void UpdateApproachingState()
        {
            if (currentActiveMonster == null || !currentActiveMonster.gameObject.activeInHierarchy)
            {
                return;
            }

            Vector3 monsterPos = currentActiveMonster.transform.position;
            Vector3 toCore = (coreCenterPos - monsterPos);
            toCore.y = 0f;
            float distToCore = toCore.magnitude;

            float distToHero = 999f;
            if (heroUnit != null && !isHeroKnockedDown)
            {
                Vector3 toHero = (heroUnit.transform.position - monsterPos);
                toHero.y = 0f;
                distToHero = toHero.magnitude;
            }

            // Quái Thần Tài không tấn công: Bay quanh đảo, nếu sau 22s không bị hạ thì bay đi mất
            if (currentArchetype == MonsterArchetype.TreasureBat)
            {
                treasureEscapeTimer -= Time.deltaTime;
                if (treasureEscapeTimer <= 0f)
                {
                    HandleTreasureEscaped();
                    return;
                }

                // Bay quanh đảo rồi nhấp nhô vui nhộn
                float orbitAngle = Time.time * 1.5f;
                float tx = coreCenterPos.x + Mathf.Cos(orbitAngle) * (islandRadius * 0.75f);
                float tz = coreCenterPos.z + Mathf.Sin(orbitAngle) * (islandRadius * 0.75f);
                Vector3 targetOrbit = new Vector3(tx, coreCenterPos.y + 0.35f + Mathf.Sin(Time.time * 6f) * 0.2f, tz);
                currentActiveMonster.transform.position = Vector3.MoveTowards(monsterPos, targetOrbit, monsterSpeed * Time.deltaTime);
                Vector3 lookDir = (targetOrbit - monsterPos).normalized;
                if (lookDir != Vector3.zero) currentActiveMonster.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);

                UpdateHeroAutoCombat();
                return;
            }

            // Quái chiến đấu: Khi tới tầm đánh -> Chuyển sang InCombat
            if (distToCore <= attackRangeFromTarget || distToHero <= attackRangeFromTarget)
            {
                SetState(DefenseState.InCombat);
                nextMonsterAttackTime = Time.time + (currentArchetype == MonsterArchetype.FastBat ? 0.25f : 0.45f);
                return;
            }

            // Tiếp tục bước đi lao về tâm đảo
            Vector3 step = toCore.normalized * (monsterSpeed * Time.deltaTime);
            float bobAmp = (currentArchetype == MonsterArchetype.FastBat) ? 0.22f : 0.08f;
            float bobFreq = (currentArchetype == MonsterArchetype.FastBat) ? 10f : 6f;
            float hop = Mathf.Abs(Mathf.Sin(Time.time * bobFreq)) * bobAmp;
            currentActiveMonster.transform.position = new Vector3(monsterPos.x + step.x, coreCenterPos.y + hop, monsterPos.z + step.z);

            if (toCore != Vector3.zero)
            {
                currentActiveMonster.transform.rotation = Quaternion.LookRotation(toCore.normalized, Vector3.up);
            }

            // Capybara tự động chạy tới tiếp cận quái
            UpdateHeroAutoCombat();
        }

        private void UpdateCombatState()
        {
            if (currentActiveMonster == null || !currentActiveMonster.gameObject.activeInHierarchy)
            {
                return;
            }

            // Quái vật tấn công luân phiên
            if (Time.time >= nextMonsterAttackTime && !currentActiveMonster.IsBusy && currentArchetype != MonsterArchetype.TreasureBat)
            {
                nextMonsterAttackTime = Time.time + monsterAttackInterval;
                ExecuteMonsterAttackAction();
            }

            // Capybara vung búa phản công
            UpdateHeroAutoCombat();
        }

        private void UpdateHeroAutoCombat()
        {
            if (heroUnit == null || isHeroKnockedDown || currentActiveMonster == null || !currentActiveMonster.gameObject.activeInHierarchy)
            {
                return;
            }

            if (Time.time >= nextHeroAutoAttackTime && !heroUnit.IsBusy)
            {
                float speedBonus = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSpeedPercent : 0f;
                float overdriveMult = isOverdriveActive ? 2.0f : 1.0f;
                float attackInterval = Mathf.Max(0.22f, 1.05f / ((1f + speedBonus / 100f) * overdriveMult));
                nextHeroAutoAttackTime = Time.time + attackInterval;
                ExecuteHeroAutoAttack();
            }
        }

        /// <summary>
        /// Triệu hồi đợt quái tiếp theo: Phân loại Archetype, Scale độ khó lũy tiến hợp lý
        /// </summary>
        public void SpawnNextMonsterWave()
        {
            HideAllMonsters();
            bossAttackCounter = 0;
            isBossChargingStomp = false;
            isBossEnraged = false;
            stompParryTapCount = 0;
            lastBossTimerAlert = 99f;

            CombatUnit3D chosenPrefab = null;
            int baseHp = 42;
            float chosenSpeed = 1.25f;
            float chosenInterval = 1.15f;
            string waveTitle = "";
            Color titleColor = Color.white;

            // Xác định loại đợt quái (Archetype)
            if (currentWave % 5 == 0)
            {
                // ĐỢT ĐẠI BOSS TITAN (Mỗi 5 đợt)
                currentArchetype = MonsterArchetype.TitanBoss;
                chosenPrefab = (mossyGuardianPrefab != null) ? mossyGuardianPrefab : (crystalSpireGolemPrefab != null ? crystalSpireGolemPrefab : prismBatPrefab);
                baseHp = 220; // Cân bằng: Wave 5 Boss có ~316 HP, thử thách gay cấn nhưng hoàn toàn beatable
                chosenSpeed = 0.70f;
                chosenInterval = 1.35f;
                waveTitle = $"👑 ĐỢT #{currentWave}: ĐẠI HỘ VỆ TITAN CỔ ĐẠI (BOSS)!";
                titleColor = new Color(1f, 0.25f, 0.35f);

                TriggerCameraShake(0.35f, 0.12f);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayBossRoarSfx();
                OnBossWarningTriggered?.Invoke("ĐẠI HỘ VỆ TITAN CỔ ĐẠI", currentWave);
            }
            else if (currentWave % 7 == 0)
            {
                // ĐỢT QUÁI THẦN TÀI PHA LÊ (Mỗi 7 đợt)
                currentArchetype = MonsterArchetype.TreasureBat;
                chosenPrefab = (prismBatPrefab != null) ? prismBatPrefab : crystalSpireGolemPrefab;
                baseHp = 80;
                chosenSpeed = 1.35f;
                chosenInterval = 999f; // Không tấn công
                treasureEscapeTimer = 22f; // Bay đi mất sau 22s
                waveTitle = $"💎 ĐỢT #{currentWave}: DƠI THẦN TÀI PHA LÊ (RƠI x4 CRYSTAL)!";
                titleColor = new Color(1f, 0.85f, 0.15f);

                if (AudioManager.Instance != null) AudioManager.Instance.PlayTreasureChimeSfx();
            }
            else if (currentWave % 2 == 1)
            {
                // ĐỢT DƠI TỐC BIẾN (Prism Bat Swarm - Cơ động né đòn)
                currentArchetype = MonsterArchetype.FastBat;
                chosenPrefab = (prismBatPrefab != null) ? prismBatPrefab : (crystalSpireGolemPrefab != null ? crystalSpireGolemPrefab : mossyGuardianPrefab);
                baseHp = 42;
                chosenSpeed = 1.6f;
                chosenInterval = 0.85f;
                waveTitle = $"⚡ ĐỢT #{currentWave}: DƠI PHA LÊ TỐC BIẾN (CƠ ĐỘNG NÉ ĐÒN)!";
                titleColor = new Color(0.35f, 0.85f, 1f);
            }
            else
            {
                // ĐỢT THẠCH NHAM GIÁP SẮT (Armored Spire Golem - Kháng 25% sát thương thường)
                currentArchetype = MonsterArchetype.ArmoredGolem;
                chosenPrefab = (crystalSpireGolemPrefab != null) ? crystalSpireGolemPrefab : (prismBatPrefab != null ? prismBatPrefab : mossyGuardianPrefab);
                baseHp = 70;
                chosenSpeed = 0.80f;
                chosenInterval = 1.45f;
                waveTitle = $"🛡️ ĐỢT #{currentWave}: GOLEM GAI ĐÁ (GIẢM 25% ĐÒN THƯỜNG)!";
                titleColor = new Color(0.85f, 0.55f, 1f);
            }

            if (chosenPrefab == null) chosenPrefab = prismBatPrefab ?? crystalSpireGolemPrefab ?? mossyGuardianPrefab;
            if (chosenPrefab == null) return;

            currentActiveMonster = chosenPrefab;
            currentActiveMonster.gameObject.SetActive(true);

            monsterSpeed = chosenSpeed;
            monsterAttackInterval = chosenInterval;

            // CÔNG THỨC TOÁN HỌC MÁU QUÁI LŨY TIẾN MƯỢT MÀ THEO WAVE (Chuẩn Idle/Incremental RPG)
            // Lũy tiến mượt 9.5% mỗi wave: Wave 1: mult=1.0, Wave 5: mult=1.438, Wave 10: mult=2.26
            double waveScaling = Math.Pow(1.095, currentWave - 1);
            currentMonsterMaxHp = Mathf.Max(35, (int)(baseHp * waveScaling));
            currentMonsterHp = currentMonsterMaxHp;

            // Thiết lập Appearance & Scale
            if (currentArchetype == MonsterArchetype.TitanBoss)
            {
                isBossSiege = true;
                bossSiegeTimer = 45f;
                currentActiveMonster.SetScaleMultiplier(1.40f);
                currentActiveMonster.SetGoldenAppearance(false);
                currentActiveMonster.SetEnrageVisual(false);
            }
            else if (currentArchetype == MonsterArchetype.TreasureBat)
            {
                isBossSiege = false;
                currentActiveMonster.SetScaleMultiplier(1.0f);
                currentActiveMonster.SetGoldenAppearance(true);
                currentActiveMonster.SetEnrageVisual(false);
            }
            else
            {
                isBossSiege = false;
                currentActiveMonster.SetScaleMultiplier(1.0f);
                currentActiveMonster.SetGoldenAppearance(false);
                currentActiveMonster.SetEnrageVisual(false);
            }

            // Thanh máu quái vật
            currentMonsterHealthBar = currentActiveMonster.GetComponentInChildren<OverheadHealthBar3D>(true);
            if (currentMonsterHealthBar != null)
            {
                currentMonsterHealthBar.BarOwner = HealthBarOwner.Monster;
                string archetypeLabel = (currentArchetype == MonsterArchetype.TitanBoss) ? "👑 [ĐẠI BOSS] TITAN CỔ ĐẠI" :
                                        (currentArchetype == MonsterArchetype.TreasureBat) ? "💎 [THẦN TÀI] DƠI VÀNG" :
                                        (currentArchetype == MonsterArchetype.ArmoredGolem) ? "🛡️ [GIÁP SẮT] GOLEM GAI ĐÁ" : "⚡ [TỐC BIẾN] DƠI PHA LÊ";
                currentMonsterHealthBar.SetupBar(archetypeLabel, currentMonsterHp, currentMonsterMaxHp, titleColor);
            }

            // Vị trí xuất hiện ngẫu nhiên ở mép đảo
            float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float spawnX = coreCenterPos.x + Mathf.Cos(randomAngle) * islandRadius;
            float spawnZ = coreCenterPos.z + Mathf.Sin(randomAngle) * islandRadius;
            Vector3 spawnPos = new Vector3(spawnX, coreCenterPos.y, spawnZ);

            Vector3 faceDir = (coreCenterPos - spawnPos).normalized;
            Quaternion spawnRot = (faceDir != Vector3.zero) ? Quaternion.LookRotation(faceDir, Vector3.up) : Quaternion.identity;

            currentActiveMonster.SetBaseStance(spawnPos, spawnRot, chosenPrefab.transform.localScale);

            // Capybara sẵn sàng nghênh chiến
            if (heroUnit != null && !isHeroKnockedDown)
            {
                heroUnit.StopIdlePatrol();
                heroUnit.StopVictory();
                Vector3 heroInterceptPos = coreCenterPos + (spawnPos - coreCenterPos).normalized * 1.35f;
                heroUnit.PlayRun(heroUnit.transform.position, heroInterceptPos, 0.35f, () =>
                {
                    heroUnit.SetBaseStance(heroInterceptPos, Quaternion.LookRotation((spawnPos - heroInterceptPos).normalized, Vector3.up), heroUnit.transform.localScale);
                });
            }

            SetState(DefenseState.Approaching);
            OnWaveStarted?.Invoke(currentWave);
            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize(waveTitle));

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(spawnPos + Vector3.up * 1.5f, $"ĐỢT #{currentWave}!", titleColor);
            }
        }

        /// <summary>
        /// Người chơi Click / Tap trực tiếp lên Quái vật để tấn công
        /// </summary>
        public void HitMonsterByPlayer(Vector3 clickPoint)
        {
            if (!IsMonsterActive) return;

            // Tích Combo & Nộ
            RegisterComboTap();
            AddRunicEnergy(isOverdriveActive ? 2.5f : 6.0f);

            // Cơ chế Parry / Chống đỡ Dậm Đất của Boss Titan
            if (isBossChargingStomp)
            {
                stompParryTapCount++;
                if (stompParryTapCount == ParryTapThreshold && FloatingTextSpawner.Instance != null && currentActiveMonster != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 2.4f, "🛡️ CHỐNG ĐỠ KÍCH HOẠT! -50% SÁT THƯƠNG!", new Color(0.25f, 1f, 0.45f));
                    if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradeSfx();
                }
            }

            double tapPower = ResourceManager.Instance != null ? ResourceManager.Instance.TapPower : 1.0;
            int bonusAtk = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusAtk : 0;
            int bonusSkillDmg = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillDamage : 0;
            float bonusCrit = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillCritRate : 0f;

            // Trong Overdrive: 100% Chí Mạng + Sát thương x3!
            bool isCrit = isOverdriveActive || (UnityEngine.Random.value < (0.22f + bonusCrit));
            float critMult = isOverdriveActive ? 3.0f : (isCrit ? 2.2f : 1.0f);

            // Dơi Pha Lê có 12% cơ hội né đòn đánh thường (Đòn Chí Mạng & Overdrive KHÔNG THỂ BỊ NÉ)
            if (currentArchetype == MonsterArchetype.FastBat && !isCrit && !isOverdriveActive && UnityEngine.Random.value < 0.12f)
            {
                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(clickPoint + Vector3.up * 0.8f, "NÉ TRÁNH! 💨", new Color(0.45f, 0.85f, 1f));
                }
                return;
            }

            int baseHit = (int)(tapPower + bonusAtk);
            int rawDamage = Mathf.Max(1, (int)(baseHit * critMult) + bonusSkillDmg);

            // Golem Giáp Sắt kháng 25% sát thương thường (trừ khi là đòn chí mạng hoặc Overdrive)
            if (currentArchetype == MonsterArchetype.ArmoredGolem && !isCrit && !isOverdriveActive)
            {
                rawDamage = Mathf.Max(1, (int)(rawDamage * 0.75f));
            }

            // Bonus sát thương nhẹ theo Combo
            if (currentComboCount >= 10) rawDamage = (int)(rawDamage * 1.15f);
            if (currentComboCount >= 25) rawDamage = (int)(rawDamage * 1.30f);

            ApplyDamageToMonster(rawDamage, isCrit, clickPoint);

            // Capybara vung búa đồng nhịp
            if (heroUnit != null && !isHeroKnockedDown && !heroUnit.IsBusy)
            {
                heroUnit.PlayImmediateStrikeTowards(currentActiveMonster.transform.position, isCrit);
            }
        }

        private void RegisterComboTap()
        {
            currentComboCount++;
            comboResetTimer = ComboTimeout;
            OnComboUpdated?.Invoke(currentComboCount);

            if (currentComboCount == 10 && FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 1.8f, "COMBO x10! ⚡", new Color(0.3f, 0.9f, 1f));
            }
            else if (currentComboCount == 25 && FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 2.0f, "SIÊU TỐC x25! 🔥", new Color(1f, 0.6f, 0.1f));
            }
        }

        /// <summary>
        /// Tích lũy điểm Nộ Cổ Ngữ (Runic Overcharge)
        /// </summary>
        public void AddRunicEnergy(float amount)
        {
            if (isOverdriveActive) return;

            currentRunicEnergy = Mathf.Clamp(currentRunicEnergy + amount, 0f, maxRunicEnergy);
            OnOverchargeChanged?.Invoke(currentRunicEnergy, maxRunicEnergy, isOverdriveActive, 0f);

            if (currentRunicEnergy >= maxRunicEnergy)
            {
                TriggerRunicOverdrive();
            }
        }

        /// <summary>
        /// Kích hoạt Siêu Kỹ Năng: RUNIC OVERDRIVE (Bão Tinh Thể Cuồng Nộ)
        /// </summary>
        public void TriggerRunicOverdrive()
        {
            if (isOverdriveActive) return;
            isOverdriveActive = true;
            overdriveRemainingTimer = overdriveDuration;

            if (heroUnit != null)
            {
                heroUnit.SetOverdriveVisual(true);
                heroUnit.PlayOverdriveRoar();
            }

            TriggerCameraShake(0.4f, 0.14f);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOverdriveActivatedSfx();
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayShockwave(coreCenterPos);
                VFXManager.Instance.PlayMilestoneBurst();
            }

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(coreCenterPos + Vector3.up * 2.1f, "⚡ RUNIC OVERDRIVE KÍCH HOẠT! SÁT THƯƠNG x3! ⚡", new Color(1f, 0.85f, 0.2f));
            }

            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize("[OVERDRIVE] CUONG NO CO NGU! SAT THUONG x3 & TOC DANH GAP DOI!"));
            OnOverchargeChanged?.Invoke(currentRunicEnergy, maxRunicEnergy, isOverdriveActive, overdriveRemainingTimer);
        }

        private void EndOverdrive()
        {
            isOverdriveActive = false;
            currentRunicEnergy = 0f;
            overdriveRemainingTimer = 0f;

            if (heroUnit != null)
            {
                heroUnit.SetOverdriveVisual(false);
            }

            OnOverchargeChanged?.Invoke(currentRunicEnergy, maxRunicEnergy, isOverdriveActive, 0f);
        }

        /// <summary>
        /// Capybara tự động vung búa đánh quái hỗ trợ
        /// </summary>
        private void ExecuteHeroAutoAttack()
        {
            if (!IsMonsterActive || heroUnit == null || isHeroKnockedDown) return;

            Vector3 monsterPos = currentActiveMonster.transform.position;

            // Trong trạng thái Cuồng nộ Overdrive: Dũng sĩ có 40% cơ hội tung tuyệt kỹ Xoay Lốc Xoáy chém bão
            if (isOverdriveActive && UnityEngine.Random.value < 0.4f)
            {
                heroUnit.PlayWhirlwindSpin(() =>
                {
                    if (!IsMonsterActive) return;

                    double tapPower = ResourceManager.Instance != null ? ResourceManager.Instance.TapPower : 1.0;
                    int bonusAtk = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusAtk : 0;
                    int bonusSkillDmg = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillDamage : 0;
                    int heroDamage = Mathf.Max(1, (int)((tapPower * 0.85f + bonusAtk) * 3.0f) + bonusSkillDmg);

                    ApplyDamageToMonster(heroDamage, true, monsterPos);
                });
                return;
            }

            heroUnit.PlayCapybaraAttack(monsterPos, onImpact: () =>
            {
                if (!IsMonsterActive) return;

                AddRunicEnergy(4.0f);

                double tapPower = ResourceManager.Instance != null ? ResourceManager.Instance.TapPower : 1.0;
                int bonusAtk = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusAtk : 0;
                int bonusSkillDmg = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillDamage : 0;
                float bonusCrit = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSkillCritRate : 0f;

                bool isCrit = isOverdriveActive || (UnityEngine.Random.value < (0.15f + bonusCrit));
                float critMult = isOverdriveActive ? 2.6f : (isCrit ? 2.0f : 1.0f);

                int baseHeroAtk = Mathf.Max(1, (int)(tapPower * 0.85f) + bonusAtk);
                int heroDamage = Mathf.Max(1, (int)(baseHeroAtk * critMult) + bonusSkillDmg);

                if (currentArchetype == MonsterArchetype.ArmoredGolem && !isCrit && !isOverdriveActive)
                {
                    heroDamage = Mathf.Max(1, (int)(heroDamage * 0.75f));
                }

                ApplyDamageToMonster(heroDamage, isCrit, monsterPos);
            });
        }

        private void ApplyDamageToMonster(int damage, bool isCrit, Vector3 impactPos)
        {
            if (!IsMonsterActive) return;

            currentMonsterHp = Mathf.Max(0, currentMonsterHp - damage);
            OnMonsterHpChanged?.Invoke(currentMonsterHp, currentMonsterMaxHp);

            if (currentMonsterHealthBar != null)
            {
                currentMonsterHealthBar.HandleHpUpdated(currentMonsterHp, currentMonsterMaxHp);
            }

            currentActiveMonster.PlayTakeHit(heroUnit != null ? heroUnit.transform.position : coreCenterPos, isCrit);

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnDamageText(impactPos + Vector3.up * 0.9f, damage, isCrit);
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayClickSpark(impactPos);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCombatAttackSfx();
            }

            if (isCrit)
            {
                TriggerCameraShake(0.12f, 0.04f);
            }

            // Boss Titan kích hoạt Trạng thái Cuồng Nộ (Enrage) khi máu dưới 35%
            if (currentArchetype == MonsterArchetype.TitanBoss && !isBossEnraged && currentMonsterHp > 0 && ((float)currentMonsterHp / currentMonsterMaxHp <= 0.35f))
            {
                isBossEnraged = true;
                currentActiveMonster.SetEnrageVisual(true);
                monsterSpeed *= 1.25f;
                monsterAttackInterval *= 0.82f;
                TriggerCameraShake(0.35f, 0.14f);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayBossRoarSfx();
                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 2.2f, "🔥 BOSS CUỒNG NỘ! TỐC ĐỘ GIA TĂNG! 🔥", new Color(1f, 0.25f, 0.2f));
                }
                OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize("[CUONG NO] BOSS TITAN BUOC VAO GIAI DOAN PHAN NO!"));
            }

            if (currentMonsterHp <= 0)
            {
                HandleMonsterDefeated();
            }
        }

        /// <summary>
        /// Quái vật thực hiện hành vi tấn công
        /// </summary>
        private void ExecuteMonsterAttackAction()
        {
            if (!IsMonsterActive || isBossChargingStomp) return;

            // Kỹ năng Boss Titan: Tụ Lực Dậm Đất Chấn Động sau mỗi 3 lượt đánh
            if (currentArchetype == MonsterArchetype.TitanBoss)
            {
                bossAttackCounter++;
                if (bossAttackCounter >= 3)
                {
                    bossAttackCounter = 0;
                    ExecuteTitanEarthQuake();
                    return;
                }
            }

            bool canAttackHero = (heroUnit != null && !isHeroKnockedDown && Vector3.Distance(currentActiveMonster.transform.position, heroUnit.transform.position) <= 2.2f);
            bool attackHeroThisTime = canAttackHero && (UnityEngine.Random.value < 0.65f);

            if (attackHeroThisTime)
            {
                ExecuteMonsterAttackHero();
            }
            else
            {
                ExecuteMonsterAttackCore();
            }
        }

        /// <summary>
        /// Kỹ năng đặc biệt của Boss: Tụ Lực & Dậm Đất Titan (Titan Earth-Quake Stomp với Telegraph Cảnh Báo)
        /// </summary>
        private void ExecuteTitanEarthQuake()
        {
            if (!IsMonsterActive) return;

            isBossChargingStomp = true;
            stompParryTapCount = 0;

            // 1. Telegraph cảnh báo 0.9 giây: Chữ nổi đỏ, Boss rung lắc tụ lực
            if (AudioManager.Instance != null) AudioManager.Instance.PlayBossRoarSfx();

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 2.2f, "⚠️ BOSS ĐANG TỤ LỰC DẬM ĐẤT! TAP ĐỂ CHỐNG ĐỠ!", new Color(1f, 0.35f, 0.15f));
            }

            currentActiveMonster.PlayTelegraphWindup(0.9f, () =>
            {
                if (!IsMonsterActive || isGameOver)
                {
                    isBossChargingStomp = false;
                    return;
                }

                // 2. Kiểm tra người chơi có chống đỡ thành công (Parry) không
                bool isParried = isOverdriveActive || (stompParryTapCount >= ParryTapThreshold);
                isBossChargingStomp = false;

                currentActiveMonster.PlayBossEarthQuakeStomp(onImpact: () =>
                {
                    if (!IsMonsterActive || isGameOver) return;

                    TriggerCameraShake(0.45f, 0.16f);

                    if (VFXManager.Instance != null)
                    {
                        VFXManager.Instance.PlayShockwave(currentActiveMonster.transform.position);
                    }

                    // Sát thương dậm đất được cân bằng lại (Wave 5: ~13 dmg thường, nếu Parry chỉ còn ~6 dmg)
                    int baseQuake = Mathf.Max(8, Mathf.RoundToInt((baseMonsterDamage + (currentWave - 1) * 0.75f) * 1.8f));
                    int quakeDamage = isParried ? Mathf.Max(4, baseQuake / 2) : baseQuake;

                    if (heroUnit != null && !isHeroKnockedDown)
                    {
                        heroCurrentHp = Mathf.Max(0, heroCurrentHp - quakeDamage);
                        OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);
                        if (heroHealthBar != null) heroHealthBar.HandleHpUpdated(heroCurrentHp, heroMaxHp);
                        heroUnit.PlayTakeHit(currentActiveMonster.transform.position, true);
                        if (heroCurrentHp <= 0) TriggerHeroDeathGameOver();
                    }

                    coreCurrentShield = Mathf.Max(0, coreCurrentShield - quakeDamage);
                    OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
                    PunchCoreFeedback();

                    if (FloatingTextSpawner.Instance != null)
                    {
                        if (isParried)
                        {
                            FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 1.8f, $"🛡️ CHỐNG ĐỠ THÀNH CÔNG! -{quakeDamage} HP (-50%)", new Color(0.25f, 1f, 0.5f));
                        }
                        else
                        {
                            FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 1.8f, $"TITAN EARTH-QUAKE! -{quakeDamage} HP", new Color(1f, 0.25f, 0.2f));
                        }
                    }

                    if (coreCurrentShield <= 0)
                    {
                        TriggerCoreDestroyedGameOver();
                    }
                }, onComplete: null);
            });
        }

        private void ExecuteMonsterAttackHero()
        {
            if (!IsMonsterActive || heroUnit == null || isHeroKnockedDown) return;

            Vector3 heroTargetPos = heroUnit.transform.position;

            currentActiveMonster.PlayCapybaraAttack(heroTargetPos, onImpact: () =>
            {
                if (!IsMonsterActive || heroUnit == null || isHeroKnockedDown) return;

                float dmgMult = (currentArchetype == MonsterArchetype.FastBat) ? 0.70f : ((currentArchetype == MonsterArchetype.TitanBoss) ? 1.25f : 1.0f);
                int damageToHero = Mathf.Max(2, Mathf.RoundToInt((baseMonsterDamage + (currentWave - 1) * 0.5f) * dmgMult));
                heroCurrentHp = Mathf.Max(0, heroCurrentHp - damageToHero);
                OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);

                if (heroHealthBar != null)
                {
                    heroHealthBar.HandleHpUpdated(heroCurrentHp, heroMaxHp);
                }

                heroUnit.PlayTakeHit(currentActiveMonster.transform.position, false);

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnDamageText(heroUnit.transform.position + Vector3.up * 1.1f, damageToHero, false);
                }

                if (VFXManager.Instance != null)
                {
                    VFXManager.Instance.PlayClickSpark(heroUnit.transform.position);
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCombatAttackSfx();
                }

                if (heroCurrentHp <= 0)
                {
                    TriggerHeroDeathGameOver();
                }
            });
        }

        public void DamageCore(int damage)
        {
            if (isGameOver) return;
            coreCurrentShield = Mathf.Max(0, coreCurrentShield - damage);
            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
            PunchCoreFeedback();
            TriggerCameraShake(0.18f, 0.05f);

            if (coreCurrentShield <= 0)
            {
                TriggerCoreDestroyedGameOver();
            }
        }

        private void ExecuteMonsterAttackCore()
        {
            if (!IsMonsterActive || crystalCoreTransform == null || isGameOver) return;

            Vector3 coreTarget = crystalCoreTransform.position;

            currentActiveMonster.PlayCapybaraAttack(coreTarget, onImpact: () =>
            {
                if (!IsMonsterActive || isGameOver) return;

                // Golem Thạch Nham nện thêm 35% sát thương lên Trụ Tinh Thể
                float coreDmgMult = (currentArchetype == MonsterArchetype.ArmoredGolem) ? 1.35f : 1.0f;
                int damageToCore = Mathf.Max(3, Mathf.RoundToInt((baseMonsterDamage + (currentWave - 1) * 0.55f) * coreDmgMult));

                if (VFXManager.Instance != null)
                {
                    VFXManager.Instance.PlayClickSpark(crystalCoreTransform.position + Vector3.down * 0.5f);
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayCombatAttackSfx();
                }

                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnFloatingText(crystalCoreTransform.position + Vector3.up * 1.2f, $"-{damageToCore} KHIÊN", new Color(1f, 0.35f, 0.3f));
                }

                DamageCore(damageToCore);
            });
        }

        private void TriggerHeroDeathGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            isHeroKnockedDown = true;

            SetState(DefenseState.Defeat);

            if (heroUnit != null)
            {
                heroUnit.StopIdlePatrol();
                heroUnit.PlayDeath();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverSfx();
            }

            if (FloatingTextSpawner.Instance != null && heroUnit != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(heroUnit.transform.position + Vector3.up * 1.4f, "NGƯỜI BẢO VỆ ĐÃ NGÃ XUỐNG!", new Color(1f, 0.2f, 0.2f));
            }

            string defeatMsg = $"Dũng Sĩ Hộ Vệ đã ngã xuống ở Đợt #{currentWave}!";
            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[THAT THU] DUNG SI DA NGA XUONG O DOT #{currentWave}!"));
            OnGameOverTriggered?.Invoke(currentWave, defeatMsg);

            if (EndGamePanelUI.Instance != null)
            {
                EndGamePanelUI.Instance.Show(currentWave, defeatMsg);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenEndGameModal(currentWave, defeatMsg);
            }
        }

        private void TriggerCoreDestroyedGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            isHeroKnockedDown = true;
            coreCurrentShield = 0;
            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);

            SetState(DefenseState.Defeat);

            if (heroUnit != null)
            {
                heroUnit.StopIdlePatrol();
                heroUnit.PlayTakeHit(coreCenterPos, true);
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayShockwave(coreCenterPos);
                VFXManager.Instance.PlayMilestoneBurst();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverSfx();
            }

            PunchCoreFeedback();
            TriggerCameraShake(0.6f, 0.22f);

            if (FloatingTextSpawner.Instance != null && crystalCoreTransform != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(crystalCoreTransform.position + Vector3.up * 1.5f, "TRỤ TINH THỂ ĐÃ BỊ PHÁ HỦY!", new Color(1f, 0.2f, 0.2f));
            }

            string defeatMsg = $"Trụ Tinh Thể Cổ Ngữ đã bị quái vật phá hủy hoàn toàn ở Đợt #{currentWave}!";
            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[THAT THU] TRU TINH THE DA SUP DO O DOT #{currentWave}!"));
            OnGameOverTriggered?.Invoke(currentWave, defeatMsg);

            if (EndGamePanelUI.Instance != null)
            {
                EndGamePanelUI.Instance.Show(currentWave, defeatMsg);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenEndGameModal(currentWave, defeatMsg);
            }
        }

        private void TriggerBossEnrageGameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            isHeroKnockedDown = true;

            SetState(DefenseState.Defeat);

            if (currentActiveMonster != null)
            {
                currentActiveMonster.PlayBossEarthQuakeStomp(null, null);
            }

            if (heroUnit != null)
            {
                heroUnit.StopIdlePatrol();
                heroUnit.PlayDeath();
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBossRoarSfx();
                AudioManager.Instance.PlayGameOverSfx();
            }

            PunchCoreFeedback();
            TriggerCameraShake(0.7f, 0.28f);

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(coreCenterPos + Vector3.up * 1.8f, "HẾT GIỜ! BOSS TITAN CUỒNG NỘ HỦY DIỆT!", new Color(1f, 0.25f, 0.2f));
            }

            string defeatMsg = $"Hết thời gian khiêu chiến! Boss Titan cuồng nộ đã hủy diệt toàn bộ hòn đảo ở Đợt #{currentWave}!";
            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[HET GIO] BOSS TITAN CUONG NO DA HUY DIET HON DAO O DOT #{currentWave}!"));
            OnGameOverTriggered?.Invoke(currentWave, defeatMsg);

            if (EndGamePanelUI.Instance != null)
            {
                EndGamePanelUI.Instance.Show(currentWave, defeatMsg);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenEndGameModal(currentWave, defeatMsg);
            }
        }

        public void RestartDefense()
        {
            StopAllCoroutines();
            HideAllMonsters();
            currentActiveMonster = null;
            EndOverdrive();
            isBossSiege = false;
            bossSiegeTimer = 45f;
            isBossChargingStomp = false;
            isBossEnraged = false;
            stompParryTapCount = 0;
            lastBossTimerAlert = 99f;
            isGameOver = false;
            isHeroKnockedDown = false;

            heroCurrentHp = heroMaxHp;
            isHeroKnockedDown = false;
            OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);
            if (heroHealthBar != null)
            {
                heroHealthBar.HandleHpUpdated(heroCurrentHp, heroMaxHp);
            }

            if (heroUnit != null)
            {
                heroUnit.gameObject.SetActive(true);
                heroUnit.SetBaseStance(new Vector3(-1.6f, 0.0f, -1.4f), Quaternion.Euler(0f, 155f, 0f), Vector3.one * 1.15f);

                float speedBonus = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusSpeedPercent : 0f;
                float patrolSpeed = 0.75f * (1f + speedBonus / 100f);
                heroUnit.StartIdlePatrol(coreCenterPos, 2.15f, patrolSpeed);
            }

            coreCurrentShield = coreMaxShield;
            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);

            currentWave = 1;
            OnWaveStarted?.Invoke(currentWave);

            SetState(DefenseState.PeacefulIntermission);
            StartCoroutine(InitialSpawnRoutine());
        }

        private void TriggerCoreShockwaveRepel()
        {
            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayShockwave(coreCenterPos);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySpellSfx();
            }

            TriggerCameraShake(0.4f, 0.12f);

            // Đẩy lùi quái ra xa 3.5m và gây choáng
            if (currentActiveMonster != null)
            {
                Vector3 pushDir = (currentActiveMonster.transform.position - coreCenterPos).normalized;
                Vector3 pushedPos = coreCenterPos + pushDir * 3.5f;
                pushedPos.y = coreCenterPos.y;
                currentActiveMonster.PlayRun(currentActiveMonster.transform.position, pushedPos, 0.35f);
                // Giảm 20% máu quái hiện tại
                currentMonsterHp = Mathf.Max(1, (int)(currentMonsterHp * 0.8f));
                OnMonsterHpChanged?.Invoke(currentMonsterHp, currentMonsterMaxHp);
            }

            // Hồi 50% Khiên và phạt lùi 1 Wave
            int oldWave = currentWave;
            currentWave = Mathf.Max(1, currentWave - 1);
            coreCurrentShield = Mathf.CeilToInt(coreMaxShield * 0.5f);
            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
            OnWaveStarted?.Invoke(currentWave);

            if (FloatingTextSpawner.Instance != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(crystalCoreTransform.position + Vector3.up * 1.6f, $"TRỤ VỠ KHIÊN! LÙI TIẾN TRÌNH VỀ ĐỢT #{currentWave}!", new Color(1f, 0.4f, 0.1f));
            }

            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[CANH BAO] KHIEN TRU VO! TIEN TRINH LUI VE DOT #{currentWave}!"));
            SetState(DefenseState.Approaching);
        }

        private void PunchCoreFeedback()
        {
            if (crystalCoreTransform == null) return;
            if (coreShakeCoroutine != null) StopCoroutine(coreShakeCoroutine);
            coreShakeCoroutine = StartCoroutine(CoreShakeRoutine());
        }

        private IEnumerator CoreShakeRoutine()
        {
            float elapsed = 0f;
            float duration = 0.25f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration);
                float shakeX = Mathf.Sin(elapsed * 45f) * 0.08f * decay;
                crystalCoreTransform.localPosition = originalCoreLocalPos + new Vector3(shakeX, 0f, 0f);
                yield return null;
            }
            crystalCoreTransform.localPosition = originalCoreLocalPos;
            coreShakeCoroutine = null;
        }

        public void TriggerCameraShake(float duration = 0.2f, float magnitude = 0.08f)
        {
            if (mainCamera == null) return;
            if (camShakeCoroutine != null) StopCoroutine(camShakeCoroutine);
            camShakeCoroutine = StartCoroutine(CameraShakeRoutine(duration, magnitude));
        }

        private IEnumerator CameraShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration);
                float rx = (UnityEngine.Random.value * 2f - 1f) * magnitude * decay;
                float ry = (UnityEngine.Random.value * 2f - 1f) * magnitude * decay;
                mainCamera.transform.position = originalCamPos + new Vector3(rx, ry, 0f);
                yield return null;
            }
            mainCamera.transform.position = originalCamPos;
            camShakeCoroutine = null;
        }

        /// <summary>
        /// Xử lý khi Quái vật bị tiêu diệt
        /// </summary>
        private void HandleMonsterDefeated()
        {
            SetState(DefenseState.PeacefulIntermission);
            isBossSiege = false;

            if (currentActiveMonster != null)
            {
                currentActiveMonster.PlayDeath();
            }

            if (heroUnit != null && !isHeroKnockedDown)
            {
                heroUnit.StopIdlePatrol();
                heroUnit.PlayVictory();
            }

            // Thưởng Crystal & Buff Lò Rèn
            double tapPower = ResourceManager.Instance != null ? ResourceManager.Instance.TapPower : 1.0;
            double prodRate = ResourceManager.Instance != null ? ResourceManager.Instance.EffectiveProductionPerSec : 1.0;

            double baseReward = Math.Max(80.0, (50.0 * currentWave + tapPower * 8.0 + prodRate * 12.0) * Math.Pow(1.06, currentWave - 1));
            float rewardMult = (currentArchetype == MonsterArchetype.TreasureBat) ? 4.0f :
                               (currentArchetype == MonsterArchetype.TitanBoss) ? 3.0f : 1.0f;
            double finalReward = baseReward * rewardMult;

            float buffMult = (currentArchetype == MonsterArchetype.TreasureBat) ? 3.0f : 2.0f;
            float buffTime = (currentArchetype == MonsterArchetype.TreasureBat) ? 20.0f : 14.0f;

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.AddDirectBonus(finalReward);
                ResourceManager.Instance.ApplyTemporaryBuff(buffMult, buffTime);
            }

            // Hồi 100% Khiên Trụ và Máu Capybara
            coreCurrentShield = coreMaxShield;
            heroCurrentHp = heroMaxHp;
            OnCoreShieldChanged?.Invoke(coreCurrentShield, coreMaxShield);
            OnHeroHpChanged?.Invoke(heroCurrentHp, heroMaxHp);

            if (heroHealthBar != null)
            {
                heroHealthBar.HandleHpUpdated(heroCurrentHp, heroMaxHp);
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayMilestoneBurst();
            }

            // Bắn tung tóe mưa ngọc chiến lợi phẩm bay về TopBar HUD
            if (LootGemSpawner.Instance != null && currentActiveMonster != null)
            {
                bool isSpecial = (currentArchetype == MonsterArchetype.TreasureBat || currentArchetype == MonsterArchetype.TitanBoss);
                int gemCount = isSpecial ? 15 : 8;
                LootGemSpawner.Instance.SpawnLootBurst(currentActiveMonster.transform.position, gemCount, finalReward, isSpecial);
            }

            if (AudioManager.Instance != null)
            {
                if (currentArchetype == MonsterArchetype.TreasureBat)
                {
                    AudioManager.Instance.PlayTreasureChimeSfx();
                }
                else
                {
                    AudioManager.Instance.PlayVictorySfx();
                }
            }

            string rewardMsg = $"+{CrystalFormat.Format(finalReward)} Crystal & BUFF x{buffMult:F1}!";
            if (FloatingTextSpawner.Instance != null && currentActiveMonster != null)
            {
                Color rwColor = (currentArchetype == MonsterArchetype.TreasureBat) ? new Color(1f, 0.9f, 0.1f) : new Color(1f, 0.85f, 0.2f);
                FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 1.5f, rewardMsg, rwColor);
            }

            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[VICTORY] HA GUC DOT #{currentWave}! {rewardMsg}"));

            currentWave++;
            StartCoroutine(NextWaveContinuousRoutine());
        }

        /// <summary>
        /// Xử lý khi Dơi Thần Tài bay thoát khỏi đảo sau thời hạn
        /// </summary>
        private void HandleTreasureEscaped()
        {
            SetState(DefenseState.PeacefulIntermission);
            isBossSiege = false;

            if (currentActiveMonster != null)
            {
                currentActiveMonster.PlayDeath();
            }

            if (FloatingTextSpawner.Instance != null && currentActiveMonster != null)
            {
                FloatingTextSpawner.Instance.SpawnFloatingText(currentActiveMonster.transform.position + Vector3.up * 1.5f, "💨 DƠI THẦN TÀI ĐÃ BAY ĐI MẤT!", new Color(1f, 0.85f, 0.3f));
            }

            OnSiegeAlertMessage?.Invoke(FontSanitizer.Sanitize($"[THOAT HIEM] DOI THAN TAI DA BAY MAT O DOT #{currentWave}!"));

            currentWave++;
            StartCoroutine(NextWaveContinuousRoutine());
        }

        private IEnumerator NextWaveContinuousRoutine()
        {
            yield return new WaitForSeconds(intermissionDuration);

            if (heroUnit != null && !isHeroKnockedDown)
            {
                heroUnit.StopVictory();
            }

            HideAllMonsters();
            currentActiveMonster = null;

            SpawnNextMonsterWave();
        }

        private void SetState(DefenseState newState)
        {
            currentState = newState;
            OnDefenseStateChanged?.Invoke(currentState);
        }

        public bool IsActiveMonsterTransform(Transform targetTransform)
        {
            if (!IsMonsterActive || targetTransform == null) return false;
            return targetTransform == currentActiveMonster.transform || targetTransform.IsChildOf(currentActiveMonster.transform);
        }
    }
}
