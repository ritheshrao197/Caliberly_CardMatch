using MemoryGame.Controller;
using MemoryGame.Events;
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

        private int _moves;
        private int _moveLimit;
        private float _timeLimit;
        private TimerService _timer;

        /// <summary>
        /// Initializes the component, finds the timer service, and registers game event handlers
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _timer = FindObjectOfType<TimerService>();
            EventBus.Instance.Subscribe<LevelStartedEvent>(OnLevelStarted);
            EventBus.Instance.Subscribe<PairMatchedEvent>(OnPairEvent);
            EventBus.Instance.Subscribe<PairMismatchedEvent>(OnPairEvent);
            EventBus.Instance.Subscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            EventBus.Instance.Subscribe<GameWonEvent>(OnGameWon);
            EventBus.Instance.Subscribe<GameLostEvent>(OnGameLost);
        }
        
        /// <summary>
        /// Unregisters game event handlers when the component is destroyed
        /// </summary>
        private void OnDestroy()
        {
            EventBus.Instance.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
            EventBus.Instance.Unsubscribe<PairMatchedEvent>(OnPairEvent);
            EventBus.Instance.Unsubscribe<PairMismatchedEvent>(OnPairEvent);
            EventBus.Instance.Unsubscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            EventBus.Instance.Unsubscribe<GameWonEvent>(OnGameWon);
            EventBus.Instance.Unsubscribe<GameLostEvent>(OnGameLost);
        }

        /// <summary>
        /// Updates the timer display each frame
        /// </summary>
        private void Update()
        {
            if (_timer != null && timerText != null)
            {
                var t = Mathf.Max(0f, _timer.elapsed);
                int m = (int)(t / 60f);
                int s = (int)(t % 60f);
                timerText.text = _timeLimit > 0f ? $"{m:00}:{s:00} / {FormatTime(_timeLimit)}" : $"{m:00}:{s:00}";
            }
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
            if (levelText) 
                levelText.text = $"Level {idx + 1}: {def.rows}x{def.cols}";
            if (movesText) 
                movesText.text = _moveLimit > 0 ? $"0 / {_moveLimit}" : "0";
            if (statusText) 
                statusText.text = "Find all pairs";
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
            Debug.Log("Game won!");
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
    }
}
