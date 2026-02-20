using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// Pause popup view that publishes resume/restart/home commands.
    /// </summary>
    public class PausePopup : UIPanel
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button homeButton;

        private EventBus _bus;

        protected override void Awake()
        {
            base.Awake();
            _bus = EventBus.Instance;

            if (resumeButton)
                resumeButton.onClick.AddListener(OnResumeClicked);

            if (restartButton)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (homeButton)
                homeButton.onClick.AddListener(OnHomeClicked);
        }

        private void OnDestroy()
        {
            if (resumeButton)
                resumeButton.onClick.RemoveListener(OnResumeClicked);

            if (restartButton)
                restartButton.onClick.RemoveListener(OnRestartClicked);

            if (homeButton)
                homeButton.onClick.RemoveListener(OnHomeClicked);
        }

        private void OnResumeClicked()
        {
            _bus.Publish(new OnResumeEvent());
        }

        private void OnRestartClicked()
        {
            _bus.Publish(new OnRestartEvent());
        }

        private void OnHomeClicked()
        {
            _bus.Publish(new OnGoHomeEvent());
        }
    }
}
