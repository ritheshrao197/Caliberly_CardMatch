using MemoryGame.Runtime;
using MemoryGame.Config;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.Services;
using MemoryGame.UI.Events;
using UnityEngine;

namespace MemoryGame
{
    /// <summary>
    /// Thin composition root that wires runtime collaborators for gameplay flow.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Data")]
        public GameConfig config;
        public CardSet cardSet;
        public LevelConfig levelConfig;

        [Header("Scene Refs")]
        public Transform boardRoot;
        public CardController cardPrefab;
        public MatchResolver matchResolver;
        public TimerService timerService;
        public LevelRules levelRules;
        [Tooltip("Assign any MonoBehaviour implementing IProgressTracker (e.g., ProgressService).")]
        public MonoBehaviour progressTracker;
        public BoardFrame frame;

        private BoardRuntime _boardRuntime;
        private LevelSessionController _levelSession;
        private GameFlowCoordinator _gameFlowCoordinator;
        private IProgressTracker _progressTracker;

        private void Start()
        {
            if (!ValidateSetup())
                return;

            ComposeRuntime();
            _gameFlowCoordinator.Enable();
            EventBus.Instance.Publish(new OnGoHomeEvent());
        }

        private void OnDisable()
        {
            _gameFlowCoordinator?.Disable();
        }

        private bool ValidateSetup()
        {
            if (matchResolver == null)
                matchResolver = FindObjectOfType<MatchResolver>();

            if (!config || !cardSet || !boardRoot || !cardPrefab || !levelConfig || !matchResolver)
            {
                Debug.LogError("GameManager: Missing required references.");
                return false;
            }

            if (progressTracker == null)
            {
                Debug.LogError("GameManager: Missing progress tracker reference.");
                return false;
            }

            _progressTracker = progressTracker as IProgressTracker;
            if (_progressTracker == null)
            {
                Debug.LogError($"GameManager: Assigned progress tracker does not implement {nameof(IProgressTracker)}.");
                return false;
            }

            return true;
        }

        private void ComposeRuntime()
        {
            if (GetComponent<ScoreTracker>() == null)
                gameObject.AddComponent<ScoreTracker>();

            _boardRuntime = new BoardRuntime(boardRoot, cardSet, cardPrefab, frame, config);
            _levelSession = new LevelSessionController(
                levelConfig,
                config,
                _boardRuntime,
                matchResolver,
                timerService,
                levelRules,
                EventBus.Instance);

            _gameFlowCoordinator = new GameFlowCoordinator(EventBus.Instance, _levelSession, _progressTracker);
        }
    }
}
