using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Audio;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    [DefaultExecutionOrder(-10)]
    public class CosmicMeltdownModalUI : MonoBehaviour
    {
        private static CosmicMeltdownModalUI instance;
        public static CosmicMeltdownModalUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindAnyObjectByType<CosmicMeltdownModalUI>(FindObjectsInactive.Include);
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("UI Text Displays")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private TMP_Text stardustWalletText;

        [Header("Talent 1: Astral Efficiency")]
        [SerializeField] private TMP_Text astralTitleText;
        [SerializeField] private TMP_Text astralDescText;
        [SerializeField] private Button astralUpgradeBtn;
        [SerializeField] private TMP_Text astralCostText;

        [Header("Talent 2: Royal Inheritance")]
        [SerializeField] private TMP_Text royalTitleText;
        [SerializeField] private TMP_Text royalDescText;
        [SerializeField] private Button royalUpgradeBtn;
        [SerializeField] private TMP_Text royalCostText;

        [Header("Talent 3: Cosmic Capybara")]
        [SerializeField] private TMP_Text cosmicTitleText;
        [SerializeField] private TMP_Text cosmicDescText;
        [SerializeField] private Button cosmicUpgradeBtn;
        [SerializeField] private TMP_Text cosmicCostText;

        [Header("Main Buttons")]
        [SerializeField] private Button meltdownButton;
        [SerializeField] private TMP_Text meltdownButtonText;
        [SerializeField] private Button closeButton;

        [Header("Canvas Group & Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform cardContainer;

        private Coroutine popupAnimationCoroutine;
        private bool isShowing = false;
        public bool IsShowing => isShowing;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            EnsureInitialized();
            if (!isShowing)
            {
                HideInstant();
            }
        }

        private void EnsureInitialized()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (cardContainer == null)
            {
                var card = transform.Find("CardContainer");
                if (card != null) cardContainer = card.GetComponent<RectTransform>();
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(HideModal);
            }

            if (meltdownButton != null)
            {
                meltdownButton.onClick.RemoveAllListeners();
                meltdownButton.onClick.AddListener(OnMeltdownClicked);
            }

            if (astralUpgradeBtn != null)
            {
                astralUpgradeBtn.onClick.RemoveAllListeners();
                astralUpgradeBtn.onClick.AddListener(OnAstralUpgradeClicked);
            }

            if (royalUpgradeBtn != null)
            {
                royalUpgradeBtn.onClick.RemoveAllListeners();
                royalUpgradeBtn.onClick.AddListener(OnRoyalUpgradeClicked);
            }

            if (cosmicUpgradeBtn != null)
            {
                cosmicUpgradeBtn.onClick.RemoveAllListeners();
                cosmicUpgradeBtn.onClick.AddListener(OnCosmicUpgradeClicked);
            }
        }

        public void OpenModal()
        {
            EnsureInitialized();
            gameObject.SetActive(true);
            isShowing = true;

            RefreshUI();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimatePopupIn());
        }

        public void HideModal()
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSfx();
            isShowing = false;

            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimatePopupOut());
        }

        public void HideInstant()
        {
            isShowing = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        public void RefreshUI()
        {
            if (PrestigeManager.Instance == null || ResourceManager.Instance == null) return;

            int pending = PrestigeManager.Instance.CalculatePendingStardust();
            int currentWallet = PrestigeManager.Instance.TotalStardust;

            if (stardustWalletText != null)
            {
                stardustWalletText.text = $"Bụi Tinh Tú: <color=#FFE855><b>{currentWallet} ⭐</b></color>";
            }

            if (summaryText != null)
            {
                if (pending > 0)
                {
                    summaryText.text = $"Lò Rèn đã tích lũy đủ năng lượng cực hạn!\nĐại nung chảy ngay sẽ thu về: <color=#00FF88><b>+{pending} Bụi Tinh Tú (Stardust)</b></color>";
                }
                else
                {
                    double lifetime = ResourceManager.Instance.LifetimeCrystal;
                    summaryText.text = $"Cần tối thiểu <color=#FFCC00>500K Lifetime Crystal</color> để nung chảy lõi.\nTiến độ hiện tại: {CrystalFormat.Format(lifetime)} / 500K";
                }
            }

            if (meltdownButton != null)
            {
                meltdownButton.interactable = pending > 0;
            }

            if (meltdownButtonText != null)
            {
                meltdownButtonText.text = pending > 0 ? $"ĐẠI NUNG CHẢY (+{pending} ⭐)" : "CHƯA ĐỦ ĐIỀU KIỆN";
            }

            // 1. Astral Talent
            int astralLvl = PrestigeManager.Instance.AstralEfficiencyLevel;
            int astralCost = PrestigeManager.Instance.AstralCost;
            if (astralTitleText != null) astralTitleText.text = $"HÀO QUANG TINH TÚ (Cấp {astralLvl})";
            if (astralDescText != null) astralDescText.text = $"+{(astralLvl * 30)}% Sản lượng toàn cầu vĩnh viễn (+30%/cấp)";
            if (astralCostText != null) astralCostText.text = $"{astralCost} ⭐";
            if (astralUpgradeBtn != null) astralUpgradeBtn.interactable = currentWallet >= astralCost;

            // 2. Royal Talent
            int royalLvl = PrestigeManager.Instance.RoyalInheritanceLevel;
            int royalCost = PrestigeManager.Instance.RoyalCost;
            if (royalTitleText != null) royalTitleText.text = $"KHỞI NGHIỆP HOÀNG GIA (Cấp {royalLvl})";
            if (royalDescText != null) royalDescText.text = royalLvl > 0 ? $"Khởi đầu với +{CrystalFormat.Format(PrestigeManager.Instance.RoyalStartingCrystal)} Crystal sau khi reset" : "Khởi đầu không có Crystal";
            if (royalCostText != null) royalCostText.text = $"{royalCost} ⭐";
            if (royalUpgradeBtn != null) royalUpgradeBtn.interactable = currentWallet >= royalCost;

            // 3. Cosmic Talent
            int cosmicLvl = PrestigeManager.Instance.CosmicCapybaraLevel;
            int cosmicCost = PrestigeManager.Instance.CosmicCost;
            if (cosmicTitleText != null) cosmicTitleText.text = $"CHIẾN TƯỚNG THIÊN HÀ (Cấp {cosmicLvl})";
            if (cosmicDescText != null) cosmicDescText.text = $"+{(cosmicLvl * 30)}% HP & ATK Capybara trong Thám hiểm";
            if (cosmicCostText != null) cosmicCostText.text = $"{cosmicCost} ⭐";
            if (cosmicUpgradeBtn != null) cosmicUpgradeBtn.interactable = currentWallet >= cosmicCost;
        }

        private void OnMeltdownClicked()
        {
            if (PrestigeManager.Instance != null && PrestigeManager.Instance.CanMeltdown())
            {
                PrestigeManager.Instance.PerformCosmicMeltdown();
                RefreshUI();
            }
        }

        private void OnAstralUpgradeClicked()
        {
            if (PrestigeManager.Instance != null && PrestigeManager.Instance.TryUpgradeAstralEfficiency())
            {
                RefreshUI();
            }
        }

        private void OnRoyalUpgradeClicked()
        {
            if (PrestigeManager.Instance != null && PrestigeManager.Instance.TryUpgradeRoyalInheritance())
            {
                RefreshUI();
            }
        }

        private void OnCosmicUpgradeClicked()
        {
            if (PrestigeManager.Instance != null && PrestigeManager.Instance.TryUpgradeCosmicCapybara())
            {
                RefreshUI();
            }
        }

        private IEnumerator AnimatePopupIn()
        {
            float duration = 0.22f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float ease = Mathf.Sin(t * Mathf.PI * 0.5f);

                if (canvasGroup != null) canvasGroup.alpha = ease;
                if (cardContainer != null) cardContainer.localScale = Vector3.LerpUnclamped(Vector3.one * 0.88f, Vector3.one, ease);

                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (cardContainer != null) cardContainer.localScale = Vector3.one;
        }

        private IEnumerator AnimatePopupOut()
        {
            float duration = 0.16f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                if (canvasGroup != null) canvasGroup.alpha = 1f - t;
                if (cardContainer != null) cardContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.9f, t);

                yield return null;
            }

            HideInstant();
        }
    }
}
