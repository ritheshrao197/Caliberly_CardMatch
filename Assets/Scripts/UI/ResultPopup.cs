using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for the result popup that appears when a level is completed or failed.
    /// Displays appropriate messages and handles user actions for proceeding to the next level or retrying.
    /// </summary>
    public class ResultPopup : MonoBehaviour
    {
        /// <summary>
        /// Text component for the popup title
        /// </summary>
        public TextMeshProUGUI titleText;
        
        /// <summary>
        /// Text component for the popup subtitle or description
        /// </summary>
        public TextMeshProUGUI subtitleText;
        
        /// <summary>
        /// Button for proceeding to the next level (win) or retrying (lose)
        /// </summary>
        public Button nextButton;
        
        /// <summary>
        /// Button for returning to the home screen
        /// </summary>
        public Button homeButton;

        /// <summary>
        /// Action to perform when the next/retry button is clicked
        /// </summary>
        Action _onNext;
        
        /// <summary>
        /// Action to perform when the home button is clicked
        /// </summary>
        Action _onHome;

        /// <summary>
        /// Configures the result popup with win/lose state and button actions
        /// </summary>
        /// <param name="win">True if the player won the level, false if they lost</param>
        /// <param name="levelIndex">The zero-based index of the completed level</param>
        /// <param name="reason">The reason for losing (if applicable)</param>
        /// <param name="onNext">Action to perform when the next/retry button is clicked</param>
        /// <param name="onHome">Action to perform when the home button is clicked</param>
        public void Bind(bool win, int levelIndex, string reason, Action onNext, Action onHome)
        {
#if UNITY_EDITOR
            Debug.Log($"[ResultPopup] Bind called with win={win}, levelIndex={levelIndex}, reason={reason}");
#endif
            _onNext = onNext; 
            _onHome = onHome;
            
            // Update title text
            if (titleText) 
                titleText.text = win ? "Level Complete" : "Level Failed";
                
            // Update subtitle text
            if (subtitleText)
                subtitleText.text = win ? $"You finished Level {levelIndex + 1}"
                                        : (reason == "time" ? "Time's up" : "Move limit reached");
                                        
            // Update next button text and listener
            if (nextButton) 
            {
                var nextButtonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
                if (nextButtonText)
                    nextButtonText.text = win ? "Next" : "Retry";
                
                nextButton.onClick.RemoveAllListeners(); 
                nextButton.onClick.AddListener(() => _onNext?.Invoke()); 
            }
            
            // Update home button listener
            if (homeButton) 
            { 
                homeButton.onClick.RemoveAllListeners(); 
                homeButton.onClick.AddListener(() => _onHome?.Invoke()); 
            }
        }
    }
}