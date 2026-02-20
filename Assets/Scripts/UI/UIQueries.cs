using UnityEngine;
using MemoryGame.Services;

namespace MemoryGame.Views
{
    /// <summary>
    /// Utility class for UI-related queries, such as determining unlocked levels.
    /// Provides helper methods for UI components to access game state information.
    /// </summary>
    public static class UIQueries
    {
        /// <summary>
        /// Gets the number of unlocked levels based on player progress.
        /// </summary>
        /// <param name="totalLevels">The total number of levels in the game</param>
        /// <returns>The number of unlocked levels (at least 1, at most totalLevels)</returns>
        public static int GetUnlockedLevelCount(int totalLevels)
        {
            int highest = SettingsStorage.GetHighestLevelIndex(); // index
            return Mathf.Clamp(highest + 1, 1, totalLevels);
        }
    }
}
