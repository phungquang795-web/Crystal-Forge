#if UNITY_EDITOR
using System.IO;
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
    public static class RelicAndPrestigeUISetupTool
    {
        [MenuItem("CrystalForge/4. Setup Relic Tabs & Cosmic Meltdown UI")]
        public static void SetupRelicAndPrestigeUI()
        {
            Undo.SetCurrentGroupName("Setup Relic Tabs & Cosmic Meltdown UI");
            int undoGroup = Undo.GetCurrentGroup();

            var canvasObj = GameObject.Find("MainCanvas");
            if (canvasObj == null)
            {
                Debug.LogError("[RelicAndPrestigeUISetupTool] Không tìm thấy MainCanvas!");
                return;
            }

            var rightPanel = canvasObj.transform.Find("RightPanel_Upgrades")?.gameObject;
            if (rightPanel != null)
            {
                SetupRelicTabs(rightPanel);
            }

            var bottomBar = canvasObj.transform.Find("BottomBar_Panel")?.gameObject;
            if (bottomBar != null)
            {
                SetupBottomBarMeltdownButton(bottomBar);
            }

            var meltdownModal = SetupCosmicMeltdownModal(canvasObj.transform);

            // Gán vào UIManager
            var managersObj = GameObject.Find("_Managers");
            if (managersObj != null)
            {
                var uiManager = managersObj.GetComponent<UIManager>();
                if (uiManager != null && meltdownModal != null)
                {
                    var so = new SerializedObject(uiManager);
                    so.FindProperty("cosmicMeltdownModalUI").objectReferenceValue = meltdownModal;
                    so.ApplyModifiedProperties();
                }

                if (managersObj.GetComponent<RelicManager>() == null)
                {
                    managersObj.AddComponent<RelicManager>();
                }
                if (managersObj.GetComponent<PrestigeManager>() == null)
                {
                    managersObj.AddComponent<PrestigeManager>();
                }
                if (managersObj.GetComponent<SkyEventSystem>() == null)
                {
                    managersObj.AddComponent<SkyEventSystem>();
                }
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Đã dựng hoàn tất 3 Tab Cổ Vật và Modal Đại Nung Chảy (Cosmic Meltdown)!</color>");
        }

        private static void SetupRelicTabs(GameObject rightPanel)
        {
            var tabForge = rightPanel.transform.Find("TabButton_Forge")?.GetComponent<RectTransform>();
            var tabPlayer = rightPanel.transform.Find("TabButton_Player")?.GetComponent<RectTransform>();

            if (tabForge != null)
            {
                tabForge.anchoredPosition = new Vector2(-124f, -14f);
                tabForge.sizeDelta = new Vector2(118f, 44f);
            }
            if (tabPlayer != null)
            {
                tabPlayer.anchoredPosition = new Vector2(0f, -14f);
                tabPlayer.sizeDelta = new Vector2(118f, 44f);
            }

            // Tab 3: CỔ VẬT
            Transform oldTabRelic = rightPanel.transform.Find("TabButton_Relics");
            if (oldTabRelic != null) Object.DestroyImmediate(oldTabRelic.gameObject);

            var tabRelicObj = CreateUIObject("TabButton_Relics", rightPanel.transform);
            var tabRelicRect = tabRelicObj.GetComponent<RectTransform>();
            SetRect(tabRelicRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(124f, -14f), new Vector2(118f, 44f));
            var tabRelicImg = tabRelicObj.AddComponent<Image>();
            tabRelicImg.sprite = LoadSprite("pill_frame.png");
            tabRelicImg.type = Image.Type.Sliced;
            tabRelicImg.color = new Color(0.16f, 0.12f, 0.28f, 0.85f);
            CreateBorder("RelicTabBorder", tabRelicObj.transform, new Color(0.4f, 0.3f, 0.6f, 0.4f), 1.5f);
            var tabRelicBtn = tabRelicObj.AddComponent<Button>();

            var tabRelicIcon = CreateSpriteImage("TabIcon", tabRelicObj.transform, LoadSprite("icon_hammer.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(12f, 0f), new Vector2(20f, 20f), new Color(0.85f, 0.85f, 0.95f));
            var tabRelicText = CreateText("Label", tabRelicObj.transform, "CỔ VẬT", 14, FontStyles.Bold, new Color(0.85f, 0.85f, 0.95f));
            SetRect(tabRelicText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(14f, 0f), new Vector2(-10f, 0f));
            tabRelicText.alignment = TextAlignmentOptions.Center;
            tabRelicText.textWrappingMode = TextWrappingModes.NoWrap;

            // Container 3: Relic Container
            Transform oldRelicCont = rightPanel.transform.Find("RelicUpgrades_Container");
            if (oldRelicCont != null) Object.DestroyImmediate(oldRelicCont.gameObject);

            var relicContainer = CreateUIObject("RelicUpgrades_Container", rightPanel.transform);
            var relicRect = relicContainer.GetComponent<RectTransform>();
            SetRect(relicRect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(0f, -36f), new Vector2(0f, -76f));

            BuildRelicCard(relicContainer.transform, "Card_RelicHammer", UpgradeCardUI.CardType.RelicHammer, "icon_hammer.png", "BÚA THIÊN THẠCH", "Tự động gõ đe rèn (10s)", "5K CR", new Vector2(0f, -6f));
            BuildRelicCard(relicContainer.transform, "Card_RelicAegis", UpgradeCardUI.CardType.RelicAegis, "icon_crystal.png", "KHIÊN BĂNG CỔ ĐẠI", "Tăng Offline & Giáp đấu", "12K CR", new Vector2(0f, -150f));
            BuildRelicCard(relicContainer.transform, "Card_RelicIgnis", UpgradeCardUI.CardType.RelicIgnis, "icon_furnace.png", "LÕI LỬA BẤT DIỆT", "Tăng sản lượng & DoT đốt", "35K CR", new Vector2(0f, -294f));

            relicContainer.SetActive(false);

            // Cập nhật UpgradeTabSwitcher
            var switcher = rightPanel.GetComponent<UpgradeTabSwitcher>();
            if (switcher != null)
            {
                var swSo = new SerializedObject(switcher);
                var forgeBtn = rightPanel.transform.Find("TabButton_Forge")?.GetComponent<Button>();
                var playerBtn = rightPanel.transform.Find("TabButton_Player")?.GetComponent<Button>();
                var forgeImg = rightPanel.transform.Find("TabButton_Forge")?.GetComponent<Image>();
                var playerImg = rightPanel.transform.Find("TabButton_Player")?.GetComponent<Image>();
                var forgeText = rightPanel.transform.Find("TabButton_Forge/Label")?.GetComponent<TMP_Text>();
                var playerText = rightPanel.transform.Find("TabButton_Player/Label")?.GetComponent<TMP_Text>();
                var forgeIcon = rightPanel.transform.Find("TabButton_Forge/TabIcon")?.GetComponent<Image>();
                var playerIcon = rightPanel.transform.Find("TabButton_Player/TabIcon")?.GetComponent<Image>();
                var forgeCont = rightPanel.transform.Find("ForgeUpgrades_Container")?.gameObject;
                var playerCont = rightPanel.transform.Find("PlayerUpgrades_Container")?.gameObject;

                swSo.FindProperty("forgeTabButton").objectReferenceValue = forgeBtn;
                swSo.FindProperty("playerTabButton").objectReferenceValue = playerBtn;
                swSo.FindProperty("relicTabButton").objectReferenceValue = tabRelicBtn;

                swSo.FindProperty("forgeTabImg").objectReferenceValue = forgeImg;
                swSo.FindProperty("playerTabImg").objectReferenceValue = playerImg;
                swSo.FindProperty("relicTabImg").objectReferenceValue = tabRelicImg;

                swSo.FindProperty("forgeTabText").objectReferenceValue = forgeText;
                swSo.FindProperty("playerTabText").objectReferenceValue = playerText;
                swSo.FindProperty("relicTabText").objectReferenceValue = tabRelicText;

                swSo.FindProperty("forgeTabIcon").objectReferenceValue = forgeIcon;
                swSo.FindProperty("playerTabIcon").objectReferenceValue = playerIcon;
                swSo.FindProperty("relicTabIcon").objectReferenceValue = tabRelicIcon;

                swSo.FindProperty("forgeContainer").objectReferenceValue = forgeCont;
                swSo.FindProperty("playerContainer").objectReferenceValue = playerCont;
                swSo.FindProperty("relicContainer").objectReferenceValue = relicContainer;
                swSo.ApplyModifiedProperties();
            }
        }

        private static void BuildRelicCard(Transform parent, string name, UpgradeCardUI.CardType cardType, string spriteName, string defaultTitle, string defaultOutput, string defaultCost, Vector2 anchoredPos)
        {
            var card = CreateUIObject(name, parent);
            var rect = card.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), anchoredPos, new Vector2(-22f, 134f));

            var bg = card.AddComponent<Image>();
            bg.sprite = LoadSprite("rounded_box.png");
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.12f, 0.08f, 0.22f, 0.96f);
            CreateBorder("CardBorder", card.transform, new Color(0.55f, 0.40f, 0.75f, 0.85f), 1.5f);

            var iconBoxObj = CreateUIObject("IconBox", card.transform);
            var iconBoxRect = iconBoxObj.GetComponent<RectTransform>();
            SetRect(iconBoxRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(58f, 58f));
            var iconBoxBg = iconBoxObj.AddComponent<Image>();
            iconBoxBg.sprite = LoadSprite("rounded_box.png");
            iconBoxBg.type = Image.Type.Sliced;
            iconBoxBg.color = new Color(0.22f, 0.15f, 0.40f, 1.0f);
            CreateBorder("IconBoxBorder", iconBoxObj.transform, new Color(0.95f, 0.75f, 0.25f, 0.55f), 1.5f);

            var iconImg = CreateSpriteImage("IconImage", iconBoxObj.transform, LoadSprite(spriteName), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-12f, -12f));

            var title = CreateText("Title", card.transform, defaultTitle, 16, FontStyles.Bold, new Color(0.98f, 0.98f, 1.0f));
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -14f), new Vector2(-205f, 26f));
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.textWrappingMode = TextWrappingModes.NoWrap;

            var level = CreateText("Level", card.transform, "Cấp 0", 14, FontStyles.Bold, new Color(0.85f, 0.75f, 1.0f));
            SetRect(level.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -42f), new Vector2(-205f, 24f));
            level.alignment = TextAlignmentOptions.MidlineLeft;
            level.textWrappingMode = TextWrappingModes.NoWrap;

            var output = CreateText("Output", card.transform, defaultOutput, 14, FontStyles.Bold, new Color(0.35f, 1.0f, 0.65f));
            SetRect(output.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(82f, -70f), new Vector2(-205f, 26f));
            output.alignment = TextAlignmentOptions.MidlineLeft;
            output.textWrappingMode = TextWrappingModes.NoWrap;

            var btnObj = CreateUIObject("BuyButton", card.transform);
            var btnRect = btnObj.GetComponent<RectTransform>();
            SetRect(btnRect, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-14f, 0f), new Vector2(104f, 64f));
            var btnImg = btnObj.AddComponent<Image>();
            btnImg.sprite = LoadSprite("pill_frame.png");
            btnImg.type = Image.Type.Sliced;
            btnImg.color = new Color(0.05f, 0.60f, 0.40f, 1.0f);
            CreateBorder("BuyButtonBorder", btnObj.transform, new Color(0.35f, 1.0f, 0.75f, 0.55f), 1.5f);
            var btn = btnObj.AddComponent<Button>();

            var btnLabel = CreateText("BtnLabel", btnObj.transform, "RÈN", 14, FontStyles.Bold, new Color(0.92f, 0.98f, 0.95f));
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

            var cardUI = card.AddComponent<UpgradeCardUI>();
            var so = new SerializedObject(cardUI);
            so.FindProperty("cardType").enumValueIndex = (int)cardType;
            so.FindProperty("iconImage").objectReferenceValue = iconImg;
            so.FindProperty("titleText").objectReferenceValue = title;
            so.FindProperty("levelText").objectReferenceValue = level;
            so.FindProperty("outputText").objectReferenceValue = output;
            so.FindProperty("costText").objectReferenceValue = costText;
            so.FindProperty("buyButton").objectReferenceValue = btn;
            so.FindProperty("buyButtonImage").objectReferenceValue = btnImg;
            so.ApplyModifiedProperties();
        }

        private static void SetupBottomBarMeltdownButton(GameObject bottomBar)
        {
            var advBtn = bottomBar.transform.Find("StartAdventureBtn")?.GetComponent<RectTransform>();
            if (advBtn != null)
            {
                advBtn.anchoredPosition = new Vector2(-60f, 0f);
                advBtn.sizeDelta = new Vector2(440f, 68f);
            }

            Transform oldMeltdown = bottomBar.transform.Find("MeltdownBtn");
            if (oldMeltdown != null) Object.DestroyImmediate(oldMeltdown.gameObject);

            var meltObj = CreateUIObject("MeltdownBtn", bottomBar.transform);
            var meltRect = meltObj.GetComponent<RectTransform>();
            SetRect(meltRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240f, 0f), new Vector2(190f, 68f));

            var meltImg = meltObj.AddComponent<Image>();
            meltImg.sprite = LoadSprite("pill_frame.png");
            meltImg.type = Image.Type.Sliced;
            meltImg.color = new Color(0.40f, 0.14f, 0.58f, 0.98f); // Tím ma thuật
            CreateBorder("MeltBorder", meltObj.transform, new Color(1.0f, 0.85f, 0.35f, 0.85f), 1.5f);

            var meltBtn = meltObj.AddComponent<Button>();
            CreateSpriteImage("MeltIcon", meltObj.transform, LoadSprite("icon_star.png"), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(14f, 0f), new Vector2(28f, 28f), new Color(1.0f, 0.88f, 0.35f));

            var meltText = CreateText("MeltText", meltObj.transform, "NUNG CHẢY", 16, FontStyles.Bold, new Color(1.0f, 0.92f, 0.45f));
            SetRect(meltText.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(20f, 0f), new Vector2(-10f, 0f));
            meltText.alignment = TextAlignmentOptions.Center;
            meltText.textWrappingMode = TextWrappingModes.NoWrap;

            meltBtn.onClick.AddListener(() =>
            {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.OpenCosmicMeltdownModal();
                }
            });
        }

        private static CosmicMeltdownModalUI SetupCosmicMeltdownModal(Transform canvas)
        {
            Transform oldModal = canvas.Find("CosmicMeltdown_Modal");
            if (oldModal != null) Object.DestroyImmediate(oldModal.gameObject);

            var modalObj = CreateUIObject("CosmicMeltdown_Modal", canvas);
            var rect = modalObj.GetComponent<RectTransform>();
            SetRect(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

            var overlayImg = modalObj.AddComponent<Image>();
            overlayImg.color = new Color(0.02f, 0.01f, 0.05f, 0.85f); // Làm tối hậu cảnh
            var canvasGroup = modalObj.AddComponent<CanvasGroup>();

            // CardContainer
            var cardObj = CreateUIObject("CardContainer", modalObj.transform);
            var cardRect = cardObj.GetComponent<RectTransform>();
            SetRect(cardRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(660f, 540f));

            var cardBg = cardObj.AddComponent<Image>();
            cardBg.sprite = LoadSprite("rounded_box.png");
            cardBg.type = Image.Type.Sliced;
            cardBg.color = new Color(0.08f, 0.05f, 0.16f, 0.98f);
            CreateBorder("CardBorder", cardObj.transform, new Color(0.95f, 0.78f, 0.25f, 0.85f), 2f);

            CreateSpriteImage("StarIcon", cardObj.transform, LoadSprite("icon_star.png"), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -32f), new Vector2(46f, 46f), new Color(1f, 0.85f, 0.25f));

            var title = CreateText("TitleText", cardObj.transform, "ĐẠI NUNG CHẢY LÕI (PRESTIGE)", 22, FontStyles.Bold, new Color(1f, 0.88f, 0.35f));
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -84f), new Vector2(600f, 32f));
            title.alignment = TextAlignmentOptions.Center;

            var walletText = CreateText("WalletText", cardObj.transform, "Bụi Tinh Tú: 0 ⭐", 16, FontStyles.Bold, new Color(0.4f, 0.95f, 1.0f));
            SetRect(walletText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -114f), new Vector2(600f, 26f));
            walletText.alignment = TextAlignmentOptions.Center;

            var summary = CreateText("SummaryText", cardObj.transform, "Nung chảy Lò Rèn để nhận Bụi Tinh Tú vĩnh viễn...", 13, FontStyles.Normal, new Color(0.85f, 0.85f, 0.95f));
            SetRect(summary.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -156f), new Vector2(600f, 44f));
            summary.alignment = TextAlignmentOptions.Center;

            // 3 Dòng Talent
            var t1 = BuildTalentRow(cardObj.transform, "TalentRow_Astral", "HÀO QUANG TINH TÚ", "+30% Sản lượng toàn cầu", new Vector2(0f, -210f));
            var t2 = BuildTalentRow(cardObj.transform, "TalentRow_Royal", "KHỞI NGHIỆP HOÀNG GIA", "Khởi đầu với Crystal sau reset", new Vector2(0f, -285f));
            var t3 = BuildTalentRow(cardObj.transform, "TalentRow_Cosmic", "CHIẾN TƯỚNG THIÊN HÀ", "+30% HP & ATK Capybara trong Adventure", new Vector2(0f, -360f));

            // Nút Đại Nung Chảy
            var meltBtn = CreateButton("MeltdownActionButton", cardObj.transform, "ĐẠI NUNG CHẢY (+0 ⭐)", new Vector2(0.5f, 0f), new Vector2(-120f, 44f), new Vector2(320f, 52f), new Color(0.55f, 0.15f, 0.65f, 0.98f));
            CreateBorder("MeltActionBorder", meltBtn.gameObject.transform, new Color(1.0f, 0.85f, 0.35f, 0.85f), 1.5f);
            var meltBtnText = meltBtn.GetComponentInChildren<TMP_Text>();
            if (meltBtnText != null) meltBtnText.fontSize = 15;

            // Nút Đóng
            var closeBtn = CreateButton("CloseButton", cardObj.transform, "ĐÓNG", new Vector2(0.5f, 0f), new Vector2(170f, 44f), new Vector2(180f, 52f), new Color(0.20f, 0.16f, 0.32f, 0.98f));
            CreateBorder("CloseBorder", closeBtn.gameObject.transform, new Color(0.45f, 0.35f, 0.65f, 0.6f), 1.5f);
            var closeBtnText = closeBtn.GetComponentInChildren<TMP_Text>();
            if (closeBtnText != null) closeBtnText.fontSize = 15;

            var modalComp = modalObj.AddComponent<CosmicMeltdownModalUI>();
            var so = new SerializedObject(modalComp);
            so.FindProperty("titleText").objectReferenceValue = title;
            so.FindProperty("summaryText").objectReferenceValue = summary;
            so.FindProperty("stardustWalletText").objectReferenceValue = walletText;

            so.FindProperty("astralTitleText").objectReferenceValue = t1.title;
            so.FindProperty("astralDescText").objectReferenceValue = t1.desc;
            so.FindProperty("astralUpgradeBtn").objectReferenceValue = t1.btn;
            so.FindProperty("astralCostText").objectReferenceValue = t1.cost;

            so.FindProperty("royalTitleText").objectReferenceValue = t2.title;
            so.FindProperty("royalDescText").objectReferenceValue = t2.desc;
            so.FindProperty("royalUpgradeBtn").objectReferenceValue = t2.btn;
            so.FindProperty("royalCostText").objectReferenceValue = t2.cost;

            so.FindProperty("cosmicTitleText").objectReferenceValue = t3.title;
            so.FindProperty("cosmicDescText").objectReferenceValue = t3.desc;
            so.FindProperty("cosmicUpgradeBtn").objectReferenceValue = t3.btn;
            so.FindProperty("cosmicCostText").objectReferenceValue = t3.cost;

            so.FindProperty("meltdownButton").objectReferenceValue = meltBtn;
            so.FindProperty("meltdownButtonText").objectReferenceValue = meltBtnText;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
            so.FindProperty("cardContainer").objectReferenceValue = cardRect;
            so.ApplyModifiedProperties();

            modalComp.HideInstant();
            return modalComp;
        }

        private struct TalentRowRefs
        {
            public TMP_Text title;
            public TMP_Text desc;
            public TMP_Text cost;
            public Button btn;
        }

        private static TalentRowRefs BuildTalentRow(Transform parent, string name, string defaultTitle, string defaultDesc, Vector2 anchoredPos)
        {
            var rowObj = CreateUIObject(name, parent);
            var rect = rowObj.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), anchoredPos, new Vector2(590f, 62f));

            var bg = rowObj.AddComponent<Image>();
            bg.sprite = LoadSprite("rounded_box.png");
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.12f, 0.08f, 0.22f, 0.95f);
            CreateBorder("RowBorder", rowObj.transform, new Color(0.4f, 0.3f, 0.6f, 0.5f), 1.5f);

            var title = CreateText("Title", rowObj.transform, defaultTitle, 15, FontStyles.Bold, Color.white);
            SetRect(title.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(16f, -8f), new Vector2(-150f, 22f));
            title.alignment = TextAlignmentOptions.MidlineLeft;

            var desc = CreateText("Desc", rowObj.transform, defaultDesc, 13, FontStyles.Normal, new Color(0.8f, 0.8f, 0.9f));
            SetRect(desc.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(16f, -32f), new Vector2(-150f, 22f));
            desc.alignment = TextAlignmentOptions.MidlineLeft;

            var btn = CreateButton("UpgradeBtn", rowObj.transform, "NÂNG CẤP", new Vector2(1f, 0.5f), new Vector2(-12f, 0f), new Vector2(120f, 44f), new Color(0.12f, 0.65f, 0.38f, 0.98f));
            CreateBorder("BtnBorder", btn.gameObject.transform, new Color(0.35f, 0.95f, 0.65f, 0.65f), 1.5f);

            var costText = CreateText("Cost", btn.transform, "2 ⭐", 14, FontStyles.Bold, new Color(1f, 0.92f, 0.35f));
            SetRect(costText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            costText.alignment = TextAlignmentOptions.Center;

            return new TalentRowRefs { title = title, desc = desc, cost = costText, btn = btn };
        }

        #region Helper Methods
        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        private static void SetRect(RectTransform rt, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = aMin;
            rt.anchorMax = aMax;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
        }

        private static Image CreateSpriteImage(string name, Transform parent, Sprite sprite, Vector2 aMin, Vector2 aMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color = default)
        {
            var obj = CreateUIObject(name, parent);
            var rt = obj.GetComponent<RectTransform>();
            SetRect(rt, aMin, aMax, pivot, pos, size);
            var img = obj.AddComponent<Image>();
            img.sprite = sprite;
            img.color = (color == default) ? Color.white : color;
            return img;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text, float size, FontStyles style, Color color)
        {
            var obj = CreateUIObject(name, parent);
            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.color = color;
            return tmp;
        }

        private static Button CreateButton(string name, Transform parent, string text, Vector2 pivot, Vector2 pos, Vector2 size, Color bgCol)
        {
            var obj = CreateUIObject(name, parent);
            var rt = obj.GetComponent<RectTransform>();
            SetRect(rt, pivot, pivot, pivot, pos, size);
            var img = obj.AddComponent<Image>();
            img.sprite = LoadSprite("pill_frame.png");
            img.type = Image.Type.Sliced;
            img.color = bgCol;
            var btn = obj.AddComponent<Button>();
            CreateText("Label", obj.transform, text, 14, FontStyles.Bold, Color.white);
            return btn;
        }

        private static void CreateBorder(string name, Transform parent, Color color, float thickness)
        {
            if (parent != null && parent.GetComponent<Button>() != null) return;
            if (name.Contains("Btn") || name.Contains("Button") || name == "RelicTabBorder" || name == "MeltBorder" || name == "MeltActionBorder") return;

            var borderObj = CreateUIObject(name, parent);
            var rt = borderObj.GetComponent<RectTransform>();
            SetRect(rt, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var img = borderObj.AddComponent<Image>();
            img.sprite = LoadSprite("rounded_box.png");
            img.type = Image.Type.Sliced;
            img.color = color;
            img.raycastTarget = false;
        }

        private static Sprite LoadSprite(string filename)
        {
            string path = Path.Combine("Assets/Textures/UI", filename).Replace("\\", "/");
            var sp = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sp == null)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }
            }
            return sp;
        }
        #endregion
    }
}
#endif
