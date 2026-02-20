using UnityEngine;

namespace MemoryGame.Services
{
    /// <summary>
    /// Service to manage player progress and level unlocking.
    /// Uses JSON storage to persistently store the highest level completed by the player.
    /// </summary>
    public class ProgressService : MonoBehaviour, IProgressTracker
    {
        /// <summary>
        /// Gets the current level index the player should start at.
        /// This is either the highest unlocked level or 0 if no progress exists.
        /// </summary>
        /// <returns>The zero-based index of the current level</returns>
        public int GetCurrentLevelIndex()
        {
            return Mathf.Clamp(SettingsStorage.GetHighestLevelIndex(), 0, int.MaxValue);
        }

        /// <summary>
        /// Unlocks the next level after completing a level.
        /// Updates JSON settings if the newly completed level is higher than previously unlocked.
        /// </summary>
        /// <param name="justCompletedIndex">The index of the level that was just completed</param>
        public void UnlockNextLevel(int justCompletedIndex)
        {
            int stored = SettingsStorage.GetHighestLevelIndex();
            int next = Mathf.Max(stored, justCompletedIndex + 1);
            SettingsStorage.SetHighestLevelIndex(next);
        }

        /// <summary>
        /// Resets all player progress to defaults.
        /// </summary>
        public void ResetProgress()
        {
            SettingsStorage.ResetProgress();
        }
    }
}
