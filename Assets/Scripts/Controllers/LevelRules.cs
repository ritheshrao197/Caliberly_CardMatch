using System.Collections;
using MemoryGame.Controller;
using MemoryGame.Events;
using MemoryGame.Services;
using UnityEngine;

namespace MemoryGame
{
    /// <summary>
    /// Manages level-specific rules and constraints such as move limits and time limits.
    /// Handles win/lose conditions based on these rules.
    /// </summary>
    public class LevelRules : MonoBehaviour
    {
        private int _moves;
        private int _moveLimit;
        private float _timeLimit;
        private bool _ended;
        private TimerService _timer;
        /// <summary>
        /// Initializes level rules for a new level based on the provided level definition and timer service.
        /// </summary>
        private void OnEnable()
        {
            EventBus.Instance.Subscribe<PairMatchedEvent>(OnPairEvent);
            EventBus.Instance.Subscribe<PairMismatchedEvent>(OnPairMismatchedEvent);
            EventBus.Instance.Subscribe<GameWonEvent>(OnGameWon);
        }


        /// <summary>
        /// Unsubscribes from game events when the component is disabled to prevent memory leaks and unintended behavior.
        /// </summary>
        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<PairMatchedEvent>(OnPairEvent);
            EventBus.Instance.Unsubscribe<PairMismatchedEvent>(OnPairMismatchedEvent);
            EventBus.Instance.Unsubscribe<GameWonEvent>(OnGameWon);
        }

        /// <summary>
        /// Initializes level rules for a new level
        /// </summary>
        /// <param name="def">Level definition containing rule parameters</param>
        /// <param name="index">Index of the level being started</param>
        /// <param name="timer">Timer service to monitor time-based constraints</param>
        public void BeginLevel(LevelDef def, int index, TimerService timer)
        {
            _moves = 0;
            _moveLimit = def.moveLimit;
            _timeLimit = def.timeLimitSec;
            _ended = false;
            _timer = timer;
        }
        /// <summary>
        /// Handles pair match events to track move count and enforce move limits
        /// </summary>
        private void Update()
        {
            if (_ended) return;
            // Check if time limit has been exceeded
            if (_timeLimit > 0f && _timer != null && _timer.elapsed >= _timeLimit)
            {
                _ended = true;
                _timer.StopTimer();
                EventBus.Instance.Publish(new GameLostEvent("time"));
            }
        }

        /// <summary>
        /// Handles pair match/mismatch events to track move count and enforce move limits
        /// </summary>
        /// <param name="a">First card in the pair</param>
        /// <param name="b">Second card in the pair</param>
        private void OnPairEvent(PairMatchedEvent evt)
        {
            if (_ended) return;
            _moves++;
            StartCoroutine(EvaluateMoveLimitAfterMatch());
        }

        private IEnumerator EvaluateMoveLimitAfterMatch()
        {
            // MatchResolver raises GameWon after PairMatched in the same frame.
            // Defer check so win state can settle first.
            yield return null;

            if (_ended)
                yield break;

            if (_moveLimit > 0 && _moves >= _moveLimit)
            {
                _ended = true;
                _timer?.StopTimer();
                EventBus.Instance.Publish(new GameLostEvent("moves"));
            }
        }

        private void OnPairMismatchedEvent(PairMismatchedEvent evt)
        {
            if (_ended) return;
            _moves++;
            // Check if move limit has been exceeded
            if (_moveLimit > 0 && _moves >= _moveLimit)
            {
                _ended = true;
                _timer?.StopTimer();
                EventBus.Instance.Publish(new GameLostEvent("moves"));
            }
        }
        /// <summary>
        /// Handles game won event to mark the level as completed
        /// </summary>
        private void OnGameWon(GameWonEvent evt)
        {
            if (_ended) return;
            _ended = true;
        }
    }
}
