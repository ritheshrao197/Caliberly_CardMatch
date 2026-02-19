using System;
using MemoryGame;
using MemoryGame.Config;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.Services;
using MemoryGame.UI.Events;
using MemoryGame.Views;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MemoryGame
{
    /// <summary>
    /// Main game bootstrap class that initializes and manages the game flow.
    /// Handles level progression, game state management, and coordination between services.
    /// </summary>
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }
        /// <summary>
        /// Game configuration data
        /// </summary>
        [Header("Data")]
        public GameConfig config;

        /// <summary>
        /// Collection of card sprites used in the game
        /// </summary>
        public CardSet cardSet;

        /// <summary>
        /// Configuration data for all game levels
        /// </summary>
        public LevelConfig levelConfig;


        /// <summary>
        /// References to scene objects and components
        /// </summary>
        [Header("Scene Refs")]
        public Transform boardRoot;
        public CardController cardPrefab;
        public MatchService matchService;
        public TimerService timerService;
        public LevelRules levelRules;
        public ProgressService progress;
        public UIManager uiFlow;
        public BoardFrame frame;
        private BoardController _board;
        private ObjectPool<CardController> _pool;
        private int _levelIndex;


        void Awake()
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
            EventBus.Instance.Subscribe<StartFromHomeEvent>(CmdStartFromHome);
            EventBus.Instance.Subscribe<StartLevelEvent>(CmdStartLevel);
            EventBus.Instance.Subscribe<RestartEvent>(CmdRestart);
            EventBus.Instance.Subscribe<GoHomeEvent>(CmdGoHome);
            EventBus.Instance.Subscribe<ShowHUDEvent>(CmdShowHUD);
            EventBus.Instance.Subscribe<PauseEvent>(CmdPause);
            EventBus.Instance.Subscribe<ResumeEvent>(CmdResume);
            EventBus.Instance.Subscribe<ResetProgressEvent>(CmdResetProgress);

        }



        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<StartFromHomeEvent>(CmdStartFromHome);
            EventBus.Instance.Unsubscribe<StartLevelEvent>(CmdStartLevel);
            EventBus.Instance.Unsubscribe<RestartEvent>(CmdRestart);
            EventBus.Instance.Unsubscribe<GoHomeEvent>(CmdGoHome);
            EventBus.Instance.Unsubscribe<ShowHUDEvent>(CmdShowHUD);
            EventBus.Instance.Unsubscribe<PauseEvent>(CmdPause);
            EventBus.Instance.Unsubscribe<ResumeEvent>(CmdResume);
            EventBus.Instance.Unsubscribe<ResetProgressEvent>(CmdResetProgress);
        }

        /// <summary>
        /// Initializes the game on start
        /// </summary>
        private void Start()
        {
            if (!config || !cardSet || !boardRoot || !cardPrefab || !levelConfig)
            {
                Debug.LogError("GameBootstrap: Assign config, cardSet, levelConfig, boardRoot, cardPrefab.");
                return;
            }

            _pool = new ObjectPool<CardController>(cardPrefab, boardRoot, prewarm: 0);
            _board = new BoardController(boardRoot, cardSet, _pool, frame);

            _levelIndex = progress ? progress.GetCurrentLevelIndex() : 0;

            if (uiFlow) uiFlow.ShowHome(new OnGoHomeEvent()); else StartLevel(_levelIndex);

            EventBus.Instance.Subscribe<GameWonEvent>(OnGameWon);
            EventBus.Instance.Subscribe<GameLostEvent>(OnGameLost);

        }

        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<GameWonEvent>(OnGameWon);
            EventBus.Instance.Unsubscribe<GameLostEvent>(OnGameLost);
        }

        /// <summary>
        /// Starts a specific level by index
        /// </summary>
        /// <param name="index">Zero-based index of the level to start</param>
        private void StartLevel(int index)
        {
            Debug.Log($"[GameBootstrap] Starting Level {index + 1}");
            if (index < 0 || index >= levelConfig.levels.Count)
            {
                Debug.Log("All levels complete. Restarting from Level 1.");
                index = 0;
            }
            _levelIndex = index;

            var def = levelConfig.levels[_levelIndex];


            // Per-level timing overrides
            if (def.flipDuration > 0) config.flipDuration = def.flipDuration;
            if (def.mismatchHideDelay > 0) config.mismatchHideDelay = def.mismatchHideDelay;

            _board.Build(def.rows, def.cols);

            matchService?.RegisterCards(_board.Cards);
            if (timerService != null) { timerService.ResetTimer(); timerService.StartTimer(); }
            levelRules?.BeginLevel(def, _levelIndex, timerService);

            EventBus.Instance.Publish(new LevelStartedEvent(def, _levelIndex));
        }

        /// <summary>
        /// Handles game won event
        /// </summary>
        private void OnGameWon(GameWonEvent @event)
        {
            EventBus.Instance.Publish(new LevelCompletedEvent(_levelIndex));
            progress?.UnlockNextLevel(_levelIndex);

            if (uiFlow)
            {
                uiFlow.ShowResult(true, _levelIndex, null,
                    onNext: () =>
                    {
                        StartLevel(_levelIndex + 1);
                        EventBus.Instance.Publish(new ShowHUDEvent());
                    },
                    onHome: () => EventBus.Instance.Publish(new GoHomeEvent()));
            }
            else StartLevel(_levelIndex + 1);
        }

        /// <summary>
        /// Handles game lost event
        /// </summary>
        /// <param name="reason">Reason for losing the game</param>
        private void OnGameLost(GameLostEvent @event)
        {
            if (uiFlow)
            {
                uiFlow.ShowResult(false, _levelIndex, @event.Reason,
                    onNext: () => StartLevel(_levelIndex),  // retry
                    onHome: () => EventBus.Instance.Publish(new GoHomeEvent()));
            }
            else StartLevel(_levelIndex);
        }

        // === UIEvents handlers ===
        /// <summary>
        /// Command handler for starting from home screen
        /// </summary>

        private void CmdStartFromHome(StartFromHomeEvent @event)
        {
            var idx = progress ? progress.GetCurrentLevelIndex() : 0;
            StartLevel(idx);
            EventBus.Instance.Publish(new ShowHUDEvent());
        }
        /// <summary>
        /// Command handler for starting a specific level
        /// </summary>
        /// <param name="index">Index of the level to start</param>
        void CmdStartLevel(StartLevelEvent @event)
        { StartLevel(@event.LevelIndex); EventBus.Instance.Publish(new ShowHUDEvent()); }

        /// <summary>
        /// Command handler for restarting the current level
        /// </summary>
        void CmdRestart(RestartEvent @event)
        {
            StartLevel(_levelIndex);
        }

        /// <summary>
        /// Command handler for returning to home screen
        /// </summary>
        void CmdGoHome(GoHomeEvent @event   ) {  timerService?.StopTimer(); }

        /// <summary>
        /// Command handler for showing the HUD
        /// </summary>
        void CmdShowHUD(ShowHUDEvent @event) {  }

        /// <summary>
        /// Command handler for pausing the game
        /// </summary>
        void CmdPause(PauseEvent @event) { timerService?.StopTimer();  }

        /// <summary>
        /// Command handler for resuming the game
        /// </summary>
        void CmdResume(ResumeEvent @event) { timerService?.StartTimer();  }

        /// <summary>
        /// Command handler for resetting player progress
        /// </summary>
        void CmdResetProgress(ResetProgressEvent @event) { progress?.ResetProgress(); }
    }
}