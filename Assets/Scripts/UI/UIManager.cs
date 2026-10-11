using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private TopBarUI topBarUI;
        [SerializeField] private MilestonePanelUI milestonePanelUI;
        [SerializeField] private GameObject rightUpgradePanel;

        [Header("Bottom Bar Buttons")]
        [SerializeField] private Button startAdventureButton;
        [SerializeField] private Button saveGameButton;
        [SerializeField] private Button quitGameButton;
        [SerializeField] private Button soundToggleButton;
        [SerializeField] private Button helpGuideButton;
        [SerializeField] private Image soundIconImage;

        [Header("Adventure Overlay Panels")]
        [SerializeField] private AdventureModalUI adventureModalUI;
        [SerializeField] private GameObject eventModalPanel;
        [SerializeField] private GameObject combatModalPanel;
        [SerializeField] private GameObject skillSelectModalPanel;
        [SerializeField] private GameObject offlineWelcomeModalPanel;
        [SerializeField] private EndGamePanelUI endGamePanelUI;
        [SerializeField] private QuitConfirmationModalUI quitModalUI;
        [SerializeField] private HelpGuideModalUI helpModalUI;
        [SerializeField] private CosmicMeltdownModalUI cosmicMeltdownModalUI;

        private bool isSoundMuted = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SanitizeAllSceneTexts();
        }

        [Header("Wave & Overdrive Banner")]
        [SerializeField] private TMP_Text waveTrackerText;
        [SerializeField] private Image startAdventureButtonImage;
        private int currentWaveCached = 1;
        private Coroutine bossWarningCoroutine;

        private int currentShieldCached = 100;
        private int maxShieldCached = 100;

        private void Start()
        {
            SanitizeAllSceneTexts();

            if (adventureModalUI != null)
            {
                adventureModalUI.gameObject.SetActive(false);
            }

            if (startAdventureButton != null)
            {
                startAdventureButtonImage = startAdventureButton.GetComponent<Image>();
                if (waveTrackerText == null)
                {
                    waveTrackerText = startAdventureButton.GetComponentInChildren<TMP_Text>(true);
                }
                startAdventureButton.onClick.AddListener(OnOverdriveButtonClicked);
            }

            if (saveGameButton != null)
            {
                saveGameButton.onClick.AddListener(OnSaveGameClicked);
            }
            if (quitGameButton != null)
            {
                quitGameButton.onClick.AddListener(OpenQuitModal);
            }
            if (soundToggleButton != null)
            {
                soundToggleButton.onClick.AddListener(OnToggleSoundClicked);
            }
            if (helpGuideButton != null)
            {
                helpGuideButton.onClick.AddListener(OpenHelpModal);
            }

            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnWaveStarted += HandleWaveStarted;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnCoreShieldChanged += HandleCoreShieldChanged;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnOverchargeChanged += HandleOverchargeChanged;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnBossWarningTriggered += HandleBossWarning;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnGameOverTriggered += HandleGameOverTriggered;

                currentWaveCached = CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentWave;
                currentShieldCached = CrystalForge.Adventure.CrystalDefenseManager.Instance.CoreCurrentShield;
                maxShieldCached = CrystalForge.Adventure.CrystalDefenseManager.Instance.CoreMaxShield;

                HandleOverchargeChanged(CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.MaxRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.IsOverdriveActive,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.OverdriveRemainingTimer);
            }
        }

        private void OnDestroy()
        {
            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnWaveStarted -= HandleWaveStarted;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnCoreShieldChanged -= HandleCoreShieldChanged;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnOverchargeChanged -= HandleOverchargeChanged;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnBossWarningTriggered -= HandleBossWarning;
                CrystalForge.Adventure.CrystalDefenseManager.Instance.OnGameOverTriggered -= HandleGameOverTriggered;
            }
        }

        private void OnOverdriveButtonClicked()
        {
            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                if (CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentRunicEnergy >= CrystalForge.Adventure.CrystalDefenseManager.Instance.MaxRunicEnergy &&
                    !CrystalForge.Adventure.CrystalDefenseManager.Instance.IsOverdriveActive)
                {
                    CrystalForge.Adventure.CrystalDefenseManager.Instance.TriggerRunicOverdrive();
                }
            }
        }

        private void HandleCoreShieldChanged(int current, int max)
        {
            currentShieldCached = current;
            maxShieldCached = max;
            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                HandleOverchargeChanged(CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.MaxRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.IsOverdriveActive,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.OverdriveRemainingTimer);
            }
        }

        private void HandleWaveStarted(int wave)
        {
            currentWaveCached = wave;
            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                HandleOverchargeChanged(CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.MaxRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.IsOverdriveActive,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.OverdriveRemainingTimer);
            }
        }

        private void HandleOverchargeChanged(float current, float max, bool isOverdrive, float remainingTime)
        {
            if (waveTrackerText == null && startAdventureButton != null)
            {
                waveTrackerText = startAdventureButton.GetComponentInChildren<TMP_Text>();
            }
            if (waveTrackerText == null) return;

            if (isOverdrive)
            {
                if (startAdventureButton != null) startAdventureButton.interactable = false;
                if (startAdventureButtonImage != null)
                {
                    startAdventureButtonImage.color = new Color(0.95f, 0.45f, 0.1f, 0.95f);
                }
                waveTrackerText.text = $"[OVERDRIVE] SÁT THƯƠNG x3 ({remainingTime:F1}s)";
            }
            else if (current >= max)
            {
                if (startAdventureButton != null) startAdventureButton.interactable = true;
                if (startAdventureButtonImage != null)
                {
                    startAdventureButtonImage.color = new Color(0.95f, 0.8f, 0.15f, 0.95f);
                }
                waveTrackerText.text = ">> KÍCH HOẠT RUNIC OVERDRIVE (BẤM)! <<";
            }
            else
            {
                if (startAdventureButton != null) startAdventureButton.interactable = false;
                int pct = Mathf.Clamp(Mathf.RoundToInt((current / max) * 100f), 0, 99);

                bool isLowShield = currentShieldCached <= (maxShieldCached * 0.35f);
                if (startAdventureButtonImage != null)
                {
                    startAdventureButtonImage.color = isLowShield
                        ? new Color(0.85f, 0.22f, 0.15f, 0.95f) // Đỏ cảnh báo nguy cấp khi khiên tụt thấp
                        : new Color(0.2f, 0.16f, 0.32f, 0.95f);
                }

                waveTrackerText.text = $"CHIẾN TRƯỜNG: ĐỢT #{currentWaveCached}  |  KHIÊN TRỤ: {currentShieldCached}/{maxShieldCached}  |  NỘ: {pct}%";
            }
        }

        private void HandleBossWarning(string bossTitle, int wave)
        {
            if (bossWarningCoroutine != null) StopCoroutine(bossWarningCoroutine);
            bossWarningCoroutine = StartCoroutine(BossWarningBannerRoutine(bossTitle, wave));
        }

        private IEnumerator BossWarningBannerRoutine(string bossTitle, int wave)
        {
            if (waveTrackerText != null && startAdventureButtonImage != null)
            {
                Color origCol = startAdventureButtonImage.color;
                float elapsed = 0f;
                float duration = 2.8f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    bool flash = (Mathf.Sin(elapsed * 18f) > 0f);
                    startAdventureButtonImage.color = flash ? new Color(0.9f, 0.15f, 0.2f, 0.95f) : new Color(0.3f, 0.05f, 0.1f, 0.95f);
                    waveTrackerText.text = $"⚠️ CẢNH BÁO: {FontSanitizer.Sanitize(bossTitle.ToUpper())} ĐỢT #{wave}! ⚠️";
                    yield return null;
                }
                startAdventureButtonImage.color = origCol;
            }
            bossWarningCoroutine = null;

            if (CrystalForge.Adventure.CrystalDefenseManager.Instance != null)
            {
                HandleOverchargeChanged(CrystalForge.Adventure.CrystalDefenseManager.Instance.CurrentRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.MaxRunicEnergy,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.IsOverdriveActive,
                                        CrystalForge.Adventure.CrystalDefenseManager.Instance.OverdriveRemainingTimer);
            }
        }

        public static void SanitizeAllSceneTexts()
        {
            var texts = Object.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include);
            foreach (var t in texts)
            {
                if (t != null && !string.IsNullOrEmpty(t.text))
                {
                    string sanitized = CrystalForge.Utils.FontSanitizer.Sanitize(t.text);
                    if (sanitized != t.text)
                    {
                        t.text = sanitized;
                    }
                }
            }
        }

        private void Update()
        {
            CheckEscapeKeyInput();
        }

        private void CheckEscapeKeyInput()
        {
            bool escapeTriggered = false;
            try
            {
                if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    escapeTriggered = true;
                }
            }
            catch {}

            if (!escapeTriggered)
            {
                try
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                    {
                        escapeTriggered = true;
                    }
                }
                catch {}
            }

            if (escapeTriggered)
            {
                ToggleQuitModal();
            }
        }

        private void OnSaveGameClicked()
        {
            if (CrystalForge.Economy.SaveLoadSystem.Instance != null)
            {
                CrystalForge.Economy.SaveLoadSystem.Instance.SaveGame(silent: false);
            }
            if (CrystalForge.Audio.AudioManager.Instance != null)
            {
                CrystalForge.Audio.AudioManager.Instance.PlayClickSfx();
            }
            Debug.Log("[UIManager] Đã bấm nút lưu game và lưu dữ liệu thành công!");
        }

        private void OnToggleSoundClicked()
        {
            isSoundMuted = !isSoundMuted;
            AudioListener.pause = isSoundMuted;
            if (CrystalForge.Audio.AudioManager.Instance != null)
            {
                CrystalForge.Audio.AudioManager.Instance.SetMute(isSoundMuted);
            }
            if (soundIconImage != null)
            {
                soundIconImage.color = isSoundMuted ? new Color(0.5f, 0.5f, 0.5f, 0.6f) : Color.white;
            }
        }

        public void OpenOfflineWelcomeModal()
        {
            if (offlineWelcomeModalPanel != null)
            {
                offlineWelcomeModalPanel.SetActive(true);
            }
        }

        public void SetEndGamePanelUI(EndGamePanelUI panel)
        {
            endGamePanelUI = panel;
        }

        public void OpenEndGameModal(int waveReached, string causeOfDeath)
        {
            if (endGamePanelUI != null)
            {
                endGamePanelUI.Show(waveReached, causeOfDeath);
            }
            else if (EndGamePanelUI.Instance != null)
            {
                EndGamePanelUI.Instance.Show(waveReached, causeOfDeath);
            }
        }

        private void HandleGameOverTriggered(int waveReached, string causeOfDeath)
        {
            OpenEndGameModal(waveReached, causeOfDeath);
        }

        public void SetQuitModalUI(QuitConfirmationModalUI modal)
        {
            quitModalUI = modal;
        }

        public void ToggleQuitModal()
        {
            var modal = quitModalUI != null ? quitModalUI : QuitConfirmationModalUI.Instance;
            if (modal != null)
            {
                if (modal.IsShowing)
                {
                    modal.Hide();
                }
                else
                {
                    modal.Show();
                }
            }
            else
            {
                QuitGameDirectly();
            }
        }

        public void OpenQuitModal()
        {
            var modal = quitModalUI != null ? quitModalUI : QuitConfirmationModalUI.Instance;
            if (modal != null)
            {
                modal.Show();
            }
            else
            {
                QuitGameDirectly();
            }
        }

        public void CloseQuitModal()
        {
            var modal = quitModalUI != null ? quitModalUI : QuitConfirmationModalUI.Instance;
            if (modal != null)
            {
                modal.Hide();
            }
        }

        public void SetHelpModalUI(HelpGuideModalUI modal)
        {
            helpModalUI = modal;
        }

        public void OpenHelpModal()
        {
            var modal = helpModalUI != null ? helpModalUI : HelpGuideModalUI.Instance;
            if (modal != null)
            {
                modal.Show();
            }
        }

        public void OpenCosmicMeltdownModal()
        {
            var modal = cosmicMeltdownModalUI != null ? cosmicMeltdownModalUI : CosmicMeltdownModalUI.Instance;
            if (modal != null)
            {
                modal.OpenModal();
            }
        }

        public void QuitGameDirectly()
        {
            if (CrystalForge.Audio.AudioManager.Instance != null)
            {
                CrystalForge.Audio.AudioManager.Instance.PlayClickSfx();
            }
            if (CrystalForge.Economy.SaveLoadSystem.Instance != null)
            {
                CrystalForge.Economy.SaveLoadSystem.Instance.SaveGame(silent: true);
            }
            Debug.Log("<color=#FF5555><b>[CrystalForge]</b> Đang thoát game...</color>");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
