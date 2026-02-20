using System;
using MemoryGame.Controller;

namespace MemoryGame.Events
{
    /// <summary>
    /// Fired when a card is selected by the player.
    /// </summary>
    public struct CardSelectedEvent : IEvent
    {
        public CardController Card;

        public CardSelectedEvent(CardController card)
        {
            Card = card;
        }
    }

    /// <summary>
    /// Fired when two selected cards match.
    /// </summary>
    public struct PairMatchedEvent : IEvent
    {
        public CardController First;
        public CardController Second;

        public PairMatchedEvent(CardController first, CardController second)
        {
            First = first;
            Second = second;
        }
    }

    /// <summary>
    /// Fired when two selected cards do not match.
    /// </summary>
    public struct PairMismatchedEvent : IEvent
    {
        public CardController First;
        public CardController Second;

        public PairMismatchedEvent(CardController first, CardController second)
        {
            First = first;
            Second = second;
        }
    }

    /// <summary>
    /// Fired when remaining pair count changes.
    /// </summary>
    public struct RemainingPairsChangedEvent : IEvent
    {
        public int Remaining;

        public RemainingPairsChangedEvent(int remaining)
        {
            Remaining = remaining;
        }
    }

    /// <summary>
    /// Fired when a level starts.
    /// </summary>
    public struct LevelStartedEvent : IEvent
    {
        public LevelDef Level;
        public int Index;

        public LevelStartedEvent(LevelDef level, int index)
        {
            Level = level;
            Index = index;
        }
    }

    /// <summary>
    /// Fired when a level is completed successfully.
    /// </summary>
    public struct LevelCompletedEvent : IEvent
    {
        public int LevelIndex;

        public LevelCompletedEvent(int index)
        {
            LevelIndex = index;
        }
    }

    /// <summary>
    /// Fired when player loses a level.
    /// </summary>
    public struct GameLostEvent : IEvent
    {
        public string Reason;

        public GameLostEvent(string reason)
        {
            Reason = reason;
        }
    }

    /// <summary>
    /// Fired when player wins a level.
    /// </summary>
    public struct GameWonEvent : IEvent
    {
    }

    /// <summary>
    /// Fired when board has been generated.
    /// </summary>
    public struct BoardBuiltEvent : IEvent
    {
        public int Pairs;

        public BoardBuiltEvent(int pairs)
        {
            Pairs = pairs;
        }
    }

    /// <summary>
    /// Requests result popup presentation.
    /// </summary>
    public struct ShowResultEvent : IEvent
    {
        public bool Win;
        public int LevelIndex;
        public string Reason;
        public Action OnNext;
        public Action OnHome;

        public ShowResultEvent(bool win, int levelIndex, string reason, Action onNext, Action onHome)
        {
            Win = win;
            LevelIndex = levelIndex;
            Reason = reason;
            OnNext = onNext;
            OnHome = onHome;
        }
    }

    /// <summary>
    /// Fired when score/combo values change.
    /// </summary>
    public struct ScoreChangedEvent : IEvent
    {
        public int Score;
        public int Combo;

        public ScoreChangedEvent(int score, int combo)
        {
            Score = score;
            Combo = combo;
        }
    }
}
