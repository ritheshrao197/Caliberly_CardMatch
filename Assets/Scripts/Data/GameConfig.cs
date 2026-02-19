using UnityEngine;
using MemoryGame.Constants;

namespace MemoryGame.Config
{
    /// <summary>
    /// Configuration data for the Memory Game
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MemoryGame/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Board Size")]
        public int rows = BoardConstants.DefaultRows;
        public int cols = BoardConstants.DefaultCols;

        [Header("Animation & Rules")]
        [Tooltip("Flip animation duration in seconds")]
        public float flipDuration = BoardConstants.DefaultFlipDuration;
        [Tooltip("Delay before hiding mismatched pair")]
        public float mismatchHideDelay = BoardConstants.DefaultMismatchHideDelay;

        [Header("Layout")]
        [Tooltip("Spacing between columns (world units)")]
        public float cellX = BoardConstants.DefaultCellX;
        [Tooltip("Spacing between rows (world units)")]
        public float cellY = BoardConstants.DefaultCellY;

        public const float ResolveBufferDelay = 0.05f;
    }
}