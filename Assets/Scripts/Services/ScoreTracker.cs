using MemoryGame.Events;
using UnityEngine;

namespace MemoryGame.Services
{
    /// <summary>
    /// Tracks score and combo state for the active level session.
    /// </summary>
    public class ScoreTracker : MonoBehaviour
    {
        [SerializeField] private int matchBaseScore = 10;
        [SerializeField] private int mismatchPenalty = 1;

        private EventBus _bus;
        private int _score;
        private int _combo;

        private void Awake()
        {
            _bus = EventBus.Instance;
        }

        private void OnEnable()
        {
            _bus.Subscribe<LevelStartedEvent>(OnLevelStarted);
            _bus.Subscribe<PairMatchedEvent>(OnPairMatched);
            _bus.Subscribe<PairMismatchedEvent>(OnPairMismatched);
            _bus.Subscribe<GameWonEvent>(OnRoundEnded);
            _bus.Subscribe<GameLostEvent>(OnRoundEnded);
        }

        private void OnDisable()
        {
            _bus.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
            _bus.Unsubscribe<PairMatchedEvent>(OnPairMatched);
            _bus.Unsubscribe<PairMismatchedEvent>(OnPairMismatched);
            _bus.Unsubscribe<GameWonEvent>(OnRoundEnded);
            _bus.Unsubscribe<GameLostEvent>(OnRoundEnded);
        }

        private void OnLevelStarted(LevelStartedEvent e)
        {
            _score = 0;
            _combo = 0;
            PublishScore();
        }

        private void OnPairMatched(PairMatchedEvent e)
        {
            _combo++;
            _score += matchBaseScore * Mathf.Max(1, _combo);
            PublishScore();
        }

        private void OnPairMismatched(PairMismatchedEvent e)
        {
            _combo = 0;
            _score = Mathf.Max(0, _score - mismatchPenalty);
            PublishScore();
        }

        private void OnRoundEnded(GameWonEvent e) => PublishScore();
        private void OnRoundEnded(GameLostEvent e) => PublishScore();

        private void PublishScore()
        {
            _bus.Publish(new ScoreChangedEvent(_score, _combo));
        }
    }
}
