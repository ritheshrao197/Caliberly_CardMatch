using System;
using MemoryGame.Config;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.Services;
using MemoryGame.UI.Events;
using UnityEngine;
using UnityEngine.Pool;

namespace MemoryGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Data")]
        public GameConfig config;
        public CardSet cardSet;
        public LevelConfig levelConfig;

        [Header("Scene Refs")]
        public Transform boardRoot;
        public CardController cardPrefab;
        public MatchService matchService;
        public TimerService timerService;
        public LevelRules levelRules;
        public ProgressService progress;
        public BoardFrame frame;

        private BoardController _board;
        private ObjectPool<CardController> _pool;
        private int _levelIndex;

        private EventBus Bus => EventBus.Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Bus.Subscribe<StartFromHomeEvent>(CmdStartFromHome);
            Bus.Subscribe<StartLevelEvent>(CmdStartLevel);
            Bus.Subscribe<OnRestartEvent>(CmdRestart);
            Bus.Subscribe<OnGoHomeEvent>(CmdGoHome);
            Bus.Subscribe<OnPauseEvent>(CmdPause);
            Bus.Subscribe<OnResumeEvent>(CmdResume);
            Bus.Subscribe<ResetProgressEvent>(CmdResetProgress);
            Bus.Subscribe<GameWonEvent>(OnGameWon);
            Bus.Subscribe<GameLostEvent>(OnGameLost);
        }

        private void OnDisable()
        {
            Bus.Unsubscribe<StartFromHomeEvent>(CmdStartFromHome);
            Bus.Unsubscribe<StartLevelEvent>(CmdStartLevel);
            Bus.Unsubscribe<OnRestartEvent>(CmdRestart);
            Bus.Unsubscribe<OnGoHomeEvent>(CmdGoHome);
            Bus.Unsubscribe<OnPauseEvent>(CmdPause);
            Bus.Unsubscribe<OnResumeEvent>(CmdResume);
            Bus.Unsubscribe<ResetProgressEvent>(CmdResetProgress);
            Bus.Unsubscribe<GameWonEvent>(OnGameWon);
            Bus.Unsubscribe<GameLostEvent>(OnGameLost);
        }

        private void Start()
        {
            ValidateSetup();

            _pool = new ObjectPool<CardController>(cardPrefab, boardRoot, 0);
            _board = new BoardController(boardRoot, cardSet, _pool, frame, config);

            _levelIndex = progress ? progress.GetCurrentLevelIndex() : 0;

            Bus.Publish(new OnGoHomeEvent());
        }

        private void ValidateSetup()
        {
            if (!config || !cardSet || !boardRoot || !cardPrefab || !levelConfig)
                Debug.LogError("GameManager: Missing required references.");
        }

        private void StartLevel(int index)
        {
            if (index < 0 || index >= levelConfig.levels.Count)
                index = 0;

            _levelIndex = index;
            var def = levelConfig.levels[_levelIndex];

            if (def.flipDuration > 0)
                config.flipDuration = def.flipDuration;

            if (def.mismatchHideDelay > 0)
                config.mismatchHideDelay = def.mismatchHideDelay;

            _board.Build(def.rows, def.cols);

            matchService?.RegisterCards(_board.Cards);

            timerService?.ResetTimer();
            timerService?.StartTimer();

            levelRules?.BeginLevel(def, _levelIndex, timerService);

            Bus.Publish(new LevelStartedEvent(def, _levelIndex));
        }

        private void OnGameWon(GameWonEvent e)
        {
            progress?.UnlockNextLevel(_levelIndex);
            Bus.Publish(new LevelCompletedEvent(_levelIndex));
            Bus.Publish(new ShowResultEvent(
                true,
                _levelIndex,
                null,
                () => StartLevel(_levelIndex + 1),
                () => Bus.Publish(new OnGoHomeEvent())
            ));
        }

        private void OnGameLost(GameLostEvent e)
        {
            Bus.Publish(new ShowResultEvent(
                false,
                _levelIndex,
                e.Reason,
                () => StartLevel(_levelIndex),
                () => Bus.Publish(new OnGoHomeEvent())
            ));
        }

        private void CmdStartFromHome(StartFromHomeEvent e)
        {
            StartLevel(progress ? progress.GetCurrentLevelIndex() : 0);
            Bus.Publish(new OnShowHUDEvent());
        }

        private void CmdStartLevel(StartLevelEvent e)
        {
            StartLevel(e.LevelIndex);
            Bus.Publish(new OnShowHUDEvent());
        }

        private void CmdRestart(OnRestartEvent e)
        {
            StartLevel(_levelIndex);
        }

    

        private void CmdGoHome(OnGoHomeEvent e)
        {
            timerService?.StopTimer();
        }

        private void CmdPause(OnPauseEvent e)
        {
            timerService?.StopTimer();
        }

        private void CmdResume(OnResumeEvent e)
        {
            timerService?.StartTimer();
        }

        private void CmdResetProgress(ResetProgressEvent e)
        {
            progress?.ResetProgress();
        }
    }
}
