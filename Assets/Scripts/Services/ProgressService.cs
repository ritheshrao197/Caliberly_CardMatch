using UnityEngine;
using MemoryGame.Constants;

namespace MemoryGame
{
    /// <summary>
    /// Service to manage player progress and level unlocking.
    /// Uses PlayerPrefs to persistently store the highest level completed by the player.
    /// </summary>
    public class ProgressService : MonoBehaviour
    {
        /// <summary>
        /// PlayerPrefs key for storing the highest level completed
        /// </summary>
        private const string KEY = "MG_HIGHEST_LEVEL";

        /// <summary>
        /// Gets the current level index the player should start at.
        /// This is either the highest unlocked level or 0 if no progress exists.
        /// </summary>
        /// <returns>The zero-based index of the current level</returns>
        public int GetCurrentLevelIndex()
        {
            return Mathf.Clamp(PlayerPrefs.GetInt(KEY, 0), 0, int.MaxValue);
        }

        /// <summary>
        /// Unlocks the next level after completing a level.
        /// Updates PlayerPrefs if the newly completed level is higher than previously unlocked.
        /// </summary>
        /// <param name="justCompletedIndex">The index of the level that was just completed</param>
        public void UnlockNextLevel(int justCompletedIndex)
        {
            int stored = PlayerPrefs.GetInt(KEY, 0);
            int next = Mathf.Max(stored, justCompletedIndex + 1);
            PlayerPrefs.SetInt(KEY, next);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Resets all player progress by deleting the progress PlayerPrefs key.
        /// </summary>
        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey(KEY);
        }
    }
}