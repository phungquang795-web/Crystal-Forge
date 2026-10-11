#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Adventure;
using CrystalForge.Core;
using CrystalForge.Data;
using CrystalForge.UI;

namespace CrystalForge.Editor
{
    public static class AdventureSetupTool
    {
        [MenuItem("CrystalForge/3. Setup Adventure System & Modal")]
        public static void BuildAdventureSystemAndModal()
        {
            Undo.SetCurrentGroupName("Setup Adventure System & Modal");
            int undoGroup = Undo.GetCurrentGroup();

            EnsureFolders();

            // 1. Tạo hoặc tải các ScriptableObjects dữ liệu
            var enemies = CreateOrLoadEnemies();
            var skills = CreateOrLoadSkills();
            var events = CreateOrLoadEvents();

            // 2. Gắn AdventureManager vào _Managers
            var advManager = SetupAdventureManager(enemies, skills, events);

            // 3. Dựng AdventureModal UI trên MainCanvas
            var modalUI = BuildAdventureModalUI();

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Đã dựng hoàn tất Hệ thống Thám hiểm Adventure & Giao diện Modal Single-Screen!</color>");
        }

        private static void EnsureFolders()
        {
            string[] dirs = {
                "Assets/Data/Enemies",
                "Assets/Data/Skills",
                "Assets/Data/Events"
            };

            foreach (var d in dirs)
            {
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                    AssetDatabase.Refresh();
                }
            }
        }

        private static EnemyDataSO[] CreateOrLoadEnemies()
        {
            var slime = GetOrCreateEnemy("CrystalSlime", "crystal_slime", "Crystal Slime", "Thạch pha lê mềm mại thường lảng vảng quanh vỉa quặng tím.", 300, 25, 5, 80.0, 1.0f, 0f, false);
            var golem = GetOrCreateEnemy("StoneGolem", "stone_golem", "Stone Golem", "Người đá cổ đại với lớp giáp đá sa thạch kiên cố.", 850, 55, 18, 300.0, 1.2f, 25f, false);
            var shadow = GetOrCreateEnemy("ShadowMinion", "shadow_minion", "Shadow Minion", "Bóng ma mỏ quặng thoắt ẩn thoắt hiện với tốc độ đánh nhanh.", 1700, 110, 15, 1000.0, 1.5f, 35f, false);
            var boss = GetOrCreateEnemy("CrystalGuardianBoss", "guardian_boss", "Crystal Guardian", "Hộ vệ cổ đại khổng lồ của Lò Rèn, khoác bộ giáp tinh thể gai nhọn phát sáng.", 4800, 220, 30, 5000.0, 2.0f, 60f, true);

            return new EnemyDataSO[] { slime, golem, shadow, boss };
        }

        private static EnemyDataSO GetOrCreateEnemy(string assetName, string id, string name, string lore, int hp, int atk, int def, double reward, float buffMult, float buffDuration, bool isBoss)
        {
            string path = $"Assets/Data/Enemies/{assetName}.asset";
            var enemy = AssetDatabase.LoadAssetAtPath<EnemyDataSO>(path);
            if (enemy == null)
            {
                enemy = ScriptableObject.CreateInstance<EnemyDataSO>();
                enemy.enemyId = id;
                enemy.enemyName = name;
                enemy.loreDescription = lore;
                enemy.maxHp = hp;
                enemy.attackPower = atk;
                enemy.defense = def;
                enemy.crystalReward = reward;
                enemy.productionBuffMultiplier = buffMult;
                enemy.buffDurationSeconds = buffDuration;
                enemy.isBoss = isBoss;

                AssetDatabase.CreateAsset(enemy, path);
            }
            return enemy;
        }

        private static SkillDataSO[] CreateOrLoadSkills()
        {
            var fireball = GetOrCreateSkill("Fireball", "fireball", "🔥 Fireball", SkillType.Attack, "Bộc phát năng lượng lửa ma thuật, gây thêm +60 sát thương sau mỗi 3 lượt đánh.", 60, 3, 0f, 0f, 0f, 0f);
            var lightning = GetOrCreateSkill("Lightning", "lightning", "⚡ Lightning Strike", SkillType.Attack, "Triệu hồi tia sét trừng phạt, có 30% tỉ lệ kích hoạt thêm +45 sát thương mỗi đòn đánh.", 45, 0, 30f, 0f, 0f, 0f);
            var lifesteal = GetOrCreateSkill("VampiricCore", "lifesteal", "🩸 Vampiric Core", SkillType.Support, "Cộng hưởng máu quái vật, hồi phục máu bằng 20% sát thương gây ra.", 0, 0, 0f, 20f, 0f, 0f);
            var shield = GetOrCreateSkill("CrystalAegis", "guardian_shield", "🛡️ Crystal Aegis", SkillType.Defense, "Lớp giáp tinh thể hộ thể, giảm 25% mọi sát thương nhận vào từ kẻ thù.", 0, 0, 0f, 0f, 25f, 0f);
            var crit = GetOrCreateSkill("PrecisionFocus", "critical_core", "🎯 Precision Focus", SkillType.Passive, "Nhắm chuẩn điểm yếu đối phương, tăng thêm +25% tỉ lệ đòn đánh chí mạng (x2 sát thương).", 0, 0, 0f, 0f, 0f, 25f);

            return new SkillDataSO[] { fireball, lightning, lifesteal, shield, crit };
        }

        private static SkillDataSO GetOrCreateSkill(string assetName, string id, string name, SkillType type, string desc, int flatDmg, int interval, float procChance, float lifesteal, float dmgReduction, float critBonus)
        {
            string path = $"Assets/Data/Skills/{assetName}.asset";
            var skill = AssetDatabase.LoadAssetAtPath<SkillDataSO>(path);
            if (skill == null)
            {
                skill = ScriptableObject.CreateInstance<SkillDataSO>();
                skill.skillId = id;
                skill.skillName = name;
                skill.skillType = type;
                skill.description = desc;
                skill.flatBonusDamage = flatDmg;
                skill.triggerIntervalTurns = interval;
                skill.procChancePercent = procChance;
                skill.lifestealPercent = lifesteal;
                skill.damageReductionPercent = dmgReduction;
                skill.critRateBonusPercent = critBonus;

                AssetDatabase.CreateAsset(skill, path);
            }
            return skill;
        }

        private static EventDataSO[] CreateOrLoadEvents()
        {
            var ancientCrystal = GetOrCreateAncientCrystalEvent();
            var merchant = GetOrCreateMerchantEvent();
            return new EventDataSO[] { ancientCrystal, merchant };
        }

        private static EventDataSO GetOrCreateAncientCrystalEvent()
        {
            string path = "Assets/Data/Events/AncientCrystalEvent.asset";
            var evt = AssetDatabase.LoadAssetAtPath<EventDataSO>(path);
            if (evt == null)
            {
                evt = ScriptableObject.CreateInstance<EventDataSO>();
                evt.eventId = "ancient_crystal";
                evt.eventTitle = "Khối Tinh Thể Cổ Đại";
                evt.storyPrompt = "Bạn bắt gặp một khối ngọc khổng lồ tỏa sáng huyền ảo trong hang mỏ. Năng lượng cổ xưa đang vẫy gọi bạn...";

                var choice1 = new EventChoice
                {
                    choiceButtonText = "Chạm vào lõi ngọc (-10% HP)",
                    outcomeDescription = "Năng lượng bùng nổ truyền thẳng về Lò Rèn!",
                    crystalChange = 250.0,
                    heroHpPercentChange = -10,
                    temporaryProductionBuff = 1.5f,
                    buffDurationSeconds = 40f
                };

                var choice2 = new EventChoice
                {
                    choiceButtonText = "Khấn nguyện tĩnh tâm (+35% HP)",
                    outcomeDescription = "Làn sương ấm áp chữa lành vết thương và ban phước kỹ năng ngẫu nhiên.",
                    crystalChange = 50.0,
                    heroHpPercentChange = 35,
                    grantsRandomSkill = true
                };

                evt.choices = new EventChoice[] { choice1, choice2 };
                AssetDatabase.CreateAsset(evt, path);
            }
            return evt;
        }

        private static EventDataSO GetOrCreateMerchantEvent()
        {
            string path = "Assets/Data/Events/MysticMerchantEvent.asset";
            var evt = AssetDatabase.LoadAssetAtPath<EventDataSO>(path);
            if (evt == null)
            {
                evt = ScriptableObject.CreateInstance<EventDataSO>();
                evt.eventId = "mystic_merchant";
                evt.eventTitle = "Thương Nhân Dị Giới";
                evt.storyPrompt = "Một thương nhân đeo mặt nạ thạch anh xuất hiện từ bóng tối và đưa ra lời mời giao dịch...";

                var choice1 = new EventChoice
                {
                    choiceButtonText = "Tặng 50 Crystal học Kỹ Năng",
                    outcomeDescription = "Thương nhân gật đầu hài lòng và truyền dạy bí kíp cổ xưa!",
                    crystalChange = -50.0,
                    grantsRandomSkill = true
                };

                var choice2 = new EventChoice
                {
                    choiceButtonText = "Từ chối và rời đi an toàn",
                    outcomeDescription = "Bạn cúi chào và tiếp tục chuyến hành trình.",
                    crystalChange = 0.0
                };

                evt.choices = new EventChoice[] { choice1, choice2 };
                AssetDatabase.CreateAsset(evt, path);
            }
            return evt;
        }

        private static AdventureManager SetupAdventureManager(EnemyDataSO[] enemies, SkillDataSO[] skills, EventDataSO[] events)
        {
            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("_Managers");
            }

            var advManager = managersObj.GetComponent<AdventureManager>() ?? managersObj.AddComponent<AdventureManager>();
            var so = new SerializedObject(advManager);

            // Gán dữ liệu vào mảng
            var enemyProp = so.FindProperty("enemyDatabase");
            enemyProp.arraySize = enemies.Length;
            for (int i = 0; i < enemies.Length; i++)
            {
                enemyProp.GetArrayElementAtIndex(i).objectReferenceValue = enemies[i];
            }

            var skillProp = so.FindProperty("skillDatabase");
            skillProp.arraySize = skills.Length;
            for (int i = 0; i < skills.Length; i++)
            {
                skillProp.GetArrayElementAtIndex(i).objectReferenceValue = skills[i];
            }

            var eventProp = so.FindProperty("eventDatabase");
            eventProp.arraySize = events.Length;
            for (int i = 0; i < events.Length; i++)
            {
                eventProp.GetArrayElementAtIndex(i).objectReferenceValue = events[i];
            }

            so.ApplyModifiedProperties();
            return advManager;
        }

        private static AdventureModalUI BuildAdventureModalUI()
        {
            var canvasObj = GameObject.Find("MainCanvas");
            if (canvasObj == null) return null;

            // Xóa Modal cũ nếu có
            var oldModal = canvasObj.transform.Find("AdventureModal");
            if (oldModal != null) Object.DestroyImmediate(oldModal.gameObject);

            // Tạo Modal Root
            var modalObj = CreateUIObject("AdventureModal", canvasObj.transform);
            var rect = modalObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var bgImg = modalObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.03f, 0.08f, 0.96f); // Dark overlay

            // --- HEADER ---
            var headerObj = CreateUIObject("HeaderPanel", modalObj.transform);
            var headerRect = headerObj.GetComponent<RectTransform>();
            SetRect(headerRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 80f));
            var headerBg = headerObj.AddComponent<Image>();
            headerBg.color = new Color(0.08f, 0.06f, 0.16f, 0.85f);

            var floorTitle = CreateText("FloorTitleText", headerObj.transform, "THAM HIEM MO QUANG - TANG 1/5", 24, FontStyles.Bold, new Color(0.9f, 0.7f, 1f));
            SetRect(floorTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600f, 50f));
            floorTitle.alignment = TextAlignmentOptions.Center;

            var retreatBtn = CreateButton("CloseRetreatBtn", headerObj.transform, "[X] Rut lui", new Vector2(1f, 0.5f), new Vector2(-120f, 0f), new Vector2(140f, 48f), new Color(0.6f, 0.2f, 0.2f));

            // --- COMBAT PANEL ---
            var combatPanel = CreateUIObject("CombatPanel", modalObj.transform);
            var combatRect = combatPanel.GetComponent<RectTransform>();
            SetRect(combatRect, new Vector2(0.1f, 0.12f), new Vector2(0.9f, 0.88f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            // Left Box: Hero
            var heroBox = CreateUIObject("HeroBox", combatPanel.transform);
            SetRect(heroBox.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(40f, 60f), new Vector2(340f, 320f));
            var heroBg = heroBox.AddComponent<Image>();
            heroBg.color = new Color(0.1f, 0.12f, 0.2f, 0.8f);

            var heroName = CreateText("HeroName", heroBox.transform, "THO REN TAP SU", 20, FontStyles.Bold, new Color(0.3f, 0.9f, 1f));
            SetRect(heroName.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 40f));
            heroName.alignment = TextAlignmentOptions.Center;

            var heroIcon = CreateText("HeroIcon", heroBox.transform, "[HERO]", 26, FontStyles.Bold, new Color(0.4f, 0.9f, 1f));
            SetRect(heroIcon.rectTransform, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(160f, 80f));
            heroIcon.alignment = TextAlignmentOptions.Center;

            var heroSlider = CreateHpSlider("HeroHpSlider", heroBox.transform, new Vector2(0.5f, 0.25f), new Vector2(280f, 26f), new Color(0.2f, 0.85f, 0.4f));
            var heroHpText = CreateText("HeroHpText", heroBox.transform, "1000 / 1000", 16, FontStyles.Bold, Color.white);
            SetRect(heroHpText.rectTransform, new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(200f, 30f));
            heroHpText.alignment = TextAlignmentOptions.Center;

            // Center Box: VS & Combat Log
            var logBox = CreateUIObject("LogBox", combatPanel.transform);
            SetRect(logBox.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(400f, 360f));
            var logBg = logBox.AddComponent<Image>();
            logBg.color = new Color(0.06f, 0.05f, 0.12f, 0.9f);

            var vsText = CreateText("VsText", logBox.transform, "- GIAO CHIEN -", 22, FontStyles.Bold, new Color(1f, 0.85f, 0.2f));
            SetRect(vsText.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 40f));
            vsText.alignment = TextAlignmentOptions.Center;

            var combatLog = CreateText("CombatLogText", logBox.transform, "Dang chuan bi giao dau...", 16, FontStyles.Normal, Color.white);
            SetRect(combatLog.rectTransform, new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.8f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            combatLog.alignment = TextAlignmentOptions.TopLeft;

            logBox.SetActive(false); // Mac dinh an bang nhat ky theo yeu cau cua Chu nhan de man hinh 3D thoang dang

            // Right Box: Enemy
            var enemyBox = CreateUIObject("EnemyBox", combatPanel.transform);
            SetRect(enemyBox.GetComponent<RectTransform>(), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-40f, 60f), new Vector2(340f, 320f));
            var enemyBg = enemyBox.AddComponent<Image>();
            enemyBg.color = new Color(0.2f, 0.1f, 0.12f, 0.8f);

            var enemyName = CreateText("EnemyName", enemyBox.transform, "CRYSTAL SLIME", 20, FontStyles.Bold, new Color(1f, 0.4f, 0.4f));
            SetRect(enemyName.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 40f));
            enemyName.alignment = TextAlignmentOptions.Center;

            var enemyIcon = CreateText("EnemyIcon", enemyBox.transform, "[ENEMY]", 26, FontStyles.Bold, new Color(1f, 0.4f, 0.4f));
            SetRect(enemyIcon.rectTransform, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(160f, 80f));
            enemyIcon.alignment = TextAlignmentOptions.Center;

            var enemySlider = CreateHpSlider("EnemyHpSlider", enemyBox.transform, new Vector2(0.5f, 0.25f), new Vector2(280f, 26f), new Color(0.95f, 0.3f, 0.3f));
            var enemyHpText = CreateText("EnemyHpText", enemyBox.transform, "300 / 300", 16, FontStyles.Bold, Color.white);
            SetRect(enemyHpText.rectTransform, new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(200f, 30f));
            enemyHpText.alignment = TextAlignmentOptions.Center;

            // --- EVENT PANEL ---
            var eventPanel = CreateUIObject("EventPanel", modalObj.transform);
            var eventRect = eventPanel.GetComponent<RectTransform>();
            SetRect(eventRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(760f, 480f));
            var eventBg = eventPanel.AddComponent<Image>();
            eventBg.color = new Color(0.08f, 0.06f, 0.16f, 0.95f);

            var eventTitle = CreateText("EventTitleText", eventPanel.transform, "SU KIEN BI AN", 24, FontStyles.Bold, new Color(1f, 0.85f, 0.2f));
            SetRect(eventTitle.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650f, 50f));
            eventTitle.alignment = TextAlignmentOptions.Center;

            var eventStory = CreateText("EventStoryText", eventPanel.transform, "Ban bat gap mot nguon nang luong co xua...", 18, FontStyles.Normal, Color.white);
            SetRect(eventStory.rectTransform, new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650f, 160f));
            eventStory.alignment = TextAlignmentOptions.Center;

            var choiceBtn1 = CreateButton("ChoiceBtn1", eventPanel.transform, "Lua chon 1", new Vector2(0.5f, 0.32f), Vector2.zero, new Vector2(550f, 55f), new Color(0.3f, 0.25f, 0.6f));
            var choice1Txt = choiceBtn1.GetComponentInChildren<TMP_Text>();

            var choiceBtn2 = CreateButton("ChoiceBtn2", eventPanel.transform, "Lua chon 2", new Vector2(0.5f, 0.16f), Vector2.zero, new Vector2(550f, 55f), new Color(0.2f, 0.45f, 0.5f));
            var choice2Txt = choiceBtn2.GetComponentInChildren<TMP_Text>();

            eventPanel.SetActive(false);

            // --- SKILL SELECTION PANEL ---
            var skillPanel = CreateUIObject("SkillSelectionPanel", modalObj.transform);
            var skillRect = skillPanel.GetComponent<RectTransform>();
            SetRect(skillRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(980f, 480f));
            var skillBg = skillPanel.AddComponent<Image>();
            skillBg.color = new Color(0.06f, 0.05f, 0.14f, 0.96f);

            var skillPanelTitle = CreateText("SkillPanelTitle", skillPanel.transform, "CHON 1 KY NANG CO DAI (ROGUELIKE)", 24, FontStyles.Bold, new Color(0.3f, 1f, 0.8f));
            SetRect(skillPanelTitle.rectTransform, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700f, 50f));
            skillPanelTitle.alignment = TextAlignmentOptions.Center;

            var skillCards = new GameObject[3];
            var skillTitleTexts = new TMP_Text[3];
            var skillDescTexts = new TMP_Text[3];
            var skillButtons = new Button[3];

            float[] cardXOffsets = { -300f, 0f, 300f };
            for (int i = 0; i < 3; i++)
            {
                var card = CreateUIObject($"SkillCard_{i}", skillPanel.transform);
                SetRect(card.GetComponent<RectTransform>(), new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.5f), new Vector2(cardXOffsets[i], 0f), new Vector2(270f, 320f));
                var cBg = card.AddComponent<Image>();
                cBg.color = new Color(0.12f, 0.10f, 0.24f, 0.95f);

                var sTitle = CreateText("SkillTitle", card.transform, $"Ky Nang {i + 1}", 19, FontStyles.Bold, new Color(1f, 0.85f, 0.3f));
                SetRect(sTitle.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250f, 40f));
                sTitle.alignment = TextAlignmentOptions.Center;

                var sDesc = CreateText("SkillDesc", card.transform, "Mo ta hieu ung ky nang trong luot tham hiem...", 15, FontStyles.Normal, Color.white);
                SetRect(sDesc.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(240f, 140f));
                sDesc.alignment = TextAlignmentOptions.Center;

                var sBtn = CreateButton("PickBtn", card.transform, "LUA CHON", new Vector2(0.5f, 0.15f), Vector2.zero, new Vector2(220f, 46f), new Color(0.2f, 0.6f, 0.4f));

                skillCards[i] = card;
                skillTitleTexts[i] = sTitle;
                skillDescTexts[i] = sDesc;
                skillButtons[i] = sBtn;
            }

            skillPanel.SetActive(false);

            // --- REWARD PANEL ---
            var rewardPanel = CreateUIObject("RewardPanel", modalObj.transform);
            var rewardRect = rewardPanel.GetComponent<RectTransform>();
            SetRect(rewardRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), new Vector2(700f, 460f));
            var rewardBg = rewardPanel.AddComponent<Image>();
            rewardBg.color = new Color(0.07f, 0.05f, 0.15f, 0.96f);

            var rewardTitle = CreateText("RewardTitle", rewardPanel.transform, "THAM HIEM THANH CONG!", 26, FontStyles.Bold, new Color(0.2f, 1f, 0.4f));
            SetRect(rewardTitle.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600f, 50f));
            rewardTitle.alignment = TextAlignmentOptions.Center;

            var rewardCrystal = CreateText("RewardCrystal", rewardPanel.transform, "+5,000 Crystal", 36, FontStyles.Bold, new Color(1f, 0.88f, 0.2f));
            SetRect(rewardCrystal.rectTransform, new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500f, 60f));
            rewardCrystal.alignment = TextAlignmentOptions.Center;

            var rewardBuff = CreateText("RewardBuff", rewardPanel.transform, "[BUFF] x2.0 San Luong Lo Ren (60s)", 20, FontStyles.Bold, new Color(0.4f, 0.8f, 1f));
            SetRect(rewardBuff.rectTransform, new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600f, 50f));
            rewardBuff.alignment = TextAlignmentOptions.Center;

            var claimBtn = CreateButton("ClaimExitBtn", rewardPanel.transform, "THU NHAN & VE LO REN", new Vector2(0.5f, 0.18f), Vector2.zero, new Vector2(380f, 60f), new Color(0.2f, 0.7f, 0.4f));

            rewardPanel.SetActive(false);

            // --- GẮN AdventureModalUI VÀ LIÊN KẾT ---
            var modalUI = modalObj.AddComponent<AdventureModalUI>();
            var so = new SerializedObject(modalUI);

            so.FindProperty("combatPanel").objectReferenceValue = combatPanel;
            so.FindProperty("eventPanel").objectReferenceValue = eventPanel;
            so.FindProperty("skillSelectionPanel").objectReferenceValue = skillPanel;
            so.FindProperty("rewardPanel").objectReferenceValue = rewardPanel;

            so.FindProperty("floorTitleText").objectReferenceValue = floorTitle;
            so.FindProperty("closeRetreatButton").objectReferenceValue = retreatBtn;

            so.FindProperty("heroNameText").objectReferenceValue = heroName;
            so.FindProperty("heroHpSlider").objectReferenceValue = heroSlider;
            so.FindProperty("heroHpText").objectReferenceValue = heroHpText;
            so.FindProperty("enemyNameText").objectReferenceValue = enemyName;
            so.FindProperty("enemyHpSlider").objectReferenceValue = enemySlider;
            so.FindProperty("enemyHpText").objectReferenceValue = enemyHpText;
            so.FindProperty("combatLogText").objectReferenceValue = combatLog;

            so.FindProperty("eventTitleText").objectReferenceValue = eventTitle;
            so.FindProperty("eventStoryText").objectReferenceValue = eventStory;
            so.FindProperty("choiceButton1").objectReferenceValue = choiceBtn1;
            so.FindProperty("choice1Text").objectReferenceValue = choice1Txt;
            so.FindProperty("choiceButton2").objectReferenceValue = choiceBtn2;
            so.FindProperty("choice2Text").objectReferenceValue = choice2Txt;

            var scProp = so.FindProperty("skillCards");
            scProp.arraySize = 3;
            var stProp = so.FindProperty("skillTitleTexts");
            stProp.arraySize = 3;
            var sdProp = so.FindProperty("skillDescTexts");
            sdProp.arraySize = 3;
            var sbProp = so.FindProperty("skillSelectButtons");
            sbProp.arraySize = 3;

            for (int i = 0; i < 3; i++)
            {
                scProp.GetArrayElementAtIndex(i).objectReferenceValue = skillCards[i];
                stProp.GetArrayElementAtIndex(i).objectReferenceValue = skillTitleTexts[i];
                sdProp.GetArrayElementAtIndex(i).objectReferenceValue = skillDescTexts[i];
                sbProp.GetArrayElementAtIndex(i).objectReferenceValue = skillButtons[i];
            }

            so.FindProperty("rewardStatusTitleText").objectReferenceValue = rewardTitle;
            so.FindProperty("rewardCrystalAmountText").objectReferenceValue = rewardCrystal;
            so.FindProperty("rewardBuffInfoText").objectReferenceValue = rewardBuff;
            so.FindProperty("claimAndExitButton").objectReferenceValue = claimBtn;

            so.ApplyModifiedProperties();

            // Gán adventureModalUI vào UIManager
            var managers = GameObject.Find("_Managers");
            if (managers != null)
            {
                var uiMgr = managers.GetComponent<UIManager>();
                if (uiMgr != null)
                {
                    var uiSo = new SerializedObject(uiMgr);
                    var modalProp = uiSo.FindProperty("adventureModalUI");
                    if (modalProp != null)
                    {
                        modalProp.objectReferenceValue = modalUI;
                        uiSo.ApplyModifiedProperties();
                    }
                }
            }

            // Mặc định ẩn modal
            modalObj.SetActive(false);

            return modalUI;
        }

        private static Slider CreateHpSlider(string name, Transform parent, Vector2 anchor, Vector2 size, Color fillColor)
        {
            var sliderObj = CreateUIObject(name, parent);
            var rect = sliderObj.GetComponent<RectTransform>();
            SetRect(rect, anchor, anchor, anchor, Vector2.zero, size);

            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            // Background
            var bgObj = CreateUIObject("Background", sliderObj.transform);
            var bgRect = bgObj.GetComponent<RectTransform>();
            SetRect(bgRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            // Fill Area
            var fillArea = CreateUIObject("Fill Area", sliderObj.transform);
            var faRect = fillArea.GetComponent<RectTransform>();
            SetRect(faRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var fill = CreateUIObject("Fill", fillArea.transform);
            var fRect = fill.GetComponent<RectTransform>();
            SetRect(fRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;

            slider.fillRect = fRect;
            slider.targetGraphic = fillImg;

            return slider;
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, float fontSize, FontStyles style, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static Button CreateButton(string name, Transform parent, string label, Vector2 anchor, Vector2 pos, Vector2 size, Color bgColor)
        {
            var btnObj = CreateUIObject(name, parent);
            var rect = btnObj.GetComponent<RectTransform>();
            SetRect(rect, anchor, anchor, anchor, pos, size);

            var img = btnObj.AddComponent<Image>();
            img.color = bgColor;
            var btn = btnObj.AddComponent<Button>();

            var text = CreateText("Text", btnObj.transform, label, 18, FontStyles.Bold, Color.white);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            text.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        private static void SetRect(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }
    }
}
#endif
