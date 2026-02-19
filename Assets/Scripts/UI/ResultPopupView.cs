using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MemoryGame.Constants; 

namespace MemoryGame.Views
{
    /// <summary>
    /// View component for the result popup that appears when a level is completed or failed.
    /// Handles showing and hiding the popup with appropriate text and button actions.
    /// </summary>
    public class ResultPopupView : MonoBehaviour
    {
        /// <summary>
        /// Canvas group for controlling popup visibility and interaction
        /// </summary>
        [Header("Wiring")]
        public CanvasGroup group;
        
        /// <summary>
        /// Text component for the popup title
        /// </summary>
        public TextMeshProUGUI titleText;
        
        /// <summary>
        /// Text component for the popup subtitle
        /// </summary>
        public TextMeshProUGUI subtitleText;
        
        /// <summary>
        /// Button for proceeding to the next level or retrying
        /// </summary>
        public Button nextButton;
        
        /// <summary>
        /// Button for returning to the home screen
        /// </summary>
        public Button homeButton;

        /// <summary>
        /// Action to perform when the next button is clicked
        /// </summary>
        public Action onNext;
        
        /// <summary>
        /// Action to perform when the home button is clicked
        /// </summary>
        public Action onHome;

        /// <summary>
        /// Automatically finds and assigns the CanvasGroup component
        /// </summary>
        void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Shows the result popup with appropriate text based on win/lose state
        /// </summary>
        /// <param name="win">True if the player won, false if they lost</param>
        /// <param name="levelIndex">The zero-based index of the completed level</param>
        /// <param name="loseReason">The reason for losing (if applicable)</param>
        public void Show(bool win, int levelIndex, string loseReason)
        {
            if (group != null)
            {
                group.alpha = 1f;
                group.blocksRaycasts = true;
                group.interactable = true;
            }

            gameObject.SetActive(true);

            if (titleText)
                titleText.text = win ? UIStrings.WinTitle : UIStrings.LoseTitle;

            if (subtitleText)
                subtitleText.text = win
                    ? $"Level {levelIndex + 1}"
                    : (string.IsNullOrEmpty(loseReason)
                        ? $"Level {levelIndex + 1}"
                        : loseReason);

            if (nextButton)
            {
                var label = nextButton.GetComponentInChildren<Text>();
                if (label)
                    label.text = win ? UIStrings.NextOnWin : UIStrings.NextOnLose;

                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() => onNext?.Invoke());
            }

            if (homeButton)
            {
                homeButton.onClick.RemoveAllListeners();
                homeButton.onClick.AddListener(() => onHome?.Invoke());
            }
        }

        /// <summary>
        /// Hides the result popup
        /// </summary>
        public void Hide()
        {
            if (group != null)
            {
                group.alpha = 0f;
                group.blocksRaycasts = false;
                group.interactable = false;
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Immediately hides the result popup
        /// </summary>
        public void HideImmediate() => Hide();
    }
}