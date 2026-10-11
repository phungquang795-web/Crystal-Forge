using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CrystalForge.Audio;
using CrystalForge.Utils;

namespace CrystalForge.UI
{
    [DefaultExecutionOrder(-10)]
    public class HelpGuideModalUI : MonoBehaviour
    {
        private static HelpGuideModalUI instance;
        public static HelpGuideModalUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindAnyObjectByType<HelpGuideModalUI>(FindObjectsInactive.Include);
                }
                return instance;
            }
            private set => instance = value;
        }

        [Header("UI Text Displays")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text guideContentText;

        [Header("Close Button")]
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text closeButtonText;

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
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(Hide);
            }

            if (!isShowing)
            {
                HideInstant();
            }
        }

        private void EnsureInitialized()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            }

            if (cardContainer == null)
            {
                var card = transform.Find("CardContainer");
                if (card != null) cardContainer = card.GetComponent<RectTransform>();
            }

            if (cardContainer != null)
            {
                initialCardScale = cardContainer.localScale;
                if (initialCardScale == Vector3.zero) initialCardScale = Vector3.one;
            }
        }

        public void Show()
        {
            EnsureInitialized();
            gameObject.SetActive(true);
            isShowing = true;

            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimateShowRoutine());

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }
        }

        public void Hide()
        {
            if (!isShowing) return;

            EnsureInitialized();
            if (popupAnimationCoroutine != null) StopCoroutine(popupAnimationCoroutine);
            popupAnimationCoroutine = StartCoroutine(AnimateHideRoutine());

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClickSfx();
            }
        }

        public void HideInstant()
        {
            EnsureInitialized();
            isShowing = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        private IEnumerator AnimateShowRoutine()
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }

            float elapsed = 0f;
            float duration = 0.22f;

            if (cardContainer != null) cardContainer.localScale = initialCardScale * 0.82f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easeOut = 1f - Mathf.Pow(1f - t, 3f);

                if (canvasGroup != null) canvasGroup.alpha = easeOut;
                if (cardContainer != null)
                {
                    cardContainer.localScale = Vector3.LerpUnclamped(initialCardScale * 0.82f, initialCardScale, easeOut);
                }

                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (cardContainer != null) cardContainer.localScale = initialCardScale;
            popupAnimationCoroutine = null;
        }

        private IEnumerator AnimateHideRoutine()
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            float elapsed = 0f;
            float duration = 0.16f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                if (canvasGroup != null) canvasGroup.alpha = 1f - t;
                if (cardContainer != null)
                {
                    cardContainer.localScale = Vector3.Lerp(initialCardScale, initialCardScale * 0.85f, t);
                }

                yield return null;
            }

            if (canvasGroup != null) canvasGroup.alpha = 0f;
            if (cardContainer != null) cardContainer.localScale = initialCardScale;

            isShowing = false;
            gameObject.SetActive(false);
            popupAnimationCoroutine = null;
        }
    }
}
