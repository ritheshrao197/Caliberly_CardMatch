using MemoryGame.Events;
using MemoryGame.UI.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for the pause popup that appears when the game is paused.
    /// Provides options to resume, restart, or return to the home screen.
    /// </summary>
    public class PausePopup : MonoBehaviour
    {
        /// <summary>
        /// Button to resume the game
        /// </summary>
        public Button resumeButton;
        
        /// <summary>
        /// Button to restart the current level
        /// </summary>
        public Button restartButton;
        
        /// <summary>
        /// Button to return to the home screen
        /// </summary>
        public Button homeButton;

        /// <summary>
        /// Initializes the component and binds button click events to UI events
        /// </summary>
        void Awake()
        {
            if (resumeButton)  
                resumeButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnResumeEvent()));
            if (restartButton) 
                restartButton.onClick.AddListener(() => EventBus.Instance.Publish(new OnRestartEvent()));
            if (homeButton)    
                homeButton.onClick.AddListener(() => EventBus.Instance.Publish(new GoHomeEvent()));
        }

        /// <summary>
        /// Cleans up button click listeners when the component is destroyed
        /// </summary>
        void OnDestroy()
        {
            if (resumeButton)  
                resumeButton.onClick.RemoveAllListeners();
            if (restartButton) 
                restartButton.onClick.RemoveAllListeners();
            if (homeButton)    
                homeButton.onClick.RemoveAllListeners();
        }
    }
}