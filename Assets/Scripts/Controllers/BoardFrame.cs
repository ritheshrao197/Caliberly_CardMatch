using UnityEngine;
using MemoryGame.Constants;
using MemoryGame.Config;

namespace MemoryGame.Controller
{
    /// <summary>
    /// BoardFrame visually represents the outer frame of the card grid.
    /// Provides bounds and padding calculations for card layout.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardFrame : MonoBehaviour
    {
        /// <summary>
        /// Optional explicit reference to the SpriteRenderer. 
        /// If not set, uses the SpriteRenderer on this GameObject.
        /// </summary>
        [Tooltip("Optional explicit reference; defaults to SpriteRenderer on this GameObject")]
        public SpriteRenderer frame;
        [SerializeField] GameConfig _cfg;

        // 

        /// <summary>
        /// Automatically assigns the SpriteRenderer on this GameObject if not set.
        /// Called when the component is first added to a GameObject.
        /// </summary>
        void Reset() { frame = GetComponent<SpriteRenderer>(); }

        /// <summary>
        /// Calculates the inner bounds of the frame, accounting for inner padding.
        /// </summary>
        /// <param name="inner">The calculated inner rectangle in world space.</param>
        /// <returns>True if bounds are valid, false otherwise.</returns>
        public bool HasBounds(out Rect inner)
        {
            // Use explicit frame if set, otherwise get SpriteRenderer on this GameObject
            var sr = frame == null ? GetComponent<SpriteRenderer>() : frame;
            if (sr == null || sr.sprite == null) { inner = default; return false; }
            var b = sr.bounds;
            float padX =BoardConstants.DefaultInnerPadding  * b.size.x;
            float padY = BoardConstants.DefaultInnerPadding * b.size.y;
            inner = new Rect(b.min.x + padX, b.min.y + padY, b.size.x - 2f * padX, b.size.y - 2f * padY);
            return true;
        }
    }
}