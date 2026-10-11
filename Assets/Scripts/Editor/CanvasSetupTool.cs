#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Core;
using CrystalForge.Economy;
using CrystalForge.UI;

namespace CrystalForge.Editor
{
    public static class CanvasSetupTool
    {
        [MenuItem("CrystalForge/2. Setup Single-Screen UI Canvas")]
        public static void BuildSingleScreenCanvas()
        {
            Undo.SetCurrentGroupName("Setup Single-Screen UI Canvas");
            int undoGroup = Undo.GetCurrentGroup();

            EnsurePrefabFolder();

            // 1. Tạo hoặc lấy EventSystem
            EnsureEventSystem();

            // 2. Tạo hoặc làm mới Canvas chính
            var canvasObj = GameObject.Find("MainCanvas");
            if (canvasObj != null) Object.DestroyImmediate(canvasObj);

            canvasObj = new GameObject("MainCanvas");
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create MainCanvas");

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // 3. Tạo FloatingText Prefab & Spawner
            var floatingPrefab = CreateOrLoadFloatingTextPrefab();
            SetupFloatingSpawner(floatingPrefab);

            // 4. Dựng Top Bar
            var topBarUI = BuildTopBar(canvasObj.transform);

            // 5. Dựng Left Panel (Milestones)
            var milestonePanelUI = BuildLeftMilestonePanel(canvasObj.transform);

            // 6. Dựng Right Panel (Upgrades)
            var rightPanelObj = BuildRightUpgradePanel(canvasObj.transform);

            // 7. Dựng Bottom Bar
            var bottomBarObj = BuildBottomBar(canvasObj.transform);

            // 8. Dựng Adventure Modal & Hệ thống Thám hiểm
            AdventureSetupTool.BuildAdventureSystemAndModal();

            // 9. Dựng VFX Particles & Âm thanh Procedural
            AudioVFXSetupTool.SetupAudioAndVFX();

            // 10. Dựng Modal End Game Panel (Khi Player ngã xuống)
            BuildEndGamePanel(canvasObj.transform);

            // 11. Dựng Modal Xác Nhận Thoát Game (Quit Confirmation Modal)
            BuildQuitModal(canvasObj.transform);

            // 12. Dựng Modal Hướng Dẫn Chiến Thuật (Tactical Help Guide Modal)
            BuildHelpModal(canvasObj.transform);

            // 13. Kết nối UIManager (Sau khi toàn bộ Panel và Modal đã được dựng)
            SetupUIManager(topBarUI, milestonePanelUI, rightPanelObj, bottomBarObj);

            // 14. Dựng World-Space Shield Bar trên đỉnh Trụ Tinh Thể
            BuildAltarShieldWorldCanvas();

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Đã dựng hoàn tất Single-Screen UI Canvas (Top HUD, Left Milestone, Right Upgrades, Bottom Bar, Altar Shield Bar, End Game Panel)!</color>");
        }

        private static void EnsureEventSystem()
        {
            var eventSys = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSys == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private static TopBarUI BuildTopBar(Transform canvas)
        {
            var topBarObj = CreateUIObject("TopBar_Panel", canvas);
            var rect = topBarObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 105f);

            var bgImg = topBarObj.AddComponent<Image>();
            bgImg.color = new Color(0.06f, 0.04f, 0.13f, 0.94f); // Dark glassmorphism cao cấp

            // Dải viền vàng kim đáy (Gold Accent Bottom Border)
            CreateHorizontalLine("BottomGoldBorder", topBarObj.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), 2f, new Color(0.95f, 0.75f, 0.22f, 0.65f));

            // Logo & Brand Name bên trái (Sprite Icon Thực Sự)
            var brandContainer = CreateUIObject("BrandContainer", topBarObj.transform);
            var brandRect = brandContainer.GetComponent<RectTransform>();
            SetRect(brandRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(25f, 0f), new Vector2(280f, 70f));

            CreateSpriteImage("LogoIcon", brandContainer.transform, LoadSprite("icon_crystal.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0f), new Vector2(38f, 38f));

            var title = CreateText("TitleText", brandContainer.transform, "CRYSTAL FORGE", 22, FontStyles.Bold, new Color(0.95f, 0.88f, 1.0f));
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(46f, -8f), new Vector2(-46f, 30f));
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;

            var subtitle = CreateText("SubtitleText", brandContainer.transform, "LO REN CO NGU", 11, FontStyles.Bold, new Color(0.72f, 0.65f, 0.88f));
            SetRect(subtitle.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(46f, 10f), new Vector2(-46f, 20f));
            subtitle.alignment = TextAlignmentOptions.MidlineLeft;
            subtitle.textWrappingMode = TextWrappingModes.NoWrap;

            // Khung Trung Tâm Dạng Viên Thuốc (Central Vault Pill Capsule)
            var vaultObj = CreateUIObject("VaultCapsule", topBarObj.transform);
            var vaultRect = vaultObj.GetComponent<RectTransform>();
            SetRect(vaultRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(460f, 84f));

            var vaultBg = vaultObj.AddComponent<Image>();
            vaultBg.sprite = LoadSprite("pill_frame.png");
            vaultBg.type = Image.Type.Sliced;
            vaultBg.color = new Color(0.12f, 0.08f, 0.24f, 0.98f);
            CreateBorder("VaultBorder", vaultObj.transform, new Color(0.95f, 0.78f, 0.25f, 0.65f), 1.5f);

            // Icon Crystal lớn nổi bật bên trái Vault Capsule
            CreateSpriteImage("VaultCrystalIcon", vaultObj.transform, LoadSprite("icon_crystal.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(24f, 0f), new Vector2(44f, 44f));

            // Current Crystal (Số Pha Lê hiện có - To nổi bật, đơn vị CR)
            var crystalText = CreateText("CurrentCrystalText", vaultObj.transform, "0 CR", 34, FontStyles.Bold, new Color(1.0f, 0.88f, 0.22f));
            SetRect(crystalText.rectTransform, new Vector2(0f, 0.46f), new Vector2(1f, 1f), new Vector2(0f, 0.5f), new Vector2(76f, 0f), new Vector2(-90f, 0f));
            crystalText.alignment = TextAlignmentOptions.MidlineLeft;
            crystalText.textWrappingMode = TextWrappingModes.NoWrap;

            // Production / s (Tốc độ sản xuất ngọc lục bảo)
            var prodText = CreateText("ProductionRateText", vaultObj.transform, "+0.00/s", 16, FontStyles.Bold, new Color(0.35f, 1.0f, 0.65f));
            SetRect(prodText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.46f), new Vector2(0f, 0.5f), new Vector2(76f, 0f), new Vector2(-90f, 0f));
            prodText.alignment = TextAlignmentOptions.MidlineLeft;
            prodText.textWrappingMode = TextWrappingModes.NoWrap;

            // Buff Badge (Huy hiệu Overdrive / Event Buff nhấp nháy bên cạnh Vault)
            var buffBadge = CreateUIObject("BuffBadge", topBarObj.transform);
            var badgeRect = buffBadge.GetComponent<RectTransform>();
            SetRect(badgeRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-245f, 0f), new Vector2(136f, 40f));
            var badgeImg = buffBadge.AddComponent<Image>();
            badgeImg.sprite = LoadSprite("pill_frame.png");
            badgeImg.type = Image.Type.Sliced;
            badgeImg.color = new Color(0.95f, 0.25f, 0.15f, 0.95f);
            CreateBorder("BuffBorder", buffBadge.transform, new Color(1.0f, 0.85f, 0.2f, 0.8f), 1.5f);

            var buffText = CreateText("BuffText", buffBadge.transform, "x3.0 OVERDRIVE", 13, FontStyles.Bold, Color.white);
            buffText.alignment = TextAlignmentOptions.Center;
            buffText.textWrappingMode = TextWrappingModes.NoWrap;
            SetRect(buffText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            buffBadge.SetActive(false);

            // Fever Badge (Huy hiệu Sốt Gõ Fever Rush cam rực rỡ bên cạnh BuffBadge)
            var feverBadge = CreateUIObject("FeverBadge", topBarObj.transform);
            var feverRect = feverBadge.GetComponent<RectTransform>();
            SetRect(feverRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-235f, 0f), new Vector2(136f, 40f));
            var feverImg = feverBadge.AddComponent<Image>();
            feverImg.sprite = LoadSprite("pill_frame.png");
            feverImg.type = Image.Type.Sliced;
            feverImg.color = new Color(0.95f, 0.45f, 0.1f, 0.95f);
            CreateBorder("FeverBorder", feverBadge.transform, new Color(1.0f, 0.75f, 0.2f, 0.85f), 1.5f);

            var feverText = CreateText("FeverText", feverBadge.transform, "FEVER x2", 13, FontStyles.Bold, Color.white);
            feverText.alignment = TextAlignmentOptions.Center;
            feverText.textWrappingMode = TextWrappingModes.NoWrap;
            SetRect(feverText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            feverBadge.SetActive(false);

            // Hai Pill Thông Số Bên Phải (Quick Stats Pills với Icon Sprite)
            var tapPill = CreateUIObject("TapPowerPill", topBarObj.transform);
            var tapPillRect = tapPill.GetComponent<RectTransform>();
            SetRect(tapPillRect, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-215f, 0f), new Vector2(185f, 46f));
            var tapPillBg = tapPill.AddComponent<Image>();
            tapPillBg.sprite = LoadSprite("pill_frame.png");
            tapPillBg.type = Image.Type.Sliced;
            tapPillBg.color = new Color(0.12f, 0.10f, 0.25f, 0.94f);
            CreateBorder("TapBorder", tapPill.transform, new Color(0.25f, 0.60f, 0.95f, 0.55f), 1.5f);

            CreateSpriteImage("TapIcon", tapPill.transform, LoadSprite("icon_hammer.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(26f, 26f));
            var tapText = CreateText("TapPowerText", tapPill.transform, "Luc go: +1", 15, FontStyles.Bold, new Color(0.35f, 0.85f, 1.0f));
            SetRect(tapText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(18f, 0f), new Vector2(-16f, 0f));
            tapText.alignment = TextAlignmentOptions.Center;
            tapText.textWrappingMode = TextWrappingModes.NoWrap;

            var lifePill = CreateUIObject("LifetimePill", topBarObj.transform);
            var lifePillRect = lifePill.GetComponent<RectTransform>();
            SetRect(lifePillRect, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-20f, 0f), new Vector2(185f, 46f));
            var lifePillBg = lifePill.AddComponent<Image>();
            lifePillBg.sprite = LoadSprite("pill_frame.png");
            lifePillBg.type = Image.Type.Sliced;
            lifePillBg.color = new Color(0.12f, 0.10f, 0.25f, 0.94f);
            CreateBorder("LifeBorder", lifePill.transform, new Color(0.65f, 0.45f, 0.95f, 0.55f), 1.5f);

            CreateSpriteImage("CrownIcon", lifePill.transform, LoadSprite("icon_crown.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(26f, 26f));
            var lifetimeText = CreateText("LifetimeText", lifePill.transform, "Tong: 0", 15, FontStyles.Bold, new Color(0.85f, 0.75f, 1.0f));
            SetRect(lifetimeText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(18f, 0f), new Vector2(-16f, 0f));
            lifetimeText.alignment = TextAlignmentOptions.Center;
            lifetimeText.textWrappingMode = TextWrappingModes.NoWrap;

            // Gán TopBarUI
            var topBarUI = topBarObj.AddComponent<TopBarUI>();
            var so = new SerializedObject(topBarUI);
            so.FindProperty("currentCrystalText").objectReferenceValue = crystalText;
            so.FindProperty("lifetimeCrystalText").objectReferenceValue = lifetimeText;
            so.FindProperty("productionRateText").objectReferenceValue = prodText;
            so.FindProperty("tapPowerText").objectReferenceValue = tapText;
            so.FindProperty("buffBadgeObj").objectReferenceValue = buffBadge;
            so.FindProperty("buffMultiplierText").objectReferenceValue = buffText;
            so.FindProperty("feverBadgeObj").objectReferenceValue = feverBadge;
            so.FindProperty("feverText").objectReferenceValue = feverText;
            so.ApplyModifiedProperties();

            return topBarUI;
        }

        private static MilestonePanelUI BuildLeftMilestonePanel(Transform canvas)
        {
            var panelObj = CreateUIObject("LeftPanel_Milestones", canvas);
            var rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(22f, -8f);
            rect.sizeDelta = new Vector2(350f, 680f);

            var bg = panelObj.AddComponent<Image>();
            bg.sprite = LoadSprite("rounded_box.png");
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.06f, 0.04f, 0.14f, 0.94f);
            CreateBorder("PanelBorder", panelObj.transform, new Color(0.85f, 0.65f, 0.25f, 0.35f), 1.5f);

            // Dải Banner Header Hoàng Gia
            var headerObj = CreateUIObject("HeaderBanner", panelObj.transform);
            var headerRect = headerObj.GetComponent<RectTransform>();
            SetRect(headerRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -14f), new Vector2(-28f, 46f));
            var headerBg = headerObj.AddComponent<Image>();
            headerBg.sprite = LoadSprite("pill_frame.png");
            headerBg.type = Image.Type.Sliced;
            headerBg.color = new Color(0.18f, 0.12f, 0.35f, 0.98f);
            CreateBorder("HeaderBorder", headerObj.transform, new Color(0.95f, 0.75f, 0.20f, 0.55f), 1.5f);

            CreateSpriteImage("TrophyIcon", headerObj.transform, LoadSprite("icon_trophy.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(28f, 28f));
            var header = CreateText("HeaderText", headerObj.transform, "DI VAT CO DAI", 16, FontStyles.Bold, new Color(1.0f, 0.88f, 0.35f));
            SetRect(header.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(22f, 0f), new Vector2(-20f, 0f));
            header.alignment = TextAlignmentOptions.Center;
            header.textWrappingMode = TextWrappingModes.NoWrap;

            // Khung Mục Tiêu Kế Tiếp (Next Milestone Target Card)
            var nextCardObj = CreateUIObject("NextMilestoneCard", panelObj.transform);
            var nextCardRect = nextCardObj.GetComponent<RectTransform>();
            SetRect(nextCardRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(-28f, 42f));
            var nextCardBg = nextCardObj.AddComponent<Image>();
            nextCardBg.sprite = LoadSprite("rounded_box.png");
            nextCardBg.type = Image.Type.Sliced;
            nextCardBg.color = new Color(0.12f, 0.09f, 0.24f, 0.92f);
            CreateBorder("NextCardBorder", nextCardObj.transform, new Color(0.45f, 0.35f, 0.65f, 0.45f), 1.0f);

            CreateSpriteImage("StarIcon", nextCardObj.transform, LoadSprite("icon_star.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(24f, 24f));
            var title = CreateText("NextTitle", nextCardObj.transform, "Di Vat: Bua Ren Runic", 15, FontStyles.Bold, Color.white);
            SetRect(title.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, 0.5f), new Vector2(44f, 0f), new Vector2(-50f, 30f));
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;

            // Thanh Tiến Độ Hoàng Kim (Milestone Slider Bar)
            var sliderObj = CreateUIObject("MilestoneSlider", panelObj.transform);
            var sliderRect = sliderObj.GetComponent<RectTransform>();
            SetRect(sliderRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -120f), new Vector2(-28f, 24f));
            var slider = sliderObj.AddComponent<Slider>();

            var slideBg = CreateUIObject("Background", sliderObj.transform);
            SetRect(slideBg.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var slideBgImg = slideBg.AddComponent<Image>();
            slideBgImg.sprite = LoadSprite("pill_frame.png");
            slideBgImg.type = Image.Type.Sliced;
            slideBgImg.color = new Color(0.12f, 0.09f, 0.24f, 1.0f);
            CreateBorder("SliderBorder", slideBg.transform, new Color(0.35f, 0.28f, 0.50f, 0.6f), 1.0f);

            var fillArea = CreateUIObject("Fill Area", sliderObj.transform);
            SetRect(fillArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var fill = CreateUIObject("Fill", fillArea.transform);
            SetRect(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var fillImg = fill.AddComponent<Image>();
            fillImg.sprite = LoadSprite("pill_frame.png");
            fillImg.type = Image.Type.Sliced;
            fillImg.color = new Color(1.0f, 0.80f, 0.18f);
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Progress text
            var ratio = CreateText("ProgressRatio", panelObj.transform, "0 / 100 (0%)", 15, FontStyles.Bold, Color.white);
            SetRect(ratio.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(-28f, 24f));
            ratio.alignment = TextAlignmentOptions.Center;
            ratio.textWrappingMode = TextWrappingModes.NoWrap;

            // Reward preview
            var reward = CreateText("RewardText", panelObj.transform, "Thuong: Mo khoa Crystal Drill", 14, FontStyles.Italic, new Color(0.40f, 1.0f, 0.75f));
            SetRect(reward.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -178f), new Vector2(-28f, 26f));
            reward.alignment = TextAlignmentOptions.Center;
            reward.textWrappingMode = TextWrappingModes.NoWrap;

            // Badges Section (Dải phân cách & Danh sách danh hiệu)
            CreateHorizontalLine("BadgesDivider", panelObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), 1.5f, new Color(0.35f, 0.28f, 0.55f, 0.5f), new Vector2(0f, -210f), -28f);

            var badgesHeader = CreateText("BadgesHeader", panelObj.transform, "DANH HIEU DA DAT", 15, FontStyles.Bold, new Color(0.85f, 0.82f, 0.95f));
            SetRect(badgesHeader.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -226f), new Vector2(-28f, 26f));
            badgesHeader.alignment = TextAlignmentOptions.Center;
            badgesHeader.textWrappingMode = TextWrappingModes.NoWrap;

            var b1 = CreateBadgeItem("Badge_Bronze", panelObj.transform, "Dong Co (100)", new Vector2(0f, -264f), new Color(0.95f, 0.65f, 0.35f), new Color(0.85f, 0.50f, 0.20f, 0.65f));
            var b2 = CreateBadgeItem("Badge_Silver", panelObj.transform, "Bac Sang (2.5K)", new Vector2(0f, -318f), new Color(0.90f, 0.92f, 1.0f), new Color(0.75f, 0.85f, 0.95f, 0.65f));
            var b3 = CreateBadgeItem("Badge_Gold", panelObj.transform, "Vang Rong (25K)", new Vector2(0f, -372f), new Color(1.0f, 0.88f, 0.25f), new Color(1.0f, 0.80f, 0.20f, 0.65f));
            var b4 = CreateBadgeItem("Badge_Platinum", panelObj.transform, "Bach Kim (250K)", new Vector2(0f, -426f), new Color(0.45f, 0.90f, 1.0f), new Color(0.35f, 0.80f, 1.0f, 0.65f));
            var b5 = CreateBadgeItem("Badge_Guardian", panelObj.transform, "Ho Ve Than (1M)", new Vector2(0f, -480f), new Color(0.95f, 0.55f, 1.0f), new Color(0.90f, 0.40f, 0.95f, 0.65f));

            // Mẹo Chiến Thuật & Hướng Dẫn Nhanh (Tactical Tip Card lấp đầy đáy panel cân đối)
            CreateHorizontalLine("GuideDivider", panelObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), 1.5f, new Color(0.35f, 0.28f, 0.55f, 0.5f), new Vector2(0f, -534f), -28f);

            var tipCardObj = CreateUIObject("TacticalTipCard", panelObj.transform);
            var tipCardRect = tipCardObj.GetComponent<RectTransform>();
            SetRect(tipCardRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -548f), new Vector2(-28f, 116f));
            var tipCardBg = tipCardObj.AddComponent<Image>();
            tipCardBg.sprite = LoadSprite("rounded_box.png");
            tipCardBg.type = Image.Type.Sliced;
            tipCardBg.color = new Color(0.11f, 0.08f, 0.22f, 0.94f);
            CreateBorder("TipCardBorder", tipCardObj.transform, new Color(0.95f, 0.78f, 0.25f, 0.45f), 1.5f);

            CreateSpriteImage("TipIcon", tipCardObj.transform, LoadSprite("icon_hammer.png"), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(14f, -14f), new Vector2(26f, 26f), new Color(1.0f, 0.85f, 0.35f));

            var tipTitle = CreateText("TipTitle", tipCardObj.transform, "MEO CHIEN THUAT", 13, FontStyles.Bold, new Color(1.0f, 0.88f, 0.35f));
            SetRect(tipTitle.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(46f, -14f), new Vector2(-55f, 22f));
            tipTitle.alignment = TextAlignmentOptions.MidlineLeft;
            tipTitle.textWrappingMode = TextWrappingModes.NoWrap;

            var tipContent = CreateText("TipContent", tipCardObj.transform, "Nhap chuot truc tiep vao Tru Tinh The hoac Quai Vat de khai thac Pha Le va sac No Co Ngu!", 12, FontStyles.Normal, new Color(0.85f, 0.88f, 0.95f));
            SetRect(tipContent.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(14f, 8f), new Vector2(-28f, -44f));
            tipContent.alignment = TextAlignmentOptions.TopLeft;
            tipContent.textWrappingMode = TextWrappingModes.Normal;

            var panelUI = panelObj.AddComponent<MilestonePanelUI>();
            var so = new SerializedObject(panelUI);
            so.FindProperty("milestoneTitleText").objectReferenceValue = title;
            so.FindProperty("progressRatioText").objectReferenceValue = ratio;
            so.FindProperty("progressBar").objectReferenceValue = slider;
            so.FindProperty("rewardPreviewText").objectReferenceValue = reward;
            so.FindProperty("bronzeBadge").objectReferenceValue = b1;
            so.FindProperty("silverBadge").objectReferenceValue = b2;
            so.FindProperty("goldBadge").objectReferenceValue = b3;
            so.FindProperty("platinumBadge").objectReferenceValue = b4;
            so.FindProperty("guardianBadge").objectReferenceValue = b5;
            so.ApplyModifiedProperties();

            return panelUI;
        }

        private static GameObject CreateBadgeItem(string name, Transform parent, string text, Vector2 anchoredPos, Color textColor, Color borderColor)
        {
            var item = CreateUIObject(name, parent);
            var rect = item.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), anchoredPos, new Vector2(-28f, 44f));

            var img = item.AddComponent<Image>();
            img.sprite = LoadSprite("pill_frame.png");
            img.type = Image.Type.Sliced;
            img.color = new Color(0.14f, 0.11f, 0.24f, 0.92f);
            CreateBorder("BadgeBorder", item.transform, borderColor, 1.5f);

            CreateSpriteImage("BadgeIcon", item.transform, LoadSprite("icon_star.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(22f, 22f), textColor);

            var label = CreateText("Label", item.transform, text, 15, FontStyles.Bold, textColor);
            SetRect(label.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(20f, 0f), new Vector2(-25f, 0f));
            label.alignment = TextAlignmentOptions.Center;
            label.textWrappingMode = TextWrappingModes.NoWrap;

            item.SetActive(false); // Mặc định ẩn, hiện khi đạt
            return item;
        }

        private static GameObject BuildRightUpgradePanel(Transform canvas)
        {
            var panelObj = CreateUIObject("RightPanel_Upgrades", canvas);
            var rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(-22f, -8f);
            rect.sizeDelta = new Vector2(410f, 680f);

            var bg = panelObj.AddComponent<Image>();
            bg.sprite = LoadSprite("rounded_box.png");
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.06f, 0.04f, 0.14f, 0.94f);
            CreateBorder("PanelBorder", panelObj.transform, new Color(0.85f, 0.65f, 0.25f, 0.35f), 1.5f);

            // Tab Bar Segmented Control (2 Tab bo tròn hiện đại với Icon Sprite)
            var tabForgeObj = CreateUIObject("TabButton_Forge", panelObj.transform);
            var tabForgeRect = tabForgeObj.GetComponent<RectTransform>();
            SetRect(tabForgeRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(-96f, -14f), new Vector2(182f, 44f));
            var tabForgeImg = tabForgeObj.AddComponent<Image>();
            tabForgeImg.sprite = LoadSprite("pill_frame.png");
            tabForgeImg.type = Image.Type.Sliced;
            tabForgeImg.color = new Color(0.95f, 0.75f, 0.22f, 0.95f);
            CreateBorder("ForgeTabBorder", tabForgeObj.transform, new Color(1.0f, 0.88f, 0.4f, 0.7f), 1.5f);
            var tabForgeBtn = tabForgeObj.AddComponent<Button>();

            var tabForgeIcon = CreateSpriteImage("TabIcon", tabForgeObj.transform, LoadSprite("icon_drill.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(16f, 0f), new Vector2(22f, 22f), new Color(0.12f, 0.08f, 0.04f));
            var tabForgeText = CreateText("Label", tabForgeObj.transform, "LO REN", 15, FontStyles.Bold, new Color(0.12f, 0.08f, 0.04f));
            SetRect(tabForgeText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(16f, 0f), new Vector2(-16f, 0f));
            tabForgeText.alignment = TextAlignmentOptions.Center;
            tabForgeText.textWrappingMode = TextWrappingModes.NoWrap;

            var tabPlayerObj = CreateUIObject("TabButton_Player", panelObj.transform);
            var tabPlayerRect = tabPlayerObj.GetComponent<RectTransform>();
            SetRect(tabPlayerRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(96f, -14f), new Vector2(182f, 44f));
            var tabPlayerImg = tabPlayerObj.AddComponent<Image>();
            tabPlayerImg.sprite = LoadSprite("pill_frame.png");
            tabPlayerImg.type = Image.Type.Sliced;
            tabPlayerImg.color = new Color(0.16f, 0.12f, 0.28f, 0.85f);
            CreateBorder("PlayerTabBorder", tabPlayerObj.transform, new Color(0.4f, 0.3f, 0.6f, 0.4f), 1.5f);
            var tabPlayerBtn = tabPlayerObj.AddComponent<Button>();

            var tabPlayerIcon = CreateSpriteImage("TabIcon", tabPlayerObj.transform, LoadSprite("icon_sword.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(16f, 0f), new Vector2(22f, 22f), new Color(0.85f, 0.85f, 0.95f));
            var tabPlayerText = CreateText("Label", tabPlayerObj.transform, "DUNG SI", 15, FontStyles.Bold, new Color(0.85f, 0.85f, 0.95f));
            SetRect(tabPlayerText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(16f, 0f), new Vector2(-16f, 0f));
            tabPlayerText.alignment = TextAlignmentOptions.Center;
            tabPlayerText.textWrappingMode = TextWrappingModes.NoWrap;

            // Dải phân cách dưới Tab Bar
            CreateHorizontalLine("TabsDivider", panelObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), 1.5f, new Color(0.35f, 0.28f, 0.55f, 0.5f), new Vector2(0f, -66f), -24f);

            // Container 1: Forge Upgrades
            var forgeContainer = CreateUIObject("ForgeUpgrades_Container", panelObj.transform);
            var forgeRect = forgeContainer.GetComponent<RectTransform>();
            SetRect(forgeRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -36f), new Vector2(0f, -76f));

            BuildUpgradeCard(forgeContainer.transform, "Card_TapPower", UpgradeCardUI.CardType.TapPower, GeneratorType.CrystalDrill, new Vector2(0f, -6f));
            BuildUpgradeCard(forgeContainer.transform, "Card_Drill", UpgradeCardUI.CardType.Generator, GeneratorType.CrystalDrill, new Vector2(0f, -150f));
            BuildUpgradeCard(forgeContainer.transform, "Card_Furnace", UpgradeCardUI.CardType.Generator, GeneratorType.ManaFurnace, new Vector2(0f, -294f));
            BuildUpgradeCard(forgeContainer.transform, "Card_Extractor", UpgradeCardUI.CardType.Generator, GeneratorType.AutoExtractor, new Vector2(0f, -438f));

            // Container 2: Player Upgrades
            var playerContainer = CreateUIObject("PlayerUpgrades_Container", panelObj.transform);
            var playerRect = playerContainer.GetComponent<RectTransform>();
            SetRect(playerRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -36f), new Vector2(0f, -76f));

            BuildUpgradeCard(playerContainer.transform, "Card_PlayerHp", UpgradeCardUI.CardType.PlayerHp, GeneratorType.CrystalDrill, new Vector2(0f, -6f));
            BuildUpgradeCard(playerContainer.transform, "Card_PlayerAtk", UpgradeCardUI.CardType.PlayerAtk, GeneratorType.CrystalDrill, new Vector2(0f, -150f));
            BuildUpgradeCard(playerContainer.transform, "Card_PlayerSpeed", UpgradeCardUI.CardType.PlayerSpeed, GeneratorType.CrystalDrill, new Vector2(0f, -294f));
            BuildUpgradeCard(playerContainer.transform, "Card_PlayerSkill", UpgradeCardUI.CardType.PlayerSkill, GeneratorType.CrystalDrill, new Vector2(0f, -438f));

            playerContainer.SetActive(false);

            // Gán UpgradeTabSwitcher
            var switcher = panelObj.AddComponent<UpgradeTabSwitcher>();
            var swSo = new SerializedObject(switcher);
            swSo.FindProperty("forgeTabButton").objectReferenceValue = tabForgeBtn;
            swSo.FindProperty("playerTabButton").objectReferenceValue = tabPlayerBtn;
            swSo.FindProperty("forgeTabImg").objectReferenceValue = tabForgeImg;
            swSo.FindProperty("playerTabImg").objectReferenceValue = tabPlayerImg;
            swSo.FindProperty("forgeTabText").objectReferenceValue = tabForgeText;
            swSo.FindProperty("playerTabText").objectReferenceValue = tabPlayerText;
            swSo.FindProperty("forgeTabIcon").objectReferenceValue = tabForgeIcon;
            swSo.FindProperty("playerTabIcon").objectReferenceValue = tabPlayerIcon;
            swSo.FindProperty("forgeContainer").objectReferenceValue = forgeContainer;
            swSo.FindProperty("playerContainer").objectReferenceValue = playerContainer;
            swSo.ApplyModifiedProperties();

            return panelObj;
        }

        private static void BuildUpgradeCard(Transform parent, string name, UpgradeCardUI.CardType cardType, GeneratorType genType, Vector2 anchoredPos)
        {
            var card = CreateUIObject(name, parent);
            var rect = card.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), anchoredPos, new Vector2(-22f, 134f));

            var bg = card.AddComponent<Image>();
            bg.sprite = LoadSprite("rounded_box.png");
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.10f, 0.07f, 0.20f, 0.96f);
            CreateBorder("CardBorder", card.transform, new Color(0.30f, 0.24f, 0.48f, 0.75f), 1.5f);

            string spriteName = "icon_hammer.png";
            string defaultTitle = "Nang Cap";
            string defaultOutput = "+0/s";
            string defaultCost = "100 CR";

            switch (cardType)
            {
                case UpgradeCardUI.CardType.TapPower:
                    spriteName = "icon_hammer.png";
                    defaultTitle = "Luc Go Pha Le";
                    defaultOutput = "+1 / Click";
                    defaultCost = "25 CR";
                    break;
                case UpgradeCardUI.CardType.Generator:
                    if (genType == GeneratorType.CrystalDrill) { spriteName = "icon_drill.png"; defaultTitle = "Crystal Drill"; defaultOutput = "+1.0/s"; defaultCost = "100 CR"; }
                    else if (genType == GeneratorType.ManaFurnace) { spriteName = "icon_furnace.png"; defaultTitle = "Mana Furnace"; defaultOutput = "+8.0/s"; defaultCost = "2.5K CR"; }
                    else if (genType == GeneratorType.AutoExtractor) { spriteName = "icon_extractor.png"; defaultTitle = "Auto Extractor"; defaultOutput = "+50.0/s"; defaultCost = "25K CR"; }
                    break;
                case UpgradeCardUI.CardType.PlayerHp:
                    spriteName = "icon_heart.png";
                    defaultTitle = "THE LUC (HP)";
                    defaultOutput = "+150 Max HP";
                    defaultCost = "40 CR";
                    break;
                case UpgradeCardUI.CardType.PlayerAtk:
                    spriteName = "icon_sword.png";
                    defaultTitle = "CONG KICH (ATK)";
                    defaultOutput = "+25 Sat Thuong";
                    defaultCost = "60 CR";
                    break;
                case UpgradeCardUI.CardType.PlayerSpeed:
                    spriteName = "icon_speed.png";
                    defaultTitle = "NHANH NHEN (SPD)";
                    defaultOutput = "+5% Toc Do Danh";
                    defaultCost = "80 CR";
                    break;
                case UpgradeCardUI.CardType.PlayerSkill:
                    spriteName = "icon_skill.png";
                    defaultTitle = "CO NGU (SKILL)";
                    defaultOutput = "+30 Dmg & +2% Crit";
                    defaultCost = "120 CR";
                    break;
            }

            // 1. Icon Box bên trái (58x58px, nền tím viền vàng, Chứa Sprite Hình Ảnh Đẹp)
            var iconBoxObj = CreateUIObject("IconBox", card.transform);
            var iconBoxRect = iconBoxObj.GetComponent<RectTransform>();
            SetRect(iconBoxRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(58f, 58f));
            var iconBoxBg = iconBoxObj.AddComponent<Image>();
            iconBoxBg.sprite = LoadSprite("rounded_box.png");
            iconBoxBg.type = Image.Type.Sliced;
            iconBoxBg.color = new Color(0.18f, 0.13f, 0.35f, 1.0f);
            CreateBorder("IconBoxBorder", iconBoxObj.transform, new Color(0.95f, 0.75f, 0.25f, 0.55f), 1.5f);

            var iconImg = CreateSpriteImage("IconImage", iconBoxObj.transform, LoadSprite(spriteName), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-12f, -12f));

            // 2. Nội dung Text giữa thẻ
            var title = CreateText("Title", card.transform, defaultTitle, 16, FontStyles.Bold, new Color(0.98f, 0.98f, 1.0f));
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -14f), new Vector2(-205f, 26f));
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;

            var level = CreateText("Level", card.transform, "Cap 0", 14, FontStyles.Bold, new Color(0.85f, 0.75f, 1.0f));
            SetRect(level.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -42f), new Vector2(-205f, 24f));
            level.alignment = TextAlignmentOptions.MidlineLeft;
            level.textWrappingMode = TextWrappingModes.NoWrap;

            var output = CreateText("Output", card.transform, defaultOutput, 15, FontStyles.Bold, new Color(0.35f, 1.0f, 0.65f));
            SetRect(output.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -70f), new Vector2(-205f, 26f));
            output.alignment = TextAlignmentOptions.MidlineLeft;
            output.textWrappingMode = TextWrappingModes.NoWrap;

            // 3. Buy Button bên phải (110x80px, đổi màu theo tài nguyên)
            var btnObj = CreateUIObject("BuyButton", card.transform);
            var btnRect = btnObj.GetComponent<RectTransform>();
            SetRect(btnRect, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-14f, 0f), new Vector2(110f, 80f));

            var btnImg = btnObj.AddComponent<Image>();
            btnImg.sprite = LoadSprite("pill_frame.png");
            btnImg.type = Image.Type.Sliced;
            btnImg.color = new Color(0.05f, 0.60f, 0.40f, 1.0f);
            CreateBorder("BuyButtonBorder", btnObj.transform, new Color(0.35f, 1.0f, 0.75f, 0.55f), 1.5f);
            var btn = btnObj.AddComponent<Button>();

            var btnLabel = CreateText("BtnLabel", btnObj.transform, "MUA", 14, FontStyles.Bold, new Color(0.92f, 0.98f, 0.95f));
            SetRect(btnLabel.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -10f), new Vector2(0f, 24f));
            btnLabel.alignment = TextAlignmentOptions.Center;
            btnLabel.textWrappingMode = TextWrappingModes.NoWrap;

            var costText = CreateText("CostText", btnObj.transform, defaultCost, 15, FontStyles.Bold, new Color(1.0f, 0.92f, 0.25f));
            SetRect(costText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0.55f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            costText.alignment = TextAlignmentOptions.Center;
            costText.textWrappingMode = TextWrappingModes.NoWrap;
            costText.enableAutoSizing = true;
            costText.fontSizeMin = 10;
            costText.fontSizeMax = 15;

            // 4. Lock Overlay (Khi chưa đạt điều kiện mở khóa)
            var lockObj = CreateUIObject("LockOverlay", card.transform);
            var lockRect = lockObj.GetComponent<RectTransform>();
            SetRect(lockRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var lockImg = lockObj.AddComponent<Image>();
            lockImg.sprite = LoadSprite("rounded_box.png");
            lockImg.type = Image.Type.Sliced;
            lockImg.color = new Color(0.06f, 0.04f, 0.12f, 0.95f);
            CreateBorder("LockBorder", lockObj.transform, new Color(0.6f, 0.25f, 0.25f, 0.6f), 1.5f);

            var lockText = CreateText("LockText", lockObj.transform, "KHOA", 15, FontStyles.Bold, new Color(1.0f, 0.45f, 0.45f));
            SetRect(lockText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            lockText.alignment = TextAlignmentOptions.Center;
            lockObj.SetActive(false);

            // Gắn Script UpgradeCardUI & Liên kết đầy đủ các Serialized Fields
            var cardUI = card.AddComponent<UpgradeCardUI>();
            var so = new SerializedObject(cardUI);
            so.FindProperty("cardType").enumValueIndex = (int)cardType;
            so.FindProperty("generatorType").enumValueIndex = (int)genType;
            so.FindProperty("iconImage").objectReferenceValue = iconImg;
            so.FindProperty("titleText").objectReferenceValue = title;
            so.FindProperty("levelText").objectReferenceValue = level;
            so.FindProperty("outputText").objectReferenceValue = output;
            so.FindProperty("costText").objectReferenceValue = costText;
            so.FindProperty("buyButton").objectReferenceValue = btn;
            so.FindProperty("buyButtonImage").objectReferenceValue = btnImg;
            so.FindProperty("lockOverlay").objectReferenceValue = lockObj;
            so.FindProperty("lockRequirementText").objectReferenceValue = lockText;
            so.ApplyModifiedProperties();
        }

        private static GameObject BuildBottomBar(Transform canvas)
        {
            var barObj = CreateUIObject("BottomBar_Panel", canvas);
            var rect = barObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 95f);

            var bgImg = barObj.AddComponent<Image>();
            bgImg.color = new Color(0.05f, 0.03f, 0.11f, 0.96f);

            // Dải viền mạ vàng đỉnh (Gold Accent Top Border)
            CreateHorizontalLine("TopGoldBorder", barObj.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), 2f, new Color(0.95f, 0.75f, 0.22f, 0.65f));

            // HUD Trung Tâm - Banner Chiến Trường & Nộ Cổ Ngữ (Pill Capsule 560x68px)
            var advBtnObj = CreateUIObject("StartAdventureBtn", barObj.transform);
            var advBtnRect = advBtnObj.GetComponent<RectTransform>();
            SetRect(advBtnRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 68f));

            var advBtnImg = advBtnObj.AddComponent<Image>();
            advBtnImg.sprite = LoadSprite("pill_frame.png");
            advBtnImg.type = Image.Type.Sliced;
            advBtnImg.color = new Color(0.18f, 0.13f, 0.32f, 0.98f);
            CreateBorder("BattleHudBorder", advBtnObj.transform, new Color(0.95f, 0.70f, 0.20f, 0.75f), 1.5f);

            var advBtn = advBtnObj.AddComponent<Button>();
            advBtn.interactable = false;

            CreateSpriteImage("BattleIcon", advBtnObj.transform, LoadSprite("icon_sword.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(22f, 0f), new Vector2(30f, 30f), new Color(1.0f, 0.85f, 0.35f));

            var battleText = CreateText("BattleHudText", advBtnObj.transform, "CHIEN TRUONG: DOT #1  |  NO: 0%", 17, FontStyles.Bold, Color.white);
            SetRect(battleText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(20f, 0f), new Vector2(-20f, 0f));
            battleText.alignment = TextAlignmentOptions.Center;
            battleText.textWrappingMode = TextWrappingModes.NoWrap;

            // Nút Âm Thanh bên trái (Cụm Công Cụ Hệ Thống)
            var soundBtn = CreateIconButton("SoundBtn", barObj.transform, "AM THANH", LoadSprite("icon_sound.png"), new Vector2(0f, 0.5f), new Vector2(35f, 0f), new Vector2(160f, 54f), new Color(0.18f, 0.14f, 0.30f, 0.95f), new Color(0.45f, 0.90f, 1.0f));
            CreateBorder("SoundBorder", soundBtn.gameObject.transform, new Color(0.45f, 0.35f, 0.65f, 0.6f), 1.5f);

            // Nút Lưu Game bên trái cạnh Âm Thanh
            var saveBtn = CreateIconButton("SaveGameBtn", barObj.transform, "LUU GAME", LoadSprite("icon_save.png"), new Vector2(0f, 0.5f), new Vector2(210f, 0f), new Vector2(160f, 54f), new Color(0.12f, 0.32f, 0.65f, 0.95f), new Color(0.40f, 0.85f, 1.0f));
            CreateBorder("SaveBorder", saveBtn.gameObject.transform, new Color(0.35f, 0.60f, 1.0f, 0.6f), 1.5f);

            // Nút Hướng Dẫn Chiến Thuật bên phải (Cụm Hỗ Trợ & Thoát)
            var helpBtn = CreateIconButton("HelpGuideBtn", barObj.transform, "HUONG DAN", LoadSprite("icon_trophy.png"), new Vector2(1f, 0.5f), new Vector2(-210f, 0f), new Vector2(160f, 54f), new Color(0.18f, 0.14f, 0.32f, 0.95f), new Color(1.0f, 0.85f, 0.30f));
            CreateBorder("HelpBorder", helpBtn.gameObject.transform, new Color(0.95f, 0.75f, 0.25f, 0.65f), 1.5f);

            // Nút Thoát Game bên phải (Ruby / Crimson Accent) với Icon Đầu Lâu
            var quitBtn = CreateIconButton("QuitGameBtn", barObj.transform, "THOAT", LoadSprite("icon_skull.png"), new Vector2(1f, 0.5f), new Vector2(-35f, 0f), new Vector2(145f, 54f), new Color(0.58f, 0.16f, 0.22f, 0.95f), new Color(1.0f, 0.45f, 0.45f));
            CreateBorder("QuitBorder", quitBtn.gameObject.transform, new Color(0.95f, 0.35f, 0.35f, 0.75f), 1.5f);

            return barObj;
        }

        private static void SetupUIManager(TopBarUI topBar, MilestonePanelUI milestonePanel, GameObject rightPanel, GameObject bottomBar)
        {
            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null) return;

            var uiManager = managersObj.GetComponent<UIManager>() ?? managersObj.AddComponent<UIManager>();
            var so = new SerializedObject(uiManager);
            so.FindProperty("topBarUI").objectReferenceValue = topBar;
            so.FindProperty("milestonePanelUI").objectReferenceValue = milestonePanel;
            so.FindProperty("rightUpgradePanel").objectReferenceValue = rightPanel;

            var advBtn = bottomBar.transform.Find("StartAdventureBtn")?.GetComponent<Button>();
            var saveBtn = bottomBar.transform.Find("SaveGameBtn")?.GetComponent<Button>();
            var quitBtn = bottomBar.transform.Find("QuitGameBtn")?.GetComponent<Button>();
            var soundBtn = bottomBar.transform.Find("SoundBtn")?.GetComponent<Button>();
            var helpBtn = bottomBar.transform.Find("HelpGuideBtn")?.GetComponent<Button>();

            if (advBtn != null)
            {
                so.FindProperty("startAdventureButton").objectReferenceValue = advBtn;
                var advImg = advBtn.GetComponent<Image>();
                if (advImg != null) so.FindProperty("startAdventureButtonImage").objectReferenceValue = advImg;
                var advText = advBtn.GetComponentInChildren<TMP_Text>();
                if (advText != null) so.FindProperty("waveTrackerText").objectReferenceValue = advText;
            }
            if (saveBtn != null) so.FindProperty("saveGameButton").objectReferenceValue = saveBtn;
            if (quitBtn != null) so.FindProperty("quitGameButton").objectReferenceValue = quitBtn;
            if (soundBtn != null) so.FindProperty("soundToggleButton").objectReferenceValue = soundBtn;
            if (helpBtn != null) so.FindProperty("helpGuideButton").objectReferenceValue = helpBtn;

            var canvasTransform = GameObject.Find("MainCanvas")?.transform;
            var endGameModal = canvasTransform != null ? canvasTransform.Find("EndGame_Modal")?.gameObject : GameObject.Find("EndGame_Modal");
            if (endGameModal != null)
            {
                var endGameComp = endGameModal.GetComponent<EndGamePanelUI>();
                if (endGameComp != null)
                {
                    so.FindProperty("endGamePanelUI").objectReferenceValue = endGameComp;
                }
            }

            var quitModal = canvasTransform != null ? canvasTransform.Find("Quit_Modal")?.gameObject : GameObject.Find("Quit_Modal");
            if (quitModal != null)
            {
                var quitComp = quitModal.GetComponent<QuitConfirmationModalUI>();
                if (quitComp != null)
                {
                    so.FindProperty("quitModalUI").objectReferenceValue = quitComp;
                }
            }

            var helpModal = canvasTransform != null ? canvasTransform.Find("Help_Modal")?.gameObject : GameObject.Find("Help_Modal");
            if (helpModal != null)
            {
                var helpComp = helpModal.GetComponent<HelpGuideModalUI>();
                if (helpComp != null)
                {
                    so.FindProperty("helpModalUI").objectReferenceValue = helpComp;
                }
            }

            so.ApplyModifiedProperties();
        }

        private static FloatingTextItem CreateOrLoadFloatingTextPrefab()
        {
            string prefabPath = "Assets/Prefabs/FloatingTextItem.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<FloatingTextItem>(prefabPath);
            if (prefab == null)
            {
                var go = new GameObject("FloatingTextItem");
                var text = go.AddComponent<TextMeshPro>();
                text.fontSize = 7;
                text.alignment = TextAlignmentOptions.Center;
                text.fontStyle = FontStyles.Bold;
                text.color = new Color(1f, 0.85f, 0.2f);

                var item = go.AddComponent<FloatingTextItem>();
                var so = new SerializedObject(item);
                so.FindProperty("textComponent").objectReferenceValue = text;
                so.ApplyModifiedProperties();

                prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath).GetComponent<FloatingTextItem>();
                Object.DestroyImmediate(go);
            }
            return prefab;
        }

        private static void SetupFloatingSpawner(FloatingTextItem prefab)
        {
            var managers = GameObject.Find("_Managers");
            if (managers == null) return;

            var spawner = managers.GetComponent<FloatingTextSpawner>() ?? managers.AddComponent<FloatingTextSpawner>();
            var so = new SerializedObject(spawner);
            so.FindProperty("floatingTextPrefab").objectReferenceValue = prefab;
            so.ApplyModifiedProperties();
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
            img.sprite = LoadSprite("pill_frame.png");
            img.type = Image.Type.Sliced;
            img.color = bgColor;
            var btn = btnObj.AddComponent<Button>();

            var text = CreateText("Text", btnObj.transform, label, 16, FontStyles.Bold, Color.white);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;

            return btn;
        }

        private static Button CreateIconButton(string name, Transform parent, string label, Sprite iconSprite, Vector2 anchor, Vector2 pos, Vector2 size, Color bgColor, Color iconTint = default)
        {
            var btnObj = CreateUIObject(name, parent);
            var rect = btnObj.GetComponent<RectTransform>();
            SetRect(rect, anchor, anchor, anchor, pos, size);

            var img = btnObj.AddComponent<Image>();
            img.sprite = LoadSprite("pill_frame.png");
            img.type = Image.Type.Sliced;
            img.color = bgColor;
            var btn = btnObj.AddComponent<Button>();

            if (iconSprite != null)
            {
                CreateSpriteImage("BtnIcon", btnObj.transform, iconSprite, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(24f, 24f), iconTint == default ? Color.white : iconTint);
            }

            var text = CreateText("Text", btnObj.transform, label, 15, FontStyles.Bold, Color.white);
            float leftOffset = iconSprite != null ? 36f : 0f;
            SetRect(text.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(leftOffset * 0.5f, 0f), new Vector2(-leftOffset * 0.5f, 0f));
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;

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

        private static void CreateBorder(string name, Transform parent, Color borderColor, float borderWidth = 1.5f)
        {
            // Không tạo khung viền hình chữ nhật bao quanh các nút bấm để đảm bảo UI bo góc phẳng đẹp
            if (parent != null && parent.GetComponent<Button>() != null) return;
            if (name.Contains("Btn") || name.Contains("Button") || name == "SoundBorder" || name == "SaveBorder" || name == "HelpBorder" || name == "QuitBorder" || name == "BattleHudBorder" || name == "MeltBorder" || name == "ForgeTabBorder" || name == "PlayerTabBorder") return;

            var borderObj = CreateUIObject(name, parent);
            var rect = borderObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var top = CreateUIObject("Border_Top", borderObj.transform);
            var topRect = top.GetComponent<RectTransform>();
            SetRect(topRect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, borderWidth));
            var topImg = top.AddComponent<Image>();
            topImg.color = borderColor;
            topImg.raycastTarget = false;

            var btm = CreateUIObject("Border_Bottom", borderObj.transform);
            var btmRect = btm.GetComponent<RectTransform>();
            SetRect(btmRect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), Vector2.zero, new Vector2(0f, borderWidth));
            var btmImg = btm.AddComponent<Image>();
            btmImg.color = borderColor;
            btmImg.raycastTarget = false;

            var left = CreateUIObject("Border_Left", borderObj.transform);
            var leftRect = left.GetComponent<RectTransform>();
            SetRect(leftRect, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0.5f), Vector2.zero, new Vector2(borderWidth, 0f));
            var leftImg = left.AddComponent<Image>();
            leftImg.color = borderColor;
            leftImg.raycastTarget = false;

            var right = CreateUIObject("Border_Right", borderObj.transform);
            var rightRect = right.GetComponent<RectTransform>();
            SetRect(rightRect, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(1f, 0.5f), Vector2.zero, new Vector2(borderWidth, 0f));
            var rightImg = right.AddComponent<Image>();
            rightImg.color = borderColor;
            rightImg.raycastTarget = false;
        }

        private static void CreateHorizontalLine(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, float height, Color color, Vector2 pos = default, float widthOffset = 0f)
        {
            var lineObj = CreateUIObject(name, parent);
            var rect = lineObj.GetComponent<RectTransform>();
            SetRect(rect, anchorMin, anchorMax, new Vector2(0.5f, 0.5f), pos, new Vector2(widthOffset, height));
            var img = lineObj.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
        }

        private static Sprite LoadSprite(string fileName)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/UI/" + fileName);
        }

        private static Image CreateSpriteImage(string name, Transform parent, Sprite sprite, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color = default)
        {
            var go = CreateUIObject(name, parent);
            var rect = go.GetComponent<RectTransform>();
            SetRect(rect, anchorMin, anchorMax, pivot, pos, size);
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = color == default ? Color.white : color;
            img.raycastTarget = false;
            return img;
        }

        public static EndGamePanelUI BuildEndGamePanel(Transform canvas)
        {
            var oldPanel = canvas.Find("EndGame_Modal");
            if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);

            // Modal Root phủ toàn màn hình
            var modalObj = CreateUIObject("EndGame_Modal", canvas);
            var rect = modalObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var bgOverlay = modalObj.AddComponent<Image>();
            bgOverlay.color = new Color(0.04f, 0.02f, 0.08f, 0.94f); // Nền tối huyền bí che mờ

            var canvasGroup = modalObj.AddComponent<CanvasGroup>();

            // Khung Card Trung Tâm (Dark Obsidian Glass)
            var cardObj = CreateUIObject("CardContainer", modalObj.transform);
            var cardRect = cardObj.GetComponent<RectTransform>();
            SetRect(cardRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(580f, 440f));

            var cardBg = cardObj.AddComponent<Image>();
            cardBg.sprite = LoadSprite("rounded_box.png");
            cardBg.type = Image.Type.Sliced;
            cardBg.color = new Color(0.08f, 0.05f, 0.16f, 0.98f);

            // Đường viền đỏ vàng cảnh báo thanh lịch 2px (KHÔNG dùng Image phủ kín gây vàng bùn)
            CreateBorder("CardBorder", cardObj.transform, new Color(0.95f, 0.35f, 0.3f, 0.85f), 2f);

            // Icon Đầu Lâu Thất Thủ (Sprite Image Thực Sự 64x64px)
            CreateSpriteImage("SkullIcon", cardObj.transform, LoadSprite("icon_skull.png"), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -38f), new Vector2(64f, 64f));

            // Tiêu đề
            var titleText = CreateText("TitleText", cardObj.transform, "CHIEN TRUONG THAT THU", 26, FontStyles.Bold, new Color(1f, 0.38f, 0.38f));
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -114f), new Vector2(520f, 36f));
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.textWrappingMode = TextWrappingModes.NoWrap;

            // Lý do / Cause of death
            var reasonText = CreateText("ReasonText", cardObj.transform, "Tru Tinh The Co Ngu da bi quai vat pha huy hoan toan!", 15, FontStyles.Normal, new Color(0.9f, 0.85f, 0.9f));
            SetRect(reasonText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -156f), new Vector2(520f, 40f));
            reasonText.alignment = TextAlignmentOptions.Center;
            reasonText.textWrappingMode = TextWrappingModes.NoWrap;

            // Đợt quái đạt được
            var waveReachedText = CreateText("WaveReachedText", cardObj.transform, "KY LUC DAT DUOC: DOT #1", 20, FontStyles.Bold, new Color(1f, 0.88f, 0.3f));
            SetRect(waveReachedText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -204f), new Vector2(520f, 35f));
            waveReachedText.alignment = TextAlignmentOptions.Center;
            waveReachedText.textWrappingMode = TextWrappingModes.NoWrap;

            // Lời khuyên nâng cấp
            var statsSummaryText = CreateText("StatsSummaryText", cardObj.transform, "Nang cap Mau Dung Si & Khien Tru de tro lai manh me hon!", 14, FontStyles.Italic, new Color(0.4f, 0.9f, 0.85f));
            SetRect(statsSummaryText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(520f, 40f));
            statsSummaryText.alignment = TextAlignmentOptions.Center;
            statsSummaryText.textWrappingMode = TextWrappingModes.NoWrap;

            // 2 Nút: Chơi Lại & Thoát Game
            var restartBtn = CreateButton("RestartButton", cardObj.transform, "CHOI LAI", new Vector2(0.5f, 0f), new Vector2(-135f, 48f), new Vector2(230f, 54f), new Color(0.12f, 0.68f, 0.42f, 0.98f));
            CreateBorder("RestartBorder", restartBtn.gameObject.transform, new Color(0.35f, 1.0f, 0.7f, 0.7f), 1.5f);
            var restartBtnText = restartBtn.GetComponentInChildren<TMP_Text>();
            if (restartBtnText != null)
            {
                restartBtnText.fontSize = 18;
                restartBtnText.color = Color.white;
                restartBtnText.textWrappingMode = TextWrappingModes.NoWrap;
            }

            var quitBtn = CreateButton("QuitButton", cardObj.transform, "THOAT GAME", new Vector2(0.5f, 0f), new Vector2(135f, 48f), new Vector2(230f, 54f), new Color(0.58f, 0.18f, 0.22f, 0.98f));
            CreateBorder("QuitBorder", quitBtn.gameObject.transform, new Color(0.95f, 0.35f, 0.35f, 0.75f), 1.5f);
            var quitBtnText = quitBtn.GetComponentInChildren<TMP_Text>();
            if (quitBtnText != null)
            {
                quitBtnText.fontSize = 18;
                quitBtnText.color = Color.white;
                quitBtnText.textWrappingMode = TextWrappingModes.NoWrap;
            }

            // Gắn Script EndGamePanelUI
            var panelComp = modalObj.AddComponent<EndGamePanelUI>();
            var so = new SerializedObject(panelComp);
            so.FindProperty("titleText").objectReferenceValue = titleText;
            so.FindProperty("reasonText").objectReferenceValue = reasonText;
            so.FindProperty("waveReachedText").objectReferenceValue = waveReachedText;
            so.FindProperty("statsSummaryText").objectReferenceValue = statsSummaryText;
            so.FindProperty("restartButton").objectReferenceValue = restartBtn;
            so.FindProperty("restartButtonText").objectReferenceValue = restartBtnText;
            so.FindProperty("quitButton").objectReferenceValue = quitBtn;
            so.FindProperty("quitButtonText").objectReferenceValue = quitBtnText;
            so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            so.ApplyModifiedProperties();

            // Mặc định ẩn hoàn toàn
            panelComp.HideInstant();
            modalObj.SetActive(false);

            return panelComp;
        }

        public static QuitConfirmationModalUI BuildQuitModal(Transform canvas)
        {
            var oldPanel = canvas.Find("Quit_Modal");
            if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);

            // Modal Root phủ toàn màn hình
            var modalObj = CreateUIObject("Quit_Modal", canvas);
            var rect = modalObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var bgOverlay = modalObj.AddComponent<Image>();
            bgOverlay.color = new Color(0.04f, 0.02f, 0.08f, 0.94f);

            var canvasGroup = modalObj.AddComponent<CanvasGroup>();

            // Khung Card Trung Tâm
            var cardObj = CreateUIObject("CardContainer", modalObj.transform);
            var cardRect = cardObj.GetComponent<RectTransform>();
            SetRect(cardRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(560f, 320f));

            var cardBg = cardObj.AddComponent<Image>();
            cardBg.sprite = LoadSprite("rounded_box.png");
            cardBg.type = Image.Type.Sliced;
            cardBg.color = new Color(0.08f, 0.05f, 0.16f, 0.98f);
            CreateBorder("CardBorder", cardObj.transform, new Color(0.95f, 0.35f, 0.35f, 0.85f), 2f);

            // Tiêu đề
            var titleText = CreateText("TitleText", cardObj.transform, "XAC NHAN THOAT GAME", 24, FontStyles.Bold, new Color(1f, 0.38f, 0.38f));
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(500f, 36f));
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.textWrappingMode = TextWrappingModes.NoWrap;

            // Lời nhắn an tâm tự động lưu
            var msgText = CreateText("MessageText", cardObj.transform, "Tien trinh cua Chua Te se duoc tu dong luu lai an toan.\nBan co chac chan muon roi khoi Thanh Dien khong?", 16, FontStyles.Normal, new Color(0.88f, 0.85f, 0.95f));
            SetRect(msgText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(500f, 60f));
            msgText.alignment = TextAlignmentOptions.Center;

            // 2 Nút: Tiếp tục chơi & Thoát game
            var cancelBtn = CreateButton("CancelButton", cardObj.transform, "TIEP TUC CHOI", new Vector2(0.5f, 0f), new Vector2(-130f, 45f), new Vector2(220f, 52f), new Color(0.15f, 0.52f, 0.35f, 0.98f));
            CreateBorder("CancelBorder", cancelBtn.gameObject.transform, new Color(0.35f, 0.95f, 0.65f, 0.7f), 1.5f);
            var cancelBtnText = cancelBtn.GetComponentInChildren<TMP_Text>();
            if (cancelBtnText != null) cancelBtnText.fontSize = 17;

            var quitBtn = CreateButton("ConfirmQuitButton", cardObj.transform, "THOAT GAME", new Vector2(0.5f, 0f), new Vector2(130f, 45f), new Vector2(220f, 52f), new Color(0.62f, 0.16f, 0.22f, 0.98f));
            CreateBorder("QuitBorder", quitBtn.gameObject.transform, new Color(0.95f, 0.35f, 0.35f, 0.8f), 1.5f);
            var quitBtnText = quitBtn.GetComponentInChildren<TMP_Text>();
            if (quitBtnText != null) quitBtnText.fontSize = 17;

            // Gắn Script QuitConfirmationModalUI
            var modalComp = modalObj.AddComponent<QuitConfirmationModalUI>();
            var so = new SerializedObject(modalComp);
            so.FindProperty("titleText").objectReferenceValue = titleText;
            so.FindProperty("messageText").objectReferenceValue = msgText;
            so.FindProperty("cancelButton").objectReferenceValue = cancelBtn;
            so.FindProperty("cancelButtonText").objectReferenceValue = cancelBtnText;
            so.FindProperty("confirmQuitButton").objectReferenceValue = quitBtn;
            so.FindProperty("confirmQuitButtonText").objectReferenceValue = quitBtnText;
            so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            so.ApplyModifiedProperties();

            modalComp.HideInstant();
            modalObj.SetActive(false);

            return modalComp;
        }

        public static HelpGuideModalUI BuildHelpModal(Transform canvas)
        {
            var oldPanel = canvas.Find("Help_Modal");
            if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);

            // Modal Root phủ toàn màn hình
            var modalObj = CreateUIObject("Help_Modal", canvas);
            var rect = modalObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var bgOverlay = modalObj.AddComponent<Image>();
            bgOverlay.color = new Color(0.04f, 0.02f, 0.08f, 0.94f);

            var canvasGroup = modalObj.AddComponent<CanvasGroup>();

            // Khung Card Trung Tâm
            var cardObj = CreateUIObject("CardContainer", modalObj.transform);
            var cardRect = cardObj.GetComponent<RectTransform>();
            SetRect(cardRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(640f, 440f));

            var cardBg = cardObj.AddComponent<Image>();
            cardBg.sprite = LoadSprite("rounded_box.png");
            cardBg.type = Image.Type.Sliced;
            cardBg.color = new Color(0.08f, 0.05f, 0.16f, 0.98f);
            CreateBorder("CardBorder", cardObj.transform, new Color(0.95f, 0.78f, 0.25f, 0.75f), 2f);

            // Icon Cúp Vàng trên đỉnh
            CreateSpriteImage("TrophyIcon", cardObj.transform, LoadSprite("icon_trophy.png"), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -36f), new Vector2(52f, 52f));

            // Tiêu đề
            var titleText = CreateText("TitleText", cardObj.transform, "CAM NANG CHIEN THUAT", 24, FontStyles.Bold, new Color(1f, 0.88f, 0.35f));
            SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(580f, 34f));
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.textWrappingMode = TextWrappingModes.NoWrap;

            // Nội dung hướng dẫn 4 bước Core Loop
            string guideText = 
                "<b>1. KHAI THAC:</b> Nhap chuot vao Tru Tinh The hoac sam May Khoan de tu dong thu hoach Pha Le.\n\n" +
                "<b>2. NANG CAP:</b> Gia tang The Luc (HP), Cong Kich (ATK), Toc Do (SPD) va Ky Nang cho Dung Si.\n\n" +
                "<b>3. PHONG THU:</b> Dung Si va Tru Tinh The tu dong danh lui cac dot quai vat xam lang.\n\n" +
                "<b>4. OVERDRIVE:</b> Tich du 100% No de kich hoat OVERDRIVE x3 Sat Thuong diet boss!";

            var contentText = CreateText("GuideContentText", cardObj.transform, guideText, 14, FontStyles.Normal, new Color(0.88f, 0.85f, 0.95f));
            SetRect(contentText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -225f), new Vector2(560f, 190f));
            contentText.alignment = TextAlignmentOptions.TopLeft;
            contentText.textWrappingMode = TextWrappingModes.Normal;

            // Nút "DA HIEU"
            var closeBtn = CreateButton("CloseButton", cardObj.transform, "DA HIEU", new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(200f, 50f), new Color(0.12f, 0.72f, 0.42f, 0.98f));
            CreateBorder("CloseBorder", closeBtn.gameObject.transform, new Color(0.35f, 1.0f, 0.70f, 0.75f), 1.5f);
            var closeBtnText = closeBtn.GetComponentInChildren<TMP_Text>();
            if (closeBtnText != null) closeBtnText.fontSize = 17;

            // Gắn Script HelpGuideModalUI
            var modalComp = modalObj.AddComponent<HelpGuideModalUI>();
            var so = new SerializedObject(modalComp);
            so.FindProperty("titleText").objectReferenceValue = titleText;
            so.FindProperty("guideContentText").objectReferenceValue = contentText;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("closeButtonText").objectReferenceValue = closeBtnText;
            so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            so.ApplyModifiedProperties();

            modalComp.HideInstant();
            modalObj.SetActive(false);

            return modalComp;
        }

        [MenuItem("CrystalForge/3. Setup Altar Shield UI")]
        public static void BuildAltarShieldWorldCanvas()
        {
            var coreObj = GameObject.Find("CrystalCore");
            if (coreObj == null)
            {
                Debug.LogWarning("[CanvasSetupTool] Không tìm thấy CrystalCore trong Scene để gắn World-Space Shield UI!");
                return;
            }

            var oldCanvas = coreObj.transform.Find("Altar_Shield_WorldCanvas");
            if (oldCanvas != null) Object.DestroyImmediate(oldCanvas.gameObject);

            // 1. Tạo Canvas World Space
            var canvasObj = new GameObject("Altar_Shield_WorldCanvas");
            canvasObj.transform.SetParent(coreObj.transform, false);
            canvasObj.transform.localPosition = new Vector3(0f, 1.45f, 0f); // 1.85 + 1.45 = 3.30m trên đỉnh viên pha lê
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = new Vector3(0.0075f, 0.0075f, 0.0075f);

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 5;

            var rect = canvasObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300f, 64f);

            canvasObj.AddComponent<GraphicRaycaster>();

            // 2. Khung nền bo tròn mạ vàng (Glass Dark Obsidian)
            var bgObj = CreateUIObject("ShieldBackground", canvasObj.transform);
            var bgRect = bgObj.GetComponent<RectTransform>();
            SetRect(bgRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = LoadSprite("pill_frame.png");
            bgImg.type = Image.Type.Sliced;
            bgImg.color = new Color(0.08f, 0.05f, 0.16f, 0.94f);
            CreateBorder("ShieldBorder", bgObj.transform, new Color(0.95f, 0.78f, 0.25f, 0.65f), 1.5f);

            // Viền phát sáng cảnh báo khi bị đánh (Glow Flash Border)
            var glowObj = CreateUIObject("BorderGlow", bgObj.transform);
            var glowRect = glowObj.GetComponent<RectTransform>();
            SetRect(glowRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var glowImg = glowObj.AddComponent<Image>();
            glowImg.color = new Color(0.95f, 0.75f, 0.25f, 0.4f);
            glowImg.raycastTarget = false;

            // 3. Icon Ngọc Pha Lê bên trái
            var iconImg = CreateSpriteImage("IconImage", bgObj.transform, LoadSprite("icon_crystal.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(16f, 0f), new Vector2(32f, 32f));

            // 4. Tiêu đề
            var title = CreateText("TitleText", bgObj.transform, "TRU TINH THE", 13, FontStyles.Bold, new Color(0.35f, 0.95f, 1f));
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(56f, -6f), new Vector2(-60f, 22f));
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;

            // 5. Thanh Khiên Slot (Shield Slot)
            var slotObj = CreateUIObject("ShieldSlot", bgObj.transform);
            var slotRect = slotObj.GetComponent<RectTransform>();
            SetRect(slotRect, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(56f, 8f), new Vector2(-70f, 24f));
            var slotImg = slotObj.AddComponent<Image>();
            slotImg.sprite = LoadSprite("pill_frame.png");
            slotImg.type = Image.Type.Sliced;
            slotImg.color = new Color(0.14f, 0.10f, 0.24f, 1.0f);
            CreateBorder("SlotBorder", slotObj.transform, new Color(0.35f, 0.28f, 0.55f, 0.6f), 1.0f);

            // 6. Delayed Fill (Vệt rút máu trễ màu vàng cam)
            var delayedObj = CreateUIObject("DelayedFill", slotObj.transform);
            var delayedRect = delayedObj.GetComponent<RectTransform>();
            SetRect(delayedRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var delayedImg = delayedObj.AddComponent<Image>();
            delayedImg.sprite = LoadSprite("pill_frame.png");
            delayedImg.type = Image.Type.Sliced;
            delayedImg.color = new Color(1f, 0.65f, 0.2f, 0.85f);

            // 7. Shield Fill (Thanh khiên Cyan chính)
            var fillObj = CreateUIObject("ShieldFill", slotObj.transform);
            var fillRect = fillObj.GetComponent<RectTransform>();
            SetRect(fillRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var fillImg = fillObj.AddComponent<Image>();
            fillImg.sprite = LoadSprite("pill_frame.png");
            fillImg.type = Image.Type.Sliced;
            fillImg.color = new Color(0.2f, 0.92f, 1f, 1f);

            // 8. Text Chỉ số Khiên
            var text = CreateText("ShieldText", slotObj.transform, "KHIEN: 100 / 100", 12, FontStyles.Bold, Color.white);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;

            // 9. Gắn Script CrystalCoreShieldUI & Wire References
            var shieldUI = canvasObj.AddComponent<CrystalCoreShieldUI>();
            var so = new SerializedObject(shieldUI);
            so.FindProperty("shieldFillRect").objectReferenceValue = fillRect;
            so.FindProperty("delayedFillRect").objectReferenceValue = delayedRect;
            so.FindProperty("shieldFillImage").objectReferenceValue = fillImg;
            so.FindProperty("delayedFillImage").objectReferenceValue = delayedImg;
            so.FindProperty("iconImage").objectReferenceValue = iconImg;
            so.FindProperty("borderGlowImage").objectReferenceValue = glowImg;
            so.FindProperty("shieldText").objectReferenceValue = text;
            so.FindProperty("titleText").objectReferenceValue = title;
            so.ApplyModifiedProperties();

            Debug.Log("<color=#00FF88><b>[CanvasSetupTool]</b> Đã dựng thành công World-Space Shield UI cho Trụ Tinh Thể tại đỉnh Lõi Pha Lê!</color>");
        }

        private static void EnsurePrefabFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
        }
    }
}
#endif
