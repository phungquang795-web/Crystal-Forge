using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Core;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class UpgradeCardUI : MonoBehaviour
    {
        public enum CardType
        {
            TapPower,
            Generator,
            PlayerHp,
            PlayerAtk,
            PlayerSpeed,
            PlayerSkill,
            RelicHammer,
            RelicAegis,
            RelicIgnis
        }

        [Header("Card Setup")]
        [SerializeField] private CardType cardType = CardType.Generator;
        [SerializeField] private GeneratorType generatorType = GeneratorType.CrystalDrill;

        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text iconText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text outputText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Image buyButtonImage;
        [SerializeField] private GameObject lockOverlay;
        [SerializeField] private TMP_Text lockRequirementText;

        private void Start()
        {
            if (buyButton != null)
            {
                buyButton.onClick.AddListener(OnBuyClicked);
            }

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCrystalChanged += CheckAffordability;
            }

            if (GeneratorSystem.Instance != null)
            {
                GeneratorSystem.Instance.OnGeneratorUpgraded += HandleGeneratorUpgraded;
                GeneratorSystem.Instance.OnGeneratorUnlocked += HandleGeneratorUnlocked;
            }

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnTapPowerUpgraded += HandleTapPowerUpgraded;
                UpgradeManager.Instance.OnPlayerStatUpgraded += HandlePlayerStatUpgraded;
            }

            if (RelicManager.Instance != null)
            {
                RelicManager.Instance.OnRelicUpgraded += HandleRelicUpgraded;
            }

            RefreshCard();
        }

        private void OnDestroy()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveListener(OnBuyClicked);
            }

            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnCrystalChanged -= CheckAffordability;
            }

            if (GeneratorSystem.Instance != null)
            {
                GeneratorSystem.Instance.OnGeneratorUpgraded -= HandleGeneratorUpgraded;
                GeneratorSystem.Instance.OnGeneratorUnlocked -= HandleGeneratorUnlocked;
            }

            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OnTapPowerUpgraded -= HandleTapPowerUpgraded;
                UpgradeManager.Instance.OnPlayerStatUpgraded -= HandlePlayerStatUpgraded;
            }

            if (RelicManager.Instance != null)
            {
                RelicManager.Instance.OnRelicUpgraded -= HandleRelicUpgraded;
            }
        }

        private void HandleGeneratorUpgraded(GeneratorType type, int level)
        {
            if (cardType == CardType.Generator && generatorType == type)
            {
                RefreshCard();
            }
        }

        private void HandleGeneratorUnlocked(GeneratorType type)
        {
            if (cardType == CardType.Generator && generatorType == type)
            {
                RefreshCard();
            }
        }

        private void HandleTapPowerUpgraded(int level, double bonus, double nextCost)
        {
            if (cardType == CardType.TapPower)
            {
                RefreshCard();
            }
        }

        private void HandlePlayerStatUpgraded(PlayerStatType stat, int level, double nextCost)
        {
            if ((cardType == CardType.PlayerHp && stat == PlayerStatType.Hp) ||
                (cardType == CardType.PlayerAtk && stat == PlayerStatType.Attack) ||
                (cardType == CardType.PlayerSpeed && stat == PlayerStatType.Speed) ||
                (cardType == CardType.PlayerSkill && stat == PlayerStatType.Skill))
            {
                RefreshCard();
            }
        }

        private void HandleRelicUpgraded(RelicType type, int level, double nextCost)
        {
            if ((cardType == CardType.RelicHammer && type == RelicType.ThunderHammer) ||
                (cardType == CardType.RelicAegis && type == RelicType.GlacialAegis) ||
                (cardType == CardType.RelicIgnis && type == RelicType.IgnisCore))
            {
                RefreshCard();
            }
        }

        private void OnBuyClicked()
        {
            if (buyButton != null)
            {
                StopAllCoroutines();
                StartCoroutine(AnimateButtonBounce(buyButton.transform));
            }

            if (cardType == CardType.TapPower)
            {
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.TryUpgradeTapPower();
                }
            }
            else if (cardType == CardType.Generator)
            {
                if (GeneratorSystem.Instance != null)
                {
                    GeneratorSystem.Instance.TryUpgrade(generatorType);
                }
            }
            else if (cardType == CardType.PlayerHp)
            {
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.TryUpgradeHp();
                }
            }
            else if (cardType == CardType.PlayerAtk)
            {
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.TryUpgradeAtk();
                }
            }
            else if (cardType == CardType.PlayerSpeed)
            {
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.TryUpgradeSpeed();
                }
            }
            else if (cardType == CardType.PlayerSkill)
            {
                if (UpgradeManager.Instance != null)
                {
                    UpgradeManager.Instance.TryUpgradeSkill();
                }
            }
            else if (cardType == CardType.RelicHammer)
            {
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.TryUpgrade(RelicType.ThunderHammer);
                }
            }
            else if (cardType == CardType.RelicAegis)
            {
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.TryUpgrade(RelicType.GlacialAegis);
                }
            }
            else if (cardType == CardType.RelicIgnis)
            {
                if (RelicManager.Instance != null)
                {
                    RelicManager.Instance.TryUpgrade(RelicType.IgnisCore);
                }
            }
        }

        private System.Collections.IEnumerator AnimateButtonBounce(Transform target)
        {
            Vector3 original = Vector3.one;
            float elapsed = 0f;
            float duration = 0.16f;

            // Co lại tạo lực
            while (elapsed < duration * 0.4f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.4f);
                target.localScale = Vector3.Lerp(original, new Vector3(0.92f, 0.92f, 1f), t);
                yield return null;
            }

            elapsed = 0f;
            // Nảy bung ra tạo cảm giác đã tay
            while (elapsed < duration * 0.6f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.6f);
                target.localScale = Vector3.Lerp(new Vector3(0.92f, 0.92f, 1f), original, t);
                yield return null;
            }

            target.localScale = original;
        }

        public void RefreshCard()
        {
            if (iconImage != null && iconImage.sprite != null)
            {
                iconImage.gameObject.SetActive(true);
                if (iconText != null) iconText.gameObject.SetActive(false);
            }
            else if (iconText != null)
            {
                iconText.gameObject.SetActive(true);
                switch (cardType)
                {
                    case CardType.TapPower: iconText.text = "TAP"; iconText.color = new Color(0.4f, 0.9f, 1.0f); break;
                    case CardType.Generator:
                        if (generatorType == GeneratorType.CrystalDrill) { iconText.text = "DRILL"; iconText.color = new Color(1.0f, 0.85f, 0.25f); }
                        else if (generatorType == GeneratorType.ManaFurnace) { iconText.text = "FURNACE"; iconText.color = new Color(1.0f, 0.45f, 0.2f); }
                        else if (generatorType == GeneratorType.AutoExtractor) { iconText.text = "EXTRACT"; iconText.color = new Color(0.4f, 1.0f, 0.7f); }
                        break;
                    case CardType.PlayerHp: iconText.text = "HP"; iconText.color = new Color(1.0f, 0.35f, 0.45f); break;
                    case CardType.PlayerAtk: iconText.text = "ATK"; iconText.color = new Color(1.0f, 0.75f, 0.25f); break;
                    case CardType.PlayerSpeed: iconText.text = "SPD"; iconText.color = new Color(0.35f, 0.85f, 1.0f); break;
                    case CardType.PlayerSkill: iconText.text = "SKILL"; iconText.color = new Color(0.85f, 0.55f, 1.0f); break;
                    case CardType.RelicHammer: iconText.text = "HAMMER"; iconText.color = new Color(0.35f, 0.95f, 1.0f); break;
                    case CardType.RelicAegis: iconText.text = "AEGIS"; iconText.color = new Color(0.4f, 1.0f, 0.85f); break;
                    case CardType.RelicIgnis: iconText.text = "IGNIS"; iconText.color = new Color(1.0f, 0.45f, 0.2f); break;
                }
            }

            if (cardType == CardType.TapPower)
            {
                if (UpgradeManager.Instance == null) return;

                if (titleText != null) titleText.text = "LỰC GÕ PHA LÊ";
                if (levelText != null) levelText.text = $"Cấp {UpgradeManager.Instance.CurrentTapLevel}";
                if (outputText != null && ResourceManager.Instance != null)
                {
                    outputText.text = $"+{CrystalFormat.Format(ResourceManager.Instance.TapPower)} / Nhấp";
                }

                double cost = UpgradeManager.Instance.CurrentTapPowerCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.Generator)
            {
                if (GeneratorSystem.Instance == null) return;

                var state = GeneratorSystem.Instance.GetState(generatorType);
                if (state == null || state.data == null) return;

                if (titleText != null) titleText.text = FontSanitizer.Sanitize(state.data.generatorName.ToUpper());
                if (levelText != null) levelText.text = $"Cấp {state.currentLevel}";
                if (outputText != null)
                {
                    outputText.text = state.currentLevel > 0
                        ? CrystalFormat.FormatRate(state.CurrentProduction)
                        : $"+{CrystalFormat.Format(state.data.baseProductionPerSec)}/s (Cấp 1)";
                }

                if (costText != null) costText.text = $"{CrystalFormat.Format(state.CurrentCost)} CR";

                bool isLocked = !state.isUnlocked;
                if (lockOverlay != null) lockOverlay.SetActive(isLocked);
                if (lockRequirementText != null && isLocked)
                {
                    lockRequirementText.text = $"Cần {CrystalFormat.Format(state.data.unlockLifetimeRequired)} Tổng Thu Hoạch";
                }

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.PlayerHp)
            {
                if (UpgradeManager.Instance == null) return;

                if (titleText != null) titleText.text = "THỂ LỰC (HP)";
                if (levelText != null) levelText.text = $"Cấp {UpgradeManager.Instance.PlayerHpLevel}";
                if (outputText != null) outputText.text = $"+{UpgradeManager.Instance.BonusMaxHp} Máu Tối Đa (+150/cấp)";

                double cost = UpgradeManager.Instance.CurrentHpCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.PlayerAtk)
            {
                if (UpgradeManager.Instance == null) return;

                if (titleText != null) titleText.text = "CÔNG KÍCH (ATK)";
                if (levelText != null) levelText.text = $"Cấp {UpgradeManager.Instance.PlayerAtkLevel}";
                if (outputText != null) outputText.text = $"+{UpgradeManager.Instance.BonusAtk} Sát Thương (+25/cấp)";

                double cost = UpgradeManager.Instance.CurrentAtkCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.PlayerSpeed)
            {
                if (UpgradeManager.Instance == null) return;

                if (titleText != null) titleText.text = "NHANH NHẸN (SPD)";
                if (levelText != null) levelText.text = $"Cấp {UpgradeManager.Instance.PlayerSpdLevel}";
                if (outputText != null) outputText.text = $"+{UpgradeManager.Instance.BonusSpeedPercent:0}% Tốc Đánh";

                double cost = UpgradeManager.Instance.CurrentSpeedCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.PlayerSkill)
            {
                if (UpgradeManager.Instance == null) return;

                if (titleText != null) titleText.text = "CỔ NGỮ (SKILL)";
                if (levelText != null) levelText.text = $"Cấp {UpgradeManager.Instance.PlayerSkillLevel}";
                if (outputText != null) outputText.text = $"+{UpgradeManager.Instance.BonusSkillDamage} ST & +{(UpgradeManager.Instance.BonusSkillCritRate * 100):0}% Chí Mạng";

                double cost = UpgradeManager.Instance.CurrentSkillCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.RelicHammer)
            {
                if (RelicManager.Instance == null) return;

                if (titleText != null) titleText.text = "BÚA THIÊN THẠCH";
                if (levelText != null) levelText.text = $"Cấp {RelicManager.Instance.HammerLevel}";
                if (outputText != null)
                {
                    outputText.text = RelicManager.Instance.HammerLevel > 0
                        ? $"+{(1.0 + 0.35 * RelicManager.Instance.HammerLevel):0.##}x Tự Động Gõ (10s)"
                        : "Tự động gõ đe rèn mỗi 10s";
                }

                double cost = RelicManager.Instance.CurrentHammerCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.RelicAegis)
            {
                if (RelicManager.Instance == null) return;

                if (titleText != null) titleText.text = "KHIÊN BĂNG CỔ ĐẠI";
                if (levelText != null) levelText.text = $"Cấp {RelicManager.Instance.AegisLevel}";
                if (outputText != null)
                {
                    outputText.text = RelicManager.Instance.AegisLevel > 0
                        ? $"+{(RelicManager.Instance.OfflineBonusRatio * 100):0}% Treo Máy & +{RelicManager.Instance.AegisShieldAmount} Giáp"
                        : "Tăng Treo Máy & Giáp chiến đấu";
                }

                double cost = RelicManager.Instance.CurrentAegisCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
            else if (cardType == CardType.RelicIgnis)
            {
                if (RelicManager.Instance == null) return;

                if (titleText != null) titleText.text = "LOI LUA BAT DIET";
                if (levelText != null) levelText.text = $"Cap {RelicManager.Instance.IgnisLevel}";
                if (outputText != null)
                {
                    outputText.text = RelicManager.Instance.IgnisLevel > 0
                        ? $"+{((RelicManager.Instance.IgnisProductionMultiplier - 1f) * 100):0}% Toan Cau & +{RelicManager.Instance.IgnisBurnDamage} DoT"
                        : "Tang san luong & Dot quai vat";
                }

                double cost = RelicManager.Instance.CurrentIgnisCost;
                if (costText != null) costText.text = $"{CrystalFormat.Format(cost)} CR";

                if (lockOverlay != null) lockOverlay.SetActive(false);

                CheckAffordability(ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0.0);
            }
        }

        private void CheckAffordability(double currentCrystal)
        {
            if (buyButton == null) return;

            double cost = 0.0;
            bool isUnlocked = true;

            if (cardType == CardType.TapPower)
            {
                cost = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentTapPowerCost : double.MaxValue;
            }
            else if (cardType == CardType.Generator)
            {
                var state = GeneratorSystem.Instance != null ? GeneratorSystem.Instance.GetState(generatorType) : null;
                if (state != null)
                {
                    cost = state.CurrentCost;
                    isUnlocked = state.isUnlocked;
                }
            }
            else if (cardType == CardType.PlayerHp)
            {
                cost = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentHpCost : double.MaxValue;
            }
            else if (cardType == CardType.PlayerAtk)
            {
                cost = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentAtkCost : double.MaxValue;
            }
            else if (cardType == CardType.PlayerSpeed)
            {
                cost = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentSpeedCost : double.MaxValue;
            }
            else if (cardType == CardType.PlayerSkill)
            {
                cost = UpgradeManager.Instance != null ? UpgradeManager.Instance.CurrentSkillCost : double.MaxValue;
            }
            else if (cardType == CardType.RelicHammer)
            {
                cost = RelicManager.Instance != null ? RelicManager.Instance.CurrentHammerCost : double.MaxValue;
            }
            else if (cardType == CardType.RelicAegis)
            {
                cost = RelicManager.Instance != null ? RelicManager.Instance.CurrentAegisCost : double.MaxValue;
            }
            else if (cardType == CardType.RelicIgnis)
            {
                cost = RelicManager.Instance != null ? RelicManager.Instance.CurrentIgnisCost : double.MaxValue;
            }

            bool canAfford = isUnlocked && (currentCrystal >= cost);
            buyButton.interactable = canAfford;

            if (buyButtonImage == null && buyButton != null)
            {
                buyButtonImage = buyButton.GetComponent<Image>();
            }

            if (buyButtonImage != null)
            {
                buyButtonImage.color = canAfford
                    ? new Color(0.12f, 0.78f, 0.42f, 1f)
                    : new Color(0.18f, 0.14f, 0.26f, 0.80f);
            }

            var borderTrans = buyButton.transform.Find("BuyButtonBorder");
            if (borderTrans != null)
            {
                var borderImgs = borderTrans.GetComponentsInChildren<Image>();
                Color borderCol = canAfford ? new Color(0.35f, 1.0f, 0.75f, 0.85f) : new Color(0.35f, 0.28f, 0.45f, 0.40f);
                foreach (var b in borderImgs)
                {
                    b.color = borderCol;
                }
            }

            if (costText != null)
            {
                costText.color = canAfford ? new Color(1.0f, 0.92f, 0.25f) : new Color(0.70f, 0.65f, 0.75f);
            }
        }
    }
}

