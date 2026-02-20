using MemoryGame.Events;
using MemoryGame.Services;
using MemoryGame.UI.Events;

namespace MemoryGame.Runtime
{
    /// <summary>
    /// Handles event-driven game flow transitions while keeping event contracts unchanged.
    /// </summary>
    public class GameFlowCoordinator
    {
        private readonly EventBus _bus;
        private readonly LevelSessionController _levelSession;
        private readonly IProgressTracker _progressTracker;

        public GameFlowCoordinator(
            EventBus bus,
            LevelSessionController levelSession,
            IProgressTracker progressTracker)
        {
            _bus = bus;
            _levelSession = levelSession;
            _progressTracker = progressTracker;
        }

        public void Enable()
        {
            _bus.Subscribe<StartFromHomeEvent>(OnStartFromHome);
            _bus.Subscribe<StartLevelEvent>(OnStartLevel);
            _bus.Subscribe<OnRestartEvent>(OnRestart);
            _bus.Subscribe<OnGoHomeEvent>(OnGoHome);
            _bus.Subscribe<OnPauseEvent>(OnPause);
            _bus.Subscribe<OnResumeEvent>(OnResume);
            _bus.Subscribe<ResetProgressEvent>(OnResetProgress);
            _bus.Subscribe<GameWonEvent>(OnGameWon);
            _bus.Subscribe<GameLostEvent>(OnGameLost);
        }

        public void Disable()
        {
            _bus.Unsubscribe<StartFromHomeEvent>(OnStartFromHome);
            _bus.Unsubscribe<StartLevelEvent>(OnStartLevel);
            _bus.Unsubscribe<OnRestartEvent>(OnRestart);
            _bus.Unsubscribe<OnGoHomeEvent>(OnGoHome);
            _bus.Unsubscribe<OnPauseEvent>(OnPause);
            _bus.Unsubscribe<OnResumeEvent>(OnResume);
            _bus.Unsubscribe<ResetProgressEvent>(OnResetProgress);
            _bus.Unsubscribe<GameWonEvent>(OnGameWon);
            _bus.Unsubscribe<GameLostEvent>(OnGameLost);
        }

        private void OnStartFromHome(StartFromHomeEvent e)
        {
            int levelIndex = _progressTracker != null ? _progressTracker.GetCurrentLevelIndex() : 0;
            _levelSession.StartLevel(levelIndex);
            _bus.Publish(new OnShowHUDEvent());
        }

        private void OnStartLevel(StartLevelEvent e)
        {
            _levelSession.StartLevel(e.LevelIndex);
            _bus.Publish(new OnShowHUDEvent());
        }

        private void OnRestart(OnRestartEvent e)
        {
            _levelSession.RestartCurrentLevel();
        }

        private void OnGoHome(OnGoHomeEvent e)
        {
            _levelSession.Stop();
        }

        private void OnPause(OnPauseEvent e)
        {
            _levelSession.Pause();
        }

        private void OnResume(OnResumeEvent e)
        {
            _levelSession.Resume();
        }

        private void OnResetProgress(ResetProgressEvent e)
        {
            _progressTracker?.ResetProgress();
        }

        private void OnGameWon(GameWonEvent e)
        {
            int currentIndex = _levelSession.CurrentLevelIndex;
            _progressTracker?.UnlockNextLevel(currentIndex);

            _bus.Publish(new LevelCompletedEvent(currentIndex));
            _bus.Publish(new ShowResultEvent(
                true,
                currentIndex,
                null,
                () => _levelSession.StartLevel(currentIndex + 1),
                () => _bus.Publish(new OnGoHomeEvent())));
        }

        private void OnGameLost(GameLostEvent e)
        {
            int currentIndex = _levelSession.CurrentLevelIndex;
            _bus.Publish(new ShowResultEvent(
                false,
                currentIndex,
                e.Reason,
                () => _levelSession.StartLevel(currentIndex),
                () => _bus.Publish(new OnGoHomeEvent())));
        }
    }
}
