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
    public class QuitConfirmationModalUI : MonoBehaviour
    {
        private static QuitConfirmationModalUI instance;
        public static QuitConfirmationModalUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindAnyObjectByType<QuitConfirmationModalUI>(FindObjectsInactive.Include);
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("UI Text Displays")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;

        [Header("Buttons")]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmQuitButton;
        [SerializeField] private TMP_Text cancelButtonText;
        [SerializeField] private TMP_Text confirmQuitButtonText;

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

            if (cardContainer != null)
            {
                initialCardScale = Vector3.one;
            }

            if (cancelButton == null && cardContainer != null)
            {
                cancelButton = cardContainer.Find("CancelButton")?.GetComponent<Button>();
            }

            if (confirmQuitButton == null && cardContainer != null)
            {
                confirmQuitButton = cardContainer.Find("ConfirmQuitButton")?.GetComponent<Button>();
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelClicked);
            }

            if (confirmQuitButton != null)
            {
                confirmQuitButton.onClick.RemoveAllListeners();
                confirmQuitButton.onClick.AddListener(OnConfirmQuitClicked);
            }
        }

        public void Show()
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
                titleText.text = "XÁC NHẬN THOÁT GAME";
            }

            if (messageText != null)
            {
                messageText.text = "Tiến trình của Chúa Tể sẽ được tự động lưu lại an toàn.\nBạn có chắc chắn muốn rời khỏi Thánh Điện không?";
            }

            if (cancelButtonText != null)
            {
                cancelButtonText.text = "TIẾP TỤC CHƠI";
            }

            if (confirmQuitButtonText != null)
            {
                confirmQuitButtonText.text = "THOÁT GAME";
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }

            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimateCardPopIn());
        }

        private IEnumerator AnimateCardPopIn()
        {
            if (cardContainer == null) yield break;

            float elapsed = 0f;
            float duration = 0.28f;
            cardContainer.localScale = Vector3.one * 0.75f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float scale = 0.75f + Mathf.Sin(t * Mathf.PI * 0.75f) * 0.3f;
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
            if (!gameObject.activeSelf || !isShowing) return;
            StartCoroutine(AnimateHideRoutine());
        }

        private IEnumerator AnimateHideRoutine()
        {
            EnsureInitialized();
            if (cardContainer != null)
            {
                float elapsed = 0f;
                float duration = 0.12f;
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

        private void OnCancelClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }
            Hide();
        }

        private void OnConfirmQuitClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }

            // Lưu dữ liệu trước khi thoát
            if (SaveLoadSystem.Instance != null)
            {
                SaveLoadSystem.Instance.SaveGame(silent: true);
            }

            Debug.Log("<color=#FF5555><b>[CrystalForge]</b> Thoát game thành công sau khi đã lưu tiến trình.</color>");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
