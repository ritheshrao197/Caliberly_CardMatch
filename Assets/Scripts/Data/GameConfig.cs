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


        /// <summary>
        /// Default number of rows in the game board
        /// </summary>
        public int DefaultRows = 3;

        /// <summary>
        /// Default number of columns in the game board
        /// </summary>
        public int DefaultCols = 4;

        /// <summary>
        /// Default duration for card flip animations in seconds
        /// </summary>
        public float DefaultFlipDuration = 0.25f;

        /// <summary>
        /// Default delay before hiding mismatched cards in seconds
        /// </summary>
        public float DefaultMismatchHideDelay = 0.7f;

        /// <summary>
        /// Default horizontal spacing between cards in world units
        /// </summary>
        public float DefaultCellX = 1.3f;

        /// <summary>
        /// Default vertical spacing between cards in world units
        /// </summary>
        public float DefaultCellY = 1.6f;

        // =========================
        // BOARD SETTINGS
        // =========================
        [Header("Board")]


        [Range(0f, 0.5f)]
        public float InnerPadding = 0.05f;

        [Range(0f, 0.5f)]
        public float CardPadding = 0.1f;

        public float CellSpacingX = 1.3f;
        public float CellSpacingY = 1.6f;

        // =========================
        // CARD SETTINGS
        // =========================
        [Header("Card")]
        public float FlipDuration = 0.25f;

        [Tooltip("Minimum scale during flip to avoid disappearing card")]
        public float MinFlipScale = 0.0001f;

        /// <summary>
        /// Half of the minimum scale value for flip animation calculations
        /// </summary>
        public float HalfMinFlipScale = 0.0001f * 0.5f;
        // =========================
        // MATCH SETTINGS
        // =========================
        [Header("Match")]
        public float mismatchHideDelay = 0.7f;

        [Tooltip("Small buffer delay for smoother transitions")]
        public float tinyBufferDelay = 0.05f;

         /// <summary>
        /// Default target width for sprite fitting in world units
        /// </summary>
        public  float DefaultTargetWidth = 2f;
        
        /// <summary>
        /// Default target height for sprite fitting in world units
        /// </summary>
        public  float DefaultTargetHeight = 2f;
        
        /// <summary>
        /// Default value for preserving aspect ratio
        /// </summary>
        public  bool DefaultPreserveAspect = true;
    }
}