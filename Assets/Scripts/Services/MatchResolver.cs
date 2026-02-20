using System.Collections;
using System.Collections.Generic;
using MemoryGame.Config;
using MemoryGame.Constants;
using MemoryGame.Controller;
using MemoryGame.Events;
using UnityEngine;

namespace MemoryGame.Services
{
    /// <summary>
    /// Resolves card pair selections and publishes match/mismatch/win events.
    /// </summary>
    public class MatchResolver : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] protected GameConfig config;

        private CardController _first;
        private CardController _second;

        private bool _busy;
        private int _remainingPairs;

        private readonly List<CardController> _allCards = new();
        private EventBus _bus;

        private WaitForSeconds _resolveDelay;
        private WaitForSeconds _mismatchDelay;
        private float _cachedMismatchDelay = -1f;

        private void Awake()
        {
            _bus = EventBus.Instance;
            _resolveDelay = new WaitForSeconds(BoardConstants.ResolveBufferDelay);
        }

        private void OnEnable()
        {
            _bus.Subscribe<CardSelectedEvent>(OnCardSelected);
            _bus.Subscribe<LevelStartedEvent>(OnLevelStarted);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<CardSelectedEvent>(OnCardSelected);
            _bus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            _remainingPairs = (e.Level.cols * e.Level.rows) / 2;

            float delay = config != null
                ? config.mismatchHideDelay
                : BoardConstants.DefaultMismatchHideDelay;
            delay = Mathf.Max(0f, delay);

            if (_mismatchDelay == null || !Mathf.Approximately(delay, _cachedMismatchDelay))
            {
                _cachedMismatchDelay = delay;
                _mismatchDelay = new WaitForSeconds(delay);
            }

            _first = null;
            _second = null;
            _busy = false;
        }

        public void RegisterCards(IEnumerable<CardController> cards)
        {
            _allCards.Clear();

            if (cards == null)
                return;

            _allCards.AddRange(cards);
        }

        private void OnCardSelected(CardSelectedEvent e)
        {
            if (_busy || e.Card == null)
                return;

            if (_first == null)
            {
                _first = e.Card;
                return;
            }

            if (e.Card == _first)
                return;

            _second = e.Card;
            StartCoroutine(ResolvePair());
        }

        private IEnumerator ResolvePair()
        {
            _busy = true;
            SetAllInput(false);

            yield return _resolveDelay;

            bool isMatch = _first.Model.Id == _second.Model.Id;

            if (isMatch)
            {
                HandleMatch();
            }
            else
            {
                yield return HandleMismatch();
            }

            ResetSelection();
        }

        private void HandleMatch()
        {
            _first.Lock();
            _second.Lock();

            _remainingPairs--;

            _bus.Publish(new PairMatchedEvent(_first, _second));
            _bus.Publish(new RemainingPairsChangedEvent(_remainingPairs));

            if (_remainingPairs <= 0)
                _bus.Publish(new GameWonEvent());
        }

        private IEnumerator HandleMismatch()
        {
            _bus.Publish(new PairMismatchedEvent(_first, _second));

            yield return _mismatchDelay;
            yield return _first.FlipRoutine(false);
            yield return _second.FlipRoutine(false);
        }

        private void ResetSelection()
        {
            _first = null;
            _second = null;

            _busy = false;

            if (_remainingPairs > 0)
                SetAllInput(true);
        }

        private void SetAllInput(bool enabled)
        {
            for (int i = 0; i < _allCards.Count; i++)
            {
                if (_allCards[i] != null)
                    _allCards[i].SetInput(enabled);
            }
        }
    }
}
