using UnityEngine;

namespace MemoryGame.Views
{
    /// <summary>
    /// Component that adjusts a RectTransform to fit within the screen's safe area.
    /// Useful for ensuring UI elements are visible on devices with notches or rounded corners.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        /// <summary>
        /// Reference to the RectTransform component
        /// </summary>
        RectTransform _rt;
        
        /// <summary>
        /// Stores the last known safe area to detect changes
        /// </summary>
        Rect _last;
        
        /// <summary>
        /// Initializes the component and applies the safe area fitting
        /// </summary>
        void Awake() { 
            _rt = GetComponent<RectTransform>(); 
            Apply(); 
        }
        
        /// <summary>
        /// Checks for safe area changes and re-applies fitting if needed
        /// </summary>
        void Update() { 
            if (Screen.safeArea != _last) 
                Apply(); 
        }
        
        /// <summary>
        /// Applies the safe area fitting by adjusting the RectTransform anchors
        /// </summary>
        void Apply()
        {
            _last = Screen.safeArea;
            var min = _last.position; 
            var max = _last.position + _last.size;
            min.x /= Screen.width;  
            min.y /= Screen.height;
            max.x /= Screen.width;  
            max.y /= Screen.height;
            _rt.anchorMin = min; 
            _rt.anchorMax = max;
            _rt.offsetMin = Vector2.zero; 
            _rt.offsetMax = Vector2.zero;
        }
    }
}