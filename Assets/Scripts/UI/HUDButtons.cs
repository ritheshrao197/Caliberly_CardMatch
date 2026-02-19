using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for HUD buttons in the Memory Game.
    /// Handles binding UI button clicks to game events.
    /// </summary>
    public class HUDButtons : MonoBehaviour
    {
        /// <summary>
        /// Reference to the pause button in the UI
        /// </summary>
        public Button pauseButton;

        /// <summary>
        /// Initializes the component and binds button click events to UI events
        /// </summary>
        void Awake() { 
            if (pauseButton) 
                pauseButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnPauseEvent())); 
        }
        
        /// <summary>
        /// Cleans up button click listeners when the component is destroyed
        /// </summary>
        void OnDestroy() { 
            if (pauseButton) 
                pauseButton.onClick.RemoveAllListeners(); 
        }
    }
}