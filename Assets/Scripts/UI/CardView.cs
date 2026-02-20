using System.Collections;
using UnityEngine;
using MemoryGame.Constants;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for a single card in the Memory Game.
    /// Handles visual representation and animations for card flipping.
    /// </summary>
    public class CardView : MonoBehaviour
    {
        /// <summary>
        /// Renderer for the card's face (front side)
        /// </summary>
        [Header("Renderers")]
        public SpriteRenderer face;
        
        /// <summary>
        /// Renderer for the card's back (hidden side)
        /// </summary>
        public SpriteRenderer back;

        /// <summary>
        /// Binds a sprite to the card's face renderer
        /// </summary>
        /// <param name="faceSprite">The sprite to display on the card's face</param>
        public void BindFace(Sprite faceSprite)
        {
            if (face != null)
            {
                face.sprite = faceSprite;
                var fitter = face.GetComponent<SpriteFitToUnits>();
                if (fitter) fitter.Apply();
            }
        }

        /// <summary>
        /// Animates the card flip transition between face up and face down states
        /// </summary>
        /// <param name="faceUp">True to flip to face up, false to flip to face down</param>
        /// <param name="duration">Duration of the flip animation in seconds</param>
        /// <param name="onComplete">Optional callback to invoke when animation completes</param>
        /// <returns>IEnumerator for coroutine execution</returns>
        public IEnumerator AnimateFlip(bool faceUp, float duration, System.Action onComplete = null)
        {
            float half = Mathf.Max(BoardConstants.HalfMinFlipScale, duration * 0.5f);
            float t = 0f;
            // First half: scale X down to 0
            while (t < half)
            {
                t += Time.deltaTime;
                float k = 1f - Mathf.Clamp01(t / half);
                transform.localScale = new Vector3(Mathf.Max(BoardConstants.MinFlipScale, k), 1f, 1f);
                yield return null;
            }

            // Swap visibility at the "thin" point
            if (face != null && back != null)
            {
                face.enabled = faceUp;
                back.enabled = !faceUp;
            }

            t = 0f;
            // Second half: scale X up to 1
            while (t < half)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / half);
                transform.localScale = new Vector3(Mathf.Max(BoardConstants.MinFlipScale, k), 1f, 1f);
                yield return null;
            }
            transform.localScale = Vector3.one;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Instantly sets the card's face state without animation
        /// </summary>
        /// <param name="faceUp">True to show face, false to show back</param>
        public void SetInstant(bool faceUp)
        {
            if (face != null && back != null)
            {
                face.enabled = faceUp;
                back.enabled = !faceUp;
            }
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Normalizes the scale of child renderers to prevent double scaling issues
        /// </summary>
        public void NormalizeChildScale()
        {
            if (face) face.transform.localScale = Vector3.one;
            if (back) back.transform.localScale = Vector3.one;
        }
    }
}
