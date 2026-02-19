namespace MemoryGame
{
    /// <summary>
    /// Static utility class for managing global input lock state.
    /// Used to prevent user input during animations or other non-interactive states.
    /// </summary>
    public static class InputLock
    {
        /// <summary>
        /// Indicates whether input is currently locked (disabled)
        /// </summary>
        public static bool IsLocked { get; private set; }

        /// <summary>
        /// Locks input (disables user interaction)
        /// </summary>
        public static void Lock()  => IsLocked = true;
        
        /// <summary>
        /// Unlocks input (enables user interaction)
        /// </summary>
        public static void Unlock() => IsLocked = false;
    }
}