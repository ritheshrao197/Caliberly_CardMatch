using UnityEngine;

namespace MemoryGame.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        [Header("Panel Settings")]
        [SerializeField] private PanelType panelType;
        public PanelType PanelType => panelType;

        protected CanvasGroup canvasGroup;

        protected virtual void Awake()
        {
            EnsureCanvasGroup();
        }

        public virtual void Show()
        {
            if (!EnsureCanvasGroup())
                return;

            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public virtual void Hide()
        {
            if (!EnsureCanvasGroup())
                return;

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            gameObject.SetActive(false);
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
    }
}
