namespace MemoryGame.Services
{
    /// <summary>
    /// Abstraction for progression read/write operations.
    /// </summary>
    public interface IProgressTracker
    {
        int GetCurrentLevelIndex();
        void UnlockNextLevel(int justCompletedIndex);
        void ResetProgress();
    }
}
