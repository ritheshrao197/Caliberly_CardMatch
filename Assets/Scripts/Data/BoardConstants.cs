namespace MemoryGame.Constants
{
    /// <summary>
    /// Constants for board-related values in the memory game.
    /// Centralizes numerical constants for board configuration and layout.
    /// </summary>
    public static class BoardConstants
    {
        /// <summary>
        /// Minimum scale for cards to prevent them from becoming too small
        /// </summary>
        public const float MinCardScale = 0.2f;

        /// <summary>
        /// Maximum scale for cards to prevent them from becoming too large
        /// </summary>
        public const float MaxCardScale = 2.0f;

        /// <summary>
        /// Numerator used in the card scale calculation formula
        /// </summary>
        public const float CardScaleNumerator = 5.5f;

        /// <summary>
        /// Divisor offset used in the card scale calculation formula
        /// </summary>
        public const float CardScaleDivisorOffset = 0.1f;

        /// <summary>
        /// Minimum value for grid padding to prevent division by zero
        /// </summary>
        public const float MinGridPadding = 0.0001f;

        /// <summary>
        /// Grid padding factor for tighter grids to prevent edge bleed
        /// </summary>
        public const float GridPaddingFactor = 0.012f;

        /// <summary>
        /// Default padding value when no board frame is provided
        /// </summary>
        public const float DefaultPadding = 0.1f;

        /// <summary>
        /// Conservative default width for cards when no sprite is available
        /// </summary>
        public const float DefaultCardWidth = 1f;

        /// <summary>
        /// Conservative default height for cards when no sprite is available
        /// </summary>
        public const float DefaultCardHeight = 1f;

        /// <summary>
        /// Default inner padding as a fraction of frame size
        /// </summary>
        public const float DefaultInnerPadding = 0.05f;

        /// <summary>
        /// Default card padding as a fraction of frame size
        /// </summary>
        public const float DefaultCardPadding = 0.1f;

        /// <summary>
        /// Minimum padding value to prevent issues with card layout
        /// </summary>
        public const float MinPadding = 0f;

        /// <summary>
        /// Maximum padding value to prevent issues with card layout
        /// </summary>
        public const float MaxPadding = 0.5f;

        /// <summary>
        /// Default number of rows in the game board
        /// </summary>
        public const int DefaultRows = 3;

        /// <summary>
        /// Default number of columns in the game board
        /// </summary>
        public const int DefaultCols = 4;

        /// <summary>
        /// Default duration for card flip animations in seconds
        /// </summary>
        public const float DefaultFlipDuration = 0.25f;

        /// <summary>
        /// Default delay before hiding mismatched cards in seconds
        /// </summary>
        public const float DefaultMismatchHideDelay = 0.7f;

        /// <summary>
        /// Default horizontal spacing between cards in world units
        /// </summary>
        public const float DefaultCellX = 1.3f;

        /// <summary>
        /// Default vertical spacing between cards in world units
        /// </summary>
        public const float DefaultCellY = 1.6f;


        /// <summary>
        /// Tiny buffer delay in seconds for smoother gameplay
        /// </summary>
        public const float ResolveBufferDelay = 0.05f;
        public const float HalfMinFlipScale = 0.0001f * 0.5f;
        public const float MinFlipScale = 0.0001f;


    }
}