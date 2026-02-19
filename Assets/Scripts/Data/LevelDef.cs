using UnityEngine;

namespace MemoryGame
{
    /// <summary>
    /// Definition of a single level in the Memory Game.
    /// Contains configuration for board size, move limits, time limits, and other level-specific settings.
    /// </summary>
    public class LevelDef
    {
        /// <summary>
        /// Number of rows in the card grid for this level
        /// </summary>
        [Header("Board")] 
        public int rows = 2;
        
        /// <summary>
        /// Number of columns in the card grid for this level
        /// </summary>
        public int cols = 2;
        
        /// <summary>
        /// Maximum number of moves allowed (0 = unlimited)
        /// </summary>
        [Header("Limits")] 
        public int moveLimit = 0; // 0 = unlimited
        
        /// <summary>
        /// Time limit in seconds (0 = unlimited)
        /// </summary>
        public float timeLimitSec = 0f;              // 0 = unlimited
        
        /// <summary>
        /// Optional override for card flip animation duration (0 = use default)
        /// </summary>
        [Header("Overrides")] 
        public float flipDuration = 0f;
        
        /// <summary>
        /// Optional override for delay before hiding mismatched cards (0 = use default)
        /// </summary>
        public float mismatchHideDelay = 0f;
    }
}