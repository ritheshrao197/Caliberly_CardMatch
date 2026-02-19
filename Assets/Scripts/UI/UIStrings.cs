namespace MemoryGame.Constants
{
    /// <summary>
    /// Constants for UI strings used throughout the memory game.
    /// Centralizes text resources for easy localization and consistency.
    /// </summary>
    public static class UIStrings
    {
        // Titles
        /// <summary>
        /// Title text displayed when a level is completed successfully
        /// </summary>
        public const string WinTitle = "Level Complete";
        
        /// <summary>
        /// Title text displayed when a level is failed
        /// </summary>
        public const string LoseTitle = "Level Failed";

        // Buttons
        /// <summary>
        /// Text for the next level button when the player wins
        /// </summary>
        public const string NextOnWin = "Next Level";
        
        /// <summary>
        /// Text for the retry button when the player loses
        /// </summary>
        public const string NextOnLose = "Retry";

        // Miscellaneous
        /// <summary>
        /// Default message displayed when the player loses a level
        /// </summary>
        public const string DefaultLoseReason = "Try Again!";
    }
}