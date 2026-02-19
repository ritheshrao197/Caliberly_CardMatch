using UnityEngine;
using MemoryGame.Constants;

namespace MemoryGame.Views
{
    /// <summary>
    /// Utility class for UI-related queries, such as determining unlocked levels.
    /// Provides helper methods for UI components to access game state information.
    /// </summary>
    public static class UIQueries
    {
        /// <summary>
        /// PlayerPrefs key for storing the highest level completed
        /// </summary>
        private const string KEY ="MG_HIGHEST_LEVEL";

        /// <summary>
        /// Gets the number of unlocked levels based on player progress.
        /// </summary>
        /// <param name="totalLevels">The total number of levels in the game</param>
        /// <returns>The number of unlocked levels (at least 1, at most totalLevels)</returns>
        public static int GetUnlockedLevelCount(int totalLevels)
        {
            int highest = PlayerPrefs.GetInt(KEY, 0); // index
            return Mathf.Clamp(highest + 1, 1, totalLevels);
        }
    }
}