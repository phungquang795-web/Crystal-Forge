using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Adventure;
using CrystalForge.Core;
using CrystalForge.Data;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class AdventureModalUI : MonoBehaviour
    {
        [Header("Root Sub-Panels")]
        [SerializeField] private GameObject combatPanel;
        [SerializeField] private GameObject eventPanel;
        [SerializeField] private GameObject skillSelectionPanel;
        [SerializeField] private GameObject rewardPanel;

        [Header("Header Elements")]
        [SerializeField] private TMP_Text floorTitleText;
        [SerializeField] private Button closeRetreatButton;

        [Header("Combat Elements")]
        [SerializeField] private TMP_Text heroNameText;
        [SerializeField] private Slider heroHpSlider;
        [SerializeField] private TMP_Text heroHpText;
        [SerializeField] private TMP_Text enemyNameText;
        [SerializeField] private Slider enemyHpSlider;
        [SerializeField] private TMP_Text enemyHpText;
        [SerializeField] private TMP_Text combatLogText;

        [Header("Event Elements")]
        [SerializeField] private TMP_Text eventTitleText;
        [SerializeField] private TMP_Text eventStoryText;
        [SerializeField] private Button choiceButton1;
        [SerializeField] private TMP_Text choice1Text;
        [SerializeField] private Button choiceButton2;
        [SerializeField] private TMP_Text choice2Text;

        [Header("Skill Selection Elements (3-Pick-1)")]
        [SerializeField] private GameObject[] skillCards; // 3 cards
        [SerializeField] private TMP_Text[] skillTitleTexts;
        [SerializeField] private TMP_Text[] skillDescTexts;
        [SerializeField] private Button[] skillSelectButtons;

        [Header("Reward / Summary Elements")]
        [SerializeField] private TMP_Text rewardStatusTitleText;
        [SerializeField] private TMP_Text rewardCrystalAmountText;
        [SerializeField] private TMP_Text rewardBuffInfoText;
        [SerializeField] private Button claimAndExitButton;

        private EventChoice currentChoice1;
        private EventChoice currentChoice2;
        private SkillDataSO[] currentOfferedSkills;
        private readonly StringBuilder logBuffer = new StringBuilder();
        private bool isInitialized = false;
        private bool isSubscribed = false;

        private void Awake()
        {
            InitButtons();
        }

        private void Start()
        {
            InitButtons();
            SubscribeEvents();
        }

        private void OnEnable()
        {
            InitButtons();
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        public void OpenModal()
        {
            gameObject.SetActive(true);
            InitButtons();
            SubscribeEvents();
        }

        private void InitButtons()
        {
            if (isInitialized) return;
            isInitialized = true;

            // Ẩn hoàn toàn HeroBox, EnemyBox và LogBox vì đã có thanh máu 3D World-Space trên đầu nhân vật
            if (combatPanel != null)
            {
                var heroBox = combatPanel.transform.Find("HeroBox");
                if (heroBox != null) heroBox.gameObject.SetActive(false);

                var enemyBox = combatPanel.transform.Find("EnemyBox");
                if (enemyBox != null) enemyBox.gameObject.SetActive(false);

                var logBox = combatPanel.transform.Find("LogBox");
                if (logBox != null) logBox.gameObject.SetActive(false);
            }

            if (closeRetreatButton != null)
            {
                closeRetreatButton.onClick.AddListener(OnRetreatClicked);
            }

            if (choiceButton1 != null)
            {
                choiceButton1.onClick.AddListener(() => OnChoiceSelected(currentChoice1));
            }
            if (choiceButton2 != null)
            {
                choiceButton2.onClick.AddListener(() => OnChoiceSelected(currentChoice2));
            }

            if (skillSelectButtons != null)
            {
                for (int i = 0; i < skillSelectButtons.Length; i++)
                {
                    int index = i;
                    if (skillSelectButtons[i] != null)
                    {
                        skillSelectButtons[i].onClick.AddListener(() => OnSkillCardClicked(index));
                    }
                }
            }

            if (skillCards != null)
            {
                for (int i = 0; i < skillCards.Length; i++)
                {
                    int index = i;
                    if (skillCards[i] != null)
                    {
                        var cardBtn = skillCards[i].GetComponent<Button>();
                        if (cardBtn == null)
                        {
                            cardBtn = skillCards[i].AddComponent<Button>();
                        }
                        cardBtn.onClick.AddListener(() => OnSkillCardClicked(index));
                    }
                }
            }

            if (claimAndExitButton != null)
            {
                claimAndExitButton.onClick.AddListener(OnClaimRewardClicked);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (isSubscribed || AdventureManager.Instance == null) return;
            isSubscribed = true;

            AdventureManager.Instance.OnStateChanged += HandleStateChanged;
            AdventureManager.Instance.OnHeroHpUpdated += UpdateHeroHp;
            AdventureManager.Instance.OnEnemyHpUpdated += UpdateEnemyHp;
            AdventureManager.Instance.OnEnemyEncountered += HandleEnemyEncountered;
            AdventureManager.Instance.OnEventEncountered += HandleEventEncountered;
            AdventureManager.Instance.OnSkillsOffered += HandleSkillsOffered;
            AdventureManager.Instance.OnCombatLogMessage += AppendCombatLog;
            AdventureManager.Instance.OnRewardSummary += HandleRewardSummary;
            AdventureManager.Instance.OnFloorChanged += UpdateFloorDisplay;
        }

        private void UnsubscribeEvents()
        {
            if (!isSubscribed || AdventureManager.Instance == null) return;
            isSubscribed = false;

            AdventureManager.Instance.OnStateChanged -= HandleStateChanged;
            AdventureManager.Instance.OnHeroHpUpdated -= UpdateHeroHp;
            AdventureManager.Instance.OnEnemyHpUpdated -= UpdateEnemyHp;
            AdventureManager.Instance.OnEnemyEncountered -= HandleEnemyEncountered;
            AdventureManager.Instance.OnEventEncountered -= HandleEventEncountered;
            AdventureManager.Instance.OnSkillsOffered -= HandleSkillsOffered;
            AdventureManager.Instance.OnCombatLogMessage -= AppendCombatLog;
            AdventureManager.Instance.OnRewardSummary -= HandleRewardSummary;
            AdventureManager.Instance.OnFloorChanged -= UpdateFloorDisplay;
        }

        private void HandleStateChanged(AdventureState newState)
        {
            if (newState == AdventureState.None)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            // Tự động điều chỉnh độ trong suốt của nền Modal:
            // Khi chiến đấu 3D: nền cực nhẹ để nhìn xuyên suốt trận đấu 3D giữa Hero và Quái
            // Khi chọn kỹ năng hoặc sự kiện: nền tối hơn để đọc rõ thẻ bài
            var bgImg = GetComponent<Image>();
            if (bgImg != null)
            {
                bgImg.color = (newState == AdventureState.AutoCombat)
                    ? new Color(0.04f, 0.03f, 0.08f, 0.15f)
                    : new Color(0.04f, 0.03f, 0.08f, 0.88f);
            }

            if (combatPanel != null)
            {
                combatPanel.SetActive(newState == AdventureState.AutoCombat);
                if (newState == AdventureState.AutoCombat)
                {
                    var hBox = combatPanel.transform.Find("HeroBox");
                    if (hBox != null) hBox.gameObject.SetActive(false);
                    var eBox = combatPanel.transform.Find("EnemyBox");
                    if (eBox != null) eBox.gameObject.SetActive(false);
                    var lBox = combatPanel.transform.Find("LogBox");
                    if (lBox != null) lBox.gameObject.SetActive(false);
                }
            }
            if (eventPanel != null) eventPanel.SetActive(newState == AdventureState.EventChoice);
            if (skillSelectionPanel != null) skillSelectionPanel.SetActive(newState == AdventureState.SkillSelection);
            if (rewardPanel != null) rewardPanel.SetActive(newState == AdventureState.AdventureReward || newState == AdventureState.RunDefeat);
        }

        private void UpdateFloorDisplay(int floor)
        {
            if (floorTitleText != null)
            {
                floorTitleText.text = $"THAM HIEM MO QUANG - TANG {floor}/5";
            }
        }

        private void UpdateHeroHp(int current, int max)
        {
            if (heroHpSlider != null)
            {
                heroHpSlider.value = max > 0 ? (float)current / max : 0f;
            }
            if (heroHpText != null)
            {
                heroHpText.text = $"{current} / {max}";
            }
        }

        private void UpdateEnemyHp(int current, int max)
        {
            if (enemyHpSlider != null)
            {
                enemyHpSlider.value = max > 0 ? (float)current / max : 0f;
            }
            if (enemyHpText != null)
            {
                enemyHpText.text = $"{current} / {max}";
            }
        }

        private void HandleEnemyEncountered(EnemyDataSO enemy)
        {
            if (enemyNameText != null)
            {
                enemyNameText.text = enemy != null ? enemy.enemyName.ToUpper() : "QUAI VAT";
            }
            logBuffer.Clear();
            if (combatLogText != null) combatLogText.text = "";
        }

        private void AppendCombatLog(string message, Color color)
        {
            if (combatLogText == null || !combatLogText.gameObject.activeInHierarchy) return;

            message = FontSanitizer.Sanitize(message);
            string hexColor = ColorUtility.ToHtmlStringRGB(color);
            logBuffer.Insert(0, $"<color=#{hexColor}>{message}</color>\n");

            // Giữ tối đa 6 dòng log mới nhất
            string[] lines = logBuffer.ToString().Split('\n');
            if (lines.Length > 7)
            {
                logBuffer.Clear();
                for (int i = 0; i < 6; i++)
                {
                    logBuffer.AppendLine(lines[i]);
                }
            }
            combatLogText.text = logBuffer.ToString();
        }

        private void HandleEventEncountered(EventDataSO evt)
        {
            if (evt == null) return;

            if (eventTitleText != null) eventTitleText.text = evt.eventTitle;
            if (eventStoryText != null) eventStoryText.text = evt.storyPrompt;

            if (evt.choices != null && evt.choices.Length >= 1)
            {
                currentChoice1 = evt.choices[0];
                if (choice1Text != null) choice1Text.text = currentChoice1.choiceButtonText;
                if (choiceButton1 != null) choiceButton1.gameObject.SetActive(true);
            }
            else
            {
                if (choiceButton1 != null) choiceButton1.gameObject.SetActive(false);
            }

            if (evt.choices != null && evt.choices.Length >= 2)
            {
                currentChoice2 = evt.choices[1];
                if (choice2Text != null) choice2Text.text = currentChoice2.choiceButtonText;
                if (choiceButton2 != null) choiceButton2.gameObject.SetActive(true);
            }
            else
            {
                if (choiceButton2 != null) choiceButton2.gameObject.SetActive(false);
            }
        }

        private void OnChoiceSelected(EventChoice choice)
        {
            if (AdventureManager.Instance != null)
            {
                AdventureManager.Instance.ResolveEventChoice(choice);
            }
        }

        private void HandleSkillsOffered(SkillDataSO[] offeredSkills)
        {
            currentOfferedSkills = offeredSkills;
            int count = offeredSkills != null ? offeredSkills.Length : 0;

            for (int i = 0; i < 3; i++)
            {
                bool active = i < count;
                if (skillCards != null && i < skillCards.Length && skillCards[i] != null)
                {
                    skillCards[i].SetActive(active);
                }

                if (active)
                {
                    var skill = offeredSkills[i];
                    if (skillTitleTexts != null && i < skillTitleTexts.Length && skillTitleTexts[i] != null)
                    {
                        skillTitleTexts[i].text = skill.skillName;
                    }
                    if (skillDescTexts != null && i < skillDescTexts.Length && skillDescTexts[i] != null)
                    {
                        skillDescTexts[i].text = skill.description;
                    }
                }
            }
        }

        private void OnSkillCardClicked(int index)
        {
            if (currentOfferedSkills != null && index >= 0 && index < currentOfferedSkills.Length)
            {
                var skill = currentOfferedSkills[index];
                if (AdventureManager.Instance != null)
                {
                    AdventureManager.Instance.SelectSkill(skill);
                }
            }
        }

        private void HandleRewardSummary(double crystals, float buffMult, float buffDuration, bool isVictory)
        {
            if (rewardStatusTitleText != null)
            {
                rewardStatusTitleText.text = isVictory ? "THAM HIEM THANH CONG!" : "HERO BI HA GUC!";
                rewardStatusTitleText.color = isVictory ? new Color(0.2f, 1f, 0.4f) : new Color(1f, 0.35f, 0.35f);
            }

            if (rewardCrystalAmountText != null)
            {
                rewardCrystalAmountText.text = $"+{CrystalFormat.Format(crystals)} Crystal";
            }

            if (rewardBuffInfoText != null)
            {
                if (buffMult > 1.0f && buffDuration > 0f)
                {
                    rewardBuffInfoText.text = $"[BUFF] x{buffMult:0.#} San Luong Lo Ren ({buffDuration:0}s)";
                    rewardBuffInfoText.gameObject.SetActive(true);
                }
                else
                {
                    rewardBuffInfoText.text = "Khong co Buff them.";
                    rewardBuffInfoText.gameObject.SetActive(false);
                }
            }
        }

        private void OnRetreatClicked()
        {
            if (AdventureManager.Instance != null)
            {
                AdventureManager.Instance.ClaimRewardAndReturnToForge();
            }
        }

        private void OnClaimRewardClicked()
        {
            if (AdventureManager.Instance != null)
            {
                AdventureManager.Instance.ClaimRewardAndReturnToForge();
            }
        }
    }
}
