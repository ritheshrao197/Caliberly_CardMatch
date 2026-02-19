using MemoryGame.Controller;

namespace MemoryGame.Events
{
    public struct CardSelectedEvent : IEvent
    {
        public CardController Card;

        public CardSelectedEvent(CardController card)
        {
            Card = card;
        }
    }
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
    public struct RemainingPairsChangedEvent : IEvent
    {
        public int Remaining;

        public RemainingPairsChangedEvent(int remaining)
        {
            Remaining = remaining;
        }
    }
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
     public struct LevelCompletedEvent : IEvent
    {
        public int LevelIndex;

        public LevelCompletedEvent(int index)
        {
            LevelIndex = index;
        }
    }public struct GameLostEvent : IEvent
    {
        public string Reason;

        public GameLostEvent(string reason)
        {
            Reason = reason;
        }
    }
     public struct GameWonEvent : IEvent
    {
    }
    public struct BoardBuiltEvent : IEvent
    {
        private int pairs;

        public BoardBuiltEvent(int pairs)
        {
            this.pairs = pairs;
        }
    }
}
