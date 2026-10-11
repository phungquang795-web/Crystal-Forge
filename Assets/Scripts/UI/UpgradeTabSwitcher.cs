using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CrystalForge.UI
{
    public class UpgradeTabSwitcher : MonoBehaviour
    {
        [Header("Tab Buttons")]
        [SerializeField] private Button forgeTabButton;
        [SerializeField] private Button playerTabButton;
        [SerializeField] private Button relicTabButton;

        [Header("Tab Visuals")]
        [SerializeField] private Image forgeTabImg;
        [SerializeField] private Image playerTabImg;
        [SerializeField] private Image relicTabImg;
        [SerializeField] private TMP_Text forgeTabText;
        [SerializeField] private TMP_Text playerTabText;
        [SerializeField] private TMP_Text relicTabText;
        [SerializeField] private Image forgeTabIcon;
        [SerializeField] private Image playerTabIcon;
        [SerializeField] private Image relicTabIcon;

        [Header("Containers")]
        [SerializeField] private GameObject forgeContainer;
        [SerializeField] private GameObject playerContainer;
        [SerializeField] private GameObject relicContainer;

        private readonly Color activeBgColor = new Color(0.95f, 0.75f, 0.22f, 0.95f);
        private readonly Color inactiveBgColor = new Color(0.16f, 0.12f, 0.28f, 0.85f);
        private readonly Color activeTextColor = new Color(0.12f, 0.08f, 0.04f);
        private readonly Color inactiveTextColor = new Color(0.85f, 0.85f, 0.95f);

        private void Start()
        {
            if (forgeTabButton != null) forgeTabButton.onClick.AddListener(SelectForgeTab);
            if (playerTabButton != null) playerTabButton.onClick.AddListener(SelectPlayerTab);
            if (relicTabButton != null) relicTabButton.onClick.AddListener(SelectRelicTab);

            // Mặc định mở Tab Lò Rèn
            SelectForgeTab();
        }

        public void SelectForgeTab()
        {
            SetContainerStates(forgeActive: true, playerActive: false, relicActive: false);

            SetTabVisual(forgeTabImg, forgeTabText, forgeTabIcon, true);
            SetTabVisual(playerTabImg, playerTabText, playerTabIcon, false);
            SetTabVisual(relicTabImg, relicTabText, relicTabIcon, false);

            RefreshCardsIn(forgeContainer);
        }

        public void SelectPlayerTab()
        {
            SetContainerStates(forgeActive: false, playerActive: true, relicActive: false);

            SetTabVisual(forgeTabImg, forgeTabText, forgeTabIcon, false);
            SetTabVisual(playerTabImg, playerTabText, playerTabIcon, true);
            SetTabVisual(relicTabImg, relicTabText, relicTabIcon, false);

            RefreshCardsIn(playerContainer);
        }

        public void SelectRelicTab()
        {
            SetContainerStates(forgeActive: false, playerActive: false, relicActive: true);

            SetTabVisual(forgeTabImg, forgeTabText, forgeTabIcon, false);
            SetTabVisual(playerTabImg, playerTabText, playerTabIcon, false);
            SetTabVisual(relicTabImg, relicTabText, relicTabIcon, true);

            RefreshCardsIn(relicContainer);
        }

        private void SetContainerStates(bool forgeActive, bool playerActive, bool relicActive)
        {
            if (forgeContainer != null) forgeContainer.SetActive(forgeActive);
            if (playerContainer != null) playerContainer.SetActive(playerActive);
            if (relicContainer != null) relicContainer.SetActive(relicActive);
        }

        private void SetTabVisual(Image bg, TMP_Text text, Image icon, bool isActive)
        {
            if (bg != null) bg.color = isActive ? activeBgColor : inactiveBgColor;
            if (text != null) text.color = isActive ? activeTextColor : inactiveTextColor;
            if (icon != null) icon.color = isActive ? activeTextColor : inactiveTextColor;
        }

        private void RefreshCardsIn(GameObject container)
        {
            if (container != null)
            {
                var cards = container.GetComponentsInChildren<UpgradeCardUI>(true);
                foreach (var card in cards)
                {
                    card.RefreshCard();
                }
            }
        }
    }
}
