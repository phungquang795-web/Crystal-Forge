using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class MilestonePanelUI : MonoBehaviour
    {
        [Header("Next Milestone Progress")]
        [SerializeField] private TMP_Text milestoneTitleText;
        [SerializeField] private TMP_Text progressRatioText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text rewardPreviewText;

        [Header("Completed Badges")]
        [SerializeField] private GameObject bronzeBadge;
        [SerializeField] private GameObject silverBadge;
        [SerializeField] private GameObject goldBadge;
        [SerializeField] private GameObject platinumBadge;
        [SerializeField] private GameObject guardianBadge;

        private void Start()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged += UpdateProgress;
                UpdateProgress(ResourceManager.Instance.LifetimeCrystal);
            }

            if (MilestoneManager.Instance != null)
            {
                MilestoneManager.Instance.OnMilestoneAchieved += (m) => RefreshBadges();
            }

            RefreshBadges();
        }

        private void OnDestroy()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.OnLifetimeCrystalChanged -= UpdateProgress;
            }
        }

        public void UpdateProgress(double lifetime)
        {
            if (MilestoneManager.Instance == null) return;

            var next = MilestoneManager.Instance.GetNextPendingMilestone();
            if (next != null)
            {
                string rName = !string.IsNullOrEmpty(next.relicName) ? $"Dị Vật: {next.relicName}" : next.milestoneTitle;
                if (milestoneTitleText != null) milestoneTitleText.text = FontSanitizer.Sanitize(rName);
                if (rewardPreviewText != null) rewardPreviewText.text = FontSanitizer.Sanitize(next.rewardSummaryText);

                float progress = (float)(lifetime / next.requiredLifetimeCrystal);
                progress = Mathf.Clamp01(progress);

                if (progressBar != null) progressBar.value = progress;
                if (progressRatioText != null)
                {
                    progressRatioText.text = $"{CrystalFormat.Format(lifetime)} / {CrystalFormat.Format(next.requiredLifetimeCrystal)} ({progress * 100f:F0}%)";
                }
            }
            else
            {
                if (milestoneTitleText != null) milestoneTitleText.text = "Tối Cao (Max Tier)";
                if (rewardPreviewText != null) rewardPreviewText.text = "Đã hoàn thành mọi mốc!";
                if (progressBar != null) progressBar.value = 1f;
                if (progressRatioText != null) progressRatioText.text = "100%";
            }

            RefreshBadges();
        }

        private void RefreshBadges()
        {
            if (MilestoneManager.Instance == null) return;

            UpdateBadgeVisual(bronzeBadge, 
                MilestoneManager.Instance.IsMilestoneCompleted(Core.MilestoneTier.Bronze),
                new Color(0.95f, 0.65f, 0.35f), 
                new Color(0.85f, 0.50f, 0.20f, 0.65f));

            UpdateBadgeVisual(silverBadge, 
                MilestoneManager.Instance.IsMilestoneCompleted(Core.MilestoneTier.Silver),
                new Color(0.90f, 0.92f, 1.0f), 
                new Color(0.75f, 0.85f, 0.95f, 0.65f));

            UpdateBadgeVisual(goldBadge, 
                MilestoneManager.Instance.IsMilestoneCompleted(Core.MilestoneTier.Gold),
                new Color(1.0f, 0.88f, 0.25f), 
                new Color(1.0f, 0.80f, 0.20f, 0.65f));

            UpdateBadgeVisual(platinumBadge, 
                MilestoneManager.Instance.IsMilestoneCompleted(Core.MilestoneTier.Platinum),
                new Color(0.45f, 0.90f, 1.0f), 
                new Color(0.35f, 0.80f, 1.0f, 0.65f));

            UpdateBadgeVisual(guardianBadge, 
                MilestoneManager.Instance.IsMilestoneCompleted(Core.MilestoneTier.Guardian),
                new Color(0.95f, 0.55f, 1.0f), 
                new Color(0.90f, 0.40f, 0.95f, 0.65f));
        }

        private void UpdateBadgeVisual(GameObject badgeObj, bool isCompleted, Color activeColor, Color activeBorderColor)
        {
            if (badgeObj == null) return;
            badgeObj.SetActive(true);

            var bg = badgeObj.GetComponent<Image>();
            if (bg != null)
            {
                bg.color = isCompleted 
                    ? new Color(0.18f, 0.12f, 0.35f, 0.95f) 
                    : new Color(0.10f, 0.08f, 0.18f, 0.50f);
            }

            var label = badgeObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.color = isCompleted 
                    ? activeColor 
                    : new Color(0.55f, 0.52f, 0.65f, 0.70f);
            }

            var icon = badgeObj.transform.Find("BadgeIcon")?.GetComponent<Image>();
            if (icon != null)
            {
                icon.color = isCompleted 
                    ? activeColor 
                    : new Color(0.40f, 0.38f, 0.50f, 0.45f);
            }

            var borderTrans = badgeObj.transform.Find("BadgeBorder");
            if (borderTrans != null)
            {
                var borderImgs = borderTrans.GetComponentsInChildren<Image>();
                Color borderCol = isCompleted ? activeBorderColor : new Color(0.28f, 0.24f, 0.38f, 0.35f);
                foreach (var b in borderImgs)
                {
                    b.color = borderCol;
                }
            }
        }
    }
}
