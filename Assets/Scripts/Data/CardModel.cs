using System;

namespace MemoryGame.Models
{
    /// <summary>
    /// Represents the data model for a single card in the memory game.
    /// Contains the card's state including its ID, whether it's face up, and whether it's matched.
    /// </summary>
    public class CardModel
    {
        /// <summary>
        /// Unique identifier for the card, typically generated from the sprite name
        /// </summary>
        public string Id { get; private set; }
        
        /// <summary>
        /// Indicates whether the card is currently face up (visible)
        /// </summary>
        public bool IsFaceUp { get; private set; }
        
        /// <summary>
        /// Indicates whether the card has been successfully matched with its pair
        /// </summary>
        public bool IsMatched { get; private set; }

        /// <summary>
        /// Flips the card face up (makes it visible)
        /// </summary>
        public void FlipUp() => IsFaceUp = true;
        
        /// <summary>
        /// Flips the card face down (hides it)
        /// </summary>
        public void FlipDown() => IsFaceUp = false;
        
        /// <summary>
        /// Locks the card as matched, preventing further interactions
        /// </summary>
        public void Lock() => IsMatched = true;

        /// <summary>
        /// Internal method to set the card's properties
        /// </summary>
        /// <param name="id">Unique identifier for the card</param>
        /// <param name="isFaceUp">Initial face up state (default: false)</param>
        /// <param name="isMatched">Initial matched state (default: false)</param>
        internal void Set(string id, bool isFaceUp=false, bool isMatched=false)
        {
            Id = id;
            IsFaceUp = isFaceUp;
            IsMatched = isMatched;
        }
       
    }
}