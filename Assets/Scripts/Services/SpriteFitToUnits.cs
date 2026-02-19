using UnityEngine;
using MemoryGame.Constants;
using MemoryGame.Config;

namespace MemoryGame
{
    /// <summary>
    /// Component to fit a SpriteRenderer's sprite to specified world unit dimensions.
    /// Automatically adjusts the transform's localScale to match target dimensions.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFitToUnits : MonoBehaviour
    {
        /// <summary>
        /// Defines how the sprite should be fitted to the target dimensions
        /// </summary>
        public enum FitMode { 
            /// <summary>
            /// Scale the sprite to fit entirely within the target dimensions
            /// </summary>
            FitInside, 
            
            /// <summary>
            /// Scale the sprite to fill the entire target area, potentially cropping edges
            /// </summary>
            Fill 
        }
        
        /// <summary>
        /// The fitting mode to use (FitInside or Fill)
        /// </summary>
        public FitMode mode = FitMode.FitInside;


        /// <summary>
        /// Target width in world units
        /// </summary>
        [Header("Target size in world units")]
        public float targetWidth = 2f;
        /// <summary>
        /// Target height in world units
        /// </summary>
        public float targetHeight = 2f;

        /// <summary>
        /// Whether to preserve the sprite's original aspect ratio
        /// </summary>
        [Header("Options")]
        public bool preserveAspect = true;

        SpriteRenderer _sr;

        /// <summary>
        /// Initializes the component and applies the fitting on enable
        /// </summary>
        void OnEnable() { 
            _sr = GetComponent<SpriteRenderer>();
           
           
             Apply();
             }
        
        /// <summary>
        /// Re-applies the fitting when component values are changed in the editor
        /// </summary>
        void OnValidate() { Apply(); }

        /// <summary>
        /// Applies the scaling to fit the sprite to the target dimensions
        /// </summary>
        public void Apply()
        {
            if (_sr == null || _sr.sprite == null) return;

            // sprite.bounds.size is in world units at current scale
            // We want to compute the required localScale so the final size matches target
            var currentScale = transform.localScale;
            var size = _sr.sprite.bounds.size; // size at scale 1

            if (size.x <= 0f || size.y <= 0f) return;

            float sx = targetWidth / size.x;
            float sy = targetHeight / size.y;

            float kx, ky;

            if (preserveAspect)
            {
                if (mode == FitMode.FitInside)
                {
                    float k = Mathf.Min(sx, sy);
                    kx = ky = k;
                }
                else // Fill
                {
                    float k = Mathf.Max(sx, sy);
                    kx = ky = k;
                }
            }
            else
            {
                kx = sx;
                ky = sy;
            }

            transform.localScale = new Vector3(kx, ky, currentScale.z);
        }
    }
}