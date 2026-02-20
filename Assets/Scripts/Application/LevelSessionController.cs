using MemoryGame.Config;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.Services;

namespace MemoryGame.Runtime
{
    /// <summary>
    /// Controls level runtime session state (start/restart/pause/resume/stop).
    /// </summary>
    public class LevelSessionController
    {
        private readonly LevelConfig _levelConfig;
        private readonly GameConfig _gameConfig;
        private readonly BoardRuntime _boardRuntime;
        private readonly MatchResolver _matchResolver;
        private readonly TimerService _timerService;
        private readonly LevelRules _levelRules;
        private readonly EventBus _bus;

        public int CurrentLevelIndex { get; private set; }

        public LevelSessionController(
            LevelConfig levelConfig,
            GameConfig gameConfig,
            BoardRuntime boardRuntime,
            MatchResolver matchResolver,
            TimerService timerService,
            LevelRules levelRules,
            EventBus bus)
        {
            _levelConfig = levelConfig;
            _gameConfig = gameConfig;
            _boardRuntime = boardRuntime;
            _matchResolver = matchResolver;
            _timerService = timerService;
            _levelRules = levelRules;
            _bus = bus;
        }

        public void StartLevel(int index)
        {
            if (_levelConfig == null || _levelConfig.levels == null || _levelConfig.levels.Count == 0)
                return;

            if (index < 0 || index >= _levelConfig.levels.Count)
                index = 0;

            CurrentLevelIndex = index;
            LevelDef def = _levelConfig.levels[CurrentLevelIndex];
            _timerService?.StopTimer();
            _boardRuntime.Clear();

            if (_gameConfig != null)
            {
                if (def.flipDuration > 0f)
                    _gameConfig.flipDuration = def.flipDuration;
                if (def.mismatchHideDelay > 0f)
                    _gameConfig.mismatchHideDelay = def.mismatchHideDelay;
            }

            var cards = _boardRuntime.Build(def.rows, def.cols);
            _matchResolver?.RegisterCards(cards);

            _timerService?.ResetTimer();
            _timerService?.StartTimer();

            _levelRules?.BeginLevel(def, CurrentLevelIndex, _timerService);
            _bus.Publish(new LevelStartedEvent(def, CurrentLevelIndex));
        }

        public void RestartCurrentLevel()
        {
            StartLevel(CurrentLevelIndex);
        }

        public void Pause()
        {
            _timerService?.StopTimer();
        }

        public void Resume()
        {
            _timerService?.StartTimer();
        }

        public void Stop()
        {
            _timerService?.StopTimer();
            _boardRuntime.Clear();
            _matchResolver?.RegisterCards(System.Array.Empty<CardController>());
        }
    }
}
