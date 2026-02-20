using MemoryGame.Events;
using MemoryGame.Services;
using TMPro;
using UnityEngine;

namespace MemoryGame.Views
{  
    /// <summary>
    /// View component for the HUD (Heads-Up Display) in the Memory Game.
    /// Displays level information, timer, move count, and game status.
    /// </summary>  
    public class HudView : UIPanel
    {
        /// <summary>
        /// Text component to display the current level information
        /// </summary>
        public TextMeshProUGUI levelText;
        
        /// <summary>
        /// Text component to display the elapsed time
        /// </summary>
        public TextMeshProUGUI timerText;
        
        /// <summary>
        /// Text component to display the move count
        /// </summary>
        public TextMeshProUGUI movesText;
        
        /// <summary>
        /// Text component to display the game status
        /// </summary>
        public TextMeshProUGUI statusText;

        /// <summary>
        /// Text component to display score and combo
        /// </summary>
        public TextMeshProUGUI scoreText;

        private EventBus _bus;
        private int _moves;
        private int _moveLimit;
        private float _timeLimit;
        private TimerService _timer;
        private int _lastTimerSecond = -1;
        private string _timeLimitFormatted;

        /// <summary>
        /// Initializes the component, finds the timer service, and registers game event handlers
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _bus = EventBus.Instance;
            _timer = FindObjectOfType<TimerService>();
        }

        private void OnEnable()
        {
            if (_bus == null)
                _bus = EventBus.Instance;

            _bus.Subscribe<LevelStartedEvent>(OnLevelStarted);
            _bus.Subscribe<PairMatchedEvent>(OnPairEvent);
            _bus.Subscribe<PairMismatchedEvent>(OnPairEvent);
            _bus.Subscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            _bus.Subscribe<GameWonEvent>(OnGameWon);
            _bus.Subscribe<GameLostEvent>(OnGameLost);
            _bus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        }
        
        /// <summary>
        /// Unregisters game event handlers when the component is destroyed
        /// </summary>
        private void OnDisable()
        {
            if (_bus == null)
                return;

            _bus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
            _bus.Unsubscribe<PairMatchedEvent>(OnPairEvent);
            _bus.Unsubscribe<PairMismatchedEvent>(OnPairEvent);
            _bus.Unsubscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            _bus.Unsubscribe<GameWonEvent>(OnGameWon);
            _bus.Unsubscribe<GameLostEvent>(OnGameLost);
            _bus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        }

        /// <summary>
        /// Updates the timer display each frame
        /// </summary>
        private void Update()
        {
            if (_timer == null || timerText == null)
                return;

            int elapsedSeconds = Mathf.Max(0, Mathf.FloorToInt(_timer.elapsed));
            if (elapsedSeconds == _lastTimerSecond)
                return;

            _lastTimerSecond = elapsedSeconds;
            int m = elapsedSeconds / 60;
            int s = elapsedSeconds % 60;
            timerText.text = _timeLimit > 0f
                ? $"{m:00}:{s:00} / {_timeLimitFormatted}"
                : $"{m:00}:{s:00}";
        }

        /// <summary>
        /// Formats a time value in seconds to MM:SS format
        /// </summary>
        /// <param name="seconds">Time in seconds</param>
        /// <returns>Formatted time string in MM:SS format</returns>
        private string FormatTime(float seconds)
        {
            int m = (int)(seconds / 60f);
            int s = (int)(seconds % 60f);
            return $"{m:00}:{s:00}";
        }

        /// <summary>
        /// Handles the level started event by initializing HUD values
        /// </summary>
        /// <param name="def">The level definition</param>
        /// <param name="idx">The zero-based level index</param>
        private void OnLevelStarted(LevelStartedEvent evt)
        {
            var def = evt.Level;
            var idx = evt.Index;
            
        
            _moves = 0;
            _moveLimit = def.moveLimit;
            _timeLimit = def.timeLimitSec;
            _timeLimitFormatted = _timeLimit > 0f ? FormatTime(_timeLimit) : string.Empty;
            _lastTimerSecond = -1;
            if (levelText) 
                levelText.text = $" {idx + 1}: {def.rows}x{def.cols}"; 
            if (movesText) 
                movesText.text = _moveLimit > 0 ? $"0 / {_moveLimit}" : "0";
            if (statusText) 
                statusText.text = "Find all pairs";
            if (scoreText)
                scoreText.text = "0";
        }

        /// <summary>
        /// Handles pair match/mismatch events by updating the move count display
        /// </summary>
        /// <param name="a">First card in the pair</param>
        /// <param name="b">Second card in the pair</param>
        private void OnPairEvent(PairMatchedEvent evt)
        {
            _moves++;
            if (movesText) 
                movesText.text = _moveLimit > 0 ? $"{_moves} / {_moveLimit}" : _moves.ToString();
        }
        private void OnPairEvent(PairMismatchedEvent evt)
        {
            _moves++;
            if (movesText) 
                movesText.text = _moveLimit > 0 ? $"{_moves} / {_moveLimit}" : _moves.ToString();
        }
        /// <summary>
        /// Handles the remaining pairs changed event by updating the status display
        /// </summary>
        /// <param name="v">Number of remaining pairs</param>
        private void OnRemainingPairsChanged(RemainingPairsChangedEvent evt) { 
            if (statusText) 
                statusText.text = $"Pairs left: {evt.Remaining}"; 
        }
        
        /// <summary>
        /// Handles the game won event by updating the status display
        /// </summary>
        private void OnGameWon(GameWonEvent evt) { 
            if (statusText) 
                statusText.text = "Level complete!"; 
        }
        
        /// <summary>
        /// Handles the game lost event by updating the status display
        /// </summary>
        /// <param name="reason">Reason for losing ("time" or "moves")</param>
        private void OnGameLost(GameLostEvent evt) { 
            if (statusText) 
                statusText.text = evt.Reason == "time" ? "Time up" : "Move limit reached"; 
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            if (!scoreText)
                return;

            scoreText.text = evt.Combo > 1
                ? $"{evt.Score} (x{evt.Combo})"
                : $"{evt.Score}";
        }
    }
}
