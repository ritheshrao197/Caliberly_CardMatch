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
        private int _remainingPairs;

        private readonly Queue<CardController> _selectionQueue = new();
        private readonly HashSet<CardController> _pendingCards = new();
        private EventBus _bus;
        private Coroutine _resolveRoutine;

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
            _selectionQueue.Clear();
            _pendingCards.Clear();

            if (_resolveRoutine != null)
            {
                StopCoroutine(_resolveRoutine);
                _resolveRoutine = null;
            }
        }

        public void RegisterCards(IEnumerable<CardController> cards)
        {
            _ = cards;
            _selectionQueue.Clear();
            _pendingCards.Clear();
        }

        private void OnCardSelected(CardSelectedEvent e)
        {
            if (e.Card == null || e.Card.Model.IsMatched)
                return;

            if (!_pendingCards.Add(e.Card))
                return;

            _selectionQueue.Enqueue(e.Card);

            if (_resolveRoutine == null)
                _resolveRoutine = StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            while (_remainingPairs > 0)
            {
                if (_selectionQueue.Count < 2)
                    break;

                _first = _selectionQueue.Dequeue();
                _second = _selectionQueue.Dequeue();

                if (_first == null || _second == null || _first == _second)
                {
                    ClearPending(_first);
                    ClearPending(_second);
                    _first = null;
                    _second = null;
                    continue;
                }

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

            _resolveRoutine = null;
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
            ClearPending(_first);
            ClearPending(_second);

            _first = null;
            _second = null;
        }

        private void ClearPending(CardController card)
        {
            if (card != null)
                _pendingCards.Remove(card);
        }
    }
}
