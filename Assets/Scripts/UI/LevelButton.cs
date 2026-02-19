using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame.Views
{
    /// <summary>
    /// Represents a single level button in the level selection UI.
    /// Displays the level number and handles click events for level selection.
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        /// <summary>
        /// Text component to display the level number
        /// </summary>
        public TextMeshProUGUI label;
        
        /// <summary>
        /// Button component for user interaction
        /// </summary>
        public Button button;

        /// <summary>
        /// Configures the level button with its level number and click handler
        /// </summary>
        /// <param name="levelNumber">The level number to display on the button</param>
        /// <param name="onClick">The action to perform when the button is clicked</param>
        public void Set(int levelNumber, Action onClick)
        {
            if (label) 
                label.text = levelNumber.ToString();
            if (button)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick?.Invoke());
            }
        }
    }
}