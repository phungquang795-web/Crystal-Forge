using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Audio;
using CrystalForge.Adventure;
using CrystalForge.Economy;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    [DefaultExecutionOrder(-10)]
    public class EndGamePanelUI : MonoBehaviour
    {
        private static EndGamePanelUI instance;
        public static EndGamePanelUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UnityEngine.Object.FindAnyObjectByType<EndGamePanelUI>(FindObjectsInactive.Include);
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("UI Text Displays")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text reasonText;
        [SerializeField] private TMP_Text waveReachedText;
        [SerializeField] private TMP_Text statsSummaryText;

        [Header("Interactive Buttons")]
        [SerializeField] private Button restartButton;
        [SerializeField] private TMP_Text restartButtonText;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text quitButtonText;

        [Header("Canvas Group & Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform cardContainer;

        private Coroutine popupAnimationCoroutine;
        private Vector3 initialCardScale = Vector3.one;
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

            EnsureInitialized();
        }

        private void OnEnable()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnGameOverTriggered -= HandleGameOver;
                CrystalDefenseManager.Instance.OnGameOverTriggered += HandleGameOver;
            }
        }

        private void OnDisable()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnGameOverTriggered -= HandleGameOver;
            }
        }

        private void Start()
        {
            EnsureInitialized();

            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnGameOverTriggered -= HandleGameOver;
                CrystalDefenseManager.Instance.OnGameOverTriggered += HandleGameOver;
            }

            // Chỉ ẩn nếu modal chưa được yêu cầu hiển thị từ trước
            if (!isShowing)
            {
                HideInstant();
            }
        }

        private void OnDestroy()
        {
            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.OnGameOverTriggered -= HandleGameOver;
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

            if (cardContainer != null)
            {
                initialCardScale = Vector3.one;
            }

            if (restartButton == null && cardContainer != null)
            {
                restartButton = cardContainer.Find("RestartButton")?.GetComponent<Button>();
            }

            if (quitButton == null && cardContainer != null)
            {
                quitButton = cardContainer.Find("QuitButton")?.GetComponent<Button>();
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        private void HandleGameOver(int waveReached, string causeOfDeath)
        {
            Show(waveReached, causeOfDeath);
        }

        public void Show(int waveReached, string causeOfDeath)
        {
            isShowing = true;
            EnsureInitialized();

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            transform.SetAsLastSibling();

            if (titleText != null)
            {
                titleText.text = "CHIẾN TRƯỜNG THẤT THỦ";
            }

            if (reasonText != null)
            {
                string reason = string.IsNullOrEmpty(causeOfDeath)
                    ? "Dũng Sĩ Hộ Vệ đã kiệt sức trước đợt quái hung hãn!"
                    : causeOfDeath;
                reasonText.text = FontSanitizer.Sanitize(reason);
            }

            if (waveReachedText != null)
            {
                waveReachedText.text = $"KỶ LỤC ĐẠT ĐƯỢC: ĐỢT #{waveReached}";
            }

            if (statsSummaryText != null)
            {
                double crystals = ResourceManager.Instance != null ? ResourceManager.Instance.CurrentCrystal : 0;
                string crystalStr = CrystalFormat.Format(crystals);
                statsSummaryText.text = $"Pha Le Tich Luy: {crystalStr} CR\nNang cap The Luc, Cong Kich Dung Si & Khien Tru de tro lai manh me hon!";
            }

            if (restartButtonText != null)
            {
                restartButtonText.text = "CHOI LAI NGAY";
            }

            if (quitButtonText != null)
            {
                quitButtonText.text = "THOAT GAME";
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            // Âm thanh thất bại
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameOverSfx();
            }

            // Hoạt họa nảy đàn hồi (Spring Juice Pop)
            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimateCardPopIn());
        }

        private IEnumerator AnimateCardPopIn()
        {
            if (cardContainer == null) yield break;

            float elapsed = 0f;
            float duration = 0.35f;
            cardContainer.localScale = Vector3.one * 0.72f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Spring overshoot bounce
                float scale = 0.72f + Mathf.Sin(t * Mathf.PI * 0.75f) * 0.34f;
                if (t >= 0.85f)
                {
                    scale = Mathf.Lerp(scale, 1.0f, (t - 0.85f) / 0.15f);
                }

                cardContainer.localScale = Vector3.one * scale;
                yield return null;
            }

            cardContainer.localScale = initialCardScale;
            popupAnimationCoroutine = null;
        }

        public void HideInstant()
        {
            isShowing = false;
            EnsureInitialized();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        public void Hide()
        {
            StartCoroutine(AnimateHideRoutine());
        }

        private IEnumerator AnimateHideRoutine()
        {
            EnsureInitialized();
            if (cardContainer != null)
            {
                float elapsed = 0f;
                float duration = 0.15f;
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = elapsed / duration;
                    cardContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t);
                    if (canvasGroup != null) canvasGroup.alpha = 1f - t;
                    yield return null;
                }
            }

            HideInstant();
        }

        private void OnRestartButtonClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }

            HideInstant();

            if (CrystalDefenseManager.Instance != null)
            {
                CrystalDefenseManager.Instance.RestartDefense();
            }
        }

        private void OnQuitButtonClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.OpenQuitModal();
            }
            else
            {
                if (SaveLoadSystem.Instance != null)
                {
                    SaveLoadSystem.Instance.SaveGame(silent: true);
                }
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
