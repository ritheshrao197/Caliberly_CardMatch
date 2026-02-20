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
        private static int _lockRevision;

        /// <summary>
        /// Locks input (disables user interaction)
        /// </summary>
        public static int Lock()
        {
            IsLocked = true;
            _lockRevision++;
            return _lockRevision;
        }
        
        /// <summary>
        /// Unlocks input (enables user interaction)
        /// </summary>
        public static void Unlock() => IsLocked = false;

        /// <summary>
        /// Unlocks input only if no newer lock operation occurred.
        /// </summary>
        public static bool UnlockIfRevision(int revision)
        {
            if (_lockRevision != revision)
                return false;

            IsLocked = false;
            return true;
        }
    }
}
