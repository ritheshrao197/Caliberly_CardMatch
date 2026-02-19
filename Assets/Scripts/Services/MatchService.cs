using System.Collections;
using System.Collections.Generic;
using MemoryGame.Config;
using MemoryGame.Controller;
using MemoryGame.Events;
using UnityEngine;
using MemoryGame.Constants;
using System;

namespace MemoryGame.Services
{
    /// <summary>
    /// Service to handle matching logic between selected cards in the memory game.
    /// Manages card selection, determines matches, and handles game progression.
    /// </summary>
    public class MatchService : MonoBehaviour
    {
        /// <summary>
        /// Reference to the game configuration settings
        /// </summary>
        [Header("Refs")]
        public GameConfig config;

        private CardController _first, _second;
        private bool _busy;
        private int _remainingPairs;
        private readonly List<CardController> _allCards = new List<CardController>();

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<CardSelectedEvent>(OnCardSelected);
            EventBus.Instance.Subscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            EventBus.Instance.Subscribe<LevelStartedEvent>(OnLevelStarted);

        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<CardSelectedEvent>(OnCardSelected);
            EventBus.Instance.Unsubscribe<RemainingPairsChangedEvent>(OnRemainingPairsChanged);
            EventBus.Instance.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
        }

        private void OnLevelStarted(LevelStartedEvent @event)
        {
            _remainingPairs = @event.Level.cols * @event.Level.rows / 2;
        }
        

        /// <summary>
        /// Registers all cards in the current game for matching logic
        /// </summary>
        /// <param name="cards">Collection of card controllers to register</param>
        public void RegisterCards(IEnumerable<CardController> cards)
        {
            _allCards.Clear();
            _allCards.AddRange(cards);
        }

        private void OnRemainingPairsChanged(RemainingPairsChangedEvent @event)
        {
            _remainingPairs = @event.Remaining;
            Debug.Log($"Remaining pairs updated: {_remainingPairs}");
        }

        /// <summary>
        /// Sets input enabled/disabled state for all registered cards
        /// </summary>
        /// <param name="enabled">True to enable input, false to disable</param>
        private void SetAllInput(bool enabled)
        {
            foreach (var c in _allCards) c.SetInput(enabled);
        }

        /// <summary>
        /// Handles card selection events from the game events system
        /// </summary>
        /// <param name="c">The card controller that was selected</param>
        private void OnCardSelected(CardSelectedEvent @event)
        {
            if (_busy) return;
            if (_first == null)
            {
                _first = @event.Card; return;
            }
            if (@event.Card == _first) return;

            _second = @event.Card;
            StartCoroutine(Resolve());
        }

        /// <summary>
        /// Coroutine that resolves the matching logic between two selected cards
        /// </summary>
        /// <returns>IEnumerator for coroutine execution</returns>
        private IEnumerator Resolve()
        {
            _busy = true;
            SetAllInput(false);

            yield return new WaitForSeconds(BoardConstants.ResolveBufferDelay); // tiny buffer

            // Check if the two selected cards match
            if (_first.Model.Id == _second.Model.Id)
            {
                // Cards match - lock them and notify listeners
                _first.Lock();
                _second.Lock();
                EventBus.Instance.Publish(new PairMatchedEvent(_first, _second));
                _remainingPairs--;
                EventBus.Instance.Publish(new RemainingPairsChangedEvent(_remainingPairs));

                // Check if all pairs have been matched (game won)
                if (_remainingPairs <= 0)
                {
                    EventBus.Instance.Publish(new GameWonEvent());
                }
            }
            else
            {
                // Cards don't match - notify listeners and flip them back
                EventBus.Instance.Publish(new PairMismatchedEvent(_first, _second));
                yield return new WaitForSeconds(Mathf.Max(0f, config != null ? config.mismatchHideDelay : BoardConstants.DefaultMismatchHideDelay));
                // Flip back
                yield return _first.StartCoroutine(_first.FlipRoutine(false));
                yield return _second.StartCoroutine(_second.FlipRoutine(false));
            }

            // Reset selection and re-enable input
            _first = _second = null;
            SetAllInput(true);
            _busy = false;
        }
    }
}