using UnityEngine;

namespace MemoryGame.Config
{
    /// <summary>
    /// Constants for board-related values in the memory game.
    /// Centralizes numerical constants for board configuration and layout.
    /// </summary>
[CreateAssetMenu(menuName = "MemoryGame/Board Settings")]
public class BoardSettings : ScriptableObject
    {
        /// <summary>
        /// Minimum scale for cards to prevent them from becoming too small
        /// </summary>
        public  float MinCardScale = 0.2f;
        
        /// <summary>
        /// Maximum scale for cards to prevent them from becoming too large
        /// </summary>
        public  float MaxCardScale = 2.0f;
        
        /// <summary>
        /// Numerator used in the card scale calculation formula
        /// </summary>
        public  float CardScaleNumerator = 3.5f;
        
        /// <summary>
        /// Divisor offset used in the card scale calculation formula
        /// </summary>
        public  float CardScaleDivisorOffset = 0.1f;
        
        /// <summary>
        /// Minimum value for grid padding to prevent division by zero
        /// </summary>
        public  float MinGridPadding = 0.0001f;
        
        /// <summary>
        /// Grid padding factor for tighter grids to prevent edge bleed
        /// </summary>
        public  float GridPaddingFactor = 0.012f;
        
        /// <summary>
        /// Default padding value when no board frame is provided
        /// </summary>
        public  float DefaultPadding = 0.1f;
        
        /// <summary>
        /// Conservative default width for cards when no sprite is available
        /// </summary>
        public  float DefaultCardWidth = 1f;
        
        /// <summary>
        /// Conservative default height for cards when no sprite is available
        /// </summary>
        public  float DefaultCardHeight = 1.4f;
    }
}