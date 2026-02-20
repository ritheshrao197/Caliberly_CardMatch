using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] private PanelType panelType;
        [SerializeField, Min(0f)] private float fadeInDuration = 0.2f;
        [SerializeField, Min(0f)] private float fadeOutDuration = 0.2f;
        [SerializeField] private bool useUnscaledTime = true;
        public PanelType PanelType => panelType;
        public bool IsVisible => _isVisible;

        protected CanvasGroup canvasGroup;
        protected GraphicRaycaster graphicRaycaster;
        private Coroutine _fadeRoutine;
        private bool _isVisible;

        protected virtual void Awake()
        {
            if (!EnsureCanvasGroup())
                return;

            EnsureGraphicRaycaster();
            _isVisible = canvasGroup.blocksRaycasts && canvasGroup.alpha > 0f;
        }

        public virtual void Show()
        {
            if (!EnsureCanvasGroup())
                return;

            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }

            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            if (graphicRaycaster != null)
                graphicRaycaster.enabled = true;
            _isVisible = true;

            if (!isActiveAndEnabled)
            {
                canvasGroup.alpha = 1f;
                return;
            }

            _fadeRoutine = StartCoroutine(FadeTo(1f, fadeInDuration));
        }

        public virtual void Hide()
        {
            if (!EnsureCanvasGroup())
                return;

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            if (graphicRaycaster != null)
                graphicRaycaster.enabled = false;
            _isVisible = false;

            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }

            if (!isActiveAndEnabled)
            {
                canvasGroup.alpha = 0f;
                return;
            }

            _fadeRoutine = StartCoroutine(FadeTo(0f, fadeOutDuration));
        }

        private bool EnsureCanvasGroup()
        {
            if (canvasGroup != null)
                return true;

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                return true;

            Debug.LogError($"UIPanel '{name}' is missing a CanvasGroup component.");
            return false;
        }

        private void EnsureGraphicRaycaster()
        {
            if (graphicRaycaster != null)
                return;

            graphicRaycaster = GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null)
                graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            if (duration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                _fadeRoutine = null;
                yield break;
            }

            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            _fadeRoutine = null;
        }
    }
}
